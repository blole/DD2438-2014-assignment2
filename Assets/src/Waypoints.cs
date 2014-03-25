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
		public float vehicleRadius;
		public static float radius;
		public static List<Waypoint> waypoints = new List<Waypoint>();
		public static Dictionary<Waypoint, List<Waypoint>> neighbors = new Dictionary<Waypoint, List<Waypoint>>();
		private Vector3 offset = Vector3.up * 0.01f;

		void Start()
		{
			radius = vehicleRadius;
			updateWaypoints();
		}

#if UNITY_EDITOR
		void Update ()
		{
			if (Application.isEditor)
			{
				radius = vehicleRadius;
				updateWaypoints();
				maybeShowWaypoints();
			}
		}
#endif

		void updateWaypoints()
		{
			waypoints.Clear();
			neighbors.Clear();

			foreach (GameObject go in GameObject.FindGameObjectsWithTag("obstacle"))
				waypoints.AddRange(go.transform.GetComponent<Collider>().outerEdges(radius*1.01f).Select(v=>new Waypoint(v, waypoints.Count)));

			foreach (Waypoint v in waypoints)
			{
				if (!neighbors.ContainsKey(v))
					neighbors[v] = new List<Waypoint>();

				foreach (Waypoint u in waypoints)
				{
					if (v != u && isClearPath(v.pos, u.pos, radius))
						neighbors[v].Add(u);
				}
			}
		}

		public static bool isClearPath(Vector3 v, Vector3 u, float radius)
		{
			return !Physics.SphereCastAll(new Ray(v, u-v), radius, (u-v).magnitude).Any(hit=>hit.transform.CompareTag("obstacle"));
		}

		void maybeShowWaypoints()
		{
			if (showWaypoints)
			{
				int segments = 16;
				foreach (Waypoint waypoint in waypoints)
				{
					Vector3 c = waypoint.pos+offset;
					for (int i=0; i<segments; i++)
						Debug.DrawLine(c+Vector2.up.turn(360f/segments*i).toVector3()*radius, c+Vector2.up.turn(360f/segments*(i+1)).toVector3()*radius, Color.green);
				}
			}
			if (showNeighbors)
			{
				foreach (KeyValuePair<Waypoint, List<Waypoint>> pair in neighbors)
				{
					Waypoint w = pair.Key;
					foreach (Waypoint u in pair.Value)
						Debug.DrawLine(w.pos+offset, u.pos+offset, Color.green);
				}
			}
		}
	}
}

































