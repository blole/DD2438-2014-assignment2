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
			updateAreas();
		}
#endif

		void updateAreas()
		{
			areas.Clear();
			foreach (Transform t in transform)
				DestroyImmediate(t.gameObject);

			Tess floor = getFloorTess();

			MeshFilter floorArea = ((MeshFilter)Instantiate(areaPrefab));
			floorArea.transform.parent = transform;

			GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");

			Vector3[] floorVertices = floor.Vertices.toVector3();

			List<Triangle> triangles = new List<Triangle>();
			List<int> triangleIndices = new List<int>();
			for (int i=0; i<floor.Elements.Length/3; i++)
			{
				Triangle triangle = new Triangle(
					floor.Vertices[floor.Elements[i*3+0]].Position.toVector3().projectDown(),
					floor.Vertices[floor.Elements[i*3+1]].Position.toVector3().projectDown(),
					floor.Vertices[floor.Elements[i*3+2]].Position.toVector3().projectDown());

				if (!obstacles.Any(go=>go.collider.rect().contains(triangle.Center)))
				{
					triangleIndices.Add(floor.Elements[i*3+0]);
					triangleIndices.Add(floor.Elements[i*3+1]);
					triangleIndices.Add(floor.Elements[i*3+2]);
					triangles.Add(triangle);
				}
			}

			floorArea.mesh = newMesh(floorVertices, triangleIndices.ToArray());

			Debug.Log(constructConvexAreas(Polygon.Null, triangles).Count());

			/*foreach (Polygon convexArea in constructConvexAreas(Polygon.Null, triangles))
			{
				MeshFilter area = ((MeshFilter)Instantiate(areaPrefab));
				area.transform.parent = transform;
				area.mesh = newMesh(convexArea);
			}*/
		}

		private Mesh newMesh(Polygon poly)
		{
			Tess tess = new Tess();
			tess.AddContour(poly.points.toContour());
			tess.Tessellate(WindingRule.Positive, ElementType.Polygons, 3);

			return newMesh(tess.Vertices.toVector3(), tess.Elements);
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

		private IEnumerable<Polygon> constructConvexAreas(Polygon poly, IEnumerable<Triangle> triangles)
		{
			bool extensionFound = false;
			int nextIndex = 0;
			foreach (Triangle tri in triangles)
			{
				nextIndex++;
				Polygon expandedPoly = poly.JoinToConvex(tri);
				if (expandedPoly != null)
				{
					extensionFound = true;
					foreach (Polygon p in constructConvexAreas(expandedPoly, triangles.Skip(nextIndex)))
						yield return p;
				}
			}

			if (!extensionFound)
				yield return poly;
		}

		Tess getFloorTess()
		{
			GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
			Tess tess = new Tess();

			var floorEdges = GameObject.Find("floor").collider.edges();
			Rectangle floor = new Rectangle(floorEdges[0].projectDown(), floorEdges[2].projectDown());
			
			tess.AddContour(GameObject.Find("floor").collider.edges().toContour());

			foreach (GameObject obstacle in obstacles)
			{
				Vector3[] edges = obstacle.collider.edges();

				Line top    = new Line(edges[0]+(edges[0]-edges[1])*1000, edges[1]-(edges[0]-edges[1])*1000);
				Line right  = new Line(edges[1]+(edges[1]-edges[2])*1000, edges[2]-(edges[1]-edges[2])*1000);
				Line bottom = new Line(edges[2]+(edges[2]-edges[3])*1000, edges[3]-(edges[2]-edges[3])*1000);
				Line left   = new Line(edges[3]+(edges[3]-edges[0])*1000, edges[0]-(edges[3]-edges[0])*1000);

				Vector2[] vertical = floor.Intersects(top).Reverse().Concat(floor.Intersects(bottom)).ToArray();
				Vector2[] horizontal = floor.Intersects(left).Reverse().Concat(floor.Intersects(right)).ToArray();

				tess.AddContour(vertical.toContour());
				tess.AddContour(horizontal.toContour());
			}


			//tess.AddContour(new Vector3[]{new Vector3(-10,0,0), new Vector3(10,0,10.01f), new Vector3(10,0,10)}.toContour());
			
			tess.Tessellate(WindingRule.Positive, ElementType.Polygons, 3);

			return tess;
		}
	}
}
