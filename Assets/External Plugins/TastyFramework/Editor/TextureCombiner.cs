using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class TextureCombiner : EditorWindow 
{
	[MenuItem ("Window/Texture combiner")]
    static void ShowWindow () 
	{
        EditorWindow.GetWindow<TextureCombiner>();
    }	
	
	private Texture2D _inputRGB;
	private Texture2D _inputAlpha;
	
	void OnGUI()
	{
		_inputRGB = EditorGUILayout.ObjectField("RGB", _inputRGB, typeof(Texture2D), true) as Texture2D;
		_inputAlpha = EditorGUILayout.ObjectField("Alpha", _inputAlpha, typeof(Texture2D), true) as Texture2D;
		
		if(_inputRGB == null || _inputAlpha == null)
		{
			GUI.enabled = false;
		} 
		else if(_inputRGB.width != _inputAlpha.width || _inputRGB.height != _inputAlpha.height)
		{
			GUILayout.TextArea("Dimensions of RGB and Alpha must be the same!");
			GUI.enabled = false;
		}			
		
		if(GUILayout.Button("Combine"))
		{
			Combine();
		}
		
		GUI.enabled = true;
	}
	
	bool MakeReadable(Texture2D texture, bool readable)
	{
		string assetPath = AssetDatabase.GetAssetPath(texture);
		TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
		
		if(importer.isReadable != readable) 
		{
			importer.isReadable = readable;
			AssetDatabase.ImportAsset(assetPath);
			return !readable;
		}		
		else return readable;
	}
	
	byte CalculateGrayscale(Color32 colorIn)
	{
		Color c = colorIn;
		return (byte) (c.grayscale * 255.0f);
	}
	
	void Combine()
	{
		EditorUtility.DisplayProgressBar("Combining textures", "Making textures readable", 0.0f);
		
		Texture2D outputTexture = new Texture2D(_inputRGB.width, _inputRGB.height, TextureFormat.ARGB32, false);
		
		bool RGBwasReadable = MakeReadable(_inputRGB, true);
		bool AlphaWasReadable = MakeReadable(_inputAlpha, true);
		Color32[] RGBcolors = _inputRGB.GetPixels32();
		Color32[] alphaColors = _inputAlpha.GetPixels32();
		Color32[] outputColors = new Color32[RGBcolors.Length];

		EditorUtility.DisplayProgressBar("Combining textures", "Combining", 0.25f);
		for(int x = 0; x<RGBcolors.Length; x++) 
		{
			outputColors[x] = RGBcolors[x];
			outputColors[x].a = CalculateGrayscale(alphaColors[x]);
		}
		
		outputTexture.SetPixels32(outputColors);
		
		EditorUtility.DisplayProgressBar("Combining textures", "Reverting readable attributes", 0.5f);
		
		MakeReadable(_inputRGB, RGBwasReadable);
		MakeReadable(_inputAlpha, AlphaWasReadable);
		
		outputTexture.Apply();
	
		string filename = AssetDatabase.GetAssetPath(_inputRGB);
		filename = filename.Replace(".", "_combined.");
		
		Debug.Log(filename);
		EditorUtility.DisplayProgressBar("Combining textures", "Saving", 0.75f);
	
		if (filename.Length > 0) {
    	    FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
        	BinaryWriter bw = new BinaryWriter(fs);
			
			bw.Write(outputTexture.EncodeToPNG());
			bw.Close();
			fs.Close();
	    }
		
		AssetDatabase.ImportAsset(filename);
		
		EditorUtility.ClearProgressBar();
		
		
/*		string assetPath = "Assets/test.png";
		AssetDatabase.CreateAsset(outputTexture, assetPath);
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = outputTexture;
				 */
	}
}
