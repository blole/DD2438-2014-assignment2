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
		public MeshFilter areaPrefab;
		
		void Start ()
		{
			updateAreas();
		}
		
#if UNITY_EDITOR
		void Update ()
		{
			if (!Application.isPlaying)
				updateAreas();
		}
#endif

		void updateAreas()
		{
			areas.Clear();
			foreach (Transform t in transform)
				DestroyImmediate(t.gameObject);

			Tess floor = getFloorTess();

			if (showAreas)
			{
				MeshFilter floorArea = ((MeshFilter)Instantiate(areaPrefab));
				floorArea.transform.parent = transform;
				floorArea.mesh = newMesh(floor.Vertices.toVector3(), floor.Elements);
			}
		}
		
		private Mesh newMesh(Vector3[] vertices, int[] triangles)
		{
			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.uv = Enumerable.Repeat(Vector2.zero, vertices.Length).ToArray();
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			return mesh;
		}
		
		Tess getFloorTess()
		{
			Tess tess = new Tess();
			
			tess.AddContour(GameObject.Find("floor").collider.edges().toContour());
			
			foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("obstacle"))
				tess.AddContour(obstacle.collider.edges().toContour());
			
			tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);
			return tess;
		}
	}
}
