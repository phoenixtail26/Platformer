using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

/*public class AnimationTreeEditor2
{
	static AnimationTreeEditor2 m_Instance = null;

	public static AnimationTreeEditor2 Instance
	{
		get 
		{ 
			if ( m_Instance == null )
			{
				m_Instance = new AnimationTreeEditor2();
			}
			return m_Instance;
		}
	}


	Texture2D lineTexture = null;
	Texture2D arrowTexture = null;

	ConnectionButton ConnectionStart;
	ConnectionButton ConnectionEnd;
	
	Vector3 startPos = new Vector3();
	Vector3 endPos = new Vector3();

	List<NodeWindow> m_Windows = new List<NodeWindow>();

	bool Dragging = false;
	NodeWindow ActiveWindow;
	AnimationTree SelectedAnimationTree;


	public void StartConnection( ConnectionButton button )
	{
		Debug.Log( "start connection" );
		if ( ConnectionStart == null )
		{
			ConnectionStart = button;
		}
		else
		{
			ConnectionEnd = button;
		}
	}

	public void Init()
	{
		lineTexture = ( Texture2D )Resources.Load( "Line Texture 2" );
		arrowTexture = ( Texture2D )Resources.Load( "ArrowHead" );


		//win = new AnimationNodeWindow( new Rect( 10, 10, 150, 50 ), "Run" );
		//win2 = new AnimationNodeWindow( new Rect( 200, 10, 150, 50 ), "Climb" );

		//m_Windows.Add( win );
		//m_Windows.Add( win2 );
	}

	public void Update()
	{
		if ( Selection.activeObject != null )
		{
			GameObject go = Selection.activeObject as GameObject;
			if ( go != null )
			{
				AnimationTree currentTree = go.GetComponent<AnimationTree>();
				if ( currentTree == null )
				{
					SelectedAnimationTree = null;
				}
				if ( currentTree != null && currentTree != SelectedAnimationTree )
				{
					SelectedAnimationTree = currentTree;
					RebuildTreeGraph();
				}
			}
		}		
	}

	public void RebuildTreeGraph()
	{
		m_Windows.Clear();
		
		if ( SelectedAnimationTree != null )
		{
			AT_Node root = SelectedAnimationTree.GetRootNode();
			if ( root != null )
			{
				for ( int i = 0; i < root.Children.Length; i++ )
				{
					AddWindowForNode( root.Children[i] );
				}
			}
		}
	}

	void AddWindowForNode( AT_Node node )
	{
		if ( node == null )
		{
			return;
		}

		NodeWindow win = null;
		
		switch ( node.NodeType )
		{
			case AT_NodeType.kResult:
				AT_ResultNode resNode = ( AT_ResultNode )node;
				win = new ResultNodeWindow( new Rect( resNode.EditorProperties.Position.x, resNode.EditorProperties.Position.y, 150, 50 ), node );
				break;

			case AT_NodeType.kAnimation:
				AT_AnimationNode animNode = (AT_AnimationNode)node;
				win = new AnimationNodeWindow( new Rect( animNode.EditorProperties.Position.x, animNode.EditorProperties.Position.y, 250, 60 ), animNode.AnimationName, node );
				break;

			case AT_NodeType.kBlend:
				AT_BlendNode blendNode = (AT_BlendNode)node;
				win = new BlendNodeWindow( new Rect( blendNode.EditorProperties.Position.x, blendNode.EditorProperties.Position.y, 150, 100 ), node );
				break;
		}

		if ( win != null )
		{
			m_Windows.Add( win );
		}

		AT_Node[] children = node.GetChildren();

		if ( children != null )
		{
			for( int i = 0; i < children.Length; i++ )
			{
				AddWindowForNode( children[i] );
			}
		}
	}

	void GraphConnections()
	{
		for ( int i = 0; i < m_Windows.Count; i++ )
		{
			NodeWindow win = m_Windows[i];

			win.DrawConnections();
		}
	}

	public NodeWindow GetCorrespondingWindow( AT_Node node )
	{
		foreach( NodeWindow win in m_Windows )
		{
			if ( win.GetTreeNode() == node )
			{
				return win;
			}
		}
		return null;
	}

	public void DrawConnectionLine( Vector2 start, Vector2 end )
	{
		if ( lineTexture )
		{
			Vector3 diff = end - start;
			Vector3 startTan = new Vector3( start.x + diff.x * 0.5f, start.y );
			Vector3 endTan = new Vector3( end.x - diff.x * 0.5f, end.y );

			Handles.BeginGUI();
			Handles.DrawBezier( start, end, startTan, endTan, Color.white, lineTexture, 0.75f );
			Handles.EndGUI();

			Matrix4x4 matrixBackup = GUI.matrix;

			Vector2 size = new Vector2( 8, 8 );
			Rect rect = new Rect( end.x - 4 - size.x * 0.5f, end.y - size.y * 0.5f, size.x , size.y  );
			Vector2 pivot = new Vector2( rect.xMin + rect.width * 0.5f, rect.yMin + rect.height * 0.5f );

			GUIUtility.RotateAroundPivot( 0, pivot );
			GUI.DrawTexture( rect, arrowTexture );
			GUI.matrix = matrixBackup;
		}
	}


	public void SelectWindow( Vector2 position )
	{
		ActiveWindow = null;
		foreach ( NodeWindow nodeWin in m_Windows )
		{
			if ( ActiveWindow == null && nodeWin.IsInWindow( position ) )
			{
				nodeWin.Active = true;
				ActiveWindow = nodeWin;
				Dragging = true;
			}
			else
			{
				nodeWin.Active = false;
			}
		}

		if ( ActiveWindow != null  )
		{
			m_Windows.Remove( ActiveWindow );
			m_Windows.Add( ActiveWindow );
		}
	}

	public void OnGUI()
	{
		if ( SelectedAnimationTree == null )
		{
			return;
		}

		switch ( Event.current.type )
		{
			case EventType.MouseDown:
				if ( Event.current.button == 0 )
				{
					SelectWindow( Event.current.mousePosition );
				}
				
				break;
			case EventType.MouseUp:
				if ( Event.current.button == 0 )
				{
					Dragging = false;
				}
				if ( Event.current.button == 1 )
				{
					ConnectionStart = null;
					ConnectionEnd = null;
				}
				break;

			case EventType.MouseDrag:
				if ( Dragging )
				{
					ActiveWindow.DeltaMove( Event.current.delta );
				}
				break;
			case EventType.MouseMove:
				endPos = Event.current.mousePosition;
				break;
		}


		foreach ( NodeWindow nodeWin in m_Windows )
		{
			nodeWin.Draw();
		}

		GraphConnections();

		//Handles.BeginGUI();
		//Handles.DrawLine( new Vector3( 0, 0, 0 ), new Vector3( 100, 100, 0 ) );	

		//if ( drawing )
		//{
			//endPos = Event.current.mousePosition;
		if ( ConnectionStart != null )
		{
			startPos = ConnectionStart.LastPosition;

			if ( ConnectionEnd != null )
			{
				endPos = ConnectionEnd.LastPosition;
			}

			Vector3 diff = endPos - startPos;
			Vector3 startTan = new Vector3( startPos.x + diff.x * 0.5f, startPos.y );
			Vector3 endTan = new Vector3( endPos.x - diff.x * 0.5f, endPos.y );



			Handles.DrawBezier( startPos, endPos, startTan, endTan, Color.white, lineTexture, 0.75f );
		}
		//}



		//	Handles.Label( new Vector3( 0, 0, 0 ), "Blah blah asdkjfha laksjdhflu alksjdghf laksjdhf " );
		//Handles.EndGUI();
	}
}*/
