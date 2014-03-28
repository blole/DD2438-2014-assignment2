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
	public class ConvexPolygons : MonoBehaviour {
		public int maxPolygonCornerns = 100;

		public bool showPolygons;
		public bool showGraph;

		public Color polygonColor = Color.white;
		public Color graphColor = Color.white;

		static public List<ConnectedPolygon> polygons = new List<ConnectedPolygon>();

		#if UNITY_EDITOR
		void Update ()
		{
			if (!Application.isPlaying)
				updateAreas();
		}
		#endif

		void updateAreas()
		{
			polygons.Clear();
			Tess floor = getFloorTess(maxPolygonCornerns);
			Vector2[] vertices = floor.Vertices.Select(v=>v.Position.toVector3().projectDown()).ToArray();

			//create polygons
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
				polygons.Add(new ConnectedPolygon(polygonVertices.ToArray()));
			}

			//connect neighbors
			for (int i=0; i<polygons.Count; i++)
			{
				for (int j=i+1; j<polygons.Count; j++)
				{
					foreach (Line line in polygons[i].lines())
					{
						if (polygons[j].lines().Contains(line))
						{
							polygons[i].neighbors.Add(polygons[j]);
							polygons[j].neighbors.Add(polygons[i]);
						}
					}
				}
			}

			Vector3 height = Vector3.up * transform.position.y;
			if (showPolygons)
			{
				foreach (Polygon poly in polygons)
				{
					foreach (Line line in poly.lines())
						Debug.DrawLine(line.a.toVector3()+height, line.b.toVector3()+height, polygonColor);
				}
			}
			
			if (showGraph)
			{
				foreach (ConnectedPolygon poly in polygons)
				{
					foreach (ConnectedPolygon neighbor in poly.neighbors)
						Debug.DrawLine(poly.Center.toVector3()+height, neighbor.Center.toVector3()+height, graphColor);

					DebugHelper.DrawCircle(poly.Center.toVector3()+height, 0.1f, 16, graphColor);
				}
			}
		}

		public class ConnectedPolygon : Polygon
		{
			public ConnectedPolygon(Vector2[] points)
				: base(points)
			{}

			public List<ConnectedPolygon> neighbors = new List<ConnectedPolygon> ();
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
			
			tess.AddContour(GameObject.Find("floor").collider.edges().toContour(), ContourOrientation.Clockwise);
			
			foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("obstacle"))
				tess.AddContour(obstacle.collider.edges().Reverse().toContour(), ContourOrientation.CounterClockwise);
			
			tess.Tessellate(WindingRule.Positive, ElementType.Polygons, maxConvexPolygonSize);
			return tess;
		}
	}
}
