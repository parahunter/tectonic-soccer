using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TransformExtensions
{
	public static Transform FindClosest(this Transform transform, List<Transform> transforms)
	{
		Transform closest;
		
		if(transform != transforms[0])
			closest = transforms[0];
		else if(transforms.Count > 1)
			closest = transforms[1];
		else
			return null;
		
		float squaredClosestDistance = (transform.position - closest.position).sqrMagnitude;
		
		for(int i = 1 ; i < transforms.Count ; ++i)
		{
			if(transforms[i] == transform)
				continue;
			
			float squaredDistance = (transform.position - transforms[i].position).sqrMagnitude;
			
			if(squaredDistance < squaredClosestDistance)
			{
				closest = transforms[i];
				squaredClosestDistance = squaredDistance;
			}
		}
		
		return closest;
	}
}
