using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public static class Factorial
	{
		public static int getFactorial(int n)
		{
			return getFactorial(n,1);
		}
		
		private static int getFactorial(int n, int result){
			if (n==1)
				return result;
			else
				return getFactorial((n-1),result*n);
		}
	}
}

