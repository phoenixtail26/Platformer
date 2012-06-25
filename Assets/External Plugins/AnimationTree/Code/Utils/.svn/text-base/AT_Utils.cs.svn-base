////////////////////////////////////////////////////////////////////////////////////////
// 
// File:        AT_Utils.cs
// Author:      Gavin Hayler
// Date:        23/06/2011
//
////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

////////////////////////////////////////////////////////////////////////////////////////
// 
// Class:       AT_Utils
// Description: Static class with functions useful for general operations and 
//              maintainance
//
////////////////////////////////////////////////////////////////////////////////////////
public static class AT_Utils
{
    // Resizes and adds a new element to a staticly sized array
	public static T[] AddElementToArray<T>( T element, T[] array ) where T : class
	{
		int length = 1;
		if ( array != null )
		{
			length = array.Length + 1;
		}

		T[] tempArray = new T[length];

		if ( array != null )
		{
			array.CopyTo( tempArray, 0 );
		}
		
		tempArray[length-1] = element;

		return tempArray;
	}

    // Determine the closest point on a line to the passed point
    public static void ClosestPointOnLine(Vector2 point, Vector2 lineP1, Vector2 lineP2, out Vector2 closestPoint)
    {
        if (lineP1 == lineP2)
        {
            closestPoint = lineP1;
            return;
        }

        Vector2 ab = lineP2 - lineP1;
        Vector2 ac = point - lineP1;

        // ac projected onto ab
        float t = Vector2.Dot(ab, ac) / Vector2.Dot(ab, ab);

        // Clamp t into the range of the line segment
        t = Mathf.Clamp(t, 0, 1);

        closestPoint = lineP1 + ab * t;
    }

    // The distance from a passed point to the closest point on the passed line
    public static float DistanceOfPointToLine(Vector2 point, Vector2 lineP1, Vector2 lineP2)
    {
        Vector2 closestPoint;
        AT_Utils.ClosestPointOnLine(point, lineP1, lineP2, out closestPoint);

        Vector2 diff = closestPoint - point;

        return diff.magnitude;
    }
}
