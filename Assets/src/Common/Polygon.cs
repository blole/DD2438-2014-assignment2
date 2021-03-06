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

		public Vector2 Center {get{return new Vector2(points.Average(v=>v.x), points.Average(v=>v.y)); }}


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

		public Vector2? firstEntry(Line l)
		{
			var intersections = Intersects(l).OrderBy(p=>(l.a-p).sqrMagnitude);

			if (intersections.Count() >= 2 && !intersections.First().almostEqual(intersections.Last()))
				return intersections.First();
			else
				return null;
		}

		public bool contains(Vector2 point)
		{
			return lines().All(line=>!line.isLeft(point));
		}

		/*public bool isConvex()
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
		}*/

		List<Line> linesCache = null;
		public List<Line> lines()
		{
			if (linesCache == null)
			{
				linesCache = new List<Line>();
				for (int i=0; i<points.Length; i++)
					linesCache.Add(new Line(points[i], points[(i+1)%points.Length]));
			}
			return linesCache;
		}
	}
}
