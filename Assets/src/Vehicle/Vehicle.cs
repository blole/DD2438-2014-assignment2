using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public class Vehicle : MonoBehaviour
	{
		public float maxVelocity = 1;
		private Vector3[] desiredDirections = new Vector3[2];
		private float[] importance = new float[2];

		public void setDesiredDirection(int index, Vector3 direction, float importance)
		{
			desiredDirections[index] = direction.normalized;
			this.importance[index] = importance;
		}

		void Update ()
		{
			Vector3 direction = Vector3.Lerp(desiredDirections[0], desiredDirections[1], importance[1]/importance.Sum()).normalized;
			rigidbody.velocity = direction * maxVelocity;
		}
	}
}