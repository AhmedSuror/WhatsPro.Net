using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WhatsPro.Security;

/// <summary>
/// Handles AES encryption and decryption of payloads using CryptoJS compatibility.
/// </summary>
public static class PayloadEncryptor
{
    /// <summary>
    /// Encrypts the specified plain text using the given password.
    /// </summary>
    /// <param name="plainText">The plain text to encrypt.</param>
    /// <param name="password">The password used for encryption.</param>
    /// <returns>The encrypted cipher text.</returns>
    public static string Encrypt(string plainText, string password)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;
            
        byte[] salt = new byte[8];
#if NETSTANDARD2_0 || NET48
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
#else
        RandomNumberGenerator.Fill(salt);
#endif

        DeriveKeyAndIv(password, salt, out byte[] key, out byte[] iv);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (MemoryStream ms = new MemoryStream())
            {
                // Write 'Salted__' (8 bytes)
                ms.Write(Encoding.ASCII.GetBytes("Salted__"), 0, 8);
                // Write salt (8 bytes)
                ms.Write(salt, 0, 8);

                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    /// <summary>
    /// Decrypts the specified cipher text using the given password.
    /// </summary>
    /// <param name="cipherText">The cipher text to decrypt.</param>
    /// <param name="password">The password used for decryption.</param>
    /// <returns>The decrypted plain text.</returns>
    public static string Decrypt(string cipherText, string password)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;
            
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        if (cipherBytes.Length < 16)
            throw new CryptographicException("Invalid cipher text format.");

        // Check for 'Salted__' prefix
        string magic = Encoding.ASCII.GetString(cipherBytes, 0, 8);
        if (magic != "Salted__")
            throw new CryptographicException("Invalid cipher text prefix.");

        byte[] salt = new byte[8];
        Array.Copy(cipherBytes, 8, salt, 0, 8);

        DeriveKeyAndIv(password, salt, out byte[] key, out byte[] iv);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (MemoryStream ms = new MemoryStream(cipherBytes, 16, cipherBytes.Length - 16))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }

    private static void DeriveKeyAndIv(string password, byte[] salt, out byte[] key, out byte[] iv)
    {
        byte[] passBytes = Encoding.UTF8.GetBytes(password);
        byte[] hash = new byte[0];
        byte[] keyAndIv = new byte[48]; // 32 bytes for key, 16 bytes for iv

        using (MD5 md5 = MD5.Create())
        {
            int currentPos = 0;
            while (currentPos < 48)
            {
                byte[] toHash = new byte[hash.Length + passBytes.Length + salt.Length];
                Buffer.BlockCopy(hash, 0, toHash, 0, hash.Length);
                Buffer.BlockCopy(passBytes, 0, toHash, hash.Length, passBytes.Length);
                Buffer.BlockCopy(salt, 0, toHash, hash.Length + passBytes.Length, salt.Length);

                hash = md5.ComputeHash(toHash);

                int remaining = 48 - currentPos;
                int toCopy = Math.Min(hash.Length, remaining);

                Buffer.BlockCopy(hash, 0, keyAndIv, currentPos, toCopy);
                currentPos += toCopy;
            }
        }

        key = new byte[32];
        iv = new byte[16];

        Buffer.BlockCopy(keyAndIv, 0, key, 0, 32);
        Buffer.BlockCopy(keyAndIv, 32, iv, 0, 16);
    }
}
