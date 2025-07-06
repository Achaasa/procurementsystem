using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace procurementsystem.utils
{
    public static class PasswordGenerator
    {
        public static string Generate(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_+=";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(chars => chars[random.Next(chars.Length)]).ToArray());
        }
    }
}