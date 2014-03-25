using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public class Police : MonoBehaviour {
		private LinkedList<Waypoint> path = new LinkedList<Waypoint>();

		void Start()
		{
		}
		
		void Update ()
		{
			if (path.Count > 0)
			{
				if (moveToward(path.First().pos))
					path.RemoveFirst();
			}
			else
			{
				if (UnityEngine.Random.Range(0, 100) == 0)
					NavigateTo(PhysicsHelper.randomPointOnFloor(Waypoints.radius));

				moveToward(transform.position);
			}
		}

		void NavigateTo(Vector3 to)
		{
			LinkedList<Waypoint> path = PathFinderAStar.find(transform.position, to);
			if (path != null)
				this.path = path;
		}

		protected bool moveToward(Vector3 goal)
		{
			Vector3 toGoal = (goal-transform.position).projectDown().toVector3();
			if (toGoal.magnitude > GetComponent<Vehicle>().maxVelocity*Time.deltaTime)
			{
				GetComponent<Vehicle>().setDesiredDirection(0, toGoal, 0.1f);
				return false;
			}
			else
			{
				GetComponent<Vehicle>().setDesiredDirection(0, Vector3.zero, 0);
				return true;
			}
		}
	}
}

































