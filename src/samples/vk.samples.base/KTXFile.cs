using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vk.Samples
{
    // A ridiculously bad KTX file parser.
    // https://www.khronos.org/opengles/sdk/tools/KTX/file_format_spec
    public class KtxFile
    {
        public KtxHeader Header { get; }
        public KtxKeyValuePair[] KeyValuePairs { get; }
        public KtxMipmap[] Mipmaps { get; }

        public KtxFile(KtxHeader header, KtxKeyValuePair[] keyValuePairs, KtxMipmap[] mipmaps)
        {
            Header = header;
            KeyValuePairs = keyValuePairs;
            Mipmaps = mipmaps;
        }

        public static KtxFile Load(Stream s, bool readKeyValuePairs)
        {
            using (BinaryReader br = new BinaryReader(s))
            {
                KtxHeader header = ReadStruct<KtxHeader>(br);

                KtxKeyValuePair[] kvps = null;
                if (readKeyValuePairs)
                {
                    int keyValuePairBytesRead = 0;
                    List<KtxKeyValuePair> keyValuePairs = new List<KtxKeyValuePair>();
                    while (keyValuePairBytesRead < header.BytesOfKeyValueData)
                    {
                        int bytesRemaining = (int)(header.BytesOfKeyValueData - keyValuePairBytesRead);
                        KtxKeyValuePair kvp = ReadNextKeyValuePair(br, out int read);
                        keyValuePairBytesRead += read;
                        keyValuePairs.Add(kvp);
                    }

                    kvps = keyValuePairs.ToArray();
                }
                else
                {
                    br.BaseStream.Seek(header.BytesOfKeyValueData, SeekOrigin.Current); // Skip over header data.
                }

                List<KtxMipmap> mipmaps = new List<KtxMipmap>();
                for (uint mipLevel = 0; mipLevel < header.NumberOfMipmapLevels; mipLevel++)
                {
                    uint imageSize = br.ReadUInt32();
                    if (header.NumberOfFaces != 1)
                    {
                        throw new NotImplementedException();
                    }
                    byte[] mipmapData = br.ReadBytes((int)imageSize);
                    mipmaps.Add(new KtxMipmap(imageSize, mipmapData, header.PixelWidth / (uint)(Math.Pow(2, mipLevel)), header.PixelHeight / (uint)(Math.Pow(2, mipLevel))));

                    uint mipPaddingBytes = 3 - ((imageSize + 3) % 4);
                    br.BaseStream.Seek(mipPaddingBytes, SeekOrigin.Current);
                }

                return new KtxFile(header, kvps, mipmaps.ToArray());
            }
        }

        private static unsafe KtxKeyValuePair ReadNextKeyValuePair(BinaryReader br, out int bytesRead)
        {
            uint keyAndValueByteSize = br.ReadUInt32();
            byte* keyAndValueBytes = stackalloc byte[(int)keyAndValueByteSize];
            ReadBytes(br, keyAndValueBytes, (int)keyAndValueByteSize);
            int paddingByteCount = (int)(3 - ((keyAndValueByteSize + 3) % 4));
            br.BaseStream.Seek(paddingByteCount, SeekOrigin.Current); // Skip padding bytes

            // Find the key's null terminator
            int i;
            for (i = 0; i < keyAndValueByteSize; i++)
            {
                if (keyAndValueBytes[i] == 0)
                {
                    break;
                }
                Debug.Assert(i != keyAndValueByteSize); // Fail
            }


            int keySize = i; // Don't include null terminator.
            string key = Encoding.UTF8.GetString(keyAndValueBytes, keySize);
            byte* valueStart = keyAndValueBytes + i + 1; // Skip null terminator
            int valueSize = (int)(keyAndValueByteSize - keySize - 1);
            byte[] value = new byte[valueSize];
            for (int v = 0; v < valueSize; v++)
            {
                value[v] = valueStart[v];
            }

            bytesRead = (int)(keyAndValueByteSize + paddingByteCount + sizeof(uint));
            return new KtxKeyValuePair(key, value);
        }

        private static unsafe T ReadStruct<T>(BinaryReader br)
        {
            int size = Unsafe.SizeOf<T>();
            byte* bytes = stackalloc byte[size];
            for (int i = 0; i < size; i++)
            {
                bytes[i] = br.ReadByte();
            }

            return Unsafe.Read<T>(bytes);
        }

        private static unsafe void ReadBytes(BinaryReader br, byte* destination, int count)
        {
            for (int i = 0; i < count; i++)
            {
                destination[i] = br.ReadByte();
            }
        }

        public ulong GetTotalSize()
        {
            ulong totalSize = 0;
            foreach (KtxMipmap mipmap in Mipmaps)
            {
                totalSize += mipmap.SizeInBytes;
            }

            return totalSize;
        }

        public byte[] GetAllTextureData()
        {
            byte[] result = new byte[GetTotalSize()];
            uint start = 0;
            foreach (KtxMipmap mipmap in Mipmaps)
            {
                mipmap.Data.CopyTo(result, (int)start);
                start += mipmap.SizeInBytes;
            }

            return result;
        }
    }

    public class KtxKeyValuePair
    {
        public string Key { get; }
        public byte[] Value { get; }
        public KtxKeyValuePair(string key, byte[] value)
        {
            Key = key;
            Value = value;
        }
    }

    public unsafe struct KtxHeader
    {
        public fixed byte Identifier[12];
        public readonly uint Endianness;
        public readonly uint GlType;
        public readonly uint GlTypeSize;
        public readonly uint GlFormat;
        public readonly uint GlInternalFormat;
        public readonly uint GlBaseInternalFormat;
        public readonly uint PixelWidth;
        public readonly uint PixelHeight;
        public readonly uint PixelDepth;
        public readonly uint NumberOfArrayElements;
        public readonly uint NumberOfFaces;
        public readonly uint NumberOfMipmapLevels;
        public readonly uint BytesOfKeyValueData;
    }

    public class KtxMipmap
    {
        public uint SizeInBytes { get; }
        public byte[] Data { get; }
        public uint Width { get; }
        public uint Height { get; }
        public KtxMipmap(uint sizeInBytes, byte[] data, uint width, uint height)
        {
            SizeInBytes = sizeInBytes;
            Data = data;
            Width = width;
            Height = height;
        }
    }
}
