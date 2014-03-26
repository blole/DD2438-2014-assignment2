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

		private bool previousRecomputeAreas = false;
		public bool recomputeAreas = false;
		public bool showAreas;
		public bool showGrid;
		[Range(0.2f,20f)]
		public float gridSize = 10f;
		[Range(1,40)]
		public int maximumConvexSize = 50;

		private int nbRow;
		private int nbCol;
		private Vector3 bottomLeftOfTheGrid;
		private Boolean[,] gridFreeSpace;
		private List<Area> areas = new List<Area>();
		private List<Vector2> pointsOfInterest = new List<Vector2>();

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
			foreach (Transform t in transform)
				DestroyImmediate(t.gameObject);

			// Creation of the grid representation
			Vector3 floorScale = GameObject.Find ("floor").transform.localScale;
			nbRow = (int)Mathf.Floor (floorScale.x / gridSize);
			nbCol = (int)Mathf.Floor (floorScale.y / gridSize);
			bottomLeftOfTheGrid = GameObject.Find ("floor").transform.position - new Vector3(floorScale.x,0f,floorScale.y) / 2f;

			gridFreeSpace = new Boolean[nbRow,nbCol];
			for(int i=0; i < nbRow ; i++){
				for(int j=0; j < nbCol ; j++){
//					Vector3 currentPos = bottomLeftOfTheGrid + new Vector3((i + 0.5f)*gridSize,0f,(j+0.5f)*gridSize);
					Vector3 bottomLeft = bottomLeftOfTheGrid + new Vector3(i*gridSize,0f,j*gridSize);
					Vector3 bottomRight = bottomLeftOfTheGrid + new Vector3((i+1)*gridSize,0f,j*gridSize);
					Vector3 topLeft = bottomLeftOfTheGrid + new Vector3(i*gridSize,0f,(j+1)*gridSize);
					Vector3 topRight = bottomLeftOfTheGrid + new Vector3((i+1)*gridSize,0f,(j+1)*gridSize);
					Boolean isOutside1 = isPointInsideObstacle(bottomLeft);
					Boolean isOutside2 = isPointInsideObstacle(bottomRight);
					Boolean isOutside3 = isPointInsideObstacle(topLeft);
					Boolean isOutside4 = isPointInsideObstacle(topRight);
					gridFreeSpace[i,j] = isOutside1 && isOutside2 && isOutside3 && isOutside4;
				}
			}

			if(showGrid)
			{
				for(int i=0; i < nbRow ; i++){
					for(int j=0; j < nbCol; j++){
						Debug.DrawLine(bottomLeftOfTheGrid + new Vector3(i*gridSize,0f,j*gridSize),bottomLeftOfTheGrid+ new Vector3((i+1)*gridSize,0f,j*gridSize),Color.cyan);
						Debug.DrawLine(bottomLeftOfTheGrid + new Vector3(i*gridSize,0f,j*gridSize),bottomLeftOfTheGrid+ new Vector3(i*gridSize,0f,(j+1)*gridSize),Color.cyan);
					}
				}
			}

			if(previousRecomputeAreas != recomputeAreas){
				previousRecomputeAreas = recomputeAreas;
				areas.Clear();
				Area.nbAreas = 0;

			// Creation of the convex set cover
			Boolean[,] coveredCells = new Boolean[nbRow, nbCol];
			int nbCellRemaining = nbRow * nbCol;
			for(int i=0;i<nbRow;i++){
				for(int j=0;j<nbCol;j++){
					coveredCells[i,j] = !gridFreeSpace[i,j];
					if(!gridFreeSpace[i,j])
						nbCellRemaining--;
				}
			}
			
			while (nbCellRemaining>0) {
				// Find a yet uncovered cell p
				int[] p = new int[2];
				p[0] = -1;
				for(int i=0;i<nbRow;i++){
					if(p[0]==-1){
						for(int j=0;j<nbCol;j++){
							if(!coveredCells[i,j]){
								p[0] = i;
								p[1] = j;
								break;
							}
						}
					}
				}

				// Start growing a rectangle ci from p until it is bounded
				// Grow in i croissant
				int pi = p[0];
				int pj = p[1];
				int maxI = pi;
				while(maxI < (nbRow-1) && (maxI-pi+1) < maximumConvexSize && !coveredCells[maxI+1,pj] && gridFreeSpace[maxI+1,pj]){
					maxI++;
				}

				// Grow in j croissant
				int maxJ = nbCol;
				for(int k=pi;k<=maxI;k++){
					int tmpMaxJ = pj;
					while(tmpMaxJ < (nbCol-1) && (tmpMaxJ-pj+1) < maximumConvexSize && !coveredCells[k,tmpMaxJ+1] && gridFreeSpace[k,tmpMaxJ+1]){
						tmpMaxJ++;
					}
					maxJ = (tmpMaxJ < maxJ) ? tmpMaxJ : maxJ;
				}
				print ("pi = " + pi + " pj = " + pj + " maxI = " + maxI + " maxJ = " + maxJ);

				// Add new rectangle to the convex set and mark covered cells as so
				Rect rectArea = new Rect(bottomLeftOfTheGrid.x + pi*gridSize, bottomLeftOfTheGrid.z + pj*gridSize, (maxI-pi+1)*gridSize, (maxJ-pj+1)*gridSize);
				areas.Add(new Area(rectArea));
				nbCellRemaining -= ((maxI-pi+1)*(maxJ-pj+1));
				for(int i=pi;i<=maxI;i++){
					for(int j=pj;j<=maxJ;j++){
						print ("i,j = " + i + "," + j);
						coveredCells[i,j] = true;
					}
				}
			}
                
			}

			if(showAreas){
				for(int i=0;i<Area.nbAreas;i++){
					Area currentArea = areas[i];
					float xmin = currentArea.corners.xMin;
					float ymin = currentArea.corners.yMin;
					float xmax = currentArea.corners.xMax;
					float ymax = currentArea.corners.yMax;
					Vector3 bottomLeft = new Vector3(xmin,0.02f,ymin);
					Vector3 bottomRight = new Vector3(xmax,0.02f,ymin);
					Vector3 topLeft = new Vector3(xmin,0.02f,ymax);
					Vector3 topRight = new Vector3(xmax,0.02f,ymax);
					Debug.DrawLine(bottomLeft,bottomRight,Color.red);
					Debug.DrawLine(bottomLeft,topLeft,Color.red);
					Debug.DrawLine(bottomRight,topRight,Color.red);
					Debug.DrawLine(topLeft,topRight,Color.red);
				}
			}
		}

		
		
		public Boolean isPointInsideObstacle(Vector3 point){
			Boolean isOutside = true;
			foreach(GameObject cube in GameObject.FindGameObjectsWithTag("obstacle")){
				if(cube.renderer.bounds.Contains(point)){
					isOutside = false;
                    break;
                }
            }
			return isOutside;
        }
		
//		private Mesh newMesh(Vector3[] vertices, int[] triangles)
//		{
//			Mesh mesh = new Mesh();
//			mesh.vertices = vertices;
//			mesh.uv = Enumerable.Repeat(Vector2.zero, vertices.Length).ToArray();
//			mesh.triangles = triangles;
//			mesh.RecalculateNormals();
//			return mesh;
//		}
		
//		Tess getFloorTess()
//		{
//			Tess tess = new Tess();
//			
//			tess.AddContour(GameObject.Find("floor").collider.edges().toContour());
//			
//			foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("obstacle"))
//				tess.AddContour(obstacle.collider.edges().toContour());
//			
//			tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);
//			return tess;
//		}
	}
}
