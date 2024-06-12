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

            try
            {
                foreach (var mixSong in sortedSongs)
                {
                    var song = _repoWrapper.Songs.GetById(mixSong.SongId);
                    if (song == null || song.UserId != userId)
                    {
                        continue;
                    }

                    var mp3Reader = new Mp3FileReader(new MemoryStream(song.SongFile));
                    readers.Add(mp3Reader);

                    var sampleProvider = mp3Reader.ToSampleProvider();

                    // Apply start and end offsets and delay by StartPosition
                    var offsetSampleProvider = new OffsetSampleProvider(sampleProvider)
                    {
                        SkipOver = TimeSpan.FromSeconds(mixSong.StartTime),
                        Take = TimeSpan.FromSeconds(mixSong.EndTime - mixSong.StartTime),
                        DelayBy = TimeSpan.FromSeconds(mixSong.StartPosition)
                    };


                    int fadeInDurationMs = (int)(mixSong.FadeInDuration * 1000);
                    int fadeOutDurationMs = (int)(mixSong.FadeOutDuration * 1000);
                    int totalDurationMs = (int)(mixSong.EndTime - mixSong.StartTime + mixSong.StartPosition) * 1000;

                    // Create the fade-in provider
                    var fadeInProvider = new CustomFadeInOutSampleProvider(
                        offsetSampleProvider,
                        fadeInDurationMs,    // Fade-in duration
                        0,                   // No fade-out duration here
                        totalDurationMs      // Total duration for the entire process
                    );

                    // Create the fade-out provider
                    var fadeOutProvider = new CustomFadeInOutSampleProvider(
                        fadeInProvider,
                        0,                   // No fade-in duration here
                        fadeOutDurationMs,   // Fade-out duration
                        totalDurationMs      // Total duration for the entire process
                    );

                    // Resample if necessary to match output format
                    var resampler = new WdlResamplingSampleProvider(fadeOutProvider, outputFormat.SampleRate);

                    // Add the resampled provider to the list of sample providers
                    sampleProviders.Add(resampler);
                }

                // Create the mixer with the array of sample providers
                var mixer = new MixingSampleProvider(sampleProviders);

                using (var memoryStream = new MemoryStream())
                {
                    using (var waveFileWriter = new WaveFileWriter(memoryStream, outputFormat))
                    {
                        var buffer = new float[1024];
                        int samplesRead;
                        while ((samplesRead = mixer.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            waveFileWriter.WriteSamples(buffer, 0, samplesRead);
                        }
                    }

                    return memoryStream.ToArray();
                }
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

    }
}