using UnityEngine;
using System.Collections;

public class ConnectionButton
{
	public enum Alignment
	{
		kLeft,
		kRight
	}

    public enum ConnectionButtonType
    {
        kAnimDataOut,
        kAnimDataIn,
        kControlDataIn,
        kControlDataOut
    }

	Vector2 buttonSize = new Vector2( 12, 12 );
	public Vector2 LastPosition { get; protected set; }
    
    public Vector2 GUICorrectPosition
    {
        get
        {
            Vector3 vec = new Vector3(LastPosition.x, LastPosition.y);
            vec = GUI.matrix.inverse.MultiplyPoint3x4(vec);
            return new Vector2(vec.x, vec.y);
        }
    }

	string ButtonLabel = "";
	Alignment LabelAlignment = Alignment.kLeft;

    public ConnectionButtonType m_type = ConnectionButtonType.kAnimDataIn;
    NodeWindow m_parentWindow;

    public string Label
    {
        get { return ButtonLabel; }
    }

    public NodeWindow ParentWindow
    {
        get { return m_parentWindow; }
    }

	public ConnectionButton( NodeWindow window, ConnectionButtonType type, string label, Alignment align )
	{
		ButtonLabel = label;
		LabelAlignment = align;
		LastPosition = new Vector2();
        m_type = type;
        m_parentWindow = window;
	}

	public bool Draw( float x, float y )
	{
		LastPosition = new Vector2( x + buttonSize.x/2.0f, y + buttonSize.y / 2.0f );

		TextAnchor oldAnchor = GUI.skin.label.alignment;

        Color oldColor = GUI.color;
        if (m_type == ConnectionButtonType.kAnimDataIn || m_type == ConnectionButtonType.kAnimDataOut)
        {
            GUI.color = new Color(0.5f, 1, 0.5f, 1);
        }
        else
        {
            GUI.color = new Color(0.5f, 0.5f, 1.0f, 1);
        }

		if ( LabelAlignment == Alignment.kRight )
		{
			GUI.skin.label.alignment = TextAnchor.UpperLeft;
			GUI.Label( new Rect( x + 3, y - buttonSize.y, 100, 30 ), ButtonLabel );
		}
		else
		{
			GUI.skin.label.alignment = TextAnchor.UpperRight;
			GUI.Label( new Rect( x - 100 - 5, y - buttonSize.y, 100, 30 ), ButtonLabel );
		}

        GUI.color = oldColor;

		GUI.skin.label.alignment = oldAnchor;

		if ( GUI.Button( new Rect( x, y, buttonSize.x, buttonSize.y ), "" ) )
		{
			//AnimationTreeEditor.instance.StartConnection( this );
			return true;
		}

		return false;
	}
}
