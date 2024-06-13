using NAudio.Wave;

namespace Music_Library_Management_Application.Utilities
{
    public class CustomFadeInOutSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private readonly int fadeInSamples;
        private readonly int fadeOutSamples;
        private readonly int totalSamples;
        private readonly int startSamples;
        private int sampleCount;

        public CustomFadeInOutSampleProvider(ISampleProvider source, int fadeInDurationMs, int fadeOutDurationMs, int totalDurationMs, int startDurationMs)
        {
            this.source = source;
            this.fadeInSamples = (int)((fadeInDurationMs / 1000.0) * source.WaveFormat.SampleRate) * source.WaveFormat.Channels;
            this.fadeOutSamples = (int)((fadeOutDurationMs / 1000.0) * source.WaveFormat.SampleRate) * source.WaveFormat.Channels;
            this.totalSamples = (int)((totalDurationMs / 1000.0) * source.WaveFormat.SampleRate) * source.WaveFormat.Channels;
            this.startSamples = (int)((startDurationMs / 1000.0) * source.WaveFormat.SampleRate) * source.WaveFormat.Channels;
        }

        public WaveFormat WaveFormat => source.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = source.Read(buffer, offset, count);
            for (int n = 0; n < samplesRead; n++)
            {
                // Apply fade-in effect
                if (sampleCount < fadeInSamples + startSamples && sampleCount > startSamples)
                {
                    buffer[offset + n] *= (float)(sampleCount-startSamples) / fadeInSamples;
                }
                // Apply fade-out effect
                else if (sampleCount > (totalSamples - fadeOutSamples))
                {
                    buffer[offset + n] *= (float)(totalSamples - sampleCount) / fadeOutSamples;
                }

                sampleCount++;
            }

            return samplesRead;
        }
    }



}
