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

		void Start ()
		{

		}

#if UNITY_EDITOR
		void LateUpdate ()
		{
			print ("----------------- RECOMPUTING! --------------------");
			if (!Application.isPlaying)
				computeGuardPath(GameObject.FindObjectOfType<PoliceSpawner>().PoliceCount);
		}
#endif

		void computeGuardPath (int nbGuard)
		{
			int nbPoint = Areas.setOfPointCoveringArea.Count ();
			print ("nbPoint = " + nbPoint + " nbGuard = " + nbGuard);
			Permutation permutationGenerator = new Permutation (nbGuard + nbPoint);

			// Compute best permutation
			int[] bestPermutation = new int[permutationGenerator.n];
			float bestLength = Mathf.Infinity;
			while (permutationGenerator.next()) {
				if(permutationGenerator.isValid(nbGuard)){
					String curPerm = "Current permutation = ";
					for(int i=0;i<permutationGenerator.n;i++){
						curPerm += permutationGenerator.array[i] + ",";
					}
					float tmpLength = 0f;
					for(int r=nbPoint;r<nbPoint+nbGuard;r++){
						tmpLength += getPathLength(r,permutationGenerator.array,nbGuard,nbPoint);
						if(tmpLength>bestLength)
						{
							print("Break case");
							break;
						}
					}
					curPerm += " has length " + tmpLength;
					print (curPerm);
					if(tmpLength > 0f && tmpLength < bestLength){
						bestLength = tmpLength;
						bestPermutation = (int[])permutationGenerator.array.Clone();
						String msgtmp = "Best Permutation so far= ";
						for(int i=0;i<permutationGenerator.n;i++){
							msgtmp += bestPermutation[i] + ",";
						}
						print (msgtmp + " with length " + bestLength);
					}
				}
			}
			String msg = "Best Permutation = ";
			for(int i=0;i<permutationGenerator.n;i++){
				msg += bestPermutation[i] + ",";
			}
			print (msg + "with length " + bestLength);
			
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

			/* DEBUG
			String debug = "Permutation ";
			for (int i=0; i<nbGuard+nbPoint; i++) {
				debug += currentPermutation[i] + ",";
			}
			debug += " for guard " + indexGuard + "has the following point attributed: ";
			*/

			while(currentIndex < nbGuard + nbPoint && currentPermutation[currentIndex] < nbPoint ){
//				print ("index = " + currentIndex + "value = " + currentPermutation[currentIndex]);
				indexPath.Add (currentPermutation[currentIndex]);
				// debug += currentPermutation[currentIndex] + ",";
            	currentIndex++;
			}
			
			// print (debug);
			if(!indexPath.Any()){
				return 0f;
			}
			GameObject[] guards = GameObject.FindGameObjectsWithTag ("police");
			PathFinderAStar.Path tmpPath = PathFinderAStar.find (guards[indexGuard-nbPoint].transform.position,
			                                                     Areas.setOfPointCoveringArea.ElementAt (indexPath.ElementAt (0)));
			float pathLength = tmpPath.lengthPath;
			String globalPathmsg = tmpPath.toString ();
			for(int i=0;i<indexPath.Count-1;i++){
				int startIndex = indexPath.ElementAt(i);
				int endIndex = indexPath.ElementAt(i+1);
				Vector3 start = Areas.setOfPointCoveringArea.ElementAt(startIndex);
				Vector3 end = Areas.setOfPointCoveringArea.ElementAt(endIndex);
				tmpPath = PathFinderAStar.find(start,end);
				pathLength += tmpPath.lengthPath;
				globalPathmsg += tmpPath.toString();
			}
			print ("Global path for guard " + indexGuard + " = " + globalPathmsg);
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
			print ("indexGuardInPermutation = " + indexGuardInPermutation + " currentIndex = " + currentIndex);
			GameObject[] guards = GameObject.FindGameObjectsWithTag ("police");
			PathFinderAStar.Path tmpPath = PathFinderAStar.find (guards[indexGuard-nbPoint].transform.position,
			                                         Areas.setOfPointCoveringArea.ElementAt (indexPath.ElementAt (0)));

			Debug.DrawLine (guards [indexGuard - nbPoint].transform.position, tmpPath.waypoints.ElementAt (0).pos, Color.blue);
			for (int i=0; i<tmpPath.waypoints.Count-1; i++) {
				Debug.DrawLine(tmpPath.waypoints.ElementAt(i).pos,tmpPath.waypoints.ElementAt(i).pos,Color.blue);		
			}

			for(int i=0;i<indexPath.Count-1;i++){
				int startIndex = indexPath.ElementAt(i);
				int endIndex = indexPath.ElementAt(i+1);
				Vector3 start = Areas.setOfPointCoveringArea.ElementAt(startIndex);
				Vector3 end = Areas.setOfPointCoveringArea.ElementAt(endIndex);
				tmpPath = PathFinderAStar.find(start,end);
				Debug.DrawLine (start, tmpPath.waypoints.ElementAt (0).pos, Color.blue);
				for (int w=0; w<tmpPath.waypoints.Count-1; w++) {
					Debug.DrawLine(tmpPath.waypoints.ElementAt(w).pos,tmpPath.waypoints.ElementAt(i).pos,Color.blue);		
				}
			}
		}
	}
}