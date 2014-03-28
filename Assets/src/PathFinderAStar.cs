using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public static class PathFinderAStar
	{
		static public Path find(Vector3 start, Vector3 goal)
		{
			float radius = Waypoints.radius;
			if (!PhysicsHelper.isClear(goal, radius))
				return null;

			if (PhysicsHelper.isClearPath(start, goal, Waypoints.radius))
				return new PathStep(new Waypoint(goal)).toPath();
			
			bool[] visited = new bool[Waypoints.waypoints.Count];
			PriorityQueue<float, PathStep> queue = new PriorityQueue<float, PathStep>();

			foreach (Waypoint waypoint in Waypoints.waypoints)
			{
				if (PhysicsHelper.isClearPath(start, waypoint.pos, Waypoints.radius))
					queue.Enqueue((start-waypoint.pos).projectDown().magnitude+(waypoint.pos-goal).projectDown().magnitude, new PathStep(waypoint));
			}

			while (!queue.IsEmpty)
			{
				PathStep step = queue.Dequeue();
				visited[step.wp.index] = true;
				
				if (PhysicsHelper.isClearPath(step.wp.pos, goal, radius))
				    return new PathStep(step, new Waypoint(goal)).toPath();
				foreach (Waypoint neighbor in Waypoints.neighbors[step.wp])
				{
					if (!visited[neighbor.index])
					{
						PathStep next = new PathStep(step, neighbor);
						queue.Enqueue(next.totalLength + (next.wp.pos-goal).projectDown().magnitude, next);
					}
				}
			}
			return null;
		}
		
		
		
		class PathStep
		{
			public PathStep previous;
			public Waypoint wp;
			public float totalLength;

			public PathStep(Waypoint wp)
			{
				this.previous = null;
				this.wp = wp;
				this.totalLength = 0;
			}
			
			public PathStep(PathStep previous, Waypoint wp)
			{
				this.previous = previous;
				this.wp = wp;
				this.totalLength = previous.totalLength+(previous.wp.pos-wp.pos).projectDown().magnitude;
			}
			
			public Path toPath()
			{
				LinkedList<Waypoint> waypoints = new LinkedList<Waypoint>();
				waypoints.AddFirst(wp);
				PathStep p = previous;
				while (p != null)
				{
					waypoints.AddFirst(p.wp);
					p = p.previous;
				}
				
				return new Path(waypoints,totalLength);
			}
		}

		public class Path
		{
			public LinkedList<Waypoint> waypoints;
			public float lengthPath;

			public Path(LinkedList<Waypoint> waypoints, float lengthPath){
				this.waypoints = waypoints;
				this.lengthPath = lengthPath;
			}

			public String toString(){
				String result = "Path = ";
				for(int i=0;i<this.waypoints.Count;i++){
					result += this.waypoints.ElementAt(i).pos.ToString() + " - ";
				}
				result += " with length " + this.lengthPath + ".";
				return result;
			}
		}
	}
}