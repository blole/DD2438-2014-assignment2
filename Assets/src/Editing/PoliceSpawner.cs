using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	[ExecuteInEditMode]
	public class PoliceSpawner : MonoBehaviour {
		[Range(0,10)]
		public int policeCount;
		public Transform policePrefab;
		public int behavior;

		#if UNITY_EDITOR
		void Update ()
		{
			if (!Application.isPlaying){
				PoliceCount = policeCount;
				Behavior = behavior;
			}
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
					DestroyImmediate(transform.GetChild(policeCount-1).gameObject);
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

		public int Behavior
		{
			get { return behavior; }
			set {
				if(value < 0)
					value = 0;
				if (value > 3)
					value = 3;

				// Refresh all police behaviors
				foreach(Transform p in transform.children()){
					Police currentScript = (Police)p.GetComponent("Police");
					currentScript.guardType = value;
				}

				behavior = value;
			}
		}

		private void addPoliceAt(Vector3 pos, Quaternion rotation)
		{
			pos.y = Waypoints.radius;
			Transform t = (Transform) Instantiate(policePrefab, pos, rotation);
			((Police)t.GetComponent ("Police")).guardType = behavior;
			((ShowFOV)t.GetComponent ("ShowFOV")).show = MyGUI.FOVstatus;
			t.localScale = Vector3.one*Waypoints.radius*2;
			t.parent = transform;
		}
	}
}

































