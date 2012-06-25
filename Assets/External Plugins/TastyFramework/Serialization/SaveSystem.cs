/*
 * SaveSystem class
 * 
 * Used for saving a save game object to the player prefs.
 * Works by using the standard XML serializer to walk through the object hierachy and serialize to and from text
 * Data is then optionally encrypted and then encoded to base64 for storage
 * 
 * The metadata structure stores date, version, encryption scheme etc of each save
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Reflection;
using System.Text;
using System.IO;

namespace Framework
{
	public static class SaveSystem
	{
		public const int SaveGameVersion = 1;
		public const EncryptionScheme defaultEncryption = EncryptionScheme.XorKey;
		public const CompressionScheme defaultCompression = CompressionScheme.None;
		
		public enum EncryptionScheme
		{
			None = 0,
			XorKey = 1
		}
		
		public enum CompressionScheme
		{
			None = 0
		}
		
		public struct SaveMetaData
		{
			public static SaveMetaData Create()
			{
				SaveMetaData data = new SaveMetaData();
				data.version = SaveSystem.SaveGameVersion;
				data.dateSaved = System.DateTime.Now.ToBinary();
				data.compressionScheme = defaultCompression;
				data.encryptionScheme = defaultEncryption;
				data.reserved1 = 0;
				data.reserved2 = 0;
				return data;
			}
			
			public void DebugPrint()
			{
				Debug.Log("Date created: " + System.DateTime.FromBinary(dateSaved).ToString() + " Save version: " + version + " Compression scheme: " + compressionScheme + " Encryption scheme: " + encryptionScheme);
			}
			
			public int version;
			public long dateSaved;			
			public EncryptionScheme encryptionScheme;
			public CompressionScheme compressionScheme;
			public int reserved1;
			public int reserved2;
		}
		
		private const string _saveDataKey = "SAVE_SLOT";
		private const string _invalidData = "INVALID_DATA";
		private const System.UInt32 _xorKey = 0x54504721;		// TPG!
		
		private static string GetSaveSlotKey(int saveSlot)
		{
			return _saveDataKey + saveSlot;
		}
		
		public static bool SaveSlotUsed(int saveSlot)
		{
			return PlayerPrefs.HasKey(GetSaveSlotKey(saveSlot));
		}
		
		private static void EncodeDecode(byte[] data, EncryptionScheme scheme)
		{
			switch(scheme)
			{
			case EncryptionScheme.None:
				return;
			case EncryptionScheme.XorKey:
				// The most ridiculously simple encryption scheme, just XORs with a private key
				for(int i = 0; i<data.Length; i++)
				{
					int shift = (i % 8);
					byte key = (byte)((_xorKey & (0xFF << (shift * 8))) >> (shift * 8));
					
					data[i] = (byte) (data[i] ^ key);
				}
				return;
			}
		}
		
		public static void SaveGameData<T> (int saveSlot, T obj) where T : class
		{
			if(obj == null)
			{
				Debug.LogError("Cannot save a null object!");
				return;
			}
			
			// Create meta data
			SaveMetaData metaData = SaveMetaData.Create();
			byte[] rawMetaData = StructSerializer.ToBytes<SaveMetaData>(metaData);
			
			// Serialize to an XML file
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			StringWriter writer = new StringWriter();
			
			serializer.Serialize(writer, obj);
			
			string serialized = writer.ToString();
			
			// Encode to a byte array
			ASCIIEncoding encoding = new ASCIIEncoding();
			byte[] rawData = encoding.GetBytes(serialized);
			
			// Perform encryption
			EncodeDecode(rawData, metaData.encryptionScheme);
			
			// Concat metadata + encrypted XML data
			byte[] saveData = new byte[rawMetaData.Length + rawData.Length];
			rawMetaData.CopyTo(saveData, 0);
			rawData.CopyTo(saveData, rawMetaData.Length);
			
			// Save to player prefs as base64 string
			PlayerPrefs.SetString(GetSaveSlotKey(saveSlot), System.Convert.ToBase64String(saveData));
			PlayerPrefs.Save();
		}
		
			
		public static T LoadGameData<T>(int saveSlot) where T : class
		{
			if(!SaveSlotUsed(saveSlot))
			{
				return null;
			}
			
			// Get base64 encoded data
			string base64Encoded = PlayerPrefs.GetString(GetSaveSlotKey(saveSlot), _invalidData);
			
			if(base64Encoded == _invalidData)
			{
				return null;
			}
			
			byte[] rawData;
			
			try
			{
				rawData = System.Convert.FromBase64String(base64Encoded);
			}
			catch(System.FormatException fe)
			{
				Debug.LogError(fe);
				return null;
			}
		
			// Retrieve metadata
			SaveMetaData metaData;
			
			long bytesRead = StructSerializer.ToStruct<SaveMetaData>(rawData, out metaData);
			
			// Separate meta data + save data
			byte[] saveData = new byte[rawData.Length - bytesRead];
			System.Array.Copy(rawData, bytesRead, saveData, 0, saveData.Length);
			
			// Perform decryption
			EncodeDecode(saveData, metaData.encryptionScheme);

			// Convert to ASCII and parse XML tree
			try
			{
				string xmlData = ASCIIEncoding.ASCII.GetString(saveData);
				
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				StringReader reader = new StringReader(xmlData);
					
				return (T) serializer.Deserialize(reader);
			}
			catch(System.Xml.XmlException xmlException)
			{
				Debug.LogError(xmlException);
				return null;
			}
			catch(System.InvalidOperationException ioe)
			{
				Debug.LogError(ioe);
				return null;
			}
		}
	}
}
