using System;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
// ReSharper disable JoinDeclarationAndInitializer

namespace BedtimeCore.NestBuilder
{
	public static class Secrets
	{
		public static bool TryGetValue(string key, out string value)
		{
			value = string.Empty;
			
			if (string.IsNullOrEmpty(key))
			{
				return false;
			}

			string output;
			#if BEDTIME_COMMANDLINE
			if (BedtimeCore.Utility.CommandLineArgumentUtility.TryGetValueAfterArgument(key, out output))
			{
				value = output;
				return true;
			}
			#endif
			
			output = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
			if (!string.IsNullOrEmpty(output))
			{
				value = output;
				return true;
			}
			
			if (TryGetLocal(key, out output))
			{
				value = output;
				return true;
			}

			return false;
		}

		public static bool TryGetLocal(string key, out string value)
		{
			value = EditorPrefs.GetString(key, string.Empty);
			bool success = !string.IsNullOrEmpty(value);
			if (success)
			{
				try
				{
					value = Decrypt(value);
				}
				catch (CryptographicException)
				{
					success = false;
				}
			}
			
			return success;
		}
		
		public static bool SetLocal(string key, string value)
		{
			if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
			{
				return false;
			}

			var output = Encrypt(value);
			EditorPrefs.SetString(key, output);
			return true;
		}
		
		//This is not meant as actual security. It's simply to not save anything in plain text
		private const string KEY = "bebtimedimgitalgams";
		private static TripleDESCryptoServiceProvider GetCryptoProvider()
		{
			var md5 = new MD5CryptoServiceProvider();
			var key = md5.ComputeHash(Encoding.UTF8.GetBytes(KEY));
			return new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
		}

		public static string Encrypt(string plainString)
		{
			var data = Encoding.UTF8.GetBytes(plainString);
			var tripleDes = GetCryptoProvider();
			var transform = tripleDes.CreateEncryptor();
			var resultsByteArray = transform.TransformFinalBlock(data, 0, data.Length);
			return Convert.ToBase64String(resultsByteArray);
		}

		public static string Decrypt(string encryptedString)
		{
			var data = Convert.FromBase64String(encryptedString);
			var tripleDes = GetCryptoProvider();
			var transform = tripleDes.CreateDecryptor();
			var resultsByteArray = transform.TransformFinalBlock(data, 0, data.Length);
			return Encoding.UTF8.GetString(resultsByteArray);
		}
	}
}