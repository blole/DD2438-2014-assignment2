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
				setPoliceCount(policeCount);
		}
		#endif
		
		void setPoliceCount(int newCount)
		{
			if (newCount < 0)
				newCount = 0;

			policeCount = transform.childCount;

			while (policeCount > newCount)
			{
				DestroyImmediate(transform.GetChild(0).gameObject);
				policeCount--;
			}

			List<Vector3> positions = transform.children().Select(t=>t.position).ToList();

			while(transform.childCount > 0)
				DestroyImmediate(transform.GetChild(0).gameObject);

			foreach (Vector3 pos in positions)
				addPoliceAt(pos);

			Vector2 maxPos = GameObject.Find("floor").collider.bounds.extents.projectDown();
			float policeRadius = (policePrefab.collider as SphereCollider).radius*policePrefab.localScale.magnitude;
			maxPos -= Vector2.one*policeRadius;

			while (policeCount < newCount)
			{
				Vector3? pos = PhysicsHelper.randomCollisionFreePointOnFloor(policeRadius, 5);
				
				if (pos.HasValue)
				{
					addPoliceAt(pos.Value);
					policeCount++;
				}
				else
					break;
			}
		}

		void addPoliceAt(Vector3 pos)
		{
			pos.y = Waypoints.radius;
			Quaternion q = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up);
			Transform t = (Transform) Instantiate(policePrefab, pos, q);

			t.localScale = Vector3.one*Waypoints.radius*2;
			t.parent = transform;
		}
	}
}

































