﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;
using System.IO;

namespace Crypto
{
	public static class CryptoManager
	{

		public static string ComputeHash( string plainText, string hashAlgorithm )
		{
			return ComputeHash( plainText, hashAlgorithm, null );
		}

		/// <summary>
		/// Generates a hash for the given plain text value and returns a
		/// base64-encoded result. Before the hash is computed, a random salt
		/// is generated and appended to the plain text. This salt is stored at
		/// the end of the hash value, so it can be used later for hash
		/// verification.
		/// </summary>
		/// <param name="plainText">
		/// Plaintext value to be hashed. The function does not check whether
		/// this parameter is null.
		/// </param>
		/// <param name="hashAlgorithm">
		/// Name of the hash algorithm. Allowed values are: "MD5", "SHA1",
		/// "SHA256", "SHA384", and "SHA512" (if any other value is specified
		/// MD5 hashing algorithm will be used). This value is case-insensitive.
		/// </param>
		/// <param name="saltBytes">
		/// Salt bytes. This parameter can be null, in which case a random salt
		/// value will be generated.
		/// </param>
		/// <returns>
		/// Hash value formatted as a base64-encoded string.
		/// </returns>
		public static string ComputeHash( string plainText, string hashAlgorithm, byte[] saltBytes )
		{
			// If salt is not specified, generate it on the fly.
			if( saltBytes == null )
			{
				// Define min and max salt sizes.
				int minSaltSize = 4;
				int maxSaltSize = 8;

				// Generate a random number for the size of the salt.
				Random random = new Random();
				int saltSize = random.Next( minSaltSize, maxSaltSize );

				// Allocate a byte array, which will hold the salt.
				saltBytes = new byte[saltSize];

				// Initialize a random number generator.
				RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

				// Fill the salt with cryptographically strong byte values.
				rng.GetNonZeroBytes( saltBytes );
			}

			// Convert plain text into a byte array.
			byte[] plainTextBytes = Encoding.UTF8.GetBytes( plainText );

			// Allocate array, which will hold plain text and salt.
			byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + saltBytes.Length];

			// Copy plain text bytes into resulting array.
			for( int i = 0; i < plainTextBytes.Length; i++ )
				plainTextWithSaltBytes[i] = plainTextBytes[i];

			// Append salt bytes to the resulting array.
			for( int i = 0; i < saltBytes.Length; i++ )
				plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

			// Because we support multiple hashing algorithms, we must define
			// hash object as a common (abstract) base class. We will specify the
			// actual hashing algorithm class later during object creation.
			HashAlgorithm hash;

			// Make sure hashing algorithm name is specified.
			if( hashAlgorithm == null )
				hashAlgorithm = "";

			// Initialize appropriate hashing algorithm class.
			switch( hashAlgorithm.ToUpper() )
			{
				case "SHA1":
					hash = new SHA1Managed();
					break;

				case "SHA256":
					hash = new SHA256Managed();
					break;

				case "SHA384":
					hash = new SHA384Managed();
					break;

				case "SHA512":
					hash = new SHA512Managed();
					break;

				default:
					hash = new MD5CryptoServiceProvider();
					break;
			}

			// Compute hash value of our plain text with appended salt.
			byte[] hashBytes = hash.ComputeHash( plainTextWithSaltBytes );

			// Create array which will hold hash and original salt bytes.
			byte[] hashWithSaltBytes = new byte[hashBytes.Length + saltBytes.Length];

			// Copy hash bytes into resulting array.
			for( int i = 0; i < hashBytes.Length; i++ )
			{
				hashWithSaltBytes[i] = hashBytes[i];
			}

			// Append salt bytes to the result.
			for( int i = 0; i < saltBytes.Length; i++ )
			{
				hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];
			}

			// Convert result into a base64-encoded string.
			string hashValue = Convert.ToBase64String( hashWithSaltBytes );

			// Return the result.
			return hashValue;
		}

		/// <summary>
		/// Compares a hash of the specified plain text value to a given hash
		/// value. Plain text is hashed with the same salt value as the original
		/// hash.
		/// </summary>
		/// <param name="plainText">
		/// Plain text to be verified against the specified hash. The function
		/// does not check whether this parameter is null.
		/// </param>
		/// <param name="hashAlgorithm">
		/// Name of the hash algorithm. Allowed values are: "MD5", "SHA1", 
		/// "SHA256", "SHA384", and "SHA512" (if any other value is specified,
		/// MD5 hashing algorithm will be used). This value is case-insensitive.
		/// </param>
		/// <param name="hashValue">
		/// Base64-encoded hash value produced by ComputeHash function. This value
		/// includes the original salt appended to it.
		/// </param>
		/// <returns>
		/// If computed hash mathes the specified hash the function the return
		/// value is true; otherwise, the function returns false.
		/// </returns>
		public static bool VerifyHash( string plainText, string hashAlgorithm, string hashValue )
		{
			// Convert base64-encoded hash value into a byte array.
			//byte[] hashWithSaltBytes = Convert.FromBase64String( hashValue );
			byte[] hashWithSaltBytes = Convert.FromBase64String(hashValue);

			// We must know size of hash (without salt).
			int hashSizeInBits, hashSizeInBytes;

			// Make sure that hashing algorithm name is specified.
			if( hashAlgorithm == null )
				hashAlgorithm = "";

			// Size of hash is based on the specified algorithm.
			switch( hashAlgorithm.ToUpper() )
			{
				case "SHA1":
					hashSizeInBits = 160;
					break;

				case "SHA256":
					hashSizeInBits = 256;
					break;

				case "SHA384":
					hashSizeInBits = 384;
					break;

				case "SHA512":
					hashSizeInBits = 512;
					break;

				default: // Must be MD5
					hashSizeInBits = 128;
					break;
			}

			// Convert size of hash from bits to bytes.
			hashSizeInBytes = hashSizeInBits / 8;

			// Make sure that the specified hash value is long enough.
			if( hashWithSaltBytes.Length < hashSizeInBytes )
			{
				return false;
			}

			// Allocate array to hold original salt bytes retrieved from hash.
			byte[] saltBytes = new byte[hashWithSaltBytes.Length - hashSizeInBytes];

			// Copy salt from the end of the hash to the new array.
			for( int i = 0; i < saltBytes.Length; i++ )
			{
				saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];
			}

			// Compute a new hash string.
			string expectedHashString = ComputeHash( plainText, hashAlgorithm, saltBytes );

			// If the computed hash matches the specified hash,
			// the plain text value must be correct.
			return ( hashValue == expectedHashString );
		}
	}
}