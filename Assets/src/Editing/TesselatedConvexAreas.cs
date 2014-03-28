using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LibTessDotNet;

namespace Agent
{
	[RequireComponent(typeof(MeshFilter))]
	[ExecuteInEditMode]
	public class TesselatedConvexAreas : MonoBehaviour {
		public bool showAreas;

		public int maxPolygonCornerns = 100;

		public Color polygonColor = Color.white;

#if UNITY_EDITOR
		void Update ()
		{
			if (!Application.isPlaying)
				updateAreas();
		}
#endif

		void updateAreas()
		{
			List<Polygon> polygons = new List<Polygon>();
			Tess floor = getFloorTess(maxPolygonCornerns);
			Vector2[] vertices = floor.Vertices.Select(v=>v.Position.toVector3().projectDown()).ToArray();

			for (int i=0; i<floor.Elements.Length/maxPolygonCornerns; i++)
			{
				List<Vector2> polygonVertices = new List<Vector2>();
				for (int j=0; j<maxPolygonCornerns; j++)
				{
					int vertexIndex = floor.Elements[maxPolygonCornerns*i+j];
					if (vertexIndex != -1)
						polygonVertices.Add(vertices[vertexIndex]);
					else
						break;
				}
				polygons.Add(new Polygon(polygonVertices.ToArray()));
			}

			Vector3 height = Vector3.up * transform.position.y;
			foreach (Polygon poly in polygons)
			{
				foreach (Line line in poly.lines())
					Debug.DrawLine(line.a.toVector3()+height, line.b.toVector3()+height, polygonColor);
			}
		}

		private Mesh newMesh(Polygon poly)
		{
			Tess tess = tesselateTriangles(poly);
			return newMesh(tess.Vertices.toVector3(), tess.Elements);
		}

		private Tess tesselateTriangles(Polygon poly)
		{
			Tess tess = new Tess();
			tess.AddContour(poly.points.toContour());
			tess.Tessellate(WindingRule.Positive, ElementType.Polygons, 3);

			return tess;
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

		Tess getFloorTess(int maxConvexPolygonSize)
		{
			Tess tess = new Tess();
			
			tess.AddContour(GameObject.Find("floor").collider.edges().toContour());
			
			foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("obstacle"))
				tess.AddContour(obstacle.collider.edges().toContour());
			
			tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, maxConvexPolygonSize);
			return tess;
		}
	}
}
