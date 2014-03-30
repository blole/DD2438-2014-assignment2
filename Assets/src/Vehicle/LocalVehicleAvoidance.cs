using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public class LocalVehicleAvoidance : MonoBehaviour {

		public float minimumDistance = 0.5f;
		private Func<float, float> distanceToForceTransform;

		void Start () {
			distanceToForceTransform = x => x<minimumDistance?1-Mathf.Pow(x/minimumDistance,2):0;
		}
		
		void Update () {
			Vector3 velocity = Vector3.zero;
			
			foreach (Vehicle vehicle in FindObjectsOfType(typeof(Vehicle)))
			{
				if (vehicle.gameObject == gameObject)
					continue;
				
				Vector3 toOther = (vehicle.transform.position-transform.position).projectDown().toVector3();
				if (toOther.magnitude == 0)
					velocity += UnityEngine.Random.insideUnitCircle.toVector3();
				else
					velocity += -toOther * distanceToForceTransform(toOther.magnitude);
			}
			velocity = Quaternion.AngleAxis (90f, Vector3.up) * velocity;
			GetComponent<Vehicle>().setDesiredDirection(1, velocity.normalized, velocity.magnitude);
		}
	}
}