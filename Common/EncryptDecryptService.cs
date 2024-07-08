using System.Security.Cryptography;
using System.Text;

namespace Hotel_Core_MVC_V1.Common
{
    public class EncryptDecryptService
    {
        public string EncryptString(string plainInput)
        {
            var key = "tkbH1omfiqg13aqVusoCialf7pE6whfU";
            var iv = new byte[16];
            byte[] array;
            using var aes = Aes.Create();

            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using (var streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(plainInput);
            }

            array = memoryStream.ToArray();


            return Convert.ToBase64String(array);
        }

        public string DecryptString(string cipherText)
        {
            var key = "tkbH1omfiqg13aqVusoCialf7pE6whfU";
            var iv = new byte[16];
            var buffer = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream(buffer);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);

            return streamReader.ReadToEnd();
        }
    }
}

