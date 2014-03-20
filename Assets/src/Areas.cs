using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LibTessDotNet;

namespace Agent
{
	[ExecuteInEditMode]
	public class Areas : MonoBehaviour {

		public bool showAreas;
		private List<List<Vector2>> areas = new List<List<Vector2>>();
		public ConvexArea convexAreaPrefab;

		void Start ()
		{
			updateAreas();
		}
		
#if UNITY_EDITOR
		void Update ()
		{
			updateAreas();
		}
#endif

		void updateAreas()
		{
			areas.Clear();
			foreach (Transform t in transform)
				DestroyImmediate(t.gameObject);

			GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
			Vector3[] vertices = new Vector3[obstacles.Length*4];
			Tess tess = new Tess();

			for (int i=0; i<obstacles.Length; i++)
			{
				Vector3[] edges = obstacles[i].collider.edges();

				for (int j=0; j<4; j++)
					vertices[4*i+j] = edges[j];
				tess.AddContour(edges.toVec3().Select(v=>new ContourVertex {Position = v}).ToArray(), ContourOrientation.CounterClockwise);
			}

			tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

			ConvexArea area = ((ConvexArea)Instantiate(convexAreaPrefab));
			area.transform.parent = transform;
			Mesh mesh = area.GetComponent<MeshFilter>().mesh;
			mesh.vertices = tess.Vertices.Select(v=>v.Position.toVector3()).ToArray();
			mesh.uv = Enumerable.Repeat(Vector2.zero, mesh.vertexCount).ToArray();
			mesh.triangles = tess.Elements;
			//Debug.Log("vertexCount:"+vertices.Length);
			//Debug.Log("triangleCount:"+tess.Elements.Length);
			//Debug.Log(tess.Elements.Aggregate("triangles:", (s,i)=>s+" "+i));
			mesh.RecalculateNormals();


			//List<Vector3> wps = GameObject.Find("waypoints").GetComponent<Waypoints>().waypoints;
			//foreach (Vector3 wp in wps)
			//	Debug.Log(area.TryAddPoint(wp));
		}
	}
}
