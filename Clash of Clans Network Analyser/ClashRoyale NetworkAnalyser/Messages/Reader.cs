using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace coc_messages_csharp
{
    public class Reader : IDisposable
    {
        private int messageId;
        public int MessageId
        {
            get
            {
                return this.messageId;
            }
        }
        private int unknown;
        public int Unknown
        {
            get
            {
                return this.unknown;
            }
        }
        private BinaryReader binReader;

        public Reader(int messageId, int unknown, byte[] initial_bytes)
        {
            this.messageId = messageId;
            this.unknown = unknown;
            MemoryStream memStream = new MemoryStream(initial_bytes);
            this.binReader = new BinaryReader(memStream);
        }

        public Reader(byte[] initial_bytes)
        {
            MemoryStream memStream = new MemoryStream(initial_bytes);
            this.binReader = new BinaryReader(memStream);
            this.messageId = BitConverter.ToInt32(new byte[2].Concat(this.binReader.ReadBytes(2)).Reverse().ToArray(), 0);
            this.binReader.BaseStream.Position += 3;
            this.unknown = BitConverter.ToInt32(new byte[2].Concat(this.binReader.ReadBytes(2)).Reverse().ToArray(), 0);
        }

        public byte readByte()
        {
            return this.binReader.ReadByte();
        }

        public byte[] readBytes(int bytes)
        {
            return this.binReader.ReadBytes(bytes);
        }

        public int readInt()
        {
            return BitConverter.ToInt32(this.readBytes(4).Reverse().ToArray(), 0);
        }

        public int readInt(int bytes)
        {
            if (bytes > 4)
                throw new System.ArgumentOutOfRangeException("Int cannot be larger than 4 bytes");
            return BitConverter.ToInt32(new byte[4 - bytes].Concat(this.readBytes(bytes)).Reverse().ToArray(), 0);
        }

        public long readLong()
        {
            return BitConverter.ToInt64(this.readBytes(8).Reverse().ToArray(), 0);
        }

        public string readString()
        {
            int length = this.readInt();
            if (length <= 0)
                return null;
            return Encoding.UTF8.GetString(this.readBytes(length));
        }

        public string readZString()
        {
            int length = this.readInt();
            if (length <= 0)
                return null;
            int zlength = this.binReader.ReadInt32();
            length -= 4;
            this.skipBytes(2);
            length -= 2;

            using (MemoryStream compressedMemoryStream = new MemoryStream(this.readBytes(length)))
            {
                using (MemoryStream decompressedMemoryStream = new MemoryStream(zlength))
                {
                    using (DeflateStream decompressionStream = new DeflateStream(compressedMemoryStream, CompressionMode.Decompress, true))
                    {
                        decompressionStream.CopyTo(decompressedMemoryStream);
                    }
                    return Encoding.UTF8.GetString(decompressedMemoryStream.GetBuffer());
                }
            }
        }

        public byte[] read()
        {
            return this.readBytes(this.availableBytes());
        }

        public int peekInt()
        {
            int peek = this.readInt();
            this.binReader.BaseStream.Position -= 4;
            return peek;
        }

        public int peekInt(int bytes)
        {
            if (bytes > 4)
                throw new System.ArgumentOutOfRangeException("Int cannot be larger than 4 bytes");
            int peek = BitConverter.ToInt32(new byte[4 - bytes].Concat(this.readBytes(bytes)).Reverse().ToArray(), 0);
            this.binReader.BaseStream.Position -= bytes;
            return peek;
        }

        public byte[] skipBytes(int bytes)
        {
            return this.readBytes(bytes);
        }

        public int availableBytes()
        {
            return (int)(this.binReader.BaseStream.Length - this.binReader.BaseStream.Position);
        }

        public void Dispose()
        {
            if (this.binReader != null)
                this.binReader.Dispose();
        }
    }
}
