namespace MailRuEncryptedCloud.Encryption
{
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;

    internal class Encryptor : IEncryptor
    {
        private const int AesKeySizeInBits = 256;

        public byte[] Encrypt(byte[] data, byte[] key, byte[] vector)
        {
            if (data == null || data.Length == 0)
            {
                return new byte[0];
            }

            using (var aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = AesKeySizeInBits;
                aes.Key = key;
                aes.IV = vector;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }

                    return ms.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] data, byte[] key, byte[] vector)
        {
            if (data == null || data.Length == 0)
            {
                return new byte[0];
            }

            using (var aes = new AesManaged())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = AesKeySizeInBits;
                aes.Key = key;
                aes.IV = vector;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                    }

                    return ms.ToArray();
                }
            }
        }

        public (byte[] key, byte[] vector) GenerateRandomKeys()
        {
            var keyBytesLength = AesKeySizeInBits / 8;
            var bytes = GetRandomBytes(keyBytesLength + 16);
            return (bytes.Take(keyBytesLength).ToArray(), bytes.Skip(keyBytesLength).ToArray());
        }

        private static byte[] GetRandomBytes(int length)
        {
            using (var provider = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[length];
                provider.GetBytes(bytes);
                return bytes;
            }
        }
    }
}
