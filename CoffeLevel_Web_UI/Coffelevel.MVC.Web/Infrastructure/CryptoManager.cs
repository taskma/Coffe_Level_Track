using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace CoffeLevel.Client.Web.Infrastructure
{
    public interface ICryptoManager
    {
        string Encrypt(string text);
        string Decrypt(string text);
    }
    public class CryptoManager: ICryptoManager
    {
     
           private const string AesKey = @"5gTR+l)kL08'!f6g}YUvG6_78?gTZxM7";
           private const string AesIV = @"g=F45e!gqT97=Gh6";

           /// AES Encryption

           public string Encrypt(string text)
           {
               // AesCryptoServiceProvider
               AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
               aes.BlockSize = 128;
               aes.KeySize = 256;
               aes.IV = Encoding.UTF8.GetBytes(AesIV);
               aes.Key = Encoding.UTF8.GetBytes(AesKey);
               aes.Mode = CipherMode.CBC;
               aes.Padding = PaddingMode.PKCS7;

               // Convert string to byte array
               byte[] src = Encoding.Unicode.GetBytes(text);

               // encryption
               using (ICryptoTransform encrypt = aes.CreateEncryptor())
               {
                   byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);

                   // Convert byte array to Base64 strings
                   return Convert.ToBase64String(dest);
               }
           }

           /// <summary>
           /// AES decryption
           /// </summary>
           public string Decrypt(string text)
           {
               // AesCryptoServiceProvider
               AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
               aes.BlockSize = 128;
               aes.KeySize = 256;
               aes.IV = Encoding.UTF8.GetBytes(AesIV);
               aes.Key = Encoding.UTF8.GetBytes(AesKey);
               aes.Mode = CipherMode.CBC;
               aes.Padding = PaddingMode.PKCS7;

               // Convert Base64 strings to byte array
               byte[] src = System.Convert.FromBase64String(text);

               // decryption
               using (ICryptoTransform decrypt = aes.CreateDecryptor())
               {
                   byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                   return Encoding.Unicode.GetString(dest);
               }
           }

        //public string Encrypt(string value, string passKey)
        //{

        //    List<char> chars = value.ToCharArray().ToList();


        //    for (int i = 0; i < chars.Length; i++)
        //    {
        //        byte bytee;
        //        bytee = Convert.ToByte(chars[i]);
        //        bytee += 5;
        //        giden += bytee.ToString() + "-";
        //    }

        //    giden = giden.Substring(0, giden.Length - 1);
        //    return giden;


        //}

        //public string Decrypt(string value, string passKey)
        //{
        //    throw new NotImplementedException();
        //}


        //public string Encrypt(string input, string key)
        //{
        //    byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
        //    TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
        //    tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
        //    tripleDES.Mode = CipherMode.ECB;
        //    tripleDES.Padding = PaddingMode.PKCS7;
        //    ICryptoTransform cTransform = tripleDES.CreateEncryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
        //    tripleDES.Clear();
        //    return Convert.ToBase64String(resultArray, 0, resultArray.Length);


        //}

        //public string Decrypt(string input, string key)
        //{
        //    byte[] inputArray = Convert.FromBase64String(input);
        //    TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
        //    tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
        //    tripleDES.Mode = CipherMode.ECB;
        //    tripleDES.Padding = PaddingMode.PKCS7;
        //    ICryptoTransform cTransform = tripleDES.CreateDecryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
        //    tripleDES.Clear();
        //    return UTF8Encoding.UTF8.GetString(resultArray);




        //}

    }
}