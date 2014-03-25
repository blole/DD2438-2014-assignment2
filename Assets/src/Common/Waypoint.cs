using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public class Waypoint {
		public Vector3 pos;
		public int index;

		public bool isEndPoint {get{return index == -1; }}

		public Waypoint(Vector3 pos)
			: this (pos, -1)
		{}

		public Waypoint(Vector3 pos, int index)
		{
			this.pos = pos;
			this.index = index;
		}
	}
}

































