using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeatDetectorDebugger : MonoBehaviour 
{
	private AudioSource _playedAudio;
	
	private BeatDetector _beatDetector;
	
	private int currentSample = 0;
	public int sampleRadius = 10;
	
	private Texture2D _spectrumTex;


	// Use this for initialization
	void Start () 
	{
		_playedAudio = GameObject.Find("TrackPlayer").GetComponent<AudioSource>();


		_beatDetector = GetComponent<BeatDetector>();
		_beatDetector.Initialize( _playedAudio.clip );
		_beatDetector.SynchAndPlay(GameObject.Find("TrackPlayer").audio);
	}
	
	void OnDrawGizmos()
	{
		if(currentSample > 50 * sampleRadius)
		{
			List<float[]> spectralFlux = new List<float[]>();
			List<float[]> beats = new List<float[]>();
			float[] bpm = _beatDetector.GetBPMRange(currentSample, sampleRadius);
			
			for(int i = 0 ; i < _beatDetector.subBands.Count ; i++)
			{
				spectralFlux.Add( _beatDetector.GetSpectralFluxRange(currentSample, sampleRadius, i));
				beats.Add( _beatDetector.GetBeatsRange(currentSample, sampleRadius, i) );
			}
				
			Vector3 start = Vector3.left * 10;
			Vector3 distance = Vector3.right * 2;
			Vector3 step = Vector3.right * 2;
			
			Vector3 lastBasePos = start;
						
			//print (data[0].length);
			
			Vector3 upLast = Vector3.zero;
			for(int x = 0 ; x < spectralFlux.Count; x++)
			{
				
				Gizmos.color = Color.blue;	
//				for(int y = 0 ; y < spectrumData[x].Length ; y ++)
//				{
//					Vector3 upNow = Vector3.up * spectrumData[x][y] * 1f;
//					
//					Vector3 currentBasePos = start + x * Vector3.forward * 2 + distance * (float)y / spectrumData[0].Length;
//					Gizmos.DrawLine(lastBasePos + upLast, currentBasePos + upNow);
//					
//					lastBasePos = currentBasePos;
//					upLast = upNow;
//				}
				
				Vector3 upFluxLast = Vector3.zero;
				Gizmos.color = Color.green;
				for(int z = 0; z < spectralFlux[x].Length ; z++)
				{
					Vector3 upFluxNow = z * Vector3.forward * 2 + Vector3.up * spectralFlux[x][z] * 1f;
					Gizmos.DrawLine(start + distance + upFluxLast, start + distance + upFluxNow);
					upFluxLast = upFluxNow;	
				}
				
				distance += Vector3.right;
				
				Vector3 upBeatLast = Vector3.zero;
				Gizmos.color = Color.red;
				for(int z = 0; z < beats[x].Length ; z++)
				{
					
					Vector3 upBeatNow = z * Vector3.forward * 2 + Vector3.up * beats[x][z] * 1f;
					Gizmos.DrawLine(start + distance + upBeatLast, start + distance + upBeatNow);
					upBeatLast = upBeatNow;	
				}
				
				distance += step;
				
//				lastBasePos = start + (x + 1) * Vector3.forward * 2;
//				
//				Gizmos.color = Color.green;
//				Vector3 upFluxNow = x * Vector3.forward * 2 + Vector3.up * spectralFlux[x] * 0.04f;
//				Gizmos.DrawLine(start + distance + upFluxLast, start + distance + upFluxNow);
//				upFluxLast = upFluxNow;
//				
//				Gizmos.color = Color.red;
//				Vector3 upBeatNow = x * Vector3.forward * 2 + Vector3.up * beats[x] * 0.04f;
//				Gizmos.DrawLine(Vector3.right + start + distance + upBeatLast,Vector3.right +  start + distance + upBeatNow);
//				upBeatLast = upBeatNow;
//				
			}
			
			Vector3 upBPMLast = Vector3.zero;
			for(int z = 0 ; z < bpm.Length ; z++)
			{
				Gizmos.color = Color.black;
				Vector3 upBPMNow = z * Vector3.forward * 2 + Vector3.up * bpm[z] * 0.04f;
				Gizmos.DrawLine(Vector3.right * 2 + start + distance + upBPMLast,Vector3.right * 2 +  start + distance + upBPMNow);
				upBPMLast = upBPMNow;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentSample = _playedAudio.timeSamples;
	}
}
