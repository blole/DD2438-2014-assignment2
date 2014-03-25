using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	[ExecuteInEditMode]
	public class Police : MonoBehaviour {
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
			float policeRadius = (policePrefab.collider as SphereCollider).radius;
			maxPos -= Vector2.one*policeRadius;

			while (policeCount < newCount)
			{
				int tries;
				for (tries=0; tries<5; tries++)
				{
					Vector3 pos = maxPos.scale(UnityEngine.Random.Range(-1f,1f), UnityEngine.Random.Range(-1f,1f)).toVector3();

					if (!Physics.CheckSphere(pos, policeRadius))
					{
						Transform t = (Transform) Instantiate(policePrefab, pos + Vector3.up*policeRadius, UnityEngine.Random.rotationUniform);
						t.parent = transform;
						policeCount++;
						break;
					}
				}
				if (tries == 5)
					break;
			}
		}
	}
}

































