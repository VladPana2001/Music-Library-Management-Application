using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Repositories.Interfaces;
using Music_Library_Management_Application.Services.Interfaces;
using Music_Library_Management_Application.Utilities;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Drawing.Printing;

namespace Music_Library_Management_Application.Services
{
    public class MixService : IMixService
    {
        private readonly IRepositoryWrapper _repoWrapper;

        public MixService(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        public async Task<MixCreateViewModel> GetMixCreateViewModelAsync(string userId)
        {
            var songs = _repoWrapper.Songs.GetAllByUserId(userId).ToList();

            return new MixCreateViewModel
            {
                AllSongs = songs
            };
        }

        public async Task<byte[]> GenerateMixAsync(Mix mix, string userId)
        {
            var sortedSongs = mix.Songs.OrderBy(s => s.StartPosition).ToList();
            var outputFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2); // 44.1 kHz, stereo
            var sampleProviders = new List<ISampleProvider>();
            var readers = new List<Mp3FileReader>(); // To keep the readers alive

            var mixDb = new MixDb
            {
                Title = mix.Name,
                Description = mix.Description,
                LimiterThreshold = mix.LimiterThreshold,
                UserId = userId
            };
            var mixSongsDb = new List<MixSongDb>();

            try
            {
                foreach (var mixSong in sortedSongs)
                {
                    var song = _repoWrapper.Songs.GetById(mixSong.SongId);
                    if (song == null || song.UserId != userId)
                    {
                        continue;
                    }

                    var mixSongDb = new MixSongDb
                    {
                        Title = song.SongTitle,
                        StartTime = mixSong.StartTime,
                        EndTime = mixSong.EndTime,
                        StartPosition = mixSong.StartPosition,
                        FadeInDuration = mixSong.FadeInDuration,
                        FadeOutDuration = mixSong.FadeOutDuration,
                        Volume = mixSong.Volume
                    };
                    mixSongsDb.Add(mixSongDb);

                    var mp3Reader = new Mp3FileReader(new MemoryStream(song.SongFile));
                    readers.Add(mp3Reader);

                    var sampleProvider = mp3Reader.ToSampleProvider();

                    var volumeProvider = new VolumeSampleProvider(sampleProvider)
                    {
                        Volume = (float)mixSong.Volume
                    };

                    // Apply start and end offsets and delay by StartPosition
                    var offsetSampleProvider = new OffsetSampleProvider(volumeProvider)
                    {
                        SkipOver = TimeSpan.FromSeconds(mixSong.StartTime),
                        Take = TimeSpan.FromSeconds(mixSong.EndTime - mixSong.StartTime),
                        DelayBy = TimeSpan.FromSeconds(mixSong.StartPosition)
                    };


                    int fadeInDurationMs = (int)(mixSong.FadeInDuration * 1000);
                    int fadeOutDurationMs = (int)(mixSong.FadeOutDuration * 1000);
                    int totalDurationMs = (int)(mixSong.EndTime - mixSong.StartTime + mixSong.StartPosition) * 1000;
                    int startDurationMs = (int)(mixSong.StartPosition * 1000);

                    // Create the fade-in provider
                    var fadeInProvider = new CustomFadeInOutSampleProvider(
                        offsetSampleProvider,
                        fadeInDurationMs,    // Fade-in duration
                        0,                   // No fade-out duration here
                        totalDurationMs,      // Total duration for the entire process
                        startDurationMs
                    );

                    // Create the fade-out provider
                    var fadeOutProvider = new CustomFadeInOutSampleProvider(
                        fadeInProvider,
                        0,                   // No fade-in duration here
                        fadeOutDurationMs,   // Fade-out duration
                        totalDurationMs,      // Total duration for the entire process
                        startDurationMs
                    );

                    // Resample if necessary to match output format
                    var resampler = new WdlResamplingSampleProvider(fadeOutProvider, outputFormat.SampleRate);

                    // Add the resampled provider to the list of sample providers
                    sampleProviders.Add(resampler);
                }

                // Create the mixer with the array of sample providers
                var mixer = new MixingSampleProvider(sampleProviders);

                float threshold = (float)mix.LimiterThreshold;
                var limitedProvider = new LimiterSampleProvider(mixer, threshold);


                using (var memoryStream = new MemoryStream())
                {
                    using (var waveFileWriter = new WaveFileWriter(memoryStream, outputFormat))
                    {
                        var buffer = new float[1024];
                        int samplesRead;
                        while ((samplesRead = limitedProvider.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            waveFileWriter.WriteSamples(buffer, 0, samplesRead);
                        }
                    }

                    mixDb.MixFile = memoryStream.ToArray();
                }

                CreateMixRecord(mixDb, mixSongsDb);

                return mixDb.MixFile;
            }
            finally
            {
                // Dispose readers after mixing
                foreach (var reader in readers)
                {
                    reader.Dispose();
                }
            }
        }

        public void CreateMixRecord(MixDb mixDb, List<MixSongDb> mixSongsDb)
        {
            mixDb.MixSongs = mixSongsDb;
            _repoWrapper.Mixes.Add(mixDb);
        }

        public async Task<FileStreamResult> PlayMixAsync(int id, string userId)
        {
            var mix = await GetMixByIdAndUserIdAsync(id, userId);
            if (mix == null)
                throw new KeyNotFoundException("Mix not found.");

            var fileStream = new MemoryStream(mix.MixFile);
            var response = new FileStreamResult(fileStream, "audio/mpeg")
            {
                EnableRangeProcessing = true
            };
            return response;
        }

        public async Task<MixDb> GetMixByIdAndUserIdAsync(int id, string userId)
        {
            return _repoWrapper.Mixes.GetByIdAndUserId(id, userId);
        }

        public async Task<byte[]> PreviewSongAsync(int songId, string userId, double startTime, double endTime, double fadeInDuration, double fadeOutDuration, double volume)
        {
            var song = _repoWrapper.Songs.GetById(songId);
            if (song == null || song.UserId != userId)
            {
                throw new ArgumentException("Invalid song ID or user ID.");
            }

            using (var mp3Reader = new Mp3FileReader(new MemoryStream(song.SongFile)))
            {
                var sampleProvider = mp3Reader.ToSampleProvider();

                var volumeProvider = new VolumeSampleProvider(sampleProvider)
                {
                    Volume = (float)volume
                };

                var offsetSampleProvider = new OffsetSampleProvider(volumeProvider)
                {
                    SkipOver = TimeSpan.FromSeconds(startTime),
                    Take = TimeSpan.FromSeconds(endTime - startTime),
                };

                int fadeInDurationMs = (int)(fadeInDuration * 1000);
                int fadeOutDurationMs = (int)(fadeOutDuration * 1000);
                int totalDurationMs = (int)((endTime - startTime) * 1000);
                int startDurationMs = (int)(0 * 1000);

                var fadeInProvider = new CustomFadeInOutSampleProvider(
                    offsetSampleProvider,
                    fadeInDurationMs,
                    0,
                    totalDurationMs,
                    startDurationMs
                );

                var fadeOutProvider = new CustomFadeInOutSampleProvider(
                    fadeInProvider,
                    0,
                    fadeOutDurationMs,
                    totalDurationMs,
                    startDurationMs
                );

                var outputFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
                var resampler = new WdlResamplingSampleProvider(fadeOutProvider, outputFormat.SampleRate);

                using (var memoryStream = new MemoryStream())
                {
                    using (var waveFileWriter = new WaveFileWriter(memoryStream, outputFormat))
                    {
                        var buffer = new float[1024];
                        int samplesRead;
                        while ((samplesRead = resampler.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            waveFileWriter.WriteSamples(buffer, 0, samplesRead);
                        }
                    }

                    return memoryStream.ToArray();
                }
            }
        }
    }
}