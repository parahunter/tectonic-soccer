using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void OnBeat();

public class BeatDetector : MonoBehaviour 
{
	public enum SampleRate {Rate64 = 64, Rate512 = 512,Rate1024 = 1024};
	public SampleRate sampleRate = SampleRate.Rate1024;

	public List<FrequencyBand> subBands;

	public float bpmWindowSize = 10f;
	public float beatThreshold = 1.5f;
	public float beatCutoffThreshold = 1f;
	
	public int thresholdRadius = 5;
	public int analyticsCyclesPerFrame = 5;
	
	public bool storeSpectrumData = false;

	private int _sampleRate;
	private List<float[]> _spectrumData;
	private List< List<float> > _spectralFlux;
	private List<float> _bpmCurve;
	private List< List<float> > _beats;
	private List<int> _summedSpectrum;
	private int _thresholdWindowSize;
	private List<float> _nextBeatVals;
	
	private float _progress = 0;
	public float Progress
	{
		get{return _progress;}	
	}
		 
	private int _spectrumSamples;
	private int _subBandWidth;
	
	private FFT _fft;
	private float[] _lastSpectrum;
	
	private AudioClip _clip;
	private AudioSource _ExtSource;
	
	private bool _isPlaying = false;
	
	private OnBeat[] _onBeatDelegates;

	public void Awake()
	{
		_onBeatDelegates = new OnBeat[subBands.Count];
	}
		
	public void AddBeatListener(OnBeat onBeatFunc, int index = 0)
	{
		_onBeatDelegates[index] += onBeatFunc;	
	}
	
	public void RemoveBeatListener(OnBeat onBeatFunc, int index = 0)
	{
		_onBeatDelegates[index] -= onBeatFunc;
	}
	
	public void SynchAndPlay(AudioSource audioSurce)
	{
		if(_clip != null)
		{
			this._ExtSource	= audioSurce;
			_isPlaying = true;
		}
	}
	
	public void Initialize (AudioClip clip, float offset = 0) 
	{
		_offset = (int) offset * clip.frequency;
		
		StartCoroutine(_AnalyseClip(clip));
	}
	
	private IEnumerator _AnalyseClip(AudioClip clip)
	{
		_clip = clip;
		
		_sampleRate = (int)sampleRate;
		_spectrumSamples = (int)(_clip.frequency * _clip.length / _sampleRate);
		_lastSpectrum = new float[_sampleRate / 2 + 1];
		_thresholdWindowSize = thresholdRadius * 2 + 1;
		
		_fft = new FFT(_sampleRate, _clip.frequency);

		//if(subBandFrequencies.Length > 1)
		{
			_fft.CustomAverages( subBands.ToArray() );
//			_fft.CustomAverages(subBandFrequencies);
//			subBands = subBandFrequencies.Length - 1;	
		}
//		else if(subBands > 1)
//		_fft.LogAverages(subBands.Count);
		
		if(storeSpectrumData)
			_spectrumData = new List<float[]>(_spectrumSamples);
		
		_subBandWidth = _sampleRate / (subBands.Count * 2);
		
		_spectralFlux = new List< List <float> >(subBands.Count);
		_beats  = new List<List <float>>(subBands.Count);
		_nextBeatVals = new List<float>(subBands.Count);
				
		for(int i = 0 ; i < subBands.Count ; i++)
		{
			_spectralFlux.Add( new List<float>(_spectrumSamples) );
			_beats.Add( new List<float>(_spectrumSamples) );
			_nextBeatVals.Add(0);
		}	
		
		_bpmCurve = new List<float>(_spectrumSamples);
		_summedSpectrum = new List<int>(_spectrumSamples);
		
		yield return StartCoroutine(_GetSpectrumData());
	}
	
	private IEnumerator _GetSpectrumData()
	{		
		//since we want to do multiple gatherings of data per frame but start with enough data to calculate and
		//ongoing average we need to calculate this amount of samples first
		
		int precalculatedValues = analyticsCyclesPerFrame + _thresholdWindowSize + (analyticsCyclesPerFrame - _thresholdWindowSize);
		
		for(int i = 0 ; i < precalculatedValues ; i++)
			_GatherSpectrumData(i * _sampleRate);
				
		StartCoroutine( _GatherBeats() );
		StartCoroutine( _GatherBPM() );
		
		for(int i = precalculatedValues / analyticsCyclesPerFrame ; i < _clip.samples / (_sampleRate * analyticsCyclesPerFrame) ; i++)
		{
			for(int k = 0 ; k < analyticsCyclesPerFrame ; k++)
			{
				_GatherSpectrumData( (i*analyticsCyclesPerFrame + k ) * _sampleRate);
			}
			
			_progress = (float)i * analyticsCyclesPerFrame * _sampleRate / _clip.samples;
						
			yield return 0;
		}
			
		_progress = 1;
		
	}
	
			
	private void _GatherSpectrumData(int sampleIndex)
	{
		float[] samples = _StereoToMonoData(sampleIndex, _sampleRate);
		
		float[] spectrum = _fft.GetSpectrum(samples);
						
		if(subBands.Count > 1)
			spectrum = _fft.GetAverages();
		
		_AddSummedSpectrum(spectrum);
		
		if(storeSpectrumData)
			_spectrumData.Add(spectrum);
		
		for(int i = 0 ; i < subBands.Count ; i++)
		{
			float spectralFlux = _GetSpectralFlux(_lastSpectrum, spectrum, i);
			_spectralFlux[i].Add (spectralFlux);
		}
		
		_lastSpectrum = (float[]) spectrum.Clone();	
	}
	
	private void _AddSummedSpectrum(float[] spectrum)
	{
		float sum = 0;
		
		foreach(float val in spectrum)
		{
			sum += val;	
		}
		
		_summedSpectrum.Add((int)(sum * 9999));
	}
	
			
	private IEnumerator _GatherBPM()
	{
		//int samplesPerBPMCalculation = (int)(bpmWindowSize * _ExtSource.clip.frequency) / _sampleRate;
		
		for(int i = 0 ; i < _spectrumSamples / analyticsCyclesPerFrame ; i++)
		{
			for(int k = 0 ; k < analyticsCyclesPerFrame ; k++)
			{
				_bpmCurve.Add( CalculateBPM(i * analyticsCyclesPerFrame + k, bpmWindowSize) );
				//_beats.Add( _GetBeat( i * repititionsPerFrame + k ) );
			}
			
			yield return 0;
		}
	}
	
	private IEnumerator _GatherBeats()
	{
		for(int i = 0 ; i < _spectrumSamples ; i++)
		{
			for(int k = 0 ; k < analyticsCyclesPerFrame ; k++)
			{
				for(int l = 0 ; l < subBands.Count ; l++)
					_beats[l].Add( _GetBeat( i * analyticsCyclesPerFrame + k , l ) );
			}
			
			_progress = (float)i * analyticsCyclesPerFrame / _spectrumSamples;
			
			yield return 0;
		}
	}
	
	
	private float _GetBeat(int index, int subBand)
	{
		float beatVal = _nextBeatVals[subBand];
		_nextBeatVals[subBand] = _GetThresholdedValue(index + 1, subBand);
				
		return beatVal > _nextBeatVals[subBand] ? beatVal : 0 ;
	}
	
	private float _GetThresholdedValue(int index, int subBand)
	{
		
		List<float> _flux = _spectralFlux[subBand];
		
		if(index >= _flux.Count)
			return 0;
		
		int start = Mathf.Max(0, index - thresholdRadius);
		int end   = Mathf.Min(_flux.Count - 1 , index + thresholdRadius); 
				
		float threshold = 0;
				
		for(int i = start ; i < end + 1 ; i++)
		{
			threshold += _flux[i];		
		}
		
		threshold /= analyticsCyclesPerFrame;
		threshold *= beatThreshold;
		
		return threshold < _flux[index] ? _flux[index] - threshold : 0 ;
	}
	
	private float _GetSpectralFlux(float[] lastSpectrum, float[] currentSpectrum, int subBand)
	{
		float val = 0;
		
		if(subBands.Count > 1)
		{
			val = currentSpectrum[subBand] - lastSpectrum[subBand];
		}
		else
		{
			for(int i = subBand * _subBandWidth ; i < (subBand + 1) * _subBandWidth ; i++)
				val += currentSpectrum[i] - lastSpectrum[i];
		}		
		val = val < 0 ? 0 : val;

		
		return val;
	}
	
	private int _lastPlayedSample = 0;
	private int _count = 10;
	private const int _minSamples = 1;
	public void Update()
	{
		//so we can easily mute the sound while we develop
		if(Application.isEditor && Input.GetKeyDown(KeyCode.M))
			_ExtSource.mute = !_ExtSource.mute;
		
		if(!_isPlaying || _onBeatDelegates == null)
			return;
		
		int currentSample = _ExtSource.timeSamples + _offset;
				
		_count = (currentSample - _lastPlayedSample) / _sampleRate;
		_count = _count > 0 ? _count : _minSamples;
		
		
		for(int b = 0 ; b < subBands.Count ; b++)
		{
		
			float[] beats = GetBeatsRange(_lastPlayedSample, _count, b);
			
			for(int i = 0 ; i < beats.Length ; i++)
			{
				
				if(beats[i] > beatCutoffThreshold)
				{
					if(_onBeatDelegates[b] != null)
						_onBeatDelegates[b]();
					
					_lastPlayedSample = currentSample;
				}
			}
		}
	}
	
	private int lastVal = 9999;
	public int GetHash()
	{
		int index = _SampleToSpectrumIndex(_lastPlayedSample);
		if(_summedSpectrum.Count < index)
		{
			lastVal += lastVal;
			return lastVal;
		}
		
		lastVal += _summedSpectrum[index];
		return lastVal;
		
	}
	
	public float GetCurrentBPM()
	{
		if(_ExtSource != null)
		{
			return GetBPMRange(_ExtSource.timeSamples, 1)[0];
		}
		else
		{
			return 0f;
		}
	}
	
	private int _offset = 0;
	private float CalculateBPM(int currentSample, float duration)
	{
		_count =  (int) (duration * _clip.frequency / _sampleRate);
		
		float BPM = 0;
		for(int b = 0 ; b < subBands.Count ; b++)
		{
			float[] beats = GetBeatsRange(currentSample, _count, b);
			
			for(int i = 0 ; i < beats.Length ; i++)
			{
				if(beats[i] > 0f)
				{
					BPM += 1;
				}
			}
		}
			
		return BPM * 60f / duration;
	}
	
	#region range getters
		
	public float[] GetSpectralFluxRange(int startSample, int count, int subBand)
	{
		int start = _SampleToSpectrumIndex(startSample);
		
		count = start + count < _spectralFlux[subBand].Count ? count : _spectralFlux[subBand].Count - start;
		
		return _spectralFlux[subBand].GetRange(start, count).ToArray();
	}
	
	public float[] GetBPMRange(int startSample, int count)
	{
		int start = _SampleToSpectrumIndex(startSample);
		
		count = start + count < _bpmCurve.Count ? count : _bpmCurve.Count - start;
		
		if(_bpmCurve.Count > start + count)
			return _bpmCurve.GetRange(start, count).ToArray();
		else
		{
			return new float[]{0};	
		}
	}
	
	public float[] GetBeatsRange(int startSample, int count, int subBand = 0)
	{
		int start = _SampleToSpectrumIndex(startSample);
		
		if(start + count < _beats[subBand].Count)
		{
			return _beats[subBand].GetRange(start, count).ToArray();
		}
		else
		{
			if(_beats.Count <= 0)
				return new float[]{0};
			else 	
				return _beats[subBand].GetRange(0, _beats[subBand].Count - 1).ToArray();
		}
	}
	
	#endregion
	
	private int _SampleToSpectrumIndex(int samplePosition)
	{
		return samplePosition / _sampleRate;
	}
	
	private float[] _StereoToMonoData(int sample, int amount)
	{
		float[] data = new float[amount * _clip.channels];
		_clip.GetData(data, sample);
			
		
		if(_clip.channels == 2)
		{
			float[] data2 = new float[amount];
			for(int i = 0, k = 0 ; k < amount * _clip.channels ; i++, k += _clip.channels)
			{
				data2[i] = data[k] + data[k + 1];
			}
			
			return data2;
		}
		else
			return data;
	}	

}
