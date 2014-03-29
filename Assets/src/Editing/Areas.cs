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

		// Control the (re)computing of areas
		private bool previousRecomputeAreas = false;
		public bool recomputeAreas = false;
		public bool initialization = false;

		// Grid
		[Range(0.2f,20f)]
		public float gridSize = 0.4f;
		private Vector3 bottomLeftOfTheGrid;
		[Range(1,40)]
		private int nbRow;
		private int nbCol;
		private Boolean[,] gridFreeSpace;

		// Maximum set cover
		public int maximumConvexSize = 50;
		public bool extraPoint = false;
		public List<Area> areas = new List<Area>();
		public static List<Vector3> setOfPointCoveringArea = new List<Vector3>();

		// Display boolean
		public bool showPointsCoveringArea = false;
		public bool showAreas;
		public bool showGrid;
		public Color areasColor = Color.red;
		public Color pointsColor = Color.yellow;

		// Static guarding
		private static int indexPointForStaticGuarding = -1;

		void Start ()
		{
			initialization = true;
			updateAreas();
		}
		
#if UNITY_EDITOR
		void Update ()
		{
			if (!Application.isPlaying)
				updateAreas();
		}
#endif

		public void updateAreas()
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

					gridFreeSpace[i,j] = 
							PhysicsHelper.isClear(bottomLeft)  &&
							PhysicsHelper.isClear(bottomRight) &&
							PhysicsHelper.isClear(topLeft)     &&
							PhysicsHelper.isClear(topRight);
				}
			}

			if(showGrid)
			{
				for(int i=0; i < nbRow ; i++){
					for(int j=0; j < nbCol; j++){
						Debug.DrawLine(bottomLeftOfTheGrid + new Vector3(i*gridSize,0f,j*gridSize),
						               bottomLeftOfTheGrid+ new Vector3((i+1)*gridSize,0f,j*gridSize),Color.cyan);
						Debug.DrawLine(bottomLeftOfTheGrid + new Vector3(i*gridSize,0f,j*gridSize),
						               bottomLeftOfTheGrid+ new Vector3(i*gridSize,0f,(j+1)*gridSize),Color.cyan);
					}
				}
			}

			if(previousRecomputeAreas != recomputeAreas || initialization){
				// Assure that the recomputation is done one
				initialization = false;
				previousRecomputeAreas = recomputeAreas;

				// Clear all previous results 
				areas.Clear();
				setOfPointCoveringArea.Clear();
				Area.nbAreas = 0;
				indexPointForStaticGuarding = -1;

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

					// Add new rectangle to the convex set and mark covered cells as so
					Rect rectArea = new Rect(bottomLeftOfTheGrid.x + pi*gridSize, bottomLeftOfTheGrid.z + pj*gridSize, (maxI-pi+1)*gridSize, 
					                         (maxJ-pj+1)*gridSize);
					areas.Add(new Area(rectArea));
					nbCellRemaining -= ((maxI-pi+1)*(maxJ-pj+1));
					for(int i=pi;i<=maxI;i++){
						for(int j=pj;j<=maxJ;j++){
							coveredCells[i,j] = true;
						}
					}
				}

				// Compute best set of points where the whole area is visible

				// Create set of potential point
				List<Vector3> potentialPoint = new List<Vector3>();
				for(int i=0; i<Waypoints.waypoints.Count;i++){
					potentialPoint.Add(Waypoints.waypoints.ElementAt(i).pos);
				}
				if(extraPoint){
					for(int i=0;i<nbRow;i++){
						for(int j=0;j<nbCol;j++){
							potentialPoint.Add (bottomLeftOfTheGrid + new Vector3((i + 0.5f)*gridSize,0f,(j+0.5f)*gridSize));
						}
					}
				}

				List<int> indexUncoveredAreas = new List<int>();
				for(int i=0;i<Area.nbAreas;i++){
					indexUncoveredAreas.Add (i);
				}

				while(indexUncoveredAreas.Any()){
					// Find the biggest visible area from interest points
					int bestNbVisible = 0;
					int bestIndex = -1;
					for(int i=1;i<potentialPoint.Count;i++){
						int tmpNbVisible = countNbVisibleFrom(potentialPoint.ElementAt(i),indexUncoveredAreas);
						if(tmpNbVisible > bestNbVisible){
							bestIndex = i;
							bestNbVisible = tmpNbVisible;
						}
					}
					// Add the point to the set of points
					Vector3 bestPos = potentialPoint.ElementAt(bestIndex);
					setOfPointCoveringArea.Add(bestPos);
					removeSeenArea(bestPos,indexUncoveredAreas);
				}
			}

			if(showAreas){
				displayAreas();
			}

			if(showPointsCoveringArea){
				int segments = 64;
				for(int k=0; k<setOfPointCoveringArea.Count;k++){
					Vector3 c = setOfPointCoveringArea.ElementAt(k)+Vector3.up * 0.01f;
					for (int i=0; i<segments; i++){
						Debug.DrawLine(c+Vector2.up.turn(360f/segments*i).toVector3()*Waypoints.radius,
						               c+Vector2.up.turn(360f/segments*(i+1)).toVector3()*Waypoints.radius, pointsColor);
						Debug.DrawLine(c+Vector2.up.turn (360f/segments*i).toVector3()*Waypoints.radius,c,pointsColor);
					}
				}
			}


		}

		public int countNbVisibleFrom(Vector3 point, List<int> indexUncoveredArea){
			int nbAreaVisible = 0;
			for(int i=0;i<indexUncoveredArea.Count;i++){
				Area currentArea = areas.ElementAt(indexUncoveredArea.ElementAt(i));
				Vector3[] corners = currentArea.getAreaCoord();
				Boolean isAreaVisible = true;
				for(int k=0;k<4;k++){
					if(!PhysicsHelper.isClearPath(corners[k],point)){
						isAreaVisible = false;
						break;
					}
				}
				if(isAreaVisible){
					nbAreaVisible++;
				}
			}
			return nbAreaVisible;
		}

		public void removeSeenArea(Vector3 point, List<int> indexUncoveredArea){
			for(int i=indexUncoveredArea.Count-1;i>=0;i--){
				Area currentArea = areas.ElementAt(indexUncoveredArea.ElementAt(i));
				Vector3[] corners = currentArea.getAreaCoord();
				Boolean isAreaVisible = true;
				for(int k=0;k<4;k++){
					if(!PhysicsHelper.isClearPath(corners[k],point)){
						isAreaVisible = false;
						break;
					}
				}
				if(isAreaVisible){
					indexUncoveredArea.RemoveAt(i);
				}
			}
		}

		public void displayAreas(){
			for(int i=0;i<Area.nbAreas;i++){
				Area currentArea = areas[i];
				Vector3[] areaCoord = currentArea.getAreaCoord();
				Debug.DrawLine(areaCoord[0],areaCoord[1],areasColor);
				Debug.DrawLine(areaCoord[0],areaCoord[2],areasColor);
				Debug.DrawLine(areaCoord[1],areaCoord[3],areasColor);
				Debug.DrawLine(areaCoord[2],areaCoord[3],areasColor);
			}
		}


		// Static guarding function
		public static int getNextIndexInterestingPoint(){
			if(indexPointForStaticGuarding >= setOfPointCoveringArea.Count-1){
				return -1;
			}
			else{
				indexPointForStaticGuarding++;
//				print ("Returning " + indexPointForStaticGuarding);
				return (indexPointForStaticGuarding);
			}
		}

		public static void decreaseIndexStaticGuarding(){
			indexPointForStaticGuarding--;
			if(indexPointForStaticGuarding<-1)
				indexPointForStaticGuarding=-1;
		}
	}
}
