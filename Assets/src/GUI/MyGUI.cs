using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public class MyGUI : MonoBehaviour {

		public static bool FOVstatus = false;

		void OnGUI () {
			if (GUI.Button(new Rect(10,10,120,30), "Add guard"))
				GameObject.FindObjectOfType<PoliceSpawner>().PoliceCount++;
			if (GUI.Button(new Rect(10,43,120,30), "Remove guard"))	{
				Areas.decreaseIndexStaticGuarding();
				GameObject.FindObjectOfType<PoliceSpawner>().PoliceCount--;
			}
			if (GUI.Button(new Rect(10,76,120,30), "Add obstacle")){
				GameObject.FindObjectOfType<PoliceSpawner>().Behavior = 0;
				GameObject.FindObjectOfType<ObstacleSpawner>().ObstacleCount++;
				GameObject.FindObjectOfType<Areas>().initialization = true;
				GameObject.FindObjectOfType<Areas>().updateAreas();
			}
			if (GUI.Button(new Rect(10,109,120,30), "Remove obstacle")){
				GameObject.FindObjectOfType<PoliceSpawner>().Behavior = 0;
				GameObject.FindObjectOfType<ObstacleSpawner>().ObstacleCount--;
				GameObject.FindObjectOfType<Areas>().initialization = true;
				GameObject.FindObjectOfType<Areas>().updateAreas();
			}

			if (GUI.Button(new Rect(10,171,120,30), (FOVstatus ? "Hide FOV" : "Show FOV"))){
				FOVstatus = !FOVstatus;
				GameObject[] guards = GameObject.FindGameObjectsWithTag("police");
				print (""+ (guards.Length) + " <----------- HERE");
				foreach(GameObject guard in guards){
					((ShowFOV)guard.GetComponent("ShowFOV")).show = FOVstatus;
				}
			}

			// Robot count
			int nbRobot = GameObject.FindObjectOfType<PoliceSpawner> ().policeCount;
			String msgRobot = "";
			if(nbRobot < 2)
				msgRobot = "There is " + nbRobot + " guard.";
			else
				msgRobot = "There are " + nbRobot + " guards.";
			GUI.Box(new Rect(10,142,120,25),msgRobot);

			// GUI FOR BEHAVIOR
			// Make a background box
			GUI.Box(new Rect(140,10,530,60), "Behavior Menu");
			if (GUI.Button(new Rect(150,30,120,30), "Random Guarding"))
				GameObject.FindObjectOfType<PoliceSpawner>().Behavior = 0;
			if (GUI.Button(new Rect(280,30,120,30), "Static Guarding"))	{
				GameObject.FindObjectOfType<PoliceSpawner>().Behavior = 1;
			}
			if (GUI.Button(new Rect(410,30,120,30), "Secure")){
				Secure.initialization = true;
				Secure.pathComputed = false;
				GameObject.FindObjectOfType<PoliceSpawner>().Behavior = 2;
			}
			if (GUI.Button(new Rect(540,30,120,30), "Search & Destroy"))
				GameObject.FindObjectOfType<PoliceSpawner>().Behavior = 3;
			
			if (GameObject.FindObjectOfType<PoliceSpawner>().behavior == 0)
				GUI.Box(new Rect(140,80,530,25), "The guards are moving randomly in the area");
			if (GameObject.FindObjectOfType<PoliceSpawner>().behavior == 1){
				String msg = "You need " + Areas.setOfPointCoveringArea.Count + " guard(s) to cover the whole area \n" 
					+ "If you have more, then the extra ones will move randomly";
				GUI.Box(new Rect(140,80,530,40), msg);
			}
		}
	}
}
