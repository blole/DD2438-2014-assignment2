using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LibTessDotNet;

namespace Agent
{
	public class Line
	{
		public readonly Vector2 a;
		public readonly Vector2 b;

		public Line(Vector2 a, Vector2 b)
		{
			this.a = a;
			this.b = b;
		}

		public Line(Vector3 a, Vector3 b)
		{
			this.a = a.projectDown();
			this.b = b.projectDown();
		}

		public Vector2? Intersects(Line l)
		{
			Vector2 s1 = b-a;
			Vector2 s2 = l.b-l.a;

			float s = (-s1.y * (a.x - l.a.x) + s1.x * (a.y - l.a.y)) / (-s2.x * s1.y + s1.x * s2.y);
			float t = ( s2.x * (a.y - l.a.y) - s2.y * (a.x - l.a.x)) / (-s2.x * s1.y + s1.x * s2.y);
			
			if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
				return a + s1*t;
			else
				return null;
		}

		public bool isLeft(Vector2 point){
			return ((b.x - a.x)*(point.y - a.y) - (b.y - a.y)*(point.x - a.x)) > 0;
		}

		override public string ToString()
		{
			return "["+a+":"+b+"]";
		}

		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;

			Line line = obj as Line;
			if (line != null)
				return Equals(line);

			return false;
		}

		public bool Equals(Line line)
		{
			return a == line.a && b == line.b ||
			       a == line.b && b == line.a;
		}

		/*public Vector2? Intersects(Ray2D ray)
		{
			Vector2 s1 = b-a;
			Vector2 s2 = l.b-l.a;
			
			float s = (-s1.y * (a.x - l.a.x) + s1.x * (a.y - l.a.y)) / (-s2.x * s1.y + s1.x * s2.y);
			float t = ( s2.x * (a.y - l.a.y) - s2.y * (a.x - l.a.x)) / (-s2.x * s1.y + s1.x * s2.y);
			
			if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
				return a + s1*t;
			else
				return null;
		}*/
	}
}
