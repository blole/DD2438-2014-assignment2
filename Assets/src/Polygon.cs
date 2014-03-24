using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LibTessDotNet;

namespace Agent
{
	public class Polygon
	{
		public static readonly Polygon Null = new Polygon(new Vector2[]{});

		public readonly Vector2[] points;

		public Polygon(Vector2[] points)
		{
			this.points = points;
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

		public Polygon JoinToConvex (Triangle tri)
		{
			if (points.Length == 0)
				return tri;

			int insertionIndex = 1;
			foreach (Line line in lines())
			{
				int index = tri.lines().ToList().IndexOf(line);
				if (index != -1)
				{
					List<Vector2> newPoints = points.ToList();
					newPoints.Insert(insertionIndex, tri.points[(index+2)%3]);
					Polygon newPoly = new Polygon(newPoints.ToArray());
					if (newPoly.isConvex())
						return newPoly;
				}
				insertionIndex++;
			}
			return null;
		}

		public bool contains(Vector2 point)
		{
			return lines().All(line=>!line.isLeft(point));
		}

		public bool isConvex()
		{
			var firstTwo = points.Take(2);
			Vector3 prev = firstTwo.First();
			Vector3 curr = firstTwo.Last();

			foreach (Vector3 next in points.Skip(2).Concat(firstTwo))
			{
				if ((curr-prev).angle() < (next-curr).angle())
					return false;
				prev = curr;
				curr = next;
			}
			
			return true;
		}

		public IEnumerable<Line> lines()
		{
			for (int i=0; i<points.Length; i++)
				yield return new Line(points[i], points[(i+1)%points.Length]);
		}
	}
}
