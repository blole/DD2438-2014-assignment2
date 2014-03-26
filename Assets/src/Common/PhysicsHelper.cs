using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public static class PhysicsHelper {
		/// <summary>
		/// Randoms a point on the floor, which is neither
		/// closer to the edge nor any other object's edge than radius.
		/// </summary>
		/// <param name="tries">The maximum number to randomize a new point if the first one isn't collision free</param>
		public static Vector3? randomCollisionFreePointOnFloor(float radius, int tries)
		{
			while (tries-- > 0)
			{
				Vector3 point = randomPointOnFloor(radius);

				if (!Physics.CheckSphere(point, radius))
					return point;
			}
			return null;
		}

		/// <summary>
		/// Randoms a random point on floor, which is not
		/// closer to the edge of the floor than radius.
		/// </summary>
		public static Vector3 randomPointOnFloor(float radius)
		{
			Vector2 maxPos = GameObject.Find("floor").collider.bounds.extents.projectDown();
			maxPos -= Vector2.one*radius;

			return maxPos.scale(UnityEngine.Random.Range(-1f,1f), UnityEngine.Random.Range(-1f,1f)).toVector3();
		}

		static Vector3 collisionHeight = Vector3.up*0.3f;
		public static bool isClearPath(Vector3 v, Vector3 u)
		{
			return !Physics.RaycastAll(v+collisionHeight, u-v, (u-v).magnitude).Any(hit=>hit.transform.tag == "obstacle");
		}

		public static bool isClearPath(Vector3 v, Vector3 u, float radius)
		{
			return !Physics.SphereCastAll(v+collisionHeight, radius, u-v, (u-v).magnitude).Any(hit=>hit.transform.tag == "obstacle");
		}
		
		public static bool isClear(Vector3 v, float radius)
		{
			return !Physics.OverlapSphere(v+collisionHeight, radius).Any(o=>o.tag == "obstacle");
		}
	}
}

































