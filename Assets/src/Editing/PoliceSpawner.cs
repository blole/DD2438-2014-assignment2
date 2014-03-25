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

		void Start()
		{
			setPoliceCount(policeCount);
		}
		
		#if UNITY_EDITOR
		void Update ()
		{
			if (Application.isEditor)
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

			Vector2 maxPos = GameObject.Find("floor").collider.bounds.extents.projectDown();
			float policeRadius = (policePrefab.collider as SphereCollider).radius*policePrefab.localScale.magnitude;
			maxPos -= Vector2.one*policeRadius;

			while (policeCount < newCount)
			{
				Vector3? pos = PhysicsHelper.randomCollisionFreePointOnFloor(policeRadius, 5);
				
				if (!pos.HasValue)
					break;
					
				Transform t = (Transform) Instantiate(policePrefab,
				                                      pos.Value + Vector3.up*/*t.up**/policeRadius,
				                                      Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up));
				t.parent = transform;
				policeCount++;
			}
		}
	}
}

































