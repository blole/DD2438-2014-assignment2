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
			while (permutationGenerator.next()) {
				String msg = "";
				for(int i=0; i < permutationGenerator.n ; i++){
					msg += permutationGenerator.array[i] + ",";
				}
				print (msg);
			}
		}
	}
}