using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LibTessDotNet;
using IntervalTreeLib;

namespace Agent
{
	[RequireComponent(typeof(MeshFilter))]
	[ExecuteInEditMode]
	public class ConvexPolygons : MonoBehaviour {
		public int maxPolygonCornerns = 100;

		public bool showPolygons;
		public bool showGraph;
		public bool showRandomCut;

		public Color polygonColor = Color.white;
		public Color graphColor = Color.white;
		public Color cutColor = Color.white;

		static public List<ConnectedPolygon> polygons = new List<ConnectedPolygon>();
		static public Dictionary<Vector2, List<GraphCut>> graphCuts = new Dictionary<Vector2, List<GraphCut>>();

		void Start()
		{
			updateAreas();
			updateCuts();
		}

		#if UNITY_EDITOR
		void Update ()
		{
			if (!Application.isPlaying)
			{
				updateAreas();
				updateCuts();

				if (showGraph)
					drawGraph();
				if (showPolygons)
					drawPolygons();
				if (showRandomCut)
				{
					while (true)
					{
						List<GraphCut> cuts = null;
						Vector2 pos = graphCuts.Keys.ElementAt(UnityEngine.Random.Range(0, graphCuts.Keys.Count-1));
						graphCuts.TryGetValue(pos, out cuts);
						if (cuts != null)
						{
							drawCut(pos, cuts);
							break;
						}
					}
				}
			}
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
		}

		void updateCuts()
		{
			IEnumerable<Vector2> lookingPositions = Waypoints.waypoints.Select(w=>w.pos.projectDown());

			GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle").ToArray();
			Polygon[] obstaclePolys = obstacles.Select(o=>o.collider.polygon()).ToArray();

			graphCuts.Clear();
			foreach (Vector2 lookingPos in lookingPositions)
			{
				List<GraphCut> cuts = new List<GraphCut>();
				graphCuts.Add(lookingPos, cuts);
				
				IntervalTree<int, float> tree = new IntervalTree<int, float>();

				List<float> angles = new List<float>();
				for (int i=0; i<obstacles.Length; i++)
				{
					Interval<int, float> angleRange = getAngleRange(lookingPos, obstacles[i].collider);
					angleRange.Data = i;
					tree.AddInterval(angleRange);
					angles.Add(angleRange.Start);
					angles.Add(angleRange.End);
				}
				angles.Sort();

				for (int i=0; i<angles.Count; i++)
				{
					Line viewRay = new Line(lookingPos, lookingPos+Vector2.up.turn(angles[i])*1000);
					Vector2? firstEntringHit = null;

					List<int> hitObstacles = tree.Get(angles[i], StubMode.ContainsStartThenEnd);
					for (int j=0; j<hitObstacles.Count; j++)
					{
						Vector2? entringHit = obstaclePolys[j].firstEntry(viewRay);
						if (entringHit.HasValue)
						{
							if (!firstEntringHit.HasValue)
								firstEntringHit = entringHit;
							else if ((firstEntringHit.Value-lookingPos).sqrMagnitude > (entringHit.Value-lookingPos).sqrMagnitude)
								firstEntringHit = entringHit;
						}
					}
					Vector2 firstHit;
					if (firstEntringHit.HasValue)
						firstHit = firstEntringHit.Value;
					else
						firstHit = viewRay.b;


					viewRay = new Line(lookingPos, firstHit+(firstHit-lookingPos)*0.01f);

					foreach (ConnectedPolygon poly in polygons)
					{
						GraphCut cut = new GraphCut(poly);

						Line[] lines = poly.lines().ToArray();
						for (int j=0; j<lines.Length; j++)
						{
							if (lines[j].Intersects(viewRay).HasValue)
								cut.edges.Add(j);
						}

						if (cut.edges.Count > 0)
							cuts.Add(cut);
					}
				}
			}
		}
		
		private Vector2 firstIntersection(Vector2 origin, float angle, Line[] obstacleLines)
		{
			float minDistance = float.PositiveInfinity;
			Vector2 firstIntersection = Vector2.zero;
			Line ray = new Line(origin, origin+Vector2.up.turn(angle)*1000);
			foreach (Line line in obstacleLines)
			{
				Vector2? intersection = ray.Intersects(line);
				if (intersection.HasValue)
				{
					float distance = (intersection.Value-origin).sqrMagnitude;
					if (minDistance > distance)
					{
						minDistance = distance;
						firstIntersection = intersection.Value;
					}
				}
			}

			return firstIntersection;
		}

		private Interval<int, float> getAngleRange(Vector2 origin, Collider collider)
		{
			float min = float.PositiveInfinity;
			float max = float.NegativeInfinity;

			foreach (Vector2 edge in collider.edges())
			{
				float angle = (edge-origin).angle();
				min = Math.Min(min, angle);
				max = Math.Max(max, angle);
			}

			return new Interval<int, float>(min, max, 0);
		}

		void drawPolygons()
		{
			Vector3 height = transform.up * transform.position.y;
			foreach (Polygon poly in polygons)
			{
				foreach (Line line in poly.lines())
					Debug.DrawLine(line.a.toVector3()+height, line.b.toVector3()+height, polygonColor);
			}
		}

		void drawGraph()
		{
			Vector3 height = transform.up * transform.position.y;
			foreach (ConnectedPolygon poly in polygons)
			{
				foreach (ConnectedPolygon neighbor in poly.neighbors)
					Debug.DrawLine(poly.Center.toVector3()+height, neighbor.Center.toVector3()+height, graphColor);
				
				DebugHelper.DrawCircle(poly.Center.toVector3()+height, 0.2f, 16, graphColor);
			}
		}

		void drawCut(Vector2 pos, List<GraphCut> cuts)
		{
			DebugHelper.DrawCircle(pos.toVector3 ().setY (transform.position.y), 0.3f, 16, cutColor);
			foreach (GraphCut cut in cuts)
			{
				foreach (Pair<Vector2> pair in cut.edges.Select(i=>cut.poly.lines()[i].Center).pairs())
					DebugHelper.DrawLine(pair.a, pair.b, transform.position.y, cutColor);
			}
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

		public class ConnectedPolygon : Polygon
		{
			public ConnectedPolygon(Vector2[] points)
				: base(points)
			{}
			
			public List<ConnectedPolygon> neighbors = new List<ConnectedPolygon> ();
			
		}

		public class GraphCut
		{
			public readonly Polygon poly;
			public List<int> edges = new List<int>();

			public GraphCut(Polygon poly)
			{
				this.poly = poly;
			}
		}
	}
}
