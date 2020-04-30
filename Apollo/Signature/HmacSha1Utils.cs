using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Com.Ctrip.Framework.Apollo.Signature
{
    public class HmacSha1Utils
    {
        public static string SignString(string data, string secret)
        {
            try
            {
                var byteData = Encoding.UTF8.GetBytes(data);
                var byteKey = Encoding.UTF8.GetBytes(secret);
                using var hmac = new HMACSHA1(byteKey);
                return Convert.ToBase64String(hmac.ComputeHash(byteData));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}
