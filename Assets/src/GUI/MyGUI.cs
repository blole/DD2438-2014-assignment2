using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public class MyGUI : MonoBehaviour {

		void OnGUI () {
			if (GUI.Button(new Rect(10,10,120,30), "Add police"))
				GameObject.FindObjectOfType<PoliceSpawner>().PoliceCount++;
			if (GUI.Button(new Rect(10,43,120,30), "Remove police"))	{
				Areas.decreaseIndexStaticGuarding();
				GameObject.FindObjectOfType<PoliceSpawner>().PoliceCount--;
			}
			if (GUI.Button(new Rect(10,76,120,30), "Add obstacle"))
				GameObject.FindObjectOfType<ObstacleSpawner>().ObstacleCount++;
			if (GUI.Button(new Rect(10,109,120,30), "Remove obstacle"))
				GameObject.FindObjectOfType<ObstacleSpawner>().ObstacleCount--;
//			if (GUI.Button (new Rect(

			// GUI FOR BEHAVIOR
			// Make a background box
			GUI.Box(new Rect(140,10,530,60), "Behavior Menu");
			if (GUI.Button(new Rect(150,30,120,30), "Random Guarding"))
				GameObject.FindObjectOfType<PoliceSpawner>().Behavior = 0;
			if (GUI.Button(new Rect(280,30,120,30), "Static Guarding"))	{
				GameObject.FindObjectOfType<PoliceSpawner>().Behavior = 1;
			}
			if (GUI.Button(new Rect(410,30,120,30), "Secure"))
				GameObject.FindObjectOfType<PoliceSpawner>().Behavior = 2;
			if (GUI.Button(new Rect(540,30,120,30), "Search & Destroy"))
				GameObject.FindObjectOfType<PoliceSpawner>().Behavior = 3;

			if (GameObject.FindObjectOfType<PoliceSpawner>().Behavior == 1) {
						
			}
		}
	}
}
