using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LibTessDotNet;

namespace Agent
{
	public class Triangle : Polygon
	{
		public Triangle(Vector2 a, Vector2 b, Vector2 c)
			: base(new Vector2[]{a, b, c})
		{
		}

		public Vector2 Center {get{return (points[0]+points[1]+points[2])/3; }}
	}
}
