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
		void Start ()
		{

		}

#if UNITY_EDITOR
		void Update ()
		{
			print ("Coucou!");
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
				for(int r=0;r<nbGuard;r++){
				
				}
			}
		}

		float getPathLength(int indexGuard, int[] currentPermutation, int nbGuard, int nbPoint){
			// Find the subset of the permutation corresponding to the indexGuard guard path
			int indexGuardInPermutation = 0;
			while(currentPermutation[indexGuardInPermutation] != indexGuard){
				indexGuardInPermutation++;
			}
			List<int> indexPath = new List<int> ();
			int currentIndex = indexGuardInPermutation+1;
			while(currentPermutation[currentIndex] < nbPoint){
				indexPath.Add (currentPermutation[currentIndex]);
            	currentIndex++;
			}
			float pathLength = 0f;
			for(int i=0;i<indexPath.Count-1;i++){
				int startIndex = indexPath.ElementAt(i);
				int endIndex = indexPath.ElementAt(i+1);
				Vector3 start = Areas.setOfPointCoveringArea.ElementAt(startIndex);
				Vector3 end = Areas.setOfPointCoveringArea.ElementAt(startIndex);
				PathFinderAStar.Path tmpPath = PathFinderAStar.find(start,end);
				pathLength += tmpPath.lengthPath;
			}
			return pathLength;
		}
	}
}