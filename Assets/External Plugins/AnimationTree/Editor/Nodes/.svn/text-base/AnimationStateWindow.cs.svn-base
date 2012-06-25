using UnityEngine;
using System.Collections;


public class AnimationStateWindow : StateWindow
{
    string m_AnimationName = "";

    public AnimationStateWindow(Rect position, string animation, AT_Node node) : base(position, "Animation", node)
    {
        m_AnimationName = animation;
    }

    public override void Draw()
    {
        base.Draw();

        GUI.skin = m_skin;
        Rect position = ScreenPosition;

        GUI.Label(new Rect(position.x, position.y, position.width - 50, position.height), m_AnimationName);
    }

}