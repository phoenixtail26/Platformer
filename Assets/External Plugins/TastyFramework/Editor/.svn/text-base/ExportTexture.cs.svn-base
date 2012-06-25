using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public static class ExportTexture 
{
	[MenuItem("Custom/Textures/Save As PNG")]
	public static void SaveAsPNG() 
	{
		object[] objects = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);
			
		foreach(object obj in objects)
		{
			Texture2D tex = obj as Texture2D;
			
			if(tex != null) 
			{
				string path = EditorUtility.SaveFilePanelInProject("Save texture as PNG",
                            tex.name + ".png", 
                            "png", 
                            "Please enter a file name to save the texture to");
				
				if(path.Length == 0)
				{
					return;
				}
				
				TextureImporter importer = TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(tex)) as TextureImporter;
				bool wasReadable = importer.isReadable;
				TextureImporterFormat oldFormat = importer.textureFormat;
				
				if(!wasReadable || oldFormat != TextureImporterFormat.ARGB32)
				{
					importer.isReadable = true;
					importer.textureFormat = TextureImporterFormat.ARGB32;
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tex));
				}	
				
				byte[] data = tex.EncodeToPNG();
				
				if(data != null) 
				{
         			File.WriteAllBytes(path, data);
		            // As we are saving to the asset folder, tell Unity to scan for modified or new assets
		        	AssetDatabase.Refresh(); 
		        }
				
				if(!wasReadable)
				{
					importer.isReadable = wasReadable;
					importer.textureFormat = oldFormat;
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tex));
				}	

				return;
			}
		}
		EditorUtility.DisplayDialog("Save As PNG", "No texture selected", "D'oh!");
	}
	
}
