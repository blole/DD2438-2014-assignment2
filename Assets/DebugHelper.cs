using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	static public class DebugHelper
	{
		static public void DrawCircle(Vector3 origin, float radius, int segments, Color color)
		{
			for (int i=0; i<segments; i++)
				Debug.DrawLine(origin+Vector2.up.turn(360f/segments*i).toVector3()*radius, origin+Vector2.up.turn(360f/segments*(i+1)).toVector3()*radius, color);
		}
		
		static public void DrawLine(Line line, float height, Color color)
		{
			DrawLine (line.a, line.b, height, color);
		}
		
		static public void DrawLine(Vector2 a, Vector2 b, float height, Color color)
		{
			Debug.DrawLine(a.toVector3().setY(height), b.toVector3().setY(height), color);
		}
	}
}
