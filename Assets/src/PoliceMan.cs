using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public class PoliceMan : MonoBehaviour {
		public float maxVelocity = 1;
		private LinkedList<Waypoint> path = new LinkedList<Waypoint>();

		void Start()
		{
		}
		
		void Update ()
		{
			if (path.Count > 0)
			{
				Debug.Log(moveToward(path.First().pos));
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

		void NavigateTo(Vector2 to)
		{
			LinkedList<Waypoint> path = PathFinderAStar.find(transform.position, to.toVector3());
			if (path != null)
			{
				Debug.Log(path.Count);
				this.path = path;
			}
		}

		protected bool moveToward(Vector3 goal)
		{
			Vector3 toGoal = (goal-transform.position).projectDown().toVector3();
			if (toGoal.magnitude > maxVelocity*Time.deltaTime)
			{
				rigidbody.velocity = toGoal.Clamp(maxVelocity, maxVelocity);
				return false;
			}
			else
			{
				rigidbody.velocity = Vector3.zero;
				return true;
			}
		}
	}
}

































