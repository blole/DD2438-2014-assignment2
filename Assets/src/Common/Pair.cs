using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LibTessDotNet;

namespace Agent
{
	public class Pair<T>
	{
		public T a;
		public T b;
		
		public Pair(T a, T b)
		{
			this.a = a;
			this.b = b;
		}
	}
}
