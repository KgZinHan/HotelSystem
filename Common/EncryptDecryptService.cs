using System.Security.Cryptography;
using System.Text;

namespace Hotel_Core_MVC_V1.Common
{
    public class EncryptDecryptService
    {
        public string EncryptString(string plainInput)
        {
            string key = "tkbH1omfiqg13aqVusoCialf7pE6whfU";
            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainInput);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public string DecryptString(string cipherText)
        {
            string key = "tkbH1omfiqg13aqVusoCialf7pE6whfU";
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}

//{
//public string EncryptString(string text, string keyString)
//{
//    var key = Encoding.UTF8.GetBytes(keyString);

//    using (var aesAlg = Aes.Create())
//    {
//        using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
//        {
//            using (var msEncrypt = new MemoryStream())
//            {
//                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
//                using (var swEncrypt = new StreamWriter(csEncrypt))
//                {
//                    swEncrypt.Write(text);
//                }

//                var iv = aesAlg.IV;

//                var decryptedContent = msEncrypt.ToArray();

//                var result = new byte[iv.Length + decryptedContent.Length];

//                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
//                Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

//                return Convert.ToBase64String(result);
//            }
//        }
//    }
//}

//public string DecryptString(string cipherText, string keyString)
//{
//    var fullCipher = Convert.FromBase64String(cipherText);

//    var iv = new byte[16];
//    var cipher = new byte[16];

//    Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
//    Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
//    var key = Encoding.UTF8.GetBytes(keyString);

//    using (var aesAlg = Aes.Create())
//    {
//        using (var decryptor = aesAlg.CreateDecryptor(key, iv))
//        {
//            string result;
//            using (var msDecrypt = new MemoryStream(cipher))
//            {
//                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
//                {
//                    using (var srDecrypt = new StreamReader(csDecrypt))
//                    {
//                        result = srDecrypt.ReadToEnd();
//                    }
//                }
//            }

//            return result;
//        }
//    }
//}
