using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LibTessDotNet;

namespace Agent
{
	public class Triangle
	{
		public readonly Vector2 a;
		public readonly Vector2 b;
		public readonly Vector2 c;

		public Triangle(Vector2 a, Vector2 b, Vector2 c)
		{
			this.a = a;
			this.b = b;
			this.c = c;
		}

		public IEnumerable<Vector2> Intersects(Line l)
		{
			foreach (Line line in lines())
			{
				Vector2? intersection = l.Intersects(line);
				if (intersection != null)
					yield return (Vector2)intersection;
			}
		}
		
		public bool contains(Vector2 point)
		{
			return lines().All(line=>!line.isLeft(point));
		}

		public IEnumerable<Line> lines()
		{
			yield return new Line(a, b);
			yield return new Line(b, c);
			yield return new Line(c, a);
		}

		public Vector2 Center {get{return (a+b+c)/3; }}
	}
}
