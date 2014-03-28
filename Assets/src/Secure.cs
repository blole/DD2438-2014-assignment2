using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
	
namespace Agent
{
	[ExecuteInEditMode]
	public class Secure : MonoBehaviour
	{
		public bool showDynamicGuarding;

		public bool recomputePaths = true;
		private bool previousRecomputePaths = false;

		private int[] bestPermutation;

		void Start ()
		{

		}

#if UNITY_EDITOR
		void LateUpdate ()
		{
			print ("----------------- RECOMPUTING! --------------------");
			if (!Application.isPlaying){
				int nbGuard = GameObject.FindObjectOfType<PoliceSpawner>().PoliceCount;
				if(nbGuard > 0){
					computeGuardPath(GameObject.FindObjectOfType<PoliceSpawner>().PoliceCount);
				}
			}
		}
#endif

		void computeGuardPath (int nbGuard)
		{
			int nbPoint = Areas.setOfPointCoveringArea.Count ();
			if(nbPoint == 0)
				return;
			print ("nbPoint = " + nbPoint + " nbGuard = " + nbGuard);

			if(previousRecomputePaths != recomputePaths){
				previousRecomputePaths = recomputePaths;
				Permutation permutationGenerator = new Permutation (nbGuard + nbPoint);

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
			if(showDynamicGuarding){
				print ("Displaying...");
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
			GameObject[] guards = GameObject.FindGameObjectsWithTag ("police");
			PathFinderAStar.Path tmpPath = PathFinderAStar.find (guards[indexGuard-nbPoint].transform.position,
			                                                     Areas.setOfPointCoveringArea.ElementAt (indexPath.ElementAt (0)));
			float pathLength = tmpPath.lengthPath;
			for(int i=0;i<indexPath.Count-1;i++){
				int startIndex = indexPath.ElementAt(i);
				int endIndex = indexPath.ElementAt(i+1);
				Vector3 start = Areas.setOfPointCoveringArea.ElementAt(startIndex);
				Vector3 end = Areas.setOfPointCoveringArea.ElementAt(endIndex);
				tmpPath = PathFinderAStar.find(start,end);
				pathLength += tmpPath.lengthPath;
			}
			return pathLength;
		}

		void displayDynamicGuarding(int[] permutation,int nbPoint,int nbGuard){
			GameObject[] guards = GameObject.FindGameObjectsWithTag ("police");
			for(int r=nbPoint;r<nbPoint+nbGuard;r++){
				print ("Displaying path for robot " + (r-nbPoint));
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

			Debug.DrawLine (guards [indexGuard - nbPoint].transform.position, tmpPath.waypoints.ElementAt (0).pos, Color.blue);
			for (int i=0; i<tmpPath.waypoints.Count-1; i++) {
				Debug.DrawLine(tmpPath.waypoints.ElementAt(i).pos,tmpPath.waypoints.ElementAt(i+1).pos,Color.blue);		
			}

			for(int i=0;i<indexPath.Count-1;i++){
				int startIndex = indexPath.ElementAt(i);
				int endIndex = indexPath.ElementAt(i+1);
				Vector3 start = Areas.setOfPointCoveringArea.ElementAt(startIndex);
				Vector3 end = Areas.setOfPointCoveringArea.ElementAt(endIndex);
				tmpPath = PathFinderAStar.find(start,end);
				Debug.DrawLine (start, tmpPath.waypoints.ElementAt (0).pos, Color.blue);
				for (int w=0; w<tmpPath.waypoints.Count-1; w++) {
					Debug.DrawLine(tmpPath.waypoints.ElementAt(w).pos,tmpPath.waypoints.ElementAt(w+1).pos,Color.blue);		
				}
			}
		}
	}
}