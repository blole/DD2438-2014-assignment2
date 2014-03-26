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
			if (GUI.Button(new Rect(10,43,120,30), "Remove police"))
				GameObject.FindObjectOfType<PoliceSpawner>().PoliceCount--;
		}
	}
}
