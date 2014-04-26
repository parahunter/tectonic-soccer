using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class URandom
{
	
	#region Fisher–Yates shuffle
	
	public static void Shuffle<T> (ref T[] list)
	{
		for(int i = list.Length - 1 ; i >= 0 ; i--)
		{
			int j = Random.Range(0, i);
			
			T val = list[i];
			list[i] = list[j];
			list[j] = val;
		}
	}
	
	public static void Shuffle<T> (ref List<T> list)
	{
		for(int i = list.Count - 1 ; i >= 0 ; i--)
		{
			int j = Random.Range(0, i);
			
			T val = list[i];
			list[i] = list[j];
			list[j] = val;
		}
	}
	
	#endregion
}
