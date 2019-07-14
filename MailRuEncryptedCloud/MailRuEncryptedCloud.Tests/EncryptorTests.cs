namespace MailRuEncryptedCloud.Tests
{
    using System.Linq;
    using System.Security.Cryptography;

    using MailRuEncryptedCloud.Encryption;

    using NUnit.Framework;

    [TestFixture]
    public class EncryptorTests
    {
        [Test]
        public void KeyGenerationTest()
        {
            var encryptor = new Encryptor();
            var (key, vector) = encryptor.GenerateRandomKeys();
            Assert.IsNotNull(key);
            Assert.IsNotNull(vector);

            Assert.AreEqual(32, key.Length);
            Assert.AreEqual(16, vector.Length);

            Assert.IsTrue(key.Any(b => b != 0));
            Assert.IsTrue(vector.Any(b => b != 0));
        }

        [Test]
        public void EmptyEncryptionTest()
        {
            var encryptor = new Encryptor();
            var (key, vector) = encryptor.GenerateRandomKeys();

            var encrypted = encryptor.Encrypt(null, key, vector);
            Assert.IsNotNull(encrypted);
            Assert.IsEmpty(encrypted);

            encrypted = encryptor.Encrypt(new byte[0], key, vector);
            Assert.IsNotNull(encrypted);
            Assert.IsEmpty(encrypted);
        }

        [Test]
        public void EmptyDecryptionTest()
        {
            var encryptor = new Encryptor();
            var (key, vector) = encryptor.GenerateRandomKeys();

            var decrypted = encryptor.Decrypt(null, key, vector);
            Assert.IsNotNull(decrypted);
            Assert.IsEmpty(decrypted);

            decrypted = encryptor.Decrypt(new byte[0], key, vector);
            Assert.IsNotNull(decrypted);
            Assert.IsEmpty(decrypted);
        }

        [Test]
        [TestCase(
            new byte [] { 1, 2, 3 },
            new byte[]
                {
                    229, 47, 255, 16, 43, 203, 155, 217, 85, 227, 166, 82, 50, 41,
                    2, 117
                },
            new byte[]
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18,
                    19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32
                },
            new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 })]
        [TestCase(
            new byte[] 
                {
                    229, 47, 255, 16, 43, 203, 155, 217, 85, 227, 166, 82, 50, 41,
                    2, 117
                },
            new byte[]
                {
                    238, 182, 216, 100, 112, 135, 203, 40, 142, 120, 136, 49, 95,
                    21, 84, 228, 58, 93, 188, 134, 103, 181, 122, 167, 193, 99,
                    188, 40, 140, 66, 162, 230
                },
            new byte[]
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18,
                    19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32
                },
            new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 })]
        public void EncryptionTest(
            byte[] data, 
            byte[] expectedEncrypted, 
            byte[] key, 
            byte[] vector)
        {
            var encryptor = new Encryptor();
            var encrypted = encryptor.Encrypt(data, key, vector);
            Assert.IsNotNull(encrypted);
            CollectionAssert.AreEqual(expectedEncrypted, encrypted);
        }

        [Test]
        [TestCase(
            new byte[] { 1, 2, 3 },
            new byte[]
                {
                    229, 47, 255, 16, 43, 203, 155, 217, 85, 227, 166, 82, 50, 41,
                    2, 117
                },
            new byte[]
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18,
                    19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32
                },
            new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 })]
        [TestCase(
            new byte[]
                {
                    229, 47, 255, 16, 43, 203, 155, 217, 85, 227, 166, 82, 50, 41,
                    2, 117
                },
            new byte[]
                {
                    238, 182, 216, 100, 112, 135, 203, 40, 142, 120, 136, 49, 95,
                    21, 84, 228, 58, 93, 188, 134, 103, 181, 122, 167, 193, 99,
                    188, 40, 140, 66, 162, 230
                },
            new byte[]
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18,
                    19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32
                },
            new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 })]
        public void DecryptionTest(
            byte[] expectedDecrypted,
            byte[] data,
            byte[] key,
            byte[] vector)
        {
            var encryptor = new Encryptor();
            var decrypted = encryptor.Decrypt(data, key, vector);
            Assert.IsNotNull(decrypted);
            CollectionAssert.AreEqual(expectedDecrypted, decrypted);
        }

        [Test]
        [TestCase(4)]
        [TestCase(20)]
        [TestCase(6000)]
        [TestCase(90000)]
        [TestCase(1000000)]
        public void RandomEncryptionDecryptionTest(int length)
        {
            byte[] data;
            using (var provider = new RNGCryptoServiceProvider())
            {
                data = new byte[length];
                provider.GetBytes(data);
            }

            var encryptor = new Encryptor();
            var (key, vector) = encryptor.GenerateRandomKeys();

            var encrypted = encryptor.Encrypt(data, key, vector);
            Assert.IsNotNull(encrypted);
            CollectionAssert.AreNotEqual(data, encrypted);

            var decrypted = encryptor.Decrypt(encrypted, key, vector);
            Assert.IsNotNull(decrypted);
            CollectionAssert.AreEqual(data, decrypted);
        }

        [Test]
        public void GenerateRandomStringTest()
        {
            var encryptor = new Encryptor();
            var str1 = encryptor.GenerateRandomString();
            Assert.IsNotNull(str1);
            var str2 = encryptor.GenerateRandomString();
            Assert.IsNotNull(str2);
            var str3 = encryptor.GenerateRandomString();
            Assert.IsNotNull(str3);
            Assert.AreNotEqual(str1, str2);
            Assert.AreNotEqual(str2, str3);
        }
    }
}