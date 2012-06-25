using UnityEngine;
using System.Collections;


public class StateMachineWindow : StateWindow
{
    public StateMachineWindow(Rect position, AT_Node node) : base( position, "State Machine", node )
    {
    }

    public override void Draw()
    {
        base.Draw();
       
        GUI.skin = m_skin;

        GUI.Label(ScreenPosition, (TreeNode as AT_State).m_stateName);
    }

}
