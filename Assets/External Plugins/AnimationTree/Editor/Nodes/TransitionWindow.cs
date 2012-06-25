using UnityEngine;
using System.Collections;

public class TransitionWindow : NodeWindow
{
    AT_StateTransition m_transistionNode;

    Vector2 m_globalStartPosition = new Vector2();
    Vector2 m_globalEndPosition = new Vector2();

    public NodeWindow m_fromWin;
    public NodeWindow m_toWin;

    public int m_fromSideIndex = 0;
    public int m_toSideIndex = 0;

    Vector2 m_startPosOffset = new Vector2();
    Vector2 m_endPosOffset = new Vector2();
    Vector2[] m_fromWinPoints = new Vector2[4];
    Vector2[] m_toWinPoints = new Vector2[4];

	public TransitionWindow( AT_StateTransition trans )
	{
        m_transistionNode = trans;
        TreeNode = trans;
	}

    public override void Destruct()
    {
        AT_StateMachine sm = TreeNode.Parent as AT_StateMachine;
        if (sm != null)
        {
            sm.RemoveTransition(TreeNode as AT_StateTransition);
        }
        else
        {
            Debug.LogWarning("Warning: Failed to remove transition from parent state-machine due to null parent");
        }

        base.Destruct();
    }

    public override void Draw()
    {
        if (m_transistionNode != null)
        {
            m_fromWin = AnimationTreeEditor.instance.GetCorrespondingWindow(m_transistionNode.m_fromState);
            m_toWin = AnimationTreeEditor.instance.GetCorrespondingWindow(m_transistionNode.m_toState);
            
			if ( m_fromWin == null || m_toWin == null )
			{
				return;
			}
			
			m_globalStartPosition = m_fromWin.LocalToGlobalCoord(m_transistionNode.m_startConnection) + m_startPosOffset;
            m_globalEndPosition = m_toWin.LocalToGlobalCoord(m_transistionNode.m_endConnection) + m_endPosOffset;
			
			Vector2 camOffset = AnimationTreeEditor.instance.m_cameraOffset;

            if (Active)
            {
                AnimationTreeEditor.instance.DrawLine(m_globalStartPosition + camOffset, m_globalEndPosition + camOffset, new Color(0.5f, 0.5f, 1, 1), false, 2);
            }
            Color col = Color.white;
            if (m_transistionNode.Progress > 0)
            {
                col = Color.red;
            }
            AnimationTreeEditor.instance.DrawConnectionLine(m_globalStartPosition + camOffset, m_globalEndPosition + camOffset, col, false);

            UpdateLinkPositions();
        }
    }

    public void UpdateLinkOffset(bool from, int index, int count)
    {
        NodeWindow win = m_toWin;
        if (from)
        {
            win = m_fromWin;
        }

        Vector2[] winVec = new Vector2[4];

        winVec[0] = new Vector2(win.Position.width * 0.5f, 0);
        winVec[1] = new Vector2(win.Position.width * 0.5f, 0);
        winVec[2] = new Vector2(0, win.Position.height * 0.5f);
        winVec[3] = new Vector2(0, win.Position.height * 0.5f);

        // Find the right spacing for all the transitions connected to the same side
        float factor = (index - ((count-1) / 2.0f)) * 0.5f;

        if ( from )
        {
            m_startPosOffset = winVec[m_fromSideIndex] * factor;
        }
        else
        {
            m_endPosOffset = winVec[m_toSideIndex] * factor;
        }
    }
    
    void UpdateLinkPositions()
    {
        m_fromWinPoints[0] = new Vector2(m_fromWin.Position.x + m_fromWin.Position.width * 0.5f, m_fromWin.Position.yMin-10);
        m_fromWinPoints[1] = new Vector2(m_fromWin.Position.x + m_fromWin.Position.width * 0.5f, m_fromWin.Position.yMax+10);
        m_fromWinPoints[2] = new Vector2(m_fromWin.Position.xMin-10, m_fromWin.Position.y + m_fromWin.Position.height * 0.5f);
        m_fromWinPoints[3] = new Vector2(m_fromWin.Position.xMax+10, m_fromWin.Position.y + m_fromWin.Position.height * 0.5f);

        m_toWinPoints[0] = new Vector2(m_toWin.Position.x + m_toWin.Position.width * 0.5f, m_toWin.Position.yMin-10);
        m_toWinPoints[1] = new Vector2(m_toWin.Position.x + m_toWin.Position.width * 0.5f, m_toWin.Position.yMax+10);
        m_toWinPoints[2] = new Vector2(m_toWin.Position.xMin-10, m_toWin.Position.y + m_toWin.Position.height * 0.5f);
        m_toWinPoints[3] = new Vector2(m_toWin.Position.xMax+10, m_toWin.Position.y + m_toWin.Position.height * 0.5f);

		float shortestDist = 0;
		int from = 0;
		int to = 0;
		for ( int fromInd = 0; fromInd < 4; fromInd++ )
		{
			for ( int toInd = 0; toInd < 4; toInd++ )
			{
				Vector2 vec = m_toWinPoints[toInd] - m_fromWinPoints[fromInd];
				float mag = vec.SqrMagnitude();
				if ( ( fromInd == 0 && toInd == 0 ) || (mag < shortestDist) )
				{
					shortestDist = mag;
					from = fromInd;
					to = toInd;
				}
			}
		}
		
		m_fromSideIndex = from;
        m_toSideIndex = to;
		
        m_transistionNode.m_startConnection = m_fromWin.GlobalToLocalCoord(m_fromWinPoints[m_fromSideIndex]);
        m_transistionNode.m_endConnection = m_toWin.GlobalToLocalCoord(m_toWinPoints[m_toSideIndex]);
    }

    public override bool IsInWindow(Vector2 position)
    {
		Vector2 camOffset = AnimationTreeEditor.instance.m_cameraOffset;
		
        float dist = AT_Utils.DistanceOfPointToLine(position, m_globalStartPosition + camOffset, m_globalEndPosition + camOffset);
        return (dist < 10);
    }
}
