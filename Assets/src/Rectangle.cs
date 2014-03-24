using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LibTessDotNet;

namespace Agent
{
	public class Rectangle : Polygon
	{
		public Rectangle(Vector2 topLeft, Vector2 bottomRight)
			: this(
				topLeft,
				new Vector2(bottomRight.x, topLeft.y),
				bottomRight,
				new Vector2(topLeft.x, bottomRight.y)
			)
		{
		}
		public Rectangle(Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft)
			: base(new Vector2[]{
				topLeft,
				topRight,
				bottomRight,
				bottomLeft
			})
		{
		}
	}
}
