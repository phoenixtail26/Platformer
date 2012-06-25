////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AnimationTreeEditor.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AnimationTreeEditor
// Description: Editor for AnimationTree, providing easily understandable GUI, tree
//              editing and browsing
//
////////////////////////////////////////////////////////////////////////////////////////
public class AnimationTreeEditor
{
    // Singleton of this class
    static AnimationTreeEditor m_instance = null;
	public static AnimationTreeEditor instance
	{
		get
		{
			if ( m_instance == null )
			{
				m_instance = new AnimationTreeEditor();
			}
			return m_instance;
		}
	}

    // Textures used for GUI
	Texture2D m_lineTexture = null;
	Texture2D m_arrowTexture = null;
    Texture2D[] m_icons = null;

    // Currently selected animation tree
	AnimationTree m_selectedAnimationTree;

    // The editor window of this editor
	public EditorWindow m_editorWindow;

    // List of all visible windows
	List<NodeWindow> m_Windows = new List<NodeWindow>();
    // List of all visible transitions
    List<TransitionWindow> m_transitions = new List<TransitionWindow>();

    // The currently active container node. Children of this node are visible in the editor
	public AT_ContainerNode m_activeContainer;
    
    // The currently active node window
    NodeWindow ActiveWindow;
    
    // Is the window being dragged
    bool Dragging = false;

    // The currently selected node
    AT_Node m_currentNode;

    // Last time a window was clicked. Used for detecting double clicks
    DateTime m_lastTimeWindowSelected = DateTime.Now;

    // Start and end of new connection currently being made
    ConnectionButton ConnectionStart;
    ConnectionButton ConnectionEnd;

    // The current mouse position in screen space
    Vector2 m_currentMousePos = new Vector2();

    // Camera controls
    public Vector3 m_camerScaleAmount = new Vector3(1, 1, 1);
    public Vector3 m_cameraOffset = new Vector3(0, 0, 0);
    bool m_panningCamera = false;

    // The control variables window
    public ControlVariablesWindow m_controlVarsWindow;

    // Variables used for drawing and creating new transitions between states
    bool m_drawingNewTransition = false;
    Vector2 m_newTransitionStart = new Vector2();
    StateWindow m_fromStateWindow = null;

    // Data used to create GUI at top of screen
    List<AT_Node> m_path = new List<AT_Node>();
    AnimationTree[] m_activeAnimationTrees;
	
	public void Init( EditorWindow window )
	{
        // Load in texture resources
		m_lineTexture = ( Texture2D )Resources.Load( "Line Texture 2" );
		m_arrowTexture = ( Texture2D )Resources.Load( "ArrowHead" );
        m_icons = new Texture2D[6];
        m_icons[0] = (Texture2D)Resources.Load("Icon - StateMachine");
        m_icons[1] = (Texture2D)Resources.Load("Icon - BlendGraph");
        m_icons[2] = (Texture2D)Resources.Load("Icon - Animation");
        m_icons[3] = (Texture2D)Resources.Load("Icon - Blend");
        m_icons[4] = (Texture2D)Resources.Load("Icon - AddBlend"); 
		m_icons[5] = (Texture2D)Resources.Load("Icon - Pose"); 
		
        // Init editor to safe state
		m_editorWindow = window;
        m_selectedAnimationTree = null;
        m_Windows.Clear();
        m_activeContainer = null;
        ActiveWindow = null;
        Dragging = false;
        ConnectionStart = null;
        ConnectionEnd = null;
        m_camerScaleAmount = new Vector2(1, 1);

        // Find all the enabled animation trees
        m_activeAnimationTrees = GameObject.FindObjectsOfType(typeof(AnimationTree)) as AnimationTree[];
	}

	public void Update()
	{
		if ( m_selectedAnimationTree != null && m_selectedAnimationTree.IsDirty )
		{
			CheckForDirtyNodes(m_selectedAnimationTree.Root);
			m_selectedAnimationTree.IsDirty = false;
		}
		
		if ( Selection.activeGameObject != null )
		{
            // Get the current selection from Unity
			GameObject go = Selection.activeGameObject;
			
            // Get the tree the current selection belongs to
			AnimationTree currentTree = GetAnimationTree( go );

            // If a child of an animation tree is currently selected...
            if (currentTree != null)
			{
				GameObject firstChild = go;
               
                // Make sure the firstChild points to an AT_Node not the AnimationTree base object
                if (currentTree.gameObject == go )
				{
                    firstChild = currentTree.Root.gameObject;
				}
                m_currentNode = firstChild.GetComponent<AT_Node>();

				AT_ContainerNode contParent;
                // If the root is not selected, find the closest container node to the selected node
                if (firstChild != currentTree.Root.gameObject)
				{
                    if (m_currentNode != null && m_currentNode.Parent != null)
                    {
                        contParent = FindNextContainerNode(m_currentNode.Parent.gameObject);
                    }
                    else
                    {
                        contParent = FindNextContainerNode(firstChild.transform.parent.gameObject);
                    }
				}
				else
				{
                    // If the root is selected, it is the active container
					contParent = firstChild.GetComponent<AT_ContainerNode>();
				}

                // Change the control variables node's parent to the currently selected container.
                // This allows us to view and select the control variables node while inside any 
                // container node without altering what the active container node is
                if (m_selectedAnimationTree != null && m_activeContainer != null)
                {
                    m_selectedAnimationTree.m_controlVarsNode.Parent = m_activeContainer;
                }

                // If we've selected a new container node, rebuild the tree graph
				if ( contParent != m_activeContainer )
				{
					m_activeContainer = contParent;
					RebuildTreeGraph();
				}

                // If the control variables window doesn't exist, create it
                if (m_controlVarsWindow == null)
                {
                    RefreshVariablesWindow();
                }
			}

            m_selectedAnimationTree = currentTree;
		}	
		else
		{
			m_selectedAnimationTree = null;
		}
	}

    // Find the next container node up (including the passed object) from the passed object
	AT_ContainerNode FindNextContainerNode( GameObject go )
	{
        if (m_selectedAnimationTree == null)
        {
            return null;
        }

		AT_ContainerNode cont = go.GetComponent<AT_ContainerNode>();

        if (go.transform.parent == null)
        {
            return null;
        }

        // If we're at the tree root, return it
		if ( go.transform.parent.gameObject == m_selectedAnimationTree.gameObject )
		{
			return cont;
		}

        // If the node is a container and is expandable, return it
		if ( cont != null && cont.Expandable )
		{
			return cont;
		}

        // Recursively find the next container up
		return FindNextContainerNode( go.transform.parent.gameObject );
	}

	// Traverses the hierarchy backwards to find the AnimationTree
	AnimationTree GetAnimationTree( GameObject go )
	{
		AnimationTree rootTree = go.GetComponent<AnimationTree>();
		if ( rootTree != null )
		{
			return rootTree;
		}

		if ( go.transform.parent != null )
		{
			rootTree = GetAnimationTree( go.transform.parent.gameObject );
		}

		return rootTree;
	}
	
	public void CheckForDirtyNodes( AT_Node node )
	{
		if ( node == null )
		{
			return;
		}
		
		if ( node.IsDirty )
		{
			//Debug.Log("dirty: " + node.name);
			EditorUtility.SetDirty(node);
			node.IsDirty = false;			
		}
		
		AT_ContainerNode cont = node as AT_ContainerNode;
		if ( cont != null )
		{
			foreach( AT_Node cNode in cont.Children )
			{
				CheckForDirtyNodes( cNode );
			}
	
	        for (int i = 0; i < cont.UnassignedChildren.Count; i++)
	        {
	            CheckForDirtyNodes( cont.UnassignedChildren[i] );
	        }
		}
	}

    // Rebuild the currently visible tree graph and recreate all the windows
	public void RebuildTreeGraph()
	{
        m_activeAnimationTrees = GameObject.FindObjectsOfType(typeof(AnimationTree)) as AnimationTree[];

		m_Windows.Clear();

        // Create a new window for all the active container's children
		if ( m_activeContainer != null )
		{
			
			foreach( AT_Node node in m_activeContainer.Children )
			{
				//for ( int i = 0; i < m_activeContainer.Children.Count; i++ )
				//{
					AddWindowForNode( node );// m_activeContainer.Children[i] );
				//}
			}

            for (int i = 0; i < m_activeContainer.UnassignedChildren.Count; i++)
            {
                AddWindowForNode( m_activeContainer.UnassignedChildren[i] );
            }
		}

        // Set the currently selected node's window as active
        foreach (NodeWindow window in m_Windows)
        {
            if (window.GetTreeNode().gameObject == Selection.activeGameObject)
            {
                window.Active = true;
            }
        }


        // Create control variables window
        RefreshVariablesWindow();

        // Build up the list of transitions
        m_transitions.Clear();
        AT_StateMachine sm = m_activeContainer as AT_StateMachine;
        if ( sm != null)
        {
            foreach (AT_StateTransition trans in sm.m_transitions)
            {
				if ( trans != null )
				{
	                TransitionWindow twin = new TransitionWindow(trans);
	                m_transitions.Add(twin);
	                m_Windows.Add(twin);
				}
            }
        }

        // The path at the top of the screen used for quick navigation
        RebuildPath();
	}

    void RebuildPath()
    {
        //Rebuild path to current node
        m_path.Clear();

        AT_ContainerNode cont = m_activeContainer;

        while (cont != null && cont.NodeType != AT_NodeType.kRoot)
        {
            // Insert each container node at the front of the list
            m_path.Insert(0, cont);
            cont = FindNextContainerNode(cont.transform.parent.gameObject);
        }
        m_path.Insert(0, cont);
    }

    // Recreate the control variables window
    public void RefreshVariablesWindow()
    {
        if (m_editorWindow == null)
        {
            return;
        }

        Rect winPos = m_editorWindow.position;
        Rect rect = new Rect(-5, winPos.height - 200, 230, 115);
        if (m_selectedAnimationTree != null)
        {
            bool selected = false;
            if ( ActiveWindow == m_controlVarsWindow && ActiveWindow != null )
            {
                selected = true;
            }

            m_controlVarsWindow = new ControlVariablesWindow(rect, m_selectedAnimationTree.m_controlVarsNode);
            m_Windows.Add(m_controlVarsWindow);

            if (selected)
            {
                m_controlVarsWindow.Active = true;
            }
        }
    }

    // Create a new window to represent the passed node
	void AddWindowForNode (AT_Node node)
	{
		if (node == null)
        {
			//Debug.LogError ("AnimationTreeEditor: Trying to add window for null node");
			return;
		}
		
        NodeWindow win = null;

        switch (node.NodeType)
        {
		case AT_NodeType.kOutput:
			AT_OutputNode resNode = (AT_OutputNode)node;
			win = new ResultNodeWindow (new Rect (resNode.EditorProperties.Position.x, resNode.EditorProperties.Position.y, 150, 50), node);
			break;

            case AT_NodeType.kAnimation:
			AT_Animation animNode = (AT_Animation)node;
			win = new AnimationNodeWindow (new Rect (node.EditorProperties.Position.x, node.EditorProperties.Position.y, 200, 60), animNode.m_animationName, node);
			break;
			
			case AT_NodeType.kPose:
			AT_Pose poseNode = (AT_Pose)node;
			win = new PoseNodeWindow (new Rect (node.EditorProperties.Position.x, node.EditorProperties.Position.y, 200, 60), poseNode.m_animationName, node);
			break;

            case AT_NodeType.kAnimationState:
			AT_AnimationState animState = (AT_AnimationState)node;
			win = new AnimationStateWindow (new Rect (node.EditorProperties.Position.x, node.EditorProperties.Position.y, 150, 60), animState.m_animation.m_animationName, node);
			break;

            case AT_NodeType.kBlendGraph:
			win = new BlendGraphWindow (new Rect (node.EditorProperties.Position.x, node.EditorProperties.Position.y, 150, 60), node);
			break;

            case AT_NodeType.kBlend:
			win = new BlendNodeWindow (new Rect (node.EditorProperties.Position.x, node.EditorProperties.Position.y, 150, 100), node);
			break;
		
            case AT_NodeType.kStateMachine:
			win = new StateMachineWindow (new Rect (node.EditorProperties.Position.x, node.EditorProperties.Position.y, 150, 60), node);
			break;

            case AT_NodeType.kBlendContainer:
			win = new BlendContainerWindow (new Rect (node.EditorProperties.Position.x, node.EditorProperties.Position.y, 200, 60), node);
			break;

            case AT_NodeType.kAdditiveBlend:
			win = new AdditiveBlendNodeWindow (new Rect (node.EditorProperties.Position.x, node.EditorProperties.Position.y, 150, 100), node);
			break;

            case AT_NodeType.kBlendStateMachine:
			win = new BlendSMWindow (new Rect (node.EditorProperties.Position.x, node.EditorProperties.Position.y, 200, 60), node);
			break;
		
		case AT_NodeType.kRoot:
			Debug.LogError ("AnimationTreeEditor: Trying to add a window for the root node");
			/*AT_StateMachine sm = node as AT_StateMachine;
			if (sm != null)
			{
				Debug.Log (sm.m_stateName);
			}*/
			
			Debug.Log(node.GetType());
			
			break;
		}

        if (win != null)
        {
			m_Windows.Add (win);
		}
		
        // If this is a contianer, create new windows for all of its children
		AT_ContainerNode cont = node as AT_ContainerNode;
		if ( cont != null && !cont.Expandable)
		{
			foreach ( AT_Node cNode in cont.Children )
			{
				//for ( int i = 0; i < cont.Children.Count; i++ )
				//{
					AddWindowForNode( cNode );//cont.Children[i] );
				//}
			}

            for (int i = 0; i < cont.UnassignedChildren.Count; i++)
            {
                AddWindowForNode(cont.UnassignedChildren[i]);
            }
		}
	}

    // Draw all visible connections
    void GraphConnections()
    {
        for (int i = 0; i < m_Windows.Count; i++)
        {
            NodeWindow win = m_Windows[i];

            win.DrawConnections();
        }
    }

    // Get the window that represents the passed node
    public NodeWindow GetCorrespondingWindow(AT_Node node)
    {
        foreach (NodeWindow win in m_Windows)
        {
            if (win.GetTreeNode() == node)
            {
                return win;
            }
        }
        return null;
    }

    // Draw a line in the editor window. Used for drawing connections and transitions
    public void DrawLine(Vector2 start, Vector2 end, Color col, bool bezier, float width)
    {
        if (m_lineTexture)
        {
            Color oldColor = GUI.color;
            GUI.color = col;

            Vector3 diff = end - start;
                        
            float offset = diff.x * 0.5f;
            int adjust = 1;
            if (diff.x < 0)
            {
                adjust = -1;
            }

            float ylimit = Mathf.Min(Math.Abs(diff.y * 0.5f), Math.Abs(diff.x*1.5f));
            offset = Mathf.Max(Mathf.Abs(offset), ylimit );
            offset *= adjust;

            // Tangents used for bezier curve
            Vector3 startTan = new Vector3(start.x + offset, start.y);
            Vector3 endTan = new Vector3(end.x - offset, end.y);

            // If we don't want a bezier line, make both tangent positions in the middle of the points
            if (!bezier)
            {
                startTan = new Vector3( start.x + diff.x * 0.5f, start.y + diff.y * 0.5f );
                endTan = startTan;
            }

            // Draw the line using the calculated values
            Handles.BeginGUI();
            Handles.DrawBezier(start, end, startTan, endTan, col, m_lineTexture, width);
            Handles.EndGUI();

            GUI.color = oldColor;
        }
    }

    // Draws a connection line. Includes arrow at end
    public void DrawConnectionLine(Vector2 start, Vector2 end, Color col, bool bezier)
    {
        if (m_lineTexture)
        {
            DrawLine(start, end, col, bezier, 0.75f);

            Color oldColor = GUI.color;
            GUI.color = col;

            Matrix4x4 matrixBackup = GUI.matrix;

            Vector2 size = new Vector2(8, 8);
            Rect rect = new Rect(end.x - 4 - size.x * 0.5f, end.y - size.y * 0.5f, size.x, size.y);
            Vector2 pivot = new Vector2(rect.xMin + rect.width, rect.yMin + rect.height * 0.5f);

            Vector3 p = new Vector3(pivot.x, pivot.y, 0);
            p = GUI.matrix.MultiplyPoint3x4(p);
            pivot.x = p.x;
            pivot.y = p.y;


            // TODO: get arrow to line up with bezier curve
            // Calculate the angle the arrow needs to be rotated by to line up with the connection line
            float angle = 0;
            if (bezier)
            {
                if (end.x < start.x)
                {
                    angle = 180;
                }
            }
            else
            {
                Vector3 diff = end - start;
                angle = Vector3.Angle(Vector3.right, diff);
                if (diff.y < 0)
                {
                    angle *= -1;
                }
            }

            // Rotate the arrow so it lines up with the connection line
            GUIUtility.RotateAroundPivot(angle, pivot);
            GUI.DrawTexture(rect, m_arrowTexture);
            GUI.matrix = matrixBackup;

            GUI.color = oldColor;
        }
    }

    public void DrawConnectionLine(Vector2 start, Vector2 end, Color col)
    {
        DrawConnectionLine(start, end, col, true);
    }

    // Add a new transition between the passed states to the current state-machine
    AT_StateTransition CreateNewTransition(StateWindow fromStateWin, StateWindow toStateWin)
    {
        if (fromStateWin == toStateWin)
        {
            return null;
        }

        AT_StateMachine sm = m_activeContainer as AT_StateMachine;
        AT_StateTransition trans = sm.AddTransition(fromStateWin.GetTreeNode() as AT_State, toStateWin.GetTreeNode() as AT_State);

        // Set these as dirty so Unity knows they need to be saved
        sm.SetDirty();
        trans.SetDirty();

        RebuildTreeGraph();
        Selection.activeGameObject = trans.gameObject;
        return trans;
    }

    // Select a window at the passed screen position
    public void SelectWindow(Vector2 position)
    {
        NodeWindow lastActiveWin = ActiveWindow;

        ActiveWindow = null;
        // Loop backwards through all the windows
        for ( int i = m_Windows.Count-1; i >=0; i-- )
        {
            NodeWindow nodeWin = m_Windows[i];
            // If the position is in the window
            if (ActiveWindow == null && nodeWin.IsInWindow(position))
            {
                nodeWin.Active = true;
                ActiveWindow = nodeWin;

                StateWindow sWin = nodeWin as StateWindow;
                if (sWin == null)
                {
                    // If the node is not a state node (no transition drawing), then we are now dragging the window
                    Dragging = true;
                }
                else
                {
                    if (!m_drawingNewTransition)
                    {
                        // If we are in the state window's dragging area and not in the transition drawing area, start dragging
                        if (sWin.IsInDraggingArea(position))
                        {
                            Dragging = true;
                        }
                        else
                        {
                            // Start drawing a new transition
                            m_drawingNewTransition = true;
                            m_newTransitionStart = position;
                            m_fromStateWindow = sWin;
                        }
                    }
                    else
                    {
                        // Finish drawing transition and create a new one
                        m_drawingNewTransition = false;
                        AT_StateTransition trans = CreateNewTransition( m_fromStateWindow, sWin );
                        ActiveWindow = GetCorrespondingWindow(trans);
                    }
                }
            }
            else
            {
                nodeWin.Active = false;
            }
        }

        if (ActiveWindow != null)
        {
            Selection.activeGameObject = ActiveWindow.GetTreeNode().gameObject;

            // Put the selected window at the back of the window list so it is drawn on top of other windows
            m_Windows.Remove(ActiveWindow);
            m_Windows.Add(ActiveWindow);
            ActiveWindow.Active = true;
            RefreshVariablesWindow();
        }
        else
        {
        }

        DateTime newClickTime = DateTime.Now;
        // If we clicked on the same window again
        if (lastActiveWin == ActiveWindow)
        {
            float timeDiff = (float)(DateTime.Now.Subtract(m_lastTimeWindowSelected).TotalSeconds);
            if (timeDiff < 1)
            {
                // Double-clicked on same window
                if (ActiveWindow != null)
                {
                    AT_ContainerNode cont = ActiveWindow.GetTreeNode() as AT_ContainerNode;
                    if (cont != null)
                    {
                        if (cont.Expandable)
                        {
                            // Select a child so that we draw all the children of this container
                            if (cont.Children.Count > 0)
                            {
                                Selection.activeGameObject = cont.Children[0].gameObject;
                            }
                            else
                            {
                                if (cont.transform.childCount > 0)
                                {
                                    Selection.activeGameObject = cont.transform.GetChild(0).gameObject;
                                }
                            }
                        }
                    }

                }
                else // Double-clicked in open space
                {
                    // Move up to the next container node in the tree
                    AT_ContainerNode cont = null;
                    if (m_activeContainer != null && m_activeContainer.Parent != null)
                    {
                         cont = FindNextContainerNode(m_activeContainer.Parent.gameObject);
                    }
                    else
                    {
                        cont = FindNextContainerNode(Selection.activeGameObject.transform.parent.gameObject);
                    }

                    // If we can move up, do so
                    if (cont != null)
                    {
                        if (cont.gameObject != m_selectedAnimationTree.Root.gameObject)
                        {
                            Selection.activeGameObject = cont.transform.GetChild(0).gameObject;
                        }
                        else
                        {
                            Selection.activeGameObject = cont.gameObject;
                        }
                    }
                }
                // Store the min clicked time to reset double click calculation
                newClickTime = DateTime.MinValue;
            }
        }

        m_lastTimeWindowSelected = newClickTime;

        // Check if we clicked the control variables window
        if (m_controlVarsWindow != null && m_controlVarsWindow.IsInWindow(position))
        {
            ActiveWindow = m_controlVarsWindow;
        }
    }

    // Used to adjust the points at which the transitions connect on the target windows, depending on 
    // how many other transitions are connecting to the same side of the windows
    void AdjustTransitionPositions()
    {
        Dictionary<NodeWindow, int[]> transitionWindowSideCounter = new Dictionary<NodeWindow, int[]>();
        Dictionary<NodeWindow, int[]> transitionWindowIndexCounter = new Dictionary<NodeWindow, int[]>();

        // Count up all the connections on each window side
        transitionWindowSideCounter.Clear();
        transitionWindowIndexCounter.Clear();
        for (int i = 0; i < m_transitions.Count; i++)
        {
            NodeWindow win = m_transitions[i].m_fromWin;
            if (win != null)
            {
                if (!transitionWindowSideCounter.ContainsKey(win))
                {
                    int[] arr = new int[4];
                    transitionWindowSideCounter.Add(win, arr);

                    int[] arr2 = new int[4];
                    transitionWindowIndexCounter.Add(win, arr2);
                }
                transitionWindowSideCounter[win][m_transitions[i].m_fromSideIndex]++;
            }

            win = m_transitions[i].m_toWin;
            if (win != null)
            {
                if (!transitionWindowSideCounter.ContainsKey(win))
                {
                    int[] arr = new int[4];
                    transitionWindowSideCounter.Add(win, arr);

                    int[] arr2 = new int[4];
                    transitionWindowIndexCounter.Add(win, arr2);
                }
                transitionWindowSideCounter[win][m_transitions[i].m_toSideIndex]++;
            }
        }

        // Tell each transition how many are connecting to the same side and which number it is in the queue
        for (int i = 0; i < m_transitions.Count; i++)
        {
            NodeWindow win = m_transitions[i].m_fromWin;
            if (win != null)
            {
                int sideIndex = m_transitions[i].m_fromSideIndex;
                m_transitions[i].UpdateLinkOffset(true, transitionWindowIndexCounter[win][sideIndex]++, transitionWindowSideCounter[win][sideIndex]);
            }

            win = m_transitions[i].m_toWin;
            if (win != null)
            {
                int sideIndex = m_transitions[i].m_toSideIndex;
                m_transitions[i].UpdateLinkOffset(false, transitionWindowIndexCounter[win][sideIndex]++, transitionWindowSideCounter[win][sideIndex]);
            }
        }

    }

    // Create a new animation tree as a child of the selected object
    void AddNewAnimTree()
    {
        GameObject treeObj = new GameObject("Animation Tree");
        AnimationTree tree =  treeObj.AddComponent<AnimationTree>();
       

        AT_StateMachine root = AT_Factory.CreateNode<AT_StateMachine>("State Machine", null, tree);
        root.SetLocalBlendWeight(1);
        root.SetParentBlendWeight(1);
        root.NodeType = AT_NodeType.kRoot;
        root.transform.parent = tree.transform;
        root.m_stateName = "Root";

        GameObject go = new GameObject("Transitions");
        go.transform.parent = root.transform;
        root.m_transitionsParent = go;

        tree.Root = root;

        AT_ControlVariables vars = AT_Factory.CreateNode<AT_ControlVariables>("Control Variables", root, tree);
        vars.transform.parent = tree.transform;

        tree.m_controlVarsNode = vars;

        if (Selection.activeGameObject != null)
        {
            treeObj.transform.parent = Selection.activeGameObject.transform;
        }

        Selection.activeGameObject = treeObj;
    }

    // Draw the editor GUI
    public void OnGUI()
	{
		// Refresh active Anim trees list
		switch (Event.current.type)
        {
            case EventType.MouseDown:
                // Left button
                if (Event.current.button == 0)
                {
					m_activeAnimationTrees = GameObject.FindObjectsOfType(typeof(AnimationTree)) as AnimationTree[];
				}
			break;
		}
		
        Matrix4x4 matrixBackup = GUI.matrix;

        
		if ( m_editorWindow != null )
		{
            bool earlyOut = false;

            // Haven't selected an animation tree
            if (m_selectedAnimationTree == null)
            {
                TextAnchor origAnchor = GUI.skin.label.alignment;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(0, 0, m_editorWindow.position.width, m_editorWindow.position.height), "No Animation Tree Selected");
                if (GUI.Button(new Rect(m_editorWindow.position.width * 0.5f - 50, m_editorWindow.position.height * 0.5f + 20, 100, 30), "Add New Tree"))
                {
                    AddNewAnimTree();
                }
                GUI.skin.label.alignment = origAnchor;
                earlyOut = true;
            }
            else
            {
                // Selected animation tree doesn't have a subject assigned
                if (m_selectedAnimationTree.m_subject == null)
                {
                    TextAnchor origAnchor = GUI.skin.label.alignment;
                    GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                    GUI.Label(new Rect(0, 0, m_editorWindow.position.width, m_editorWindow.position.height), "Set up the animation tree's subject before editing the tree\n\nDrag the 'Animation' onto 'Subject' in the inspector");
                    GUI.skin.label.alignment = origAnchor;
                    earlyOut = true;
                }
            }

            // If we're not going to draw this tree...
            if (earlyOut)
            {
                Rect winPos = m_editorWindow.position;

                GUI.skin = null;

                // Top info bar
                GUI.Box(new Rect(0, 0, winPos.width, 24), "");
                GUILayout.BeginArea(new Rect(9, 4, winPos.width, 20));

                GUILayout.BeginVertical(GUILayout.ExpandHeight(true));

                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                DrawAnimTreeSelectionGUI();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.EndArea();
                return;
            }
		}

        // Handle the GUI events
        switch (Event.current.type)
        {
            case EventType.MouseDown:
                // Left button
                if (Event.current.button == 0)
                {
                    // Make sure we're not clicking in the top or bottom GUI portions of the editor. 
                    // (This hardcoded method is fairly ass, but right now I don't care)
                    if ((Event.current.mousePosition.y < m_editorWindow.position.height - 90) && ( Event.current.mousePosition.y > 24 ) )
                    {
                        SelectWindow(Event.current.mousePosition);
                    }
                }

                // Middle button
                if (Event.current.button == 2)
                {
                    m_panningCamera = true;
                }

                break;
            case EventType.MouseUp:
                // Left button
                if (Event.current.button == 0)
                {
                    // Stop dragging the current window
                    Dragging = false;

                    // Finished drawing the transition if we were drawing one
                    if (m_drawingNewTransition)
                    {
                        SelectWindow(Event.current.mousePosition);
                        m_drawingNewTransition = false;
                    }
                }

                // Right button
                if (Event.current.button == 1)
                {
                    // Cancel creation of connection
                    ConnectionStart = null;
                    ConnectionEnd = null;
                }

                // Middle button
                if (Event.current.button == 2)
                {
                    m_panningCamera = false;
                }
                break;

            case EventType.MouseDrag:
                if (Dragging)
                {
                    // Send dragged values to active window
                    ActiveWindow.DeltaMove(Event.current.delta);
                }
                if (m_panningCamera)
                {
                    m_cameraOffset += new Vector3(Event.current.delta.x, Event.current.delta.y, 0);
                }
                m_currentMousePos = Event.current.mousePosition;
                break;
            case EventType.MouseMove:
                m_currentMousePos = Event.current.mousePosition;
                break;

            case EventType.ScrollWheel:
                // Zoom camera
                m_camerScaleAmount += new Vector3(-Event.current.delta.y, -Event.current.delta.y, 0) * 0.01f;
                break;

            case EventType.KeyDown:
                // Delete currently selected node
                if (Event.current.keyCode == KeyCode.Backspace || Event.current.keyCode == KeyCode.Delete)
                {
                    DeleteCurrentSelection();
                }                
                break;
        }
		
		// Handle camera movement and zooming. 
        // TODO: Fix all the issues with having an adjustable camera
        //GUI.matrix = Matrix4x4.TRS(m_cameraOffset, Quaternion.identity, new Vector3(m_camerScaleAmount.x, m_camerScaleAmount.x, 1)) * GUI.matrix;
		
        // Set the positions that the transitions connect to on each window
        AdjustTransitionPositions();

        // Draw each window
        foreach (NodeWindow nodeWin in m_Windows)
        {
            if (nodeWin as ControlVariablesWindow == null)
            {
                nodeWin.Draw();
            }
        }

        //Matrix4x4 beforeControls = GUI.matrix;

        //GUI.matrix = matrixBackup;
		
		Vector2 offsetBackup = m_cameraOffset;
		m_cameraOffset = Vector2.zero;

        // Draw the control variables window
        if (m_controlVarsWindow != null)
        {
            m_controlVarsWindow.Draw();
        }
		
		m_cameraOffset = offsetBackup;
		
        //GUI.matrix = beforeControls;

        GraphConnections();

        // Draw the transition that is currently being made
        if (m_drawingNewTransition)
        {
            DrawConnectionLine(m_newTransitionStart, m_currentMousePos, Color.white, false);
        }
       
        // Draw connection in progress
        if (ConnectionStart != null)
        {
            Vector2 startPos = ConnectionStart.LastPosition;
            Vector2 endPos = m_currentMousePos;


            if (ConnectionEnd != null)
            {
                endPos = ConnectionEnd.LastPosition;
            }

           /* Vector3 diff = endPos - startPos;
            Vector3 startTan = new Vector3(startPos.x + diff.x * 0.5f, startPos.y);
            Vector3 endTan = new Vector3(endPos.x - diff.x * 0.5f, endPos.y);*/

            //Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, m_lineTexture, 0.75f);
            Color col = Color.white;
            switch (ConnectionStart.m_type)
            {
                case ConnectionButton.ConnectionButtonType.kAnimDataIn:
                case ConnectionButton.ConnectionButtonType.kAnimDataOut:
                    col = new Color(0.5f, 1.0f, 0.5f, 1);
                    break;
                case ConnectionButton.ConnectionButtonType.kControlDataIn:
                case ConnectionButton.ConnectionButtonType.kControlDataOut:
                    col = new Color(0.5f, 0.5f, 1, 1);
                    break;
            }

            DrawConnectionLine(startPos, endPos, col);
        }

        GUI.matrix = matrixBackup;

        // Draw global GUI
        DrawInterface();

        GUI.matrix = Matrix4x4.identity;//beforeControls.inverse;
	}

    // Delete the currently selected node
    void DeleteCurrentSelection()
    {
        if (ActiveWindow != null)
        {
            ActiveWindow.Destruct();
            if (m_activeContainer.Children.Count > 0 && m_activeContainer.Children[0] != null)
            {
                Selection.activeGameObject = m_activeContainer.Children[0].gameObject;
            }
            else
            {
                Selection.activeGameObject = m_activeContainer.gameObject;
            }
            RebuildTreeGraph();
        }
    }

    // Draw the GUI used for selecting animation trees
    void DrawAnimTreeSelectionGUI ()
    {
    	EditorGUIUtility.LookLikeControls ();

        string[] trees = new string[m_activeAnimationTrees.Length + 2];
    	trees[0] = "None";
    	trees[1] = "";

        int counter = 2;
    	int selected = 0;
    	foreach (AnimationTree tree in m_activeAnimationTrees)
        {
    		if (tree != null)
			{
    			trees[counter] = tree.name;
    			if (m_selectedAnimationTree == tree)
            {
    				selected = counter;
    			}
    			counter++;
			}
        }

        GUILayout.Label("Animation Tree:", GUILayout.ExpandWidth(false));
        int newSel = EditorGUILayout.Popup(selected, trees, GUILayout.ExpandWidth(false));
        if (newSel != selected)
        {
            if (newSel <= 1)
            {
                Selection.activeGameObject = null;
            }
            else
            {
                Selection.activeGameObject = m_activeAnimationTrees[newSel-2].gameObject;
            }
            m_controlVarsWindow = null;
            m_activeContainer = null;
            m_selectedAnimationTree = null;
            m_Windows.Clear();
        }
    }

    // Draw the GUI at the top of the screen
    void DrawTopInterface ()
    {
    	Rect winPos = m_editorWindow.position;

        GUI.skin = null;

        // Top info bar
    	GUI.Box (new Rect (0, 0, winPos.width, 48), "");
    	GUILayout.BeginArea (new Rect (5, 4, winPos.width, 40));

        GUILayout.BeginVertical (GUILayout.ExpandHeight (true));

        
        GUILayout.BeginHorizontal (GUILayout.ExpandWidth (true));
    	DrawAnimTreeSelectionGUI ();
  
		EditorGUILayout.Space ();
    	if (GUILayout.Button ("Refresh", GUILayout.MaxWidth (70)))
		{
    		RebuildTreeGraph ();
		}
		
        GUILayout.EndHorizontal();

        // Path Selection GUI
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

        GUILayout.Label("Path:", GUILayout.ExpandWidth(false));

        foreach (AT_Node node in m_path)
        {
            string label = "";

            if (node != null)
            {
                AT_State state = node as AT_State;
                if (state != null)
                {
                    label += state.m_stateName;
                }

                label += " (";

                switch (node.NodeType)
                {
                    case AT_NodeType.kStateMachine:
                    case AT_NodeType.kBlendStateMachine:
                    case AT_NodeType.kRoot:
                        label += "State Machine";
                        break;

                    case AT_NodeType.kBlendGraph:
                    case AT_NodeType.kBlendContainer:
                        label += "Blend Graph";
                        break;
                }

                label += ")";
            }

            // Make the clicked container the new active container
            if (GUILayout.Button(label, GUILayout.ExpandWidth(false)))
            {
                GameObject target = node.gameObject;
                if ( node.transform.childCount > 0 )
                {
                    target = node.transform.GetChild(0).gameObject;
                }
                Selection.activeGameObject = target;
            }

            if (node != m_path[m_path.Count - 1])
            {
                GUILayout.Label("/", GUILayout.ExpandWidth(false));
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
        GUILayout.EndArea();
    }

    // Draw global GUI
    void DrawInterface()
    {
        Rect winPos = m_editorWindow.position;

        DrawTopInterface();

        GUISkin skin = null;
        if (skin == null)
        {
            skin = (GUISkin)Resources.Load("NodeWindowSkin", typeof(GUISkin));
        }

        GUI.skin = skin;
        GUI.color = new Color(1, 1, 1, 0.75f);
        GUI.Box(new Rect(-10,winPos.height - 90,winPos.width+20,100), "Toolkit");

        GUI.skin = null;

        GUILayout.BeginArea(new Rect(0, winPos.height - 75, winPos.width, 75));
        GUILayout.BeginHorizontal(GUILayout.MinWidth(10), GUILayout.ExpandWidth(false));

        if (m_activeContainer != null)
        {
            if (m_activeContainer.NodeType == AT_NodeType.kRoot || m_activeContainer.NodeType == AT_NodeType.kStateMachine || m_activeContainer.NodeType == AT_NodeType.kBlendStateMachine )
            {
                if (DrawStateMachineInterface())
                {
                    m_activeContainer.SetDirty();
                }
            }
            else
            {
                if (DrawBlendGraphInterface())
                {
                    m_activeContainer.SetDirty();
                }
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    // Draw the toolkit for editing a blend graph
    bool DrawBlendGraphInterface()
    {
        bool clicked = false;
        GUI.skin.button.imagePosition = ImagePosition.ImageAbove;
        float height = 70;

        if (GUILayout.Button(new GUIContent("Animation", m_icons[2], "Add a new Animation"), GUILayout.MaxWidth(100), GUILayout.MaxHeight(height)))
        {
            AddAnimNode();
            clicked = true;
        }
        GUILayout.Space(5);
		if (GUILayout.Button(new GUIContent("Pose", m_icons[5], "Add a new Pose"), GUILayout.MaxWidth(100), GUILayout.MaxHeight(height)))
        {
            AddPoseNode();
            clicked = true;
        }
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Blend Graph", m_icons[1], "Add a new Blend Graph"), GUILayout.MaxWidth(100), GUILayout.MaxHeight(height)))
        {
            AddBlendContainer();
            clicked = true;
        }
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("State Machine", m_icons[0], "Add a new State Machine"), GUILayout.MaxWidth(100), GUILayout.MaxHeight(height)))
        {
            AddBlendStateMachine();
            clicked = true;
        }
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Blend", m_icons[3], "Add a new Blend"), GUILayout.MaxWidth(100), GUILayout.MaxHeight(height)))
        {
            AddBlendNode();
            clicked = true;
        }
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Additive Blend", m_icons[4], "Add a new Additive Blend"), GUILayout.MaxWidth(100), GUILayout.MaxHeight(height)))
        {
            AddAdditiveBlendNode();
            clicked = true;
        }       

        return clicked;
    }

    // Draw the toolkit for editing a state-machine
    bool DrawStateMachineInterface()
    {
        bool clicked = false;
        float height = 70;
        GUI.skin.button.imagePosition = ImagePosition.ImageAbove;

        if (GUILayout.Button(new GUIContent("Animation", m_icons[2], "Add a new Animation"), GUILayout.MaxWidth(100), GUILayout.MaxHeight(height)))
        {
            AddAnimState();
            clicked = true;
        }
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Blend Graph", m_icons[1], "Add a new Blend Graph"), GUILayout.MaxWidth(100), GUILayout.MaxHeight(height)))
        {
            AddBlendGraph();
            clicked = true;
        }
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("State Machine", m_icons[0], "Add a new State Machine"), GUILayout.MaxWidth(100), GUILayout.MaxHeight(height)))
        {
            AddStateMachine();
            clicked = true;
        }

        return clicked;
    }

    // Create a new data connection in a blend graph
    public void MakeConnection(ConnectionButton button)
    {
        if (ConnectionStart == null)
        {
            // Start new connection
            ConnectionStart = button;
        }
        else
        {
            // Finish and create a new connection
            ConnectionEnd = button;

            ConnectionButton[] buttons = new ConnectionButton[2];
            buttons[0] = ConnectionStart;
            buttons[1] = ConnectionEnd;

            ConnectionButton inConnection = null;
            ConnectionButton outConnection = null;

            // Get references to the in and out connections
            for (int i = 0; i < 2; i++)
            {
                if (buttons[i].m_type == ConnectionButton.ConnectionButtonType.kAnimDataOut || buttons[i].m_type == ConnectionButton.ConnectionButtonType.kControlDataOut)
                {
                    outConnection = buttons[i];
                }
                if (buttons[i].m_type == ConnectionButton.ConnectionButtonType.kAnimDataIn || buttons[i].m_type == ConnectionButton.ConnectionButtonType.kControlDataIn)
                {
                    inConnection = buttons[i];
                }
            }

            // Make sure that we are connecting the same types of data paths together
            bool valid = false;
            if ( inConnection != null && outConnection != null )
            {
                if ((inConnection.m_type == ConnectionButton.ConnectionButtonType.kAnimDataIn && outConnection.m_type == ConnectionButton.ConnectionButtonType.kAnimDataOut) ||
                   (inConnection.m_type == ConnectionButton.ConnectionButtonType.kControlDataIn && outConnection.m_type == ConnectionButton.ConnectionButtonType.kControlDataOut))
                {
                    if (inConnection.ParentWindow != outConnection.ParentWindow)
                    {
                        inConnection.ParentWindow.MakeConnection(outConnection, inConnection);
                        valid = true;
                    }
                }
            }

            if (valid)
            {
                ConnectionStart = null;
            }

            ConnectionEnd = null;

            
        }
    }

    // Detach a node from its current parent and make it an unassigned child of the active container
    public void DetachNode( AT_Node node )
    {
        if (node == null)
        {
            return;
        }

        m_activeContainer.AddChild(node, true);
        node.SetParentBlendWeight(0);

        m_activeContainer.SetDirty();
        node.SetDirty();
    }


    #region Functions To Add In Different Types of Nodes
    /// <summary>
    /// These functions are reactions to clicking buttons in the toolkit and will add new instances
    /// of various nodes to the active container
    /// </summary>
    void AddBlendStateMachine()
    {
        AT_BlendStateMachine node = AT_Factory.CreateNode<AT_BlendStateMachine>("State Machine", m_activeContainer, m_selectedAnimationTree);
        Selection.activeGameObject = node.gameObject;
        m_activeContainer.RemoveChild(node);
        m_activeContainer.AddChild(node, false);
        node.SetLocalBlendWeight(0);
        node.SetParentBlendWeight(0);

        GameObject go = new GameObject("Transitions");
        go.transform.parent = node.transform;

        node.m_transitionsParent = go;

        RebuildTreeGraph();                
    }

    void AddStateMachine()
    {
        AT_StateMachine node = AT_Factory.CreateNode<AT_StateMachine>("State Machine", m_activeContainer, m_selectedAnimationTree);
        Selection.activeGameObject = node.gameObject;
        m_activeContainer.RemoveChild(node);
        m_activeContainer.AddChild(node, false);
        node.SetLocalBlendWeight(0);
        node.SetParentBlendWeight(0);

        GameObject go = new GameObject("Transitions");
        go.transform.parent = node.transform;

        node.m_transitionsParent = go;

        node.SetDirty();

        RebuildTreeGraph();                
    }

    void AddState(AT_State state)
    {
        Selection.activeGameObject = state.gameObject;
        m_activeContainer.RemoveChild(state);
        m_activeContainer.AddChild(state, false);

        AT_StateMachine sm = m_activeContainer as AT_StateMachine;
        if (sm.m_startUpState == null)
        {
            sm.m_startUpState = state;
        }

        state.SetLocalBlendWeight(0);
        state.SetParentBlendWeight(0);
		
		state.SetDirty();
		sm.SetDirty();
                
        RebuildTreeGraph();
    }

    void AddBlendContainer()
    {
		Debug.Log("help");
        AT_BlendContainer node = AT_Factory.CreateNode<AT_BlendContainer>("Blend Graph", m_activeContainer, m_selectedAnimationTree);

        AT_OutputNode output = AT_Factory.CreateNode<AT_OutputNode>("Output", node, m_selectedAnimationTree);
        output.EditorProperties.Position = new Rect(m_editorWindow.position.width - 200, 50, 150, 60);
        node.RemoveChild(output);
        node.AddChild(output, false);

        node.SetParentBlendWeight(0);

        Selection.activeGameObject = node.gameObject;

        node.SetDirty();
        output.SetDirty();

        RebuildTreeGraph();
    }

    void AddBlendGraph()
    {		
        AT_BlendGraph node = AT_Factory.CreateNode<AT_BlendGraph>("Blend Graph", m_activeContainer, m_selectedAnimationTree);

        AT_OutputNode output = AT_Factory.CreateNode<AT_OutputNode>("Output", node, m_selectedAnimationTree);
        output.EditorProperties.Position = new Rect(m_editorWindow.position.width - 200, 50, 150, 60);
        node.RemoveChild(output);
        node.AddChild(output, false);

        node.SetDirty();
        output.SetDirty();

        AddState(node);
    }

    void AddAnimState()
    {
        AT_AnimationState node = AT_Factory.CreateNode<AT_AnimationState>("AnimationState", m_activeContainer, m_selectedAnimationTree);
        AT_Animation anim = node.gameObject.AddComponent<AT_Animation>();
        anim.ParentTree = m_selectedAnimationTree;
        anim.m_subject = m_selectedAnimationTree.m_subject;
        node.m_animation = anim;
        node.Expandable = false;
        m_activeContainer.RemoveChild(node);
        m_activeContainer.AddChild(node, false);

        Rect pos = node.EditorProperties.Position;
        pos.width = 200;
        pos.height = 60;
        node.EditorProperties.Position = pos;

        node.SetDirty();
        anim.SetDirty();

        AddState(node);
    }

    void AddBlendNode()
    {
        AT_Node node = AT_Factory.CreateNode<AT_Blend>("Blend", m_activeContainer, m_selectedAnimationTree);
        Selection.activeGameObject = node.gameObject;
        node.SetDirty();
        RebuildTreeGraph();
    }

    void AddAdditiveBlendNode()
    {
        AT_Node node = AT_Factory.CreateNode<AT_AdditiveBlend>("Additive Blend", m_activeContainer, m_selectedAnimationTree);
        Selection.activeGameObject = node.gameObject;
        node.SetDirty();
        RebuildTreeGraph();
    }

    void AddAnimNode()
    {
        AT_Animation node = AT_Factory.CreateNode<AT_Animation>("Animation", m_activeContainer, m_selectedAnimationTree);
        node.m_subject = m_selectedAnimationTree.m_subject;

        Rect pos = node.EditorProperties.Position;
        pos.width = 200;
        pos.height = 60;
        node.EditorProperties.Position = pos;

        node.SetParentBlendWeight(0);
        node.SetLocalBlendWeight(0);

        Selection.activeGameObject = node.gameObject;

        node.SetDirty();
        RebuildTreeGraph();
    }
	
	void AddPoseNode()
	{
		AT_Pose node = AT_Factory.CreateNode<AT_Pose>("Pose", m_activeContainer, m_selectedAnimationTree);
        node.m_subject = m_selectedAnimationTree.m_subject;

        Rect pos = node.EditorProperties.Position;
        pos.width = 200;
        pos.height = 60;
        node.EditorProperties.Position = pos;

        node.SetParentBlendWeight(0);
        node.SetLocalBlendWeight(0);

        Selection.activeGameObject = node.gameObject;

        node.SetDirty();
        RebuildTreeGraph();
	}
	
    #endregion


}