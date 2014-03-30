using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
	
namespace Agent
{
	[ExecuteInEditMode]
	public class Secure : MonoBehaviour
	{
		public bool showDynamicGuarding;

		public bool initialization = false;
		
		public bool initWithTabu = false;

		private static int[] bestPermutation;
		private float[,] costs;

		public Color dynamicPathColor = Color.blue;

		void Start ()
		{

		}

#if UNITY_EDITOR
		void Update ()
		{
//			print ("----------------- RECOMPUTING! --------------------");
			if (!Application.isPlaying){
				int nbGuard = ((PoliceSpawner)FindObjectOfType(typeof(PoliceSpawner))).PoliceCount;
				if(nbGuard > 0){
					computeGuardPath(nbGuard);
				}
			}
		}
#endif

		public void computeGuardPath (int nbGuard)
		{
			int nbPoint = Areas.setOfPointCoveringArea.Count ();
			if(nbPoint == 0)
				return;
//			print ("nbPoint = " + nbPoint + " nbGuard = " + nbGuard);

			if(initialization){
				initialization = false;
				PermutationGenerator permutationGenerator = new PermutationGenerator (nbGuard + nbPoint);

				// Create a matrix with all cost
				costs = new float[nbGuard+nbPoint,nbGuard+nbPoint];
				GameObject[] guards = GameObject.FindGameObjectsWithTag ("police");

				for(int i=0;i<nbGuard+nbPoint;i++){
					for(int j=0;j<nbGuard+nbPoint;j++){
						Vector3 start = (i<nbPoint) ? Areas.setOfPointCoveringArea.ElementAt(i) : guards[i-nbPoint].transform.position;
						Vector3 end = (j < nbPoint) ? Areas.setOfPointCoveringArea.ElementAt(j) : guards[j-nbPoint].transform.position;
						PathFinderAStar.Path path = PathFinderAStar.find (start,end);
//						if(path==null)
//							costs[i,j] = Mathf.Infinity;
//						else
							costs[i,j] = path.lengthPath;
					}
				}

				// Compute best permutation
				bestPermutation = new int[permutationGenerator.n];
				float bestLength = Mathf.Infinity;
				while (permutationGenerator.next()) {
					if(permutationGenerator.isValid(nbGuard)){
						float tmpLength = 0f;
						for(int r=nbPoint;r<nbPoint+nbGuard;r++){
							tmpLength += getPathLength(r,permutationGenerator.array,nbGuard,nbPoint);
							if(tmpLength>bestLength)
							{
								break;
							}
						}
						if(tmpLength > 0f && tmpLength < bestLength){
							bestLength = tmpLength;
							bestPermutation = (int[])permutationGenerator.array.Clone();
						}
					}
				}
				String msg = "Best Permutation = ";
				for(int i=0;i<permutationGenerator.n;i++){
					msg += bestPermutation[i] + ",";
				}
				print (msg + "with length " + bestLength);
			}
			
			if(initWithTabu){
				initWithTabu = false;

				// Create a matrix with all cost
				costs = new float[nbGuard+nbPoint,nbGuard+nbPoint];
				GameObject[] guards = GameObject.FindGameObjectsWithTag ("police");

				for(int i=0;i<nbGuard+nbPoint;i++){
					for(int j=0;j<nbGuard+nbPoint;j++){
						Vector3 start = (i<nbPoint) ? Areas.setOfPointCoveringArea.ElementAt(i) : guards[i-nbPoint].transform.position;
						Vector3 end = (j < nbPoint) ? Areas.setOfPointCoveringArea.ElementAt(j) : guards[j-nbPoint].transform.position;
						PathFinderAStar.Path path = PathFinderAStar.find (start,end);
						costs[i,j] = path.lengthPath;
					}
				}

				// Compute best permutation
				bestPermutation = TabuSearch.Search(nbGuard,nbPoint,costs);
				String msg = "Best Permutation = ";
				for(int i=0;i<bestPermutation.Length;i++){
					msg += bestPermutation[i] + ",";
				}
				
				float length = 0f;
				for(int r = nbPoint; r < nbPoint+nbGuard ; r++){
					length += getPathLength(r,bestPermutation,nbGuard,nbPoint);
				}
				
				print (msg + " with length " + length);
			}
			
			
			if(showDynamicGuarding){
//				print ("Displaying...");
				displayDynamicGuarding(bestPermutation,nbPoint,nbGuard);
			}

		}

		float getPathLength(int indexGuard, int[] currentPermutation, int nbGuard, int nbPoint){
			int indexGuardInPermutation = 0;
			while(currentPermutation[indexGuardInPermutation] != indexGuard){
				indexGuardInPermutation++;
			}
			List<int> indexPath = new List<int> ();
			int currentIndex = indexGuardInPermutation+1;
			if(currentIndex >= nbGuard+nbPoint)
				return 0f;

			while(currentIndex < nbGuard + nbPoint && currentPermutation[currentIndex] < nbPoint ){
				indexPath.Add (currentPermutation[currentIndex]);
            	currentIndex++;
			}

			if(!indexPath.Any()){
				return 0f;
			}
//			GameObject[] guards = GameObject.FindGameObjectsWithTag ("police");
			float pathLength = costs [indexGuard, indexPath.ElementAt (0)];


			for(int i=0;i<indexPath.Count-1;i++){
				pathLength += costs[indexPath.ElementAt(i),indexPath.ElementAt(i+1)];
			}
			return pathLength;
		}

		void displayDynamicGuarding(int[] permutation,int nbPoint,int nbGuard){
			for(int r=nbPoint;r<nbPoint+nbGuard;r++){
//				print ("Displaying path for robot " + (r-nbPoint));
				displayGuardPath(r, permutation, nbGuard, nbPoint);
			}
		}

		void displayGuardPath(int indexGuard, int[] permutation, int nbGuard, int nbPoint){
			int indexGuardInPermutation = 0; 
			while(permutation[indexGuardInPermutation] != indexGuard){
				indexGuardInPermutation++;
			}
			List<int> indexPath = new List<int> ();
			int currentIndex = indexGuardInPermutation+1;
			if(currentIndex >= nbGuard+nbPoint)
				return;
			while(currentIndex < nbGuard + nbPoint && permutation[currentIndex] < nbPoint ){
				indexPath.Add (permutation[currentIndex]);
				currentIndex++;
			}
			if(!indexPath.Any()){
				return;
			}
			GameObject[] guards = GameObject.FindGameObjectsWithTag ("police");
			PathFinderAStar.Path tmpPath = PathFinderAStar.find (guards[indexGuard-nbPoint].transform.position,
			                                         Areas.setOfPointCoveringArea.ElementAt (indexPath.ElementAt (0)));

			Debug.DrawLine (guards [indexGuard - nbPoint].transform.position, tmpPath.waypoints.ElementAt (0).pos, dynamicPathColor);
			for (int i=0; i<tmpPath.waypoints.Count-1; i++) {
				Debug.DrawLine(tmpPath.waypoints.ElementAt(i).pos,tmpPath.waypoints.ElementAt(i+1).pos,dynamicPathColor);		
			}

			for(int i=0;i<indexPath.Count-1;i++){
				int startIndex = indexPath.ElementAt(i);
				int endIndex = indexPath.ElementAt(i+1);
				Vector3 start = Areas.setOfPointCoveringArea.ElementAt(startIndex);
				Vector3 end = Areas.setOfPointCoveringArea.ElementAt(endIndex);
				tmpPath = PathFinderAStar.find(start,end);
				Debug.DrawLine (start, tmpPath.waypoints.ElementAt (0).pos, dynamicPathColor);
				for (int w=0; w<tmpPath.waypoints.Count-1; w++) {
					Debug.DrawLine(tmpPath.waypoints.ElementAt(w).pos,tmpPath.waypoints.ElementAt(w+1).pos,dynamicPathColor);		
				}
			}
		}

		public static List<int> getIndexPointToVisit(int indexGuard){
			int nbGuard = ((PoliceSpawner)FindObjectOfType(typeof(PoliceSpawner))).policeCount;
			int nbPoint = bestPermutation.Length - nbGuard;
			indexGuard += nbPoint;
			int indexGuardInPermutation = 0; 
			while(bestPermutation[indexGuardInPermutation] != indexGuard){
				indexGuardInPermutation++;
			}
			int currentIndex = indexGuardInPermutation+1;
			List<int> result = new List<int> ();
			if(currentIndex >= nbGuard+nbPoint)
				return result;
			while(currentIndex < nbGuard + nbPoint && bestPermutation[currentIndex] < nbPoint ){
				result.Add (bestPermutation[currentIndex]);
				currentIndex++;
			}
			return result;

		}
	}
}