using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using LibTessDotNet;

namespace Agent
{
	public static class ExtensionsLinq
	{
		public static IEnumerable<Pair<T>> pairs<T>(this IEnumerable<T> elements)
		{
			T prev = elements.First();
			foreach (T curr in elements.Skip(1))
			{
				yield return new Pair<T>(prev, curr);
				prev = curr;
			}
		}
	}
}