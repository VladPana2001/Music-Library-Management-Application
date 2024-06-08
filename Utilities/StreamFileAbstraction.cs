namespace Music_Library_Management_Application.Utilities
{
    public class StreamFileAbstraction : TagLib.File.IFileAbstraction
    {
        private readonly Stream _stream;

        public StreamFileAbstraction(string name, Stream stream)
        {
            Name = name;
            _stream = stream;
        }

        public void CloseStream(Stream stream)
        {}

        public string Name { get; }

        public Stream ReadStream => _stream;

        public Stream WriteStream => _stream;
    }
}
