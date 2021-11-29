using System;
using NUnit.Framework;
using System.IO;
using NAudio.Wave;
using System.Diagnostics;
using NAudio.MediaFoundation;
using NAudio.Wave.SampleProviders;
using NAudioTests.Utils;

namespace NAudioTests.Mp3
{
    [TestFixture]
    public class LazyMp3FileReaderTests
    {
        [Test]
        public void CanGetFrames()
        {
            string inputFile = @"\\smb-dcsweb.ngt.dbb.dk\Dist1\UNI0\150__\15000\D202\dtb_0026.mp3";
            //TimeSpan start = TimeSpan.FromMinutes(2);
            TimeSpan excerptDuration = TimeSpan.FromMinutes(5);

            using (FileStream outStream = File.Create("lazyloadedframes.mp3"))
            using (LazyMp3FileReader reader = new LazyMp3FileReader(inputFile))
            {
                
                outStream.Write(reader.Id3v2Tag.RawData, 0, reader.Id3v2Tag.RawData.Length);

                Mp3Frame frame = reader.ReadNextFrame();
                while (frame != null && reader.CurrentTime < excerptDuration)
                {
                    outStream.Write(frame.RawData, 0, frame.RawData.Length);
                    frame = reader.ReadNextFrame();
                }
            }
        }

        [Test]
        [Category("IntegrationTest")]
        public void CanLoadAndReadVariousProblemMp3Files()
        {
            string testDataFolder = @"C:\Users\B044551\Desktop\WmBatchTest\Input";
            if (!Directory.Exists(testDataFolder))
            {
                Assert.Ignore("{0} not found", testDataFolder);
            }
            foreach (string file in Directory.GetFiles(testDataFolder, "*.mp3"))
            {
                string mp3File = Path.Combine(testDataFolder, file);
                Debug.WriteLine($"Opening {mp3File}");
                using (var reader = new LazyMp3FileReader(mp3File))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    int total = 0;
                    do
                    {
                        bytesRead = reader.Read(buffer, 0, buffer.Length);
                        total += bytesRead;
                    } while (bytesRead > 0);
                    Debug.WriteLine($"Read {total} bytes");
                }
            }
        }

        [Test]
        public void ReadFrameAdvancesPosition()
        {
            var file = TestFileBuilder.CreateMp3File(5);
            try
            {
                using (var mp3FileReader = new Mp3FileReader(file))
                {
                    var lastPos = mp3FileReader.Position;
                    while ((mp3FileReader.ReadNextFrame()) != null)
                    {
                        Assert.IsTrue(mp3FileReader.Position > lastPos);
                        lastPos = mp3FileReader.Position;
                    }
                    Assert.AreEqual(mp3FileReader.Length, mp3FileReader.Position);
                    Assert.IsTrue(mp3FileReader.Length > 0);
                }
            }
            finally
            {
                File.Delete(file);
            }
        }

        [Test]
        public void CopesWithZeroLengthMp3()
        {
            var ms = new MemoryStream(new byte[0]);
            Assert.Throws<InvalidDataException>(() => new Mp3FileReader(ms));
        }
    }
}
