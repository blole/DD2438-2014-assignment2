using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public static class TabuSearch
	{
		private static int STEP_MAX = 1000;
		private static int TABU_LIST_SIZE = 100;
		private static HashSet<Permutation> tabuHashSet = new HashSet<Permutation>();
		private static LinkedList<Permutation> tabuLinkedList = new LinkedList<Permutation>();

		static public int[] Search(int nbGuard, int nbPoint, float[,] costs)
		{
			Permutation bestPermutation = getValidPermutation(nbGuard,nbPoint);
			float bestCost = getCost (nbGuard, bestPermutation, costs);

			int step = 0;
			while((step++)<STEP_MAX){
				List<Permutation> neighbors = getNeighbors(nbGuard,bestPermutation);
				Permutation bestPermSoFar = new Permutation(new int[nbGuard+nbPoint]);
				float bestCostSoFar = Mathf.Infinity;
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
			}

			return bestPermutation.arrayPerm;
		}

		// Return the first valid permutation (starting from (1,...,nbGuard+nbPoint))
		static private Permutation getValidPermutation(int nbGuard, int nbPoint){
			PermutationGenerator permutationGenerator = new PermutationGenerator (nbGuard + nbPoint);
			while(!isValid (new Permutation(permutationGenerator.array),nbGuard))
				permutationGenerator.next();
			Permutation validPerm = new Permutation (permutationGenerator.array);
			return validPerm;
		}

		// Need a valid permutation
		static private float getCost(int nbGuard, Permutation permutation, float[,] costs){
			float length = 0f;
			int nbPoint = permutation.arrayPerm.Length - nbGuard;
			int currentIndex = 0;
			while(currentIndex+1<nbGuard+nbPoint){
				if(permutation.arrayPerm[currentIndex+1] >= nbPoint)
					currentIndex++;
				else{
					length += costs[permutation.arrayPerm[currentIndex],permutation.arrayPerm[currentIndex+1]];
                    currentIndex++;
				}
			}
			return length;
		}

		// Generate neighbors for a given permutation
		static private List<Permutation> getNeighbors(int nbGuard, Permutation permutation){
			List<Permutation> neighbors = new List<Permutation> ();

		}

		// Swap next to each other element
		static private List<Permutation> swap1(Permutation permutation, int nbGuard){
			List<Permutation> swap1 = new List<Permutation> ();
			int n = permutation.arrayPerm.Length;
			for (int i=0; i<n-1; i++) {
				Permutation newPerm = new Permutation(permutation.arrayPerm);
				newPerm.arrayPerm[i] = newPerm.arrayPerm[i+1];
				newPerm.arrayPerm[i+1] = tmp;
				if(isValid(newPerm))
					swap1.Add (newPerm);
			}
		}

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
