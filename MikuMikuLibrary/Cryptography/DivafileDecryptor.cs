﻿using System.Security.Cryptography;
using MikuMikuLibrary.IO.Common;

namespace MikuMikuLibrary.Cryptography;

public static class DivafileDecryptor
{
    private static Aes GetAes()
    {
        var aes = Aes.Create();
        aes.KeySize = 128;
        aes.Key = new byte[]
        {
            // file access deny
            0x66, 0x69, 0x6C, 0x65, 0x20, 0x61, 0x63, 0x63, 0x65, 0x73, 0x73, 0x20, 0x64, 0x65, 0x6E, 0x79
        };
        aes.BlockSize = 128;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.Zeros;
        aes.IV = new byte[16];
        return aes;
    }

    private static readonly Aes sAes = GetAes();

    public static void ReadHeader(Stream source, bool skipSignature, out uint encryptedSize, out uint unencryptedSize)
    {
        var header = new byte[skipSignature ? 8 : 16];
        source.Read(header, 0, header.Length);

        if (!skipSignature)
        {
            string signature = Encoding.UTF8.GetString(header, 0, 8);

            if (signature != "DIVAFILE")
                throw new InvalidDataException($"Invalid signature (expected DIVAFILE)");
        }

        int offset = skipSignature ? 0 : 8;

        encryptedSize = BitConverter.ToUInt32(header, offset);
        unencryptedSize = BitConverter.ToUInt32(header, offset + 4);
    }

    public static CryptoStream CreateDecryptorStream(Stream source, bool readHeader = true, bool skipSignature = false)
    {
        var decryptor = sAes.CreateDecryptor();

        if (!readHeader)
            return new CryptoStream(source, decryptor, CryptoStreamMode.Read);

        ReadHeader(source, skipSignature, out uint encryptedSize, out _);

        var streamView = new StreamView(source, source.Position, encryptedSize, true);

        return new CryptoStream(streamView, decryptor, CryptoStreamMode.Read);
    }

    public static MemoryStream DecryptToMemoryStream(Stream source, bool readHeader = true,
        bool skipSignature = false)
    {
        var memoryStream = new MemoryStream();

        using (var cryptoStream = CreateDecryptorStream(source, readHeader, skipSignature))
            cryptoStream.CopyTo(memoryStream);

        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }
}