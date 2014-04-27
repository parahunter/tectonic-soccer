using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour 
{
	public float shakeStayTime = 0.1f;
	
	private static CameraShake instance;
	
	public void Awake()
	{
		instance = this;	
	}
	
	
	public static void Shake(AnimationCurve curve)
	{
		instance.StartCoroutine(instance._Shake(curve));
	}
	
	public static void Shake(float duration, float magnitude)
	{
		instance.StartCoroutine(instance._Shake(duration, magnitude));
	}
	
	IEnumerator _Shake(AnimationCurve curve)
	{
		Vector3 startPos = transform.position;
		
		float duration = curve.lastTime();
		
		yield return StartCoroutine(pTween.RealtimeTo(duration, 0f, duration, t =>
		{
			float magnitude = (curve.Evaluate(t) / 360f) * Camera.main.orthographicSize;
			
			Vector3 randPos = new Vector3(Random.Range(-magnitude, magnitude), 0, Random.Range(-magnitude, magnitude));
			
			transform.position = startPos + randPos;
			
			t += shakeStayTime / Time.timeScale;
		}));
		
		transform.position = startPos;
	}
	
	IEnumerator _Shake(float duration, float magnitude)
	{
		
		Vector3 startPos = transform.position;
		
		yield return StartCoroutine(pTween.RealtimeTo(duration, 0, duration, t =>
		{
			Vector3 randPos = new Vector3(Random.Range(-magnitude, magnitude), 0, Random.Range(-magnitude, magnitude));
			
			transform.position = startPos + randPos;
			
			t += shakeStayTime / Time.timeScale;
			
		}));
		
		transform.position = startPos;
	}
}
