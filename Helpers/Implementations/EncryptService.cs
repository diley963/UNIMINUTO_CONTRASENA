using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Implementations
{
    public class EncryptService
    {
        #region METHODS

        /// <summary>
        /// Decryption method
        /// </summary>
        /// <param name="key">Decryption key from database</param>
        /// <param name="cipherText">The text to be decrypted</param>
        /// <returns></returns>
        public string DecryptString(string key, string cipherText)
        {

            // Byte Array iv
            byte[] iv = new byte[16];
            // Retrieve the value to decrypted in byte array (Buffer)
            byte[] buffer = Convert.FromBase64String(cipherText);

            // Aes --> To manage encryption
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                // Decrypt using the decryption key and the value
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Save the buffer in a MemoryStream
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

        /// <summary>
        /// Encryption method
        /// </summary>
        /// <param name="key">Encryption key</param>
        /// <param name="plainText">Text to encrypt</param>
        /// <returns></returns>
        public string EncryptString(string key, string plainText)
        {
            // Byte Array iv
            byte[] iv = new byte[16];
            byte[] array;

            // Aes --> To manage encryption
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
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }

        #endregion

    }
}
