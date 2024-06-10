using Music_Library_Management_Application.Models;
using Music_Library_Management_Application.Models.DbModels;
using Music_Library_Management_Application.Repositories.Interfaces;
using Music_Library_Management_Application.Services.Interfaces;
using NAudio.Wave;

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
            var sortedSongs = mix.Songs.OrderBy(s => s.Order).ToList();
            var outputFormat = new WaveFormat(44100, 2); // 44.1 kHz, stereo

            using (var memoryStream = new MemoryStream())
            {
                using (var waveFileWriter = new WaveFileWriter(memoryStream, outputFormat))
                {
                    foreach (var mixSong in sortedSongs)
                    {
                        var song = _repoWrapper.Songs.GetById(mixSong.SongId);
                        if (song == null || song.UserId != userId)
                        {
                            continue;
                        }

                        using (var mp3Reader = new Mp3FileReader(new MemoryStream(song.SongFile)))
                        {
                            // Trim the song
                            mp3Reader.CurrentTime = TimeSpan.FromSeconds(mixSong.StartTime);
                            var endTime = mixSong.EndTime > 0 ? TimeSpan.FromSeconds(mixSong.EndTime) : mp3Reader.TotalTime;

                            // Resample if necessary
                            using (var resampler = new MediaFoundationResampler(mp3Reader, outputFormat))
                            {
                                resampler.ResamplerQuality = 60; // Quality of the conversion
                                var buffer = new byte[1024];
                                while (mp3Reader.CurrentTime < endTime)
                                {
                                    var bytesRequired = Math.Min((int)(endTime - mp3Reader.CurrentTime).TotalSeconds * mp3Reader.WaveFormat.AverageBytesPerSecond, buffer.Length);
                                    var bytesRead = resampler.Read(buffer, 0, bytesRequired);
                                    if (bytesRead == 0) break;

                                    waveFileWriter.Write(buffer, 0, bytesRead);
                                }
                            }
                        }
                    }
                }

                return memoryStream.ToArray();
            }
        }
    }
}
