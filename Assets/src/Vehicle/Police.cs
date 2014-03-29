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

		// Dynamic guarding behavior
		public bool asADynamicPath = false;
		private List<int> setPointToVisit = new List<int>();
		private int indexInSet = 0;
		private bool increasing = true;

		public int indexIdPolice = -1;

		void Start()
		{
		}
		
		void Update ()
		{
			if(guardType == 0){
				if(asADynamicPath){
					asADynamicPath = false;
				}
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
				if(asADynamicPath){
					asADynamicPath = false;
				}
				if(!asAStaticGoal){
					int indexNextPoint = Areas.getNextIndexInterestingPoint();
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
			else if(guardType == 2){
				if(asAStaticGoal){
					asAStaticGoal = false;
					Areas.decreaseIndexStaticGuarding();
				}

				if(!asADynamicPath){
						indexInSet = 0;
						increasing = true;
//						print ("Determining path for guard " + indexIdPolice);
						setPointToVisit.Clear();
						List<int> tmpSet = Secure.getIndexPointToVisit(indexIdPolice);
						foreach(int i in tmpSet){
							setPointToVisit.Add (i);
						}
//						String debug = "Guard " + this.indexIdPolice + "'s path is now ";
//						foreach(int l in setPointToVisit){
//							debug += l + " - ";	
//						}
//						print (debug);
						asADynamicPath = true;
						if(setPointToVisit.Count != 0)
							NavigateTo(Areas.setOfPointCoveringArea.ElementAt(setPointToVisit.ElementAt(indexInSet)));
				}

				if(path.Count > 0){
					if (moveToward(path.First().pos))
						path.RemoveFirst();
				}
				else{
					// If we have reach a checkpoint
//					print ("Guard " + indexIdPolice + " has " + setPointToVisit.Count + " points to visit and is going to" + indexInSet);
//					print ("Next point to visit = " + Areas.setOfPointCoveringArea.ElementAt(setPointToVisit.ElementAt(indexInSet)));
					if(setPointToVisit.Count != 0 && 
					   (Areas.setOfPointCoveringArea.ElementAt(setPointToVisit.ElementAt(indexInSet)) - transform.position).magnitude < .5f){
						// Find index next point to visit

						if(increasing){
							if(indexInSet+1 < this.setPointToVisit.Count)
								indexInSet++;
							else{
								increasing = false;
								indexInSet--;
							}
						}
						else
						{
							if(indexInSet > 0)
								indexInSet--;
							else{
								increasing = true;
								indexInSet++;
							}
						}
						if(this.setPointToVisit.Count==1)
							indexInSet=0;

						NavigateTo(Areas.setOfPointCoveringArea.ElementAt(setPointToVisit.ElementAt(indexInSet)));
					}
				}

			}
		}

		void NavigateTo(Vector3 to)
		{
			PathFinderAStar.Path path = PathFinderAStar.find(transform.position, to);
			if (path != null)
				this.path = path.waypoints;
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

































