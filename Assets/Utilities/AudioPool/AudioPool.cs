using UnityEngine;
using System.Collections;

public class AudioPool : MonoBehaviour 
{
	public GameObject audioObjectPrefab;
		
	private static PoolingSystem<AudioPlayer> pool;
	public int poolSize = 5;
	
	private static AudioPool _instance;
	
	// Use this for initialization
	void Awake() 
	{
		if(_instance == null)
		{
			_instance = this;
			pool = new PoolingSystem<AudioPlayer>(audioObjectPrefab, poolSize);
		}
	}
	
	public static void PlayAudio(AudioClip clip, Vector3 position, float volume)
	{
		if(pool.Count >= 1)
		{
			AudioPlayer player = pool.GetElement();
			player.transform.position = position;
			player.Play(clip, volume);
		}
	}
	
	public static void ReleaseAudio(AudioPlayer player)
	{
		pool.ReleaseElement(player);
	}
}
