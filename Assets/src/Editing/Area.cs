using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public class Area
	{
		public static int nbAreas = 0;
		public readonly int index;
		public readonly Rect corners;
		
		public Area (Rect corners)
		{
			this.corners = corners;
			this.index = nbAreas;
			nbAreas++;
		}
	}
}
