using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Agent
{
	public class Permutation
	{
		public int n;
		public int[] array;
		private int[] state;
		private int position = 0;

		public Permutation(int n){
			this.n = n;
			this.position = 0;
			this.array = new int[n];
			for(int i=0 ; i < n ; i++){
				this.array[i] = i;
			}
			this.state = new int[n];
		}

		public Boolean next(){
			if(position==0){
				position++;
				return true;
			}
			while(position<n){
				if(state[position]<position){
					int index = (position%2)==0 ? 0 : state[position];
					swap(position,index);
					state[position]++;
					position=1;
					return true;
				}
				else{
					state[position] = 0;
					position++;
				}
			}
			return false;
		}

		private void swap(int i, int j){
			int tmp = array [i];
			array [i] = array [j];
			array [j] = tmp;
		}

		public bool isValid(int nbGuard){
			int nbPoint = n - nbGuard;
			int nbPointRemaining = nbPoint;
			int currentIndex = 0;
			float tmp = (float)nbPoint / (float)nbGuard;
			tmp = (nbPoint / nbGuard == tmp) ? tmp : tmp + 1f;
			int maxPointToCheck = (int)tmp;
			if(array[currentIndex]<nbPoint){
				return false;
			}
			else{
				currentIndex++;
			}
			int startingIndex = currentIndex;
			while(currentIndex < n && (currentIndex-startingIndex)<=maxPointToCheck ){
				if(array[currentIndex]<nbPoint){
					nbPointRemaining--;
					currentIndex++;
				}
				else{
					currentIndex++;
					startingIndex = currentIndex;
				}
			}
			return (nbPointRemaining==0);
		}

		public String toString(){
			String msg = "{";
			for(int i=0; i <n-1;i++){
				msg += array[i] + ",";
			}
			msg += array [n - 1] + "}";
			return msg;
		}
   }
}