namespace MailRuEncryptedCloud.Encryption
{
    public interface IEncryptor
    {
        byte[] Encrypt(byte[] data, byte[] key, byte[] vector);

        byte[] Decrypt(byte[] data, byte[] key, byte[] vector);

        (byte[] key, byte[] vector) GenerateRandomKeys();
    }
}