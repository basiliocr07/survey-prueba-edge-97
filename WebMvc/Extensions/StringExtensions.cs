
using System;

namespace System
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Length > 1 
                ? char.ToUpper(input[0]) + input.Substring(1) 
                : input.ToUpper();
        }
    }
}
