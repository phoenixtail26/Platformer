////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AnimationTreeEditorWindow.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using UnityEditor;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AnimationTreeEditorWindow
// Description: Editor window for animation tree editor
//
////////////////////////////////////////////////////////////////////////////////////////
class AnimationTreeEditorWindow : EditorWindow
{
    // Reference to editor
	private static AnimationTreeEditor m_Editor = null;

	[MenuItem( "Window/Animation Tree Editor" )]
	public static void Init()
	{
		AnimationTreeEditorWindow window = GetWindow( typeof( AnimationTreeEditorWindow ) ) as AnimationTreeEditorWindow;
        window.Initialize();
	}
		
	// Update is called once per frame
	void Update()
	{
		if ( m_Editor == null )
		{
			Initialize();
		}

		m_Editor.Update();

		this.Repaint();
	}

	private void Initialize()
	{
		m_Editor = AnimationTreeEditor.instance;
		m_Editor.Init( this );
		
		//this.

		this.wantsMouseMove = true;
		
	}

	public void OnGUI()
	{
		if ( m_Editor == null )
		{
			Initialize();
		}
		
		m_Editor.OnGUI();
	}
}
