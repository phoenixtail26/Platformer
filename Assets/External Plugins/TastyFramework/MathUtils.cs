using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class MathUtils
    {
        public static float LAMBDA = 0.00001f;

        public static bool LineSegmentToLineSegmentIntersection( Vector3 line1P1, Vector3 line1P2, Vector3 line2P1, Vector3 line2P2, out Vector3 intersectPoint )
        {
            float r, s;
            if ( LineToLineIntersection( line1P1, line1P2, line2P1, line2P2, out r, out s ) )
            {
                if ( r >= 0 && r <= 1 &&
                     s >= 0 && s <= 1 )
                {
                    intersectPoint = line1P1 + ( ( line1P2 - line1P1 ) * r );
                    return true;
                }
            }
            intersectPoint = new Vector3();
            return false;
        }

        public static bool LineToLineIntersection( Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out float r, out float s )
        {
            float d = 0;

            Vector3 v1 = p2 - p1;
            Vector3 v2 = p4 - p3;

            if ( v1.z / v1.x != v2.z / v2.x )
            {
                d = v1.x * v2.z - v1.z * v2.x;

                if ( d != 0 )
                {
                    Vector3 v3 = p1 - p3;
                    r = ( v3.z * v2.x - v3.x * v2.z ) / d;
                    s = ( v3.z * v1.x - v3.x * v1.z ) / d;
                    return true;
                }
            }
            r = 0;
            s = 0;
            return false;
        }

        // NB: The ray is not considered to be intersecting if its start point is on the line
        public static bool RayToLineSegmentIntersection( Vector3 rayP1, Vector3 rayP2, Vector3 lineP1, Vector3 lineP2 )
        {
            float r, s;
            if ( LineToLineIntersection( rayP1, rayP2, lineP1, lineP2, out r, out s ) )
            {
                if ( r > LAMBDA )
                {
                    if ( s >= 0 && s <= 1 )
                    {
                        //result.InsertSolution(vertex1 + (vertex2 - vertex1) * r);
                        // Can calculate the intersection point by using:
                        // intersectPoint = rayP1 + ( (rayP2 - rayP1) * r );
                        return true;
                    }
                }
            }
            return false;
        }
		
		public static void ClosestPointOnLine( Vector3 point, Vector3 lineP1, Vector3 lineP2, out Vector3 closestPoint )
		{
			float t; 
			ClosestPointOnLine( point, lineP1, lineP2, out closestPoint, out t );
		}
		
        public static void ClosestPointOnLine( Vector3 point, Vector3 lineP1, Vector3 lineP2, out Vector3 closestPoint, out float t )
        {
            if ( lineP1 == lineP2 )
            {
                closestPoint = lineP1;
				t = 0;
                return;
            }

            Vector3 ab = lineP2 - lineP1;
            Vector3 ac = point - lineP1;

            // ac projected onto ab
            t = Vector3.Dot( ab, ac ) / Vector3.Dot( ab, ab );

            // Clamp t into the range of the line segment
            t = Mathf.Clamp( t, 0, 1 );

            closestPoint = lineP1 + ab * t;
        }

        public static bool ClosestPointOnLineToLine( Vector3 line1P1, Vector3 line1P2, Vector3 line2P1, Vector3 line2P2, out Vector3 closestPoint )
        {
            float r, s;
            if ( LineToLineIntersection( line1P1, line1P2, line2P1, line2P2, out r, out s ) )
            {
                r = Mathf.Clamp( r, 0, 1 );
                closestPoint = line1P1 + ( ( line1P2 - line1P1 ) * r );
                return true;
            }
            else
            {
                // TODO: this is dodgy maths
                float dist = 0;
                Vector3 closest = line1P1;
                dist = ( line1P1 - line2P1 ).sqrMagnitude;

                Vector3 diff = line1P1 - line2P2;
                if ( diff.sqrMagnitude < dist )
                {
                    closest = line1P1;
                    dist = diff.sqrMagnitude;
                }
                diff = line1P2 - line2P1;
                if ( diff.sqrMagnitude < dist )
                {
                    closest = line1P2;
                    dist = diff.sqrMagnitude;
                }
                diff = line1P2 - line2P2;
                if ( diff.sqrMagnitude < dist )
                {
                    closest = line1P2;
                    dist = diff.sqrMagnitude;
                }

                closestPoint = closest;
                return false;
            }
        }
    }
}