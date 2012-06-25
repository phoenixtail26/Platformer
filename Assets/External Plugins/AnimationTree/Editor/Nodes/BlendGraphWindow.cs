using UnityEngine;
using System.Collections;


public class BlendGraphWindow : StateWindow
{
    public BlendGraphWindow(Rect position, AT_Node node) : base( position, "Blend Graph", node )
    {
    }

    public override void Draw()
    {
        base.Draw();
       
        GUI.skin = m_skin;

        GUI.Label(ScreenPosition, (TreeNode as AT_State).m_stateName);
    }

}
