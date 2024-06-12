using NAudio.Wave;

namespace Music_Library_Management_Application.Utilities
{
    public class CustomFadeInOutSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private readonly int fadeInSamples;
        private readonly int fadeOutSamples;
        private readonly int totalSamples;
        private int sampleCount;
        private bool fadingIn;
        private bool fadingOut;

        public CustomFadeInOutSampleProvider(ISampleProvider source, int fadeInDurationMs, int fadeOutDurationMs, int totalDurationMs)
        {
            this.source = source;
            this.fadeInSamples = (int)((fadeInDurationMs / 1000.0) * source.WaveFormat.SampleRate) * source.WaveFormat.Channels;
            this.fadeOutSamples = (int)((fadeOutDurationMs / 1000.0) * source.WaveFormat.SampleRate) * source.WaveFormat.Channels;
            this.totalSamples = (int)((totalDurationMs / 1000.0) * source.WaveFormat.SampleRate) * source.WaveFormat.Channels;
            this.fadingIn = fadeInSamples > 0;
            this.fadingOut = fadeOutSamples > 0;
        }

        public WaveFormat WaveFormat => source.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = source.Read(buffer, offset, count);
            for (int n = 0; n < samplesRead; n++)
            {
                if (fadingIn && sampleCount < fadeInSamples)
                {
                    buffer[offset + n] *= (float)sampleCount / fadeInSamples;
                }
                else if (fadingOut && sampleCount > (totalSamples - fadeOutSamples))
                {
                    buffer[offset + n] *= (float)(totalSamples - sampleCount) / fadeOutSamples;
                }

                sampleCount++;
            }

            return samplesRead;
        }
    }


}
