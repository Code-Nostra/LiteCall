using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Infrastructure
{
    internal static class PasswordExtensions
    {
        internal static string GetSha1(this string content)
        {
            byte[] hash;
            if (string.IsNullOrEmpty(content)) return string.Empty;
            
            using var sha1 = new SHA1Managed();
            hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(content));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }
    }
}
