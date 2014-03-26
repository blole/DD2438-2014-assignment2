using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LibTessDotNet;

namespace Agent
{
	public static class Extensions
	{
		public static Vector2 projectDown(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		public static Vector3 toVector3(this Vector2 v)
		{
			return new Vector3 (v.x, 0, v.y);
		}
		
		public static Vector3 toVector3(this Vec3 v)
		{
			return new Vector3(v.X, v.Y, v.Z);
		}

		public static Vec3 toVec3(this Vector3 v)
		{
			return new Vec3 {X=v.x, Y=v.y, Z=v.z};
		}
		
		public static ContourVertex[] toContour(this IEnumerable<Vector3> vs)
		{
			return vs.Select(v=>new ContourVertex {Position = v.toVec3()}).ToArray();
		}
		
		public static Vector3[] toVector3(this IEnumerable<ContourVertex> vs)
		{
			return vs.Select(v=>v.Position.toVector3()).ToArray();
		}
		
		public static Vector3[] edges(this Collider collider)
		{
			Vector3[] edges;

			if (collider is BoxCollider)
			{
				BoxCollider box = collider as BoxCollider;
				edges = new Vector3[4];
				edges[0] = box.size.scale(-.5f, 0,  .5f);
				edges[1] = box.size.scale( .5f, 0,  .5f);
				edges[2] = box.size.scale( .5f, 0, -.5f);
				edges[3] = box.size.scale(-.5f, 0, -.5f);

				for (int i=0; i<edges.Length; i++)
					edges[i] = collider.transform.TransformPoint(box.center+edges[i]).scale(1,0,1);
			}
			else if (collider is MeshCollider)
			{
				MeshCollider mesh = collider as MeshCollider;
				edges = new Vector3[4];
				edges[0] = mesh.bounds.extents.scale(-1, 0,  1);
				edges[1] = mesh.bounds.extents.scale( 1, 0,  1);
				edges[2] = mesh.bounds.extents.scale( 1, 0, -1);
				edges[3] = mesh.bounds.extents.scale(-1, 0, -1);
			}
			else
				throw new NotImplementedException("Collider.edges() only works for boxes");

			return edges;
		}

		public static Vector3[] outerEdges(this Collider collider, float radius)
		{
			BoxCollider box = collider as BoxCollider;
			if (box != null)
			{
				Vector3[] edges = new Vector3[4];
				edges[0] = new Vector3(-box.size.x/2-radius/box.transform.localScale.x, 0,  box.size.z/2+radius/box.transform.localScale.z);
				edges[1] = new Vector3( box.size.x/2+radius/box.transform.localScale.x, 0,  box.size.z/2+radius/box.transform.localScale.z);
				edges[2] = new Vector3( box.size.x/2+radius/box.transform.localScale.x, 0, -box.size.z/2-radius/box.transform.localScale.z);
				edges[3] = new Vector3(-box.size.x/2-radius/box.transform.localScale.x, 0, -box.size.z/2-radius/box.transform.localScale.z);

				Vector3 bottomCenter = box.center-new Vector3(0, box.size.y/2, 0);
				for (int i=0; i<4; i++)
					edges[i] = box.transform.TransformPoint(bottomCenter+edges[i]);
				return edges;
			}
			else
				throw new NotImplementedException("Collider.outerEdges() only works for boxes");
		}

		public static IEnumerable<Transform> children(this Transform transform)
		{
			foreach (Transform child in transform)
				yield return child;
		}





		public static Vector3 scale(this Vector3 v, float x, float y, float z)
		{
			return new Vector3(v.x*x, v.y*y, v.z*z);
		}
		
		public static Vector3 scale(this Vector3 v, Vector3 u)
		{
			return v.scale(u.x, u.y, u.z);
		}
		
		public static Vector2 scale(this Vector2 v, float x, float y)
		{
			return new Vector2(v.x*x, v.y*y);
		}
		
		public static Vector2 project(this Vector2 a, Vector2 onto)
		{
			onto.Normalize();
			return onto*Vector2.Dot(a, onto);
		}
		
		public static float angleBetween(this Vector2 v, Vector2 u)
		{
			return (v.angle()-u.angle()).mod(-180, 180);
		}
		
		public static Vector2 sign(this Vector2 v)
		{
			return new Vector2(Mathf.Sign(v.x), Mathf.Sign(v.y));
		}
		
		public static Vector2 sign(this Vector2 v, Vector2 up)
		{
			v = v.turn(-up.angle());
			return new Vector2(Mathf.Sign(v.x), Mathf.Sign(v.y));
		}

		public static float angle(this Vector2 v)
		{
			return Mathf.Atan2(v.y, v.x)*180/Mathf.PI;
		}

		public static Vector2 turn(this Vector2 v, float degrees)
		{
			return Quaternion.AngleAxis(degrees, new Vector3(0,0,1)) * v;
		}
		
		public static Vector2 Clamp(this Vector2 v, float max)
		{
			return v.normalized*Mathf.Clamp(v.magnitude, 0, max);
		}
		
		public static Vector2 Clamp(this Vector2 v, float min, float max)
		{
			return v.normalized*Mathf.Clamp(v.magnitude, min, max);
		}
		
		public static Vector3 Clamp(this Vector3 v, float max)
		{
			return v.normalized*Mathf.Clamp(v.magnitude, 0, max);
		}
		
		public static Vector3 Clamp(this Vector3 v, float min, float max)
		{
			return v.normalized*Mathf.Clamp(v.magnitude, min, max);
		}



		public static float mod(this float f, float rhs)
		{
			return ((f%rhs)+rhs)%rhs;
		}
		public static float mod(this float f, float min, float max)
		{
			return (f-min).mod(max-min) + min;
		}
	}
}