using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public static class TextureImportSettings 
{
#region Generic texture importer modification methods
	public delegate bool ModifyDelegate(TextureImporter importer);
	
	public static int ModifySelectedTextures(string processDescription, ModifyDelegate modify)
	{
		List<TextureImporter> importers = new List<TextureImporter>();
	
		Object[] textures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);

		if(textures.Length == 0)
		{
			if (!EditorUtility.DisplayDialog(processDescription,
                    "You did not select any textures. Do you want to apply this to all textures in the project?",
                    "Apply", "Cancel"))
        	{
            	return 0;
        	}
			
			AddTextureImportersToList(importers, "*.png");
			AddTextureImportersToList(importers, "*.psd");
			AddTextureImportersToList(importers, "*.bmp");
			AddTextureImportersToList(importers, "*.jpg");
		}
		else
		{
			foreach(Object texture in textures)
			{
				TextureImporter importer = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
				
				if(importer != null)
				{
					importers.Add(importer);
				}
			}
		}
		
		if(importers.Count > 10) 
		{
            if (!EditorUtility.DisplayDialog(processDescription,
                    "This will affect " + importers.Count + " textures. Continue?",
                    "Apply", "Cancel"))
            {
                return 0;
            }
        }
		
		if(importers.Count == 0)
		{
			EditorUtility.DisplayDialog(processDescription, "No textures found to process", "Ok");
			return 0;
		}
		
		int count = 0;
		int modificationsMade = 0;
		
		foreach(TextureImporter importer in importers)
		{
			EditorUtility.DisplayProgressBar(processDescription, "Processing " + importer.assetPath, (float)(count++) / (float)importers.Count);
			
			if(modify(importer))
			{
				AssetDatabase.ImportAsset(importer.assetPath);
				modificationsMade ++;
			}
		}
		
		EditorUtility.ClearProgressBar();
		
		return modificationsMade;
	}
	
	static void AddTextureImportersToList(List<TextureImporter> importers, string extension)
	{
		string[] filenames = DeepSearchAssets(extension);
		
		foreach(string path in filenames)
		{
			TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
			if(importer != null)
			{
				importers.Add(importer);
			}
		}
	}
	
	static string[] DeepSearchAssets(string extension)
	{
		string[] filenames = System.IO.Directory.GetFiles(".", extension, System.IO.SearchOption.AllDirectories);
		
		for(int n = 0; n<filenames.Length; n++)
        {
            filenames[n] = filenames[n].Replace(@"\", "/");
            filenames[n] = filenames[n].Replace("./", "");
        }
        return filenames;
	}
#endregion
	
#region Mipmapping
	[MenuItem("Custom/Textures/Mipmapping/Enable")]
	public static void EnableMipmapping()
	{
		ModifySelectedTextures("Enable Mipmapping", (TextureImporter importer) => {
			if(!importer.mipmapEnabled)
			{
				importer.mipmapEnabled = true;
				return true;
			}
			return false;
		});
	}

	[MenuItem("Custom/Textures/Mipmapping/Disable")]
	public static void DisableMipmapping()
	{
		ModifySelectedTextures("Disable Mipmapping", (TextureImporter importer) => {
			if(importer.mipmapEnabled)
			{
				importer.mipmapEnabled = false;
				return true;
			}
			return false;
		});
	}
#endregion
	
#region Maximum size
	static ModifyDelegate SetMaximumTextureSize(int size)
	{
		return (TextureImporter importer) => {
			if(importer.maxTextureSize != size)
			{
				importer.maxTextureSize = size;
				return true;
			}
			return false;
		};
	}
	
	[MenuItem("Custom/Textures/Set maximum texture size/1024")]
	public static void SetMaximumTextureSize1024()
	{
		ModifySelectedTextures("Set maximum texture size", SetMaximumTextureSize(1024));
	}

	[MenuItem("Custom/Textures/Set maximum texture size/512")]
	public static void SetMaximumTextureSize512()
	{
		ModifySelectedTextures("Set maximum texture size", SetMaximumTextureSize(512));
	}

	[MenuItem("Custom/Textures/Set maximum texture size/256")]
	public static void SetMaximumTextureSize256()
	{
		ModifySelectedTextures("Set maximum texture size", SetMaximumTextureSize(256));
	}
#endregion

#region Platform overrides
	static ModifyDelegate EnablePlatformOverride(string platform)
	{
		return (TextureImporter importer) => {
			int maxTextureSize;
			int compressionQuality;
			TextureImporterFormat textureFormat;
			
			if(!importer.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat, out compressionQuality))
			{
				importer.SetPlatformTextureSettings(platform, importer.maxTextureSize, importer.textureFormat, -1);	
				return true;
			}
			
			return false;
		};
	}
	
	static ModifyDelegate DisablePlatformOverride(string platform)
	{
		return (TextureImporter importer) => {
			int maxTextureSize;
			int compressionQuality;
			TextureImporterFormat textureFormat;
			
			if(importer.GetPlatformTextureSettings(platform, out maxTextureSize, out textureFormat, out compressionQuality))
			{
				importer.ClearPlatformTextureSettings(platform);
				return true;
			}
			
			return false;
		};
	}
	
#if UNITY_IPHONE
	[MenuItem("Custom/Textures/iPhone/Enable override for iPhone")]
	public static void EnableOverrideiPhone()
	{
		ModifySelectedTextures("Enable override for iPhone", EnablePlatformOverride("iPhone"));
	}
	
	[MenuItem("Custom/Textures/iPhone/Disable override for iPhone")]
	public static void DisableOverrideiPhone()
	{
		ModifySelectedTextures("Disable override for iPhone", DisablePlatformOverride("iPhone"));
	}
#endif
	
#endregion
	
#region Presets
	[MenuItem("Custom/Textures/Presets/Icons and splashscreens")]
	public static void PresetIcons()
	{
		ModifySelectedTextures("Icons and splashscreens", (TextureImporter importer) => {
			bool changed = 	importer.mipmapEnabled != false
				|| importer.textureFormat != TextureImporterFormat.RGB24
				|| importer.npotScale != TextureImporterNPOTScale.None;
			
			if(changed)
			{
				importer.mipmapEnabled = false;
				importer.textureFormat = TextureImporterFormat.RGB24;
				importer.npotScale = TextureImporterNPOTScale.None;
			}
			return changed;
		});
	}
#endregion
	
#region Formats
	static ModifyDelegate ChangeTextureFormat(TextureImporterFormat newFormat)
	{
		return (TextureImporter importer) => {
			if(importer.textureFormat != newFormat)
			{
				importer.textureFormat = newFormat;
				return true;
			}
			return false;
		};
	}
	
	[MenuItem("Custom/Textures/Formats/Compressed automatic")]
	public static void SetFormatsCompressedAutomatic()
	{
		ModifySelectedTextures("Compressed automatic", ChangeTextureFormat(TextureImporterFormat.AutomaticCompressed));
	}

	[MenuItem("Custom/Textures/Formats/True color automatic")]
	public static void SetFormatsCompressedTrueColor()
	{
		ModifySelectedTextures("True color automatic", ChangeTextureFormat(TextureImporterFormat.AutomaticTruecolor));
	}
#endregion	
}
