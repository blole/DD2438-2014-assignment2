using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
		public class Permutation
		{
				public readonly int[] arrayPerm;
				public readonly int hashCode;
			
				public Permutation (int[] permutation)
				{
					this.arrayPerm = (int[])permutation.Clone();
					this.hashCode = this.arrayPerm.Length;
					for(int i=0;i<this.arrayPerm.Length;i++){
						this.hashCode=unchecked(this.hashCode*314159+this.arrayPerm[i]);
					}
				}

				public override int GetHashCode ()
				{
					return this.hashCode;
				}
		}
}

