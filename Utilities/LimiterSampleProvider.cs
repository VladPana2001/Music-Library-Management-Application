using NAudio.Wave;

namespace Music_Library_Management_Application.Utilities
{
    public class LimiterSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private readonly float threshold;

        public LimiterSampleProvider(ISampleProvider source, float threshold)
        {
            this.source = source;
            this.threshold = threshold;
        }

        public WaveFormat WaveFormat => source.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = source.Read(buffer, offset, count);
            for (int n = 0; n < samplesRead; n++)
            {
                if (buffer[offset + n] > threshold)
                {
                    buffer[offset + n] = threshold;
                }
                else if (buffer[offset + n] < -threshold)
                {
                    buffer[offset + n] = -threshold;
                }
            }

            return samplesRead;
        }
    }

}
