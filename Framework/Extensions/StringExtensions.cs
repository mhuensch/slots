using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Foogenda.Framework
{
	public static class StringExtensions
	{
		public static bool HasValue(this string source)
		{
			return string.IsNullOrWhiteSpace(source) == false;
		}

		public static bool IsEmpty(this string source)
		{
			return string.IsNullOrWhiteSpace(source);
		}

		public static string Slugify(this string phrase)
		{
			byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(phrase);
			var str = Encoding.ASCII.GetString(bytes).ToLower();
			str = Regex.Replace(str, @"[^a-z0-9\s-]", "");	// Remove all non valid chars
			str = Regex.Replace(str, @"\s+", " ").Trim();		// Convert multiple spaces into one space
			str = Regex.Replace(str, @"\s", "-");						// Replace spaces by dashes
			return str;
		}

		public static string GetHash(this string input)
		{
			var hashAlgorithm = new SHA256CryptoServiceProvider();

			byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

			byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

			return Convert.ToBase64String(byteHash);
		}
	}
}
