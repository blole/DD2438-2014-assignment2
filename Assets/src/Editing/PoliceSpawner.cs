using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	[ExecuteInEditMode]
	public class PoliceSpawner : MonoBehaviour {
		public int policeCount;
		public Transform policePrefab;

		#if UNITY_EDITOR
		void Update ()
		{
			if (!Application.isPlaying)
				PoliceCount = policeCount;
		}
		#endif

		public int PoliceCount
		{
			get { return policeCount; }
			set {
				if (value < 0)
					value = 0;

				policeCount = transform.childCount;

				while (policeCount > value)
				{
					DestroyImmediate(transform.GetChild(0).gameObject);
					policeCount--;
				}

				if (!Application.isPlaying)
				{
					//refresh all current police
					List<Vector3> positions = transform.children().Select(t=>t.position).ToList();
					List<Quaternion> rotations = transform.children().Select(t=>t.rotation).ToList();
					while(transform.childCount > 0)
						DestroyImmediate(transform.GetChild(0).gameObject);
					for (int i=0; i<positions.Count; i++)
						addPoliceAt(positions[i], rotations[i]);
				}

				while (policeCount < value)
				{
					Vector3? pos = PhysicsHelper.randomCollisionFreePointOnFloor(Waypoints.radius, 10);
					
					if (pos.HasValue)
					{
						Quaternion randomRotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up);
						addPoliceAt(pos.Value, randomRotation);
						policeCount++;
					}
					else
						break;
				}
			}
		}

		private void addPoliceAt(Vector3 pos, Quaternion rotation)
		{
			pos.y = Waypoints.radius;
			Transform t = (Transform) Instantiate(policePrefab, pos, rotation);

			t.localScale = Vector3.one*Waypoints.radius*2;
			t.parent = transform;
		}
	}
}

































