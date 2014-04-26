using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ListExtensions
{
	public static T GetLast<T>(this List<T> list) 
	{
       return list[list.Count - 1];
    }
	
	public static T GetRandom<T>(this List<T> list) 
	{
		if(list.Count > 1)
	        return list[Random.Range(0,list.Count)];
		else
			return list[0];
    }
}