using System.Security.Cryptography;
using System.Text;

namespace SmartCityWebApi.Extensions
{
    public static class StringExtension
    {

        /// <summary>
        /// Sha256加密
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static string ToSha256Encrypt(this string strData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(strData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string ToMd5(this string source) 
        {
            using (var md5 = MD5.Create())
            {
               return string.Concat(md5.ComputeHash(Encoding.UTF8.GetBytes(source))
                  .Select(x => x.ToString("x2")));
            }

        }
    }
}
