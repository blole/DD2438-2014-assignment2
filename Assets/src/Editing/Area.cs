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

		public Vector3[] getAreaCoord ()
		{
				float xmin = this.corners.xMin;
				float ymin = this.corners.yMin;
				float xmax = this.corners.xMax;
				float ymax = this.corners.yMax;
				Vector3 bottomLeft = new Vector3 (xmin, 0.02f, ymin);
				Vector3 bottomRight = new Vector3 (xmax, 0.02f, ymin);
				Vector3 topLeft = new Vector3 (xmin, 0.02f, ymax);
				Vector3 topRight = new Vector3 (xmax, 0.02f, ymax);
				Vector3[] result = new Vector3[4];
				result [0] = bottomLeft;
				result [1] = bottomRight;
				result [2] = topLeft;
				result [3] = topRight;
				return result;
		}
	}
}
