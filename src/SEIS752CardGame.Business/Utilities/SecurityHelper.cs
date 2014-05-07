using SEIS752CardGame.Business.Services;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SEIS752CardGame.Business.Utilities
{
	public class SecurityHelper
	{
		#region ENCRYPTION/DECRYPTION

		public string Encrypt(String text)
		{
			var encryptConfig = ConfigurationService.Instance.GetEncryptionConfig();

			var result = Encrypt(text, encryptConfig.Password, encryptConfig.Salt, encryptConfig.Hash, encryptConfig.Iterations,
				encryptConfig.IV, encryptConfig.KeySize);

			return result;
		}

		public string Decrypt(String encryptedText)
		{
			var encryptConfig = ConfigurationService.Instance.GetEncryptionConfig();

			var result = Decrypt(encryptedText, encryptConfig.Password, encryptConfig.Salt, encryptConfig.Hash, encryptConfig.Iterations,
				encryptConfig.IV, encryptConfig.KeySize);

			return result;
		}

		private string Encrypt(String text, String password, string salt, string hash, int iterations, string iv, int keySize)
		{
			try
			{
				byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(iv);
				byte[] SaltValueBytes = Encoding.ASCII.GetBytes(salt);
				byte[] PlainTextBytes = Encoding.UTF8.GetBytes(text);
				PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(password, SaltValueBytes, hash, iterations);
				byte[] KeyBytes = DerivedPassword.GetBytes(keySize/8);
				RijndaelManaged SymmetricKey = new RijndaelManaged();
				SymmetricKey.Mode = CipherMode.CBC;
				ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes);
				MemoryStream MemStream = new MemoryStream();
				CryptoStream cryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write);
				cryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);
				cryptoStream.FlushFinalBlock();
				byte[] CipherTextBytes = MemStream.ToArray();

				// Clean up
				MemStream.Close();
				cryptoStream.Close();
				MemStream.Dispose();
				cryptoStream.Dispose();
				Encryptor.Dispose();

				return Convert.ToBase64String(CipherTextBytes);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private string Decrypt(string encryptedText, string password, string salt, string hash, int iterations, string IV,
			int keySize)
		{
			try
			{
				byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(IV);
				byte[] SaltValueBytes = Encoding.ASCII.GetBytes(salt);
				byte[] CipherTextBytes = Convert.FromBase64String(encryptedText);
				PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(password, SaltValueBytes, hash, iterations);
				byte[] KeyBytes = DerivedPassword.GetBytes(keySize/8);
				RijndaelManaged SymmetricKey = new RijndaelManaged();
				SymmetricKey.Mode = CipherMode.CBC;
				ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes);
				MemoryStream MemStream = new MemoryStream(CipherTextBytes);
				CryptoStream cryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read);
				byte[] PlainTextBytes = new byte[CipherTextBytes.Length];
				int ByteCount = cryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);

				// Clean up
				MemStream.Close();
				cryptoStream.Close();
				MemStream.Dispose();
				cryptoStream.Dispose();
				Decryptor.Dispose();

				return Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		#endregion
	}
}
