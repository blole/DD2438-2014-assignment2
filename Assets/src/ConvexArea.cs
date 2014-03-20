using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	[ExecuteInEditMode]
	public class ConvexArea : MonoBehaviour {
		private List<Vector3> corners = new List<Vector3>();

		void Start ()
		{
			GetComponent<MeshFilter>().mesh = new Mesh();
		}

		void Update()
		{
			/*Vector3[] vertices = mesh.vertices;
			Vector3[] normals = mesh.normals;
			int i = 0;
			while (i < vertices.Length) {
				vertices[i] += normals[i] * Mathf.Sin(Time.time*3) * 0.01f;
				i++;
			}
			mesh.vertices = vertices;*/

			Vector3[] vertices = new Vector3[10];
			Vector2[] uv = new Vector2[10];
			int[] triangles = new int[15];

			for (int i=0; i<vertices.Length; i++)
			{
				uv[i] = vertices[i] = UnityEngine.Random.onUnitSphere;
			}

			//for (int i=0; i<triangles.Length; i++)
			//	triangles[i] = UnityEngine.Random.Range(1, 3);

			Mesh mesh = GetComponent<MeshFilter> ().mesh = new Mesh();
			mesh.RecalculateNormals();
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.triangles = triangles;
			GetComponent<MeshFilter> ().mesh = mesh;
			//((MeshCollider)collider).sharedMesh = mesh;*/

			Debug.Log(GameObject.FindGameObjectsWithTag("obstacle").Select(go => go.rigidbody).Any(r => r.SweepTestAll (Vector3.down).Any(hit => hit.collider == collider)));
		}

		public bool TryAddPoint(Vector3 v)
		{
			int insertAt = -1;

			if (corners.Count < 2)
				insertAt = corners.Count;
			else
			{
				for (int a=0; a<corners.Count; a++)
				{
					int b = (a+1)%corners.Count;
					int c = (b+1)%corners.Count;
					int d = (c+1)%corners.Count;

					if (Vector2.Angle((corners[a]-corners[b]).projectDown(),          (v-corners[b]).projectDown()) <= 180 &&
					    Vector2.Angle(         (v-corners[c]).projectDown(), (corners[d]-corners[c]).projectDown()) <= 180)
					{
						insertAt = c;
						break;
					}
				}
			}

			if (insertAt == -1)
			    return false;
		    else
		    {
				InsertVertex(v, insertAt);
			    return true;
			}
		}
		
		private void InsertVertex(Vector3 vertice, int index)
		{
			corners.Insert(index, vertice);

			Mesh mesh = GetComponent<MeshFilter> ().mesh;

			mesh.vertices = corners.ToArray();
			mesh.uv = corners.Select(v=>v.projectDown()).ToArray();

			int[] triangles = new int[Mathf.Max((corners.Count - 2) * 3, 0)];
			for (int i=0; i<triangles.Length/3; i++)
			{
				triangles[i*3+0] = 0;
				triangles[i*3+1] = i+1;
				triangles[i*3+2] = i+2;
			}

			mesh.RecalculateNormals();
			GetComponent<MeshFilter>().mesh = mesh;
			Debug.Log(mesh.vertexCount);
		}
	}
}
