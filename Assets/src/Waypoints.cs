using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	[ExecuteInEditMode]
	public class Waypoints : MonoBehaviour {
		public bool showWaypoints;
		public bool showNeighbors;
		public float radius;
		public List<Vector3> waypoints = new List<Vector3>();
		public Dictionary<Vector3, List<Vector3>> neighbors = new Dictionary<Vector3, List<Vector3>>();
		private Vector3 offset = Vector3.up * 0.01f;

		void Start()
		{
			updateWaypoints (radius);
		}

#if UNITY_EDITOR
		void Update ()
		{
			updateWaypoints (radius);
			maybeShowWaypoints ();
		}
#endif

		void updateWaypoints(float radius)
		{
			waypoints.Clear();
			neighbors.Clear();

			foreach (GameObject go in GameObject.FindGameObjectsWithTag("obstacle"))
				waypoints.AddRange(go.transform.GetComponent<Collider>().outerEdges(radius*1.01f));

			foreach (Vector3 w in waypoints)
			{
				if (!neighbors.ContainsKey(w))
					neighbors[w] = new List<Vector3>();

				foreach (Vector3 u in waypoints)
				{
					if (w != u && !Physics.SphereCast(new Ray(w, u-w), radius, (u-w).magnitude))
						neighbors[w].Add(u);
				}
			}
		}

		void maybeShowWaypoints()
		{
			if (showWaypoints)
			{
				int segments = 16;
				foreach (Vector3 waypoint in waypoints)
				{
					Vector3 c = waypoint+offset;
					for (int i=0; i<segments; i++)
						Debug.DrawLine(c+Vector3.forward.turn(360f/segments*i)*radius, c+Vector3.forward.turn(360f/segments*(i+1))*radius, Color.green);
				}
			}
			if (showNeighbors)
			{
				foreach (KeyValuePair<Vector3, List<Vector3>> pair in neighbors)
				{
					Vector3 w = pair.Key;
					foreach (Vector3 u in pair.Value)
						Debug.DrawLine(w+offset, u+offset, Color.green);
				}
			}
		}
	}
}

































