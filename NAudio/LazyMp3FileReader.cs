using System.IO;

// ReSharper disable once CheckNamespace
namespace NAudio.Wave
{

    /// <summary>
    /// Class for reading from MP3 files
    /// </summary>
    public class LazyMp3FileReader : LazyMp3FileReaderBase
    {
        /// <summary>Supports opening a MP3 file</summary>
        public LazyMp3FileReader(string mp3FileName)
            : base(File.OpenRead(mp3FileName), Mp3FileReader.CreateAcmFrameDecompressor, true)
        {
        }

        /// <summary>
        /// Opens MP3 from a stream rather than a file
        /// Will not dispose of this stream itself
        /// </summary>
        /// <param name="inputStream">The incoming stream containing MP3 data</param>
        public LazyMp3FileReader(Stream inputStream)
            : base(inputStream, Mp3FileReader.CreateAcmFrameDecompressor, false)
        {

        }
    }
}
