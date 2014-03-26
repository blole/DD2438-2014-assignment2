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
		private static Waypoints singleton;
		public static float radius {get{return singleton==null?0:singleton.vehicleRadius; }}
		public static List<Waypoint> waypoints = new List<Waypoint>();
		public static Dictionary<Waypoint, List<Waypoint>> neighbors = new Dictionary<Waypoint, List<Waypoint>>();
		private Vector3 offset = Vector3.up * 0.01f;

		void Awake()
		{
			singleton = this;
		}

		void Start()
		{
			updateWaypoints();
		}

#if UNITY_EDITOR
		void Update ()
		{
			if (!Application.isPlaying)
			{
				singleton = this;
				updateWaypoints();
				drawWaypoints(showWaypoints);
				drawNeighbors(showNeighbors);
			}
		}
#endif

		public static void updateWaypoints()
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
					if (v != u && PhysicsHelper.isClearPath(v.pos, u.pos, radius))
						neighbors[v].Add(u);
				}
			}
		}



		void drawWaypoints(bool show)
		{
			if (show)
			{
				int segments = 16;
				foreach (Waypoint waypoint in waypoints)
				{
					Vector3 c = waypoint.pos+offset;
					for (int i=0; i<segments; i++)
						Debug.DrawLine(c+Vector2.up.turn(360f/segments*i).toVector3()*radius, c+Vector2.up.turn(360f/segments*(i+1)).toVector3()*radius, Color.green);
				}
			}
		}
		void drawNeighbors(bool show)
		{
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

































