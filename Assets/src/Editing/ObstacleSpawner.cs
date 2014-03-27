using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	[ExecuteInEditMode]
	public class ObstacleSpawner : MonoBehaviour {
		[Range(0,30)]
		public int obstacles = 4;
		public Transform obstaclePrefab;
		public float minSide = 0.5f;
		public float maxSide = 4f;
		public float height = 1f;

		#if UNITY_EDITOR
		void Update ()
		{
			if (!Application.isPlaying)
				ObstacleCount = obstacles;
		}
		#endif

		public int ObstacleCount {
			get { return transform.childCount; }
			set
			{
				value = Mathf.Max(value, 0);
				while (ObstacleCount > value)
					DestroyImmediate(transform.GetChild(ObstacleCount-1).gameObject);

				while (ObstacleCount < value)
					spawnObstacle();

				Waypoints.updateWaypoints();

				obstacles = ObstacleCount;
			}
		}

		private void spawnObstacle()
		{
			float x = Mathf.Pow(UnityEngine.Random.Range(0f,1f),2)*(maxSide-minSide)+minSide;
			float z = Mathf.Pow(UnityEngine.Random.Range(0f,1f),2)*(maxSide-minSide)+minSide;

			Transform obstacle = Instantiate(obstaclePrefab) as Transform;
			obstacle.parent = transform;

			obstacle.localScale = new Vector3(x, height, z);
			obstacle.position = PhysicsHelper.randomPointOnFloor(0).setY(height/2);
		}
	}
}
