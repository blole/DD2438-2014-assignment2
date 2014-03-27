using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{

	public class Police : MonoBehaviour {
		private LinkedList<Waypoint> path = new LinkedList<Waypoint>();
		[Range(0,1)]
		public int guardType = 0; // 0 - random // 1 - static guarding // 2 - secure // 3 - search & destroy

		// Static guarding behavior
		private bool asAStaticGoal = false;
		private Vector3 staticGuardingGoal;

		void Start()
		{
		}
		
		void Update ()
		{
			if(guardType == 0){
				if(asAStaticGoal){
					asAStaticGoal = false;
					Areas.decreaseIndexStaticGuarding();
				}
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
			else if(guardType == 1 ){
				if(!asAStaticGoal){
					int indexNextPoint = Areas.getNextIndexInterestingPoint();
					print (indexNextPoint);
					if(indexNextPoint != -1){
						staticGuardingGoal = Areas.setOfPointCoveringArea.ElementAt(indexNextPoint);
						NavigateTo(staticGuardingGoal);
						asAStaticGoal = true;
					}
					else
					{
						if (UnityEngine.Random.Range(0, 100) == 0){
							staticGuardingGoal = PhysicsHelper.randomPointOnFloor(Waypoints.radius);
							NavigateTo(staticGuardingGoal);
						}
					}
				}
				if(path.Count > 0){
					if (moveToward(path.First().pos))
						path.RemoveFirst();
				}
				else
					if((staticGuardingGoal - transform.position).magnitude > .1f)
						NavigateTo(staticGuardingGoal);
					else
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

































