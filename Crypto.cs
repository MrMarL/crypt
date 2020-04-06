using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WindowsFormsApplication9
{
    class Crypto
    {
        private static readonly byte[] Salt = Encoding.ASCII.GetBytes("o6806642kbM7c5");

        /// <summary>
        /// Зашифровывает данную строку, используя AES. Строка может быть расшифрована с помощью 
        /// DecryptStringAES().  Параметр sharedSecret должны совпадать.
        /// </summary>
        /// <param name="plainText">Текст для шифрования.</param>
        /// <param name="sharedSecret">Пароль, используемый для генерации ключа для шифрования.</param>
        public static string EncryptStringAES(string plainText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            string outStr;                       // Зашифрованная строка для return
            RijndaelManaged aesAlg = null;              // Объект RijndaelManaged используется для шифрования данных.

            try
            {
                // Генерация ключ из sharedSecret и Salt.
                var key = new Rfc2898DeriveBytes(sharedSecret, Salt);

                // Создание объекта RijndaelManaged
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Создание расшифровщика для преобразования потока.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Создание потоков, используемых для шифрования.
                using (var msEncrypt = new MemoryStream())
                {
                    // подготовка IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Запись всех данных в поток.
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Очистка RijndaelManaged.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Возвращает зашифрованные байты из потока.
            return outStr;
        }

        /// <summary>
        /// Расшифровывает данную строку. Предполагается, что строка была зашифрована с использованием
        /// EncryptStringAES(), используя идентичный sharedSecret.
        /// </summary>
        /// <param name="cipherText">Текст для расшифровки.</param>
        /// <param name="sharedSecret">Пароль, используемый для генерации ключа расшифровки.</param>
        public static string DecryptStringAES(string cipherText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            // Объявление объекта RijndaelManaged
            // используемого для расшифровки данных.
            RijndaelManaged aesAlg = null;

            // Объявление строки, используемой для хранения
            // расшифрованного текста.
            string plaintext;

            try
            {
                // генерирует ключ из sharedSecret и Salt
                var key = new Rfc2898DeriveBytes(sharedSecret, Salt);

                // Создание потоков, используемых для расшифровки.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (var msDecrypt = new MemoryStream(bytes))
                {
                    // Создание объекта RijndaelManaged
                    // с указанным ключом и IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Получение вектора инициализации из зашифрованного потока
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Создание дешифратора для выполнения потокового преобразования.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                            // Чтение дешифрованных байтов из потока дешифрования
                            // и помещение их в строку.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Очистка RijndaelManaged.
                if (aesAlg != null)
                    aesAlg.Clear();
            }
            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            var rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) == rawLength.Length)
            {
                var buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
                if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
                {
                    throw new SystemException("Не правильно прочитан byte массив");
                }
                return buffer;
            }
            throw new SystemException("Поток не содержит правильно отформатированный byte массив");
        }
    }
}