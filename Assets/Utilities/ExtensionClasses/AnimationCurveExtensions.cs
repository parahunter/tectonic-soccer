using UnityEngine;
using System.Collections;

public static class AnimationCurveExtensions
{
	public static float lastTime(this AnimationCurve curve)
	{
		return curve[curve.length - 1].time;
	}	
	
}
