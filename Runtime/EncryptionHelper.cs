using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace com.zumstudios.carriersubscription
{
    public static class EncryptionHelper
    {
        public static string Encrypt(string value)
        {
            string keyword = "AN65Bef2sW6n";
            string salt = CalculateSalt(keyword);
            string result = CalculateHash(salt, value);
            return result;
        }

        private static string CalculateSalt(string keyword)
        {
            string keywordHash = CalculateSHA1(keyword);
            string salt = CalculateMD5(keywordHash.Substring(3, 24));
            return salt;
        }

        private static string CalculateHash(string salt, string value)
        {
            string concatenatedString = $"{salt.Substring(0, 10)}{value}{salt.Substring(10, 22)}";
            string hash = CalculateSHA1(concatenatedString);
            return hash;
        }

        private static string CalculateSHA1(string input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private static string CalculateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
