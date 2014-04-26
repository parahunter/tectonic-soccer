using UnityEngine;
using System.Collections;

/*
 * This class is based on code by Damien Di Fede <ddf@compartmental.net>
 * with the help of his awesome tutorial on beat detection at:
 * http://www.badlogicgames.com/wordpress/?cat=18
 */
public class FFT
{
	private int[] reverse;
	
	private int timeSize;
	private int sampleRate;
	private float bandWidth;
	private float[] real;
	private float[] imag;
	private float[] spectrum;
	private float[] averages;
	private float[] sinlookup;
	private float[] coslookup;
	
	public const int HAMMING = 1;
	
	private enum WhichAverage{noAverage, linear, Log, logBands, custom};
  	
	private FrequencyBand[] customFrequencyBands;

	private WhichAverage whichAverage;
	private int octaves;
	private int avgPerOctave;	
	
	private const float twoPi = 6.28318530718f;
	
	public FFT(int timeSize, int sampleRate)
	{
		this.timeSize = timeSize;
		this.sampleRate = sampleRate;
				
		real = new float[timeSize];
		imag = new float[timeSize];
		spectrum = new float[timeSize / 2 + 1];
		bandWidth = (2f / timeSize) * ((float)sampleRate / 2f);
		
		buildReverseTable();
		buildTrigTables();
	}
	
	private void buildReverseTable()
	{
		int N = timeSize;
		reverse = new int[N];
		
		// set up the bit reversing table
		reverse[0] = 0;
		for (int limit = 1, bit = N / 2; limit < N ; limit <<= 1, bit >>= 1)
		{
			for (int i = 0; i < limit; i++)
		    	reverse[i + limit] = reverse[i] + bit;
		}
	}
	
	public float[] GetSpectrum(float[] samples)
	{
		_DoHammingWindow(ref samples);
				
		float val = 0;
		for(int i = 0 ; i < real.Length ; i++)
			val += real[i] + imag[i];
		
		_ReverseSamples(samples);
		
		val = 0;
		for(int i = 0 ; i < real.Length ; i++)
			val += real[i] + imag[i];
		
		val = 0;
		for(int i = 0 ; i < spectrum.Length ; i++)
			val += spectrum[i];
		
		_FFT();
			
		_GetSpectrum();
		
		val = 0;
		for(int i = 0 ; i < spectrum.Length ; i++)
			
			val += spectrum[i];
			
		return spectrum;
	}
	
	private void _FFT()
	{
	
	    for (int halfSize = 1; halfSize < real.Length; halfSize *= 2)
	    {
			// float k = -(float)Math.PI/halfSize;
			// phase shift step
			// float phaseShiftStepR = (float)Math.cos(k);
			// float phaseShiftStepI = (float)Math.sin(k);
			// using lookup table
			float phaseShiftStepR = _Cos(halfSize);
			float phaseShiftStepI = _Sin(halfSize);
			// current phase shift
			float currentPhaseShiftR = 1.0f;
			float currentPhaseShiftI = 0.0f;
			for (int fftStep = 0; fftStep < halfSize; fftStep++)
			{
				for (int i = fftStep; i < real.Length; i += 2 * halfSize)
				{
					int off = i + halfSize;
					float tr = (currentPhaseShiftR * real[off]) - (currentPhaseShiftI * imag[off]);
					float ti = (currentPhaseShiftR * imag[off]) + (currentPhaseShiftI * real[off]);
					real[off] = real[i] - tr;
					imag[off] = imag[i] - ti;
					real[i] += tr;
					imag[i] += ti;
				}
				
				float tmpR = currentPhaseShiftR;
				currentPhaseShiftR = (tmpR * phaseShiftStepR) - (currentPhaseShiftI * phaseShiftStepI);
				currentPhaseShiftI = (tmpR * phaseShiftStepI) + (currentPhaseShiftI * phaseShiftStepR);
			}
	    }
	}
	
	private void _ReverseSamples(float[] samples)
	{
		for (int i = 0; i < samples.Length; i++)
		{
			
			real[i] = samples[reverse[i]];
			imag[i] = 0.0f;
		}		
	}
	
	private void _DoHammingWindow(ref float[] samples)
	{
		for (int i = 0; i < samples.Length; i++)
    	{
      		samples[i] *= (0.54f - 0.46f * Mathf.Cos(twoPi * i / (samples.Length - 1)));
    	}	
	}
	
	private void _GetSpectrum()
	{
		for (int i = 0; i < spectrum.Length; i++)
    	{
      		spectrum[i] = Mathf.Sqrt(real[i] * real[i] + imag[i] * imag[i]);
    	}

		float loFreq, highFreq;
		switch(whichAverage)
    	{
		case WhichAverage.custom:
			for(int i = 0 ; i < customFrequencyBands.Length ; i++)
			{
				FrequencyBand band = customFrequencyBands[i];
				averages[i] = calcAvg(band.lowerFrequency, band.higherFrequency);
			}
			break;
		case WhichAverage.linear:
			int avgWidth = (int) spectrum.Length / averages.Length;
			for (int i = 0; i < averages.Length; i++)
			{
		        float avg = 0;
		        int j;
		        for (j = 0; j < avgWidth; j++)
	            {
					int offset = j + i * avgWidth;
					if (offset < spectrum.Length)
					{
					avg += spectrum[offset];
					}
					else
					{
					break;
					}
				}
				avg /= j + 1;
				averages[i] = avg;
			}
			break;
		case WhichAverage.logBands:
			highFreq = (float) sampleRate / 2f;
			
			for(int i = averages.Length - 1 ; i >= 0 ; i--)
			{
				loFreq = highFreq / 2;
				
				averages[i] = calcAvg(loFreq, highFreq);
				//Debug.Log("Band " + i + " " + loFreq + " " + highFreq);
				
				highFreq = loFreq;
			}
			//Debug.Break();
			break;
		case WhichAverage.Log:
			for (int i = 0; i < octaves; i++)
			{
				float freqStep;
				if (i == 0)
				{
				  loFreq = 0;
				}
		        else
		        {
		          loFreq = (sampleRate / 2) / Mathf.Pow(2, octaves - i);
		        }
		        highFreq = (sampleRate / 2) / Mathf.Pow(2, octaves - i - 1);
		        freqStep = (highFreq - loFreq) / avgPerOctave;
		        float f = loFreq;
		        for (int j = 0; j < avgPerOctave; j++)
        		{
					int offset = j + i * avgPerOctave;
					averages[offset] = calcAvg(f, f + freqStep);
					f += freqStep;
        		}
			}
			break;
		default:
			break;
		}
	}
	
	public float[] GetAverages()
	{
		return averages;	
	}
	
	public void CustomAverages(FrequencyBand[] customFrequencyBands)
	{
		this.whichAverage = WhichAverage.custom;
		this.customFrequencyBands = customFrequencyBands;
		
		averages = new float[customFrequencyBands.Length];
	}
	
	private int amountOfBands;
	public void LogAverages(int amountOfbands)
	{
		this.amountOfBands = amountOfBands;
		averages = new float[amountOfbands];
		whichAverage = WhichAverage.logBands;
		//Debug.Break();
	}
	
  public void LogAverages(int minBandwidth, int bandsPerOctave)
  {
    float nyq = (float) sampleRate / 2f;
    octaves = 1;
    while ((nyq /= 2) > minBandwidth)
    {
      octaves++;
    }
		

		
    avgPerOctave = bandsPerOctave;
    averages = new float[octaves * bandsPerOctave];
    whichAverage = WhichAverage.Log;
  }	

  public float calcAvg(float lowFreq, float hiFreq)
  {
    int lowBound = freqToIndex(lowFreq);
    int hiBound = freqToIndex(hiFreq);
    float avg = 0;
    for (int i = lowBound; i <= hiBound; i++)
    {
      avg += spectrum[i];
    }
    avg /= (hiBound - lowBound + 1);
    return avg;
  }	
	
  public int freqToIndex(float freq)
  {
    // special case: freq is lower than the bandwidth of spectrum[0]
    if (freq < getBandWidth() / 2) return 0;
    // special case: freq is within the bandwidth of spectrum[spectrum.length - 1]
    if (freq > sampleRate / 2 - getBandWidth() / 2) return spectrum.Length - 1;
    // all other cases
    float fraction = freq / (float) sampleRate;
    int i = (int)Mathf.Round(timeSize * fraction);
    return i;
  }	
	
  public float getBandWidth()
  {
    return bandWidth;
  }	
	
  private float _Sin(int i)
  {
    return sinlookup[i];
  }

  private float _Cos(int i)
  {
    return coslookup[i];
  }

  private void buildTrigTables()
  {
    int N = timeSize;
    sinlookup = new float[N];
    coslookup = new float[N];
		
    for (int i = 0; i < N; i++)
    {
      sinlookup[i] = Mathf.Sin(-(float) Mathf.PI / i);
      coslookup[i] = Mathf.Cos(-(float) Mathf.PI / i);
    }
  }
}
