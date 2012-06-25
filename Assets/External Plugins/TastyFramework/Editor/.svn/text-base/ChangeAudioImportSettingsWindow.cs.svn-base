using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class ChangeAudioImportSettingsWindow : EditorWindow
{
    AudioImporterFormat m_format = AudioImporterFormat.Native;
    int m_bitrate = 128;
    AudioImporterLoadType m_loadType = AudioImporterLoadType.CompressedInMemory;
    bool m_3dSound = true;
    bool m_forceToMono = false;
    bool m_hardwareDecoding = true;
    bool m_gaplessLooping = false;

    public void Init()
    {
        this.title = "Audio Importer"; 

        Object[] audioclips = GetSelectedAudioclips();
        if (audioclips.Length > 0)
        {
            string path = AssetDatabase.GetAssetPath(audioclips[0]);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;

            m_format = audioImporter.format;
            m_bitrate = audioImporter.compressionBitrate / 1000;
            m_loadType = audioImporter.loadType;
            m_3dSound = audioImporter.threeD;
            m_forceToMono = audioImporter.forceToMono;
            m_hardwareDecoding = audioImporter.hardware;
            m_gaplessLooping = audioImporter.loopable;
        }
    }

    void OnGUI()
    {
        m_format = (AudioImporterFormat) EditorGUILayout.EnumPopup("Audio Format", m_format);

        /*m_3dSound = EditorGUILayout.Toggle("3D Sound", m_3dSound, null);

        m_forceToMono = EditorGUILayout.Toggle("Force To Mono", m_forceToMono, null);
        
        m_loadType = (AudioImporterLoadType)EditorGUILayout.EnumPopup("Load Type", m_loadType);

        m_hardwareDecoding = EditorGUILayout.Toggle("Hardware Decoding", m_hardwareDecoding, null);

        m_gaplessLooping = EditorGUILayout.Toggle("Gapless Looping", m_gaplessLooping, null);*/

        m_bitrate = EditorGUILayout.IntSlider("Compression (kbps)", m_bitrate, 32, 240, null);

        if (GUILayout.Button("Apply"))
        {
            ApplySettingsToSelectedClips();
            this.Close();
        }
    }

    void ApplySettingsToSelectedClips()
    {

        Object[] audioclips = GetSelectedAudioclips();
        Selection.objects = new Object[0];

        AssetDatabase.StartAssetEditing();

        foreach (AudioClip audioclip in audioclips)
        {
            string path = AssetDatabase.GetAssetPath(audioclip);
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;

            audioImporter.format = m_format;
            audioImporter.compressionBitrate = m_bitrate * 1000;
            audioImporter.loadType = m_loadType;
            audioImporter.threeD = m_3dSound;
            audioImporter.forceToMono = m_forceToMono;
            audioImporter.hardware = m_hardwareDecoding;
            audioImporter.loopable = m_gaplessLooping;

            AssetDatabase.ImportAsset(path,ImportAssetOptions.Default);
        }

        AssetDatabase.StopAssetEditing();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static Object[] GetSelectedAudioclips()
    {
        return Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets);
    }
}