using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TheDialgaTeam.DiscordBot.Extension.System.Security.Cryptography
{
    internal static class CryptographyExtensionMethods
    {
        private const int Keysize = 128;
        private const int Derivationiterations = 1000;

        public static string EncryptString(this string plainText, string passPhrase)
        {
            var saltStringBytes = GenerateRandomEntropy();
            var ivStringBytes = GenerateRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, Derivationiterations))
            {
                using (var symmetricKey = new RijndaelManaged())
                {
                    var keyBytes = password.GetBytes(Keysize / 8);

                    symmetricKey.BlockSize = Keysize;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;

                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        MemoryStream memoryStream;

                        using (var cryptoStream = new CryptoStream(memoryStream = new MemoryStream(), encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();

                            var finalResult = new byte[saltStringBytes.Length + ivStringBytes.Length + memoryStream.Length];

                            Buffer.BlockCopy(saltStringBytes, 0, finalResult, 0, saltStringBytes.Length);
                            Buffer.BlockCopy(ivStringBytes, 0, finalResult, saltStringBytes.Length, ivStringBytes.Length);
                            Buffer.BlockCopy(memoryStream.ToArray(), 0, finalResult, saltStringBytes.Length + ivStringBytes.Length, Convert.ToInt32(memoryStream.Length));

                            return Convert.ToBase64String(finalResult);
                        }
                    }
                }
            }
        }

        public static string DecryptString(this string cipherText, string passPhrase)
        {
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            var saltStringBytes = new byte[Keysize / 8];
            var ivStringBytes = new byte[Keysize / 8];
            var cipherTextBytes = new byte[cipherTextBytesWithSaltAndIv.Length - Keysize / 8 * 2];

            Buffer.BlockCopy(cipherTextBytesWithSaltAndIv, 0, saltStringBytes, 0, Keysize / 8);
            Buffer.BlockCopy(cipherTextBytesWithSaltAndIv, Keysize / 8, ivStringBytes, 0, Keysize / 8);
            Buffer.BlockCopy(cipherTextBytesWithSaltAndIv, Keysize / 8 * 2, cipherTextBytes, 0, cipherTextBytesWithSaltAndIv.Length - Keysize / 8 * 2);

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, Derivationiterations))
            {
                using (var symmetricKey = new RijndaelManaged())
                {
                    var keyBytes = password.GetBytes(Keysize / 8);

                    symmetricKey.BlockSize = Keysize;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;

                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var cryptoStream = new CryptoStream(new MemoryStream(cipherTextBytes), decryptor, CryptoStreamMode.Read))
                        {
                            var plainTextBytes = new byte[cipherTextBytes.Length];
                            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
        }

        private static byte[] GenerateRandomEntropy()
        {
            var randomBytes = new byte[Keysize / 8];

            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
                return randomBytes;
            }
        }
    }
}