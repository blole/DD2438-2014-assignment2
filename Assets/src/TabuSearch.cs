using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public static class TabuSearch
	{
		private static int STEP_MAX = 100;
		private static int TABU_LIST_SIZE = 100;
		private static int noImprovement_MAX = 10;
		private static HashSet<Permutation> tabuHashSet = new HashSet<Permutation>();
		private static LinkedList<Permutation> tabuLinkedList = new LinkedList<Permutation>();
		private static PermutationGenerator permutationGenerator;
		private static int RANDMAX = 1000;

		static public int[] Search(int nbGuard, int nbPoint, float[,] costs)
		{
//			int RANDMAX = Factorial.getFactorial(nbPoint+nbGuard) / Factorial.getFactorial((nbPoint+nbGuard)/2);
			permutationGenerator = new PermutationGenerator(nbGuard+nbPoint);
//			Permutation bestPermutation = randomPermutation(nbGuard,nbPoint,RandomRange);
			Permutation bestPermutation = randomPermutation(nbGuard);
			Permutation savedbestPermutation = bestPermutation;
			float bestCost = getCost (nbGuard, bestPermutation, costs);
			Debug.Log ("First permutation has cost = " + bestCost);
			float savedBestCost = bestCost;
			int step = 0;
			int noImprovementCount = noImprovement_MAX;
			Permutation previousBestPermutation = bestPermutation;
			while((step++)<STEP_MAX){
				List<Permutation> neighbors = getNeighbors(nbGuard,bestPermutation);
				Permutation bestPermSoFar = new Permutation(new int[nbGuard+nbPoint]);
				float bestCostSoFar = Mathf.Infinity;
				
				// Debug printing
				Debug.Log("Best permutation = " + bestPermutation.toString() + " with cost = " + bestCost);
				
				foreach(Permutation neighbor in neighbors){
					if(!tabuHashSet.Contains(neighbor)){
						float cost = getCost(nbGuard,neighbor,costs);
						if(cost<bestCostSoFar){
							bestCostSoFar = cost;
							bestPermSoFar = neighbor;
						}
					}
				}
				if(bestCostSoFar < bestCost){
					bestPermutation = bestPermSoFar;
					bestCost = bestCostSoFar;
					updateTabu(bestPermutation);
				}
				
				// Checking for improvement
				if(previousBestPermutation == bestPermutation){
					noImprovementCount--;
				}
				else{
					noImprovementCount=noImprovement_MAX;
				}
				previousBestPermutation = bestPermutation;
				
				// In case that there is no improvement, go elsewhere
				if(noImprovementCount == 0){
					if(savedBestCost > bestCost){
						savedBestCost = bestCost;
						savedbestPermutation = bestPermutation;
					}
//					bestPermutation = randomPermutation(nbGuard,nbPoint,RandomRange);
					bestPermutation = randomPermutation(nbGuard);
					bestCost = getCost(nbGuard,bestPermutation,costs);
					noImprovementCount = noImprovement_MAX;
				}
			}

			return savedbestPermutation.arrayPerm;
		}

		static private Permutation randomPermutation(int nbGuard){
//			int random = UnityEngine.Random.Range(0,RANDMAX);
//			while(random-->0)
//				permutationGenerator.next();
//			while(!isValid (new Permutation(permutationGenerator.array),nbGuard))
//				permutationGenerator.next();
//			Permutation validPerm = new Permutation (permutationGenerator.array);
//			return validPerm;
			int nbIntFound = 0;
			int currentIndex = 0;
			int n = permutationGenerator.array.Length;
			Permutation randomPerm = new Permutation(new int[n]);
			while(!isValid(randomPerm,nbGuard)){
				currentIndex = 0;
				for(int i=0;i<n;i++){
					randomPerm.arrayPerm[i] = 0;
				}
				while(currentIndex<n){
					int rand = UnityEngine.Random.Range(0,n);
					bool isCorrect = true;
					for(int i=0;i<currentIndex;i++){
						if(rand==randomPerm.arrayPerm[i]){
							isCorrect = false;
							break;
						}
					}
					if(isCorrect){
						randomPerm.arrayPerm[currentIndex] = rand;
						currentIndex++;
					}
				}
			}
//			Debug.Log ("Found random perm =" + randomPerm.toString());
			return randomPerm;
		}

		// Need a valid permutation
//		static private float getCost(int nbGuard, Permutation permutation, float[,] costs){
//			float length = 0f;
//			int nbPoint = permutation.arrayPerm.Length - nbGuard;
//			int currentIndex = 0;
//			
//			float currentLength = 0;
//			while(currentIndex+1<nbGuard+nbPoint){
//				if(permutation.arrayPerm[currentIndex+1] >= nbPoint)
//					currentIndex++;
//				else{
//					length += costs[permutation.arrayPerm[currentIndex],permutation.arrayPerm[currentIndex+1]];
//                    currentIndex++;
//				}
//			}
//			return length;
//		}

		static private float getCost(int nbGuard, Permutation permutation, float[,] costs){
			int nbPoint = permutation.arrayPerm.Length - nbGuard;
//			return permutation.arrayPerm.Split(i=>i>=nbPoint).Select(list=>
//				{
//					float length = 0;
//					for (int i=0; i<list.Count-1; i++)
//						length += costs[list[i], list[i+1]];
//					return length;
//				}).Max();
			int currentIndex = 0;
			float length = 0f;
			float currentLength = 0f;
			while(currentIndex+1<nbGuard+nbPoint){
				if(permutation.arrayPerm[currentIndex+1] >= nbPoint)
				{
//					Debug.Log ("GET COST, lengthprevious = " + length);
					length = Mathf.Max(length, currentLength);
//					Debug.Log("GET COST, length new = " + length);
					
					currentLength = 0f;
					currentIndex++;
				}
				else{
					currentLength += costs[permutation.arrayPerm[currentIndex],permutation.arrayPerm[currentIndex+1]];
                    currentIndex++;
				}
			}
			return Mathf.Max (length,currentLength);
		}

		// Generate neighbors for a given permutation
		static private List<Permutation> getNeighbors(int nbGuard, Permutation permutation){
			List<Permutation> neighbors = new List<Permutation> ();
			neighbors.AddRange(swap1(permutation,nbGuard));
			neighbors.AddRange(swap2(permutation,nbGuard));
			return neighbors;	
		}

		// Swap next to each other element
		static private List<Permutation> swap1(Permutation permutation, int nbGuard){
			List<Permutation> swap1 = new List<Permutation> ();
			int n = permutation.arrayPerm.Length;
			for (int i=0; i<n-1; i++) {
				Permutation newPerm = new Permutation(permutation.arrayPerm);
				int tmp = newPerm.arrayPerm[i];
				newPerm.arrayPerm[i] = newPerm.arrayPerm[i+1];
				newPerm.arrayPerm[i+1] = tmp;
				if(isValid(newPerm,nbGuard))
					swap1.Add (newPerm);
			}
			return swap1;
		}

		// Swap two next to each other element with the two next ones
		static private List<Permutation> swap2(Permutation permutation, int nbGuard){
			List<Permutation> swap2 = new List<Permutation> ();
			int n = permutation.arrayPerm.Length;
			for (int i=0; i<n-3; i++) {
				Permutation newPerm = new Permutation(permutation.arrayPerm);
				int t1 = newPerm.arrayPerm[i];
				int t2 = newPerm.arrayPerm[i+1];
				int t3 = newPerm.arrayPerm[i+2];
				int t4 = newPerm.arrayPerm[i+3];
				newPerm.arrayPerm[i] = t3;
				newPerm.arrayPerm[i+1] = t4;
				newPerm.arrayPerm[i+2] = t1;
				newPerm.arrayPerm[i+3] = t2;
				if(isValid(newPerm,nbGuard))
					swap2.Add (newPerm);
			}
			return swap2;
		}
		
		// Check if a permutation is valid or not
		static private bool isValid(Permutation permutation, int nbGuard){
			int n = permutation.arrayPerm.Length;
			int nbPoint = n - nbGuard;
			int nbPointRemaining = nbPoint;
			int currentIndex = 0;

			if(permutation.arrayPerm[currentIndex]<nbPoint){
				return false;
			}
			else{
				currentIndex++;
			}

			int startingIndex = currentIndex;
			while(currentIndex < n){
				if(permutation.arrayPerm[currentIndex]<nbPoint){
					nbPointRemaining--;
					currentIndex++;
				}
				else{
					currentIndex++;
					startingIndex = currentIndex;
				}
			}
			return (nbPointRemaining==0);
		}





		// Add the new permutation to the tabuHashSet and tabuLinkedList and remove (eventually) the oldest one
		static private void updateTabu(Permutation newPermutation){
			if(tabuHashSet.Count < TABU_LIST_SIZE){
				tabuHashSet.Add (newPermutation);
				tabuLinkedList.AddLast(newPermutation);
			}
			else{
				// Remove oldest
				Permutation oldestPerm = tabuLinkedList.First();
				tabuHashSet.Remove(oldestPerm);
				tabuLinkedList.RemoveFirst();
				// Add new
				tabuHashSet.Add (newPermutation);
				tabuLinkedList.AddLast(newPermutation);
			}
		}
	}
}
