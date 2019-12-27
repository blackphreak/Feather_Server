using System;
using System.Security.Cryptography;

namespace Feather_Server.ServerRelated
{
    public class AES
    {
        // src: https://stackoverflow.com/questions/32939288/how-can-i-use-aes-to-encrypt-a-single-block
        private ICryptoTransform encryptor;
        private ICryptoTransform decryptor;

        public AES(byte[] userSecret)
        {
            var aesAlg = new AesManaged();
            aesAlg.Mode = CipherMode.ECB;

            aesAlg.BlockSize = 128;
            aesAlg.KeySize = 128;
            aesAlg.Padding = PaddingMode.Zeros;
            aesAlg.Key = userSecret;

            encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        }

        private byte[] pureEncrypt(byte[] plainText)
        {
            byte[] output_buffer = new byte[plainText.Length];
            encryptor.TransformBlock(plainText, 0, plainText.Length, output_buffer, 0);
            return output_buffer;
        }

        private byte[] pureDecrypt(byte[] cipherText)
        {
            byte[] output_buffer = new byte[cipherText.Length];
            decryptor.TransformBlock(cipherText, 0, cipherText.Length, output_buffer, 0);
            return output_buffer;
        }

        public byte[] encrypt(byte[] plainText)
        {
            var blockCount = (plainText.Length + 15) >> 4;
            var szBlock = plainText.Length % 16;
            var cipher = new byte[blockCount * 16];
            var tmp_block = new byte[16];
            for (int i = 0; i < blockCount; i++)
            {
                Array.Clear(tmp_block, 0, tmp_block.Length);
                Buffer.BlockCopy(plainText, i * 16, tmp_block, 0, (i == blockCount - 1) ? szBlock : 16);
                Buffer.BlockCopy(pureEncrypt(tmp_block), 0, cipher, i * 16, 16);
            }
            return cipher;
        }

        public byte[] decrypt(byte[] cipherText)
        {
            var blockCount = (cipherText.Length + 15) >> 4;
            var szBlock = cipherText.Length % 16;
            szBlock = szBlock == 0 ? 16 : szBlock;
            var plainText = new byte[blockCount * 16];
            var tmp_block = new byte[16];
            for (int i = 0; i < blockCount; i++)
            {
                Array.Clear(tmp_block, 0, tmp_block.Length);
                Buffer.BlockCopy(cipherText, i * 16, tmp_block, 0, (i == blockCount - 1) ? szBlock : 16);
                Buffer.BlockCopy(pureDecrypt(tmp_block), 0, plainText, i * 16, 16);
            }
            return plainText;
        }
    }
}
