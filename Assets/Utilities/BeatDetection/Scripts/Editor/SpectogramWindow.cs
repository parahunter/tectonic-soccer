using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class SpectogramWindow :  EditorWindow
{
	AudioClip currentClip;
	Texture2D spectogramTexture;
	int currentXCoord = 0;
	int fftSampleRate = 256;
	FFT fft;
	int textureWidth = 1024;
	int textureHeight = 128;
	int textureUpdateThreshold = 50;
	int textureUpdateCount = 0;
	BeatSettings settings;
	const int sidebarWidth = 165;

	//Rect leftRect;
	Rect rightRect;
	
	bool visualiseBands = true;
	
	BeatDetector detector;
	static SpectogramWindow window;
	public static void OpenWindow(BeatDetector detector)
	{
		window = GetWindow<SpectogramWindow>();
		window.detector = detector;
	}

	void OnEnable()
	{
		settings = Resources.LoadAssetAtPath<BeatSettings>("Assets/Utilities/BeatDetection/BeatSettings.asset");
	
		spectogramTexture = EditorGUIUtility.whiteTexture;
	}

	void Update()
	{
		DoAnalysis();
	}

	void OnGUI()
	{
		//Rect windowRect = 
		
		EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
			EditorGUILayout.BeginHorizontal(GUILayout.Width(sidebarWidth));
				DrawSidebar();
			EditorGUILayout.EndHorizontal();	
			
			EditorGUILayout.BeginVertical();
			
				EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true));
					AudioClip clip = currentClip;
					EditorGUILayout.PrefixLabel("Active clip");
					clip = (AudioClip) EditorGUILayout.ObjectField(clip, typeof(AudioClip), false);
					if(clip != currentClip)
					{
						currentClip = clip;
						
						if(currentClip != null)
							StartAnalysis();
					}
	
				EditorGUILayout.EndHorizontal();
	
				Rect lowerRect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
					
					//leftRect = new Rect(lowerRect.x, lowerRect.y, sidebarWidth, lowerRect.height);
					rightRect = new Rect(lowerRect.x, lowerRect.y, lowerRect.width, lowerRect.height);
					
					if(currentClip == null)
					{
						
					}
					else
					{
						GUI.DrawTexture(rightRect, spectogramTexture);
						
						if(visualiseBands)			
							DrawFrequencyBands();
					}
					
				EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();	
	}

	void DrawSidebar()
	{
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel("Visualise Bands");
			visualiseBands = EditorGUILayout.Toggle(visualiseBands);
		GUILayout.EndHorizontal();
		
		GUILayout.Label("Beat Bands");

		int indexToDelete = -1;
		Color standardColor = GUI.color;

		for(int i = 0 ; i < detector.subBands.Count ; i++)
		{
			FrequencyBand band = detector.subBands[i];
			int colorIndex = i % settings.bandColors.Length;
			GUI.color = settings.bandColors[colorIndex];

			GUILayout.BeginHorizontal();
				GUILayout.Label(i.ToString());
				float frequency;

				EditorGUI.BeginChangeCheck();
				frequency = EditorGUILayout.FloatField(band.lowerFrequency);
				EndChangeCheck( () => 
				               {
					band.lowerFrequency = frequency;
				});
				
				EditorGUI.BeginChangeCheck();
				frequency = EditorGUILayout.FloatField(band.higherFrequency);
				EndChangeCheck( () =>
				               {
					band.higherFrequency = frequency;
				});
				
				if(GUILayout.Button("X", EditorStyles.miniButton))
					indexToDelete = i;

			GUILayout.EndHorizontal();
		}

		if(indexToDelete >= 0)
		{	
			Undo.RecordObject(detector, "Deleted a band in the beat dector");
			detector.subBands.RemoveAt(indexToDelete);
		}

		GUI.color = standardColor;
		
		GUILayout.EndVertical();
	}

	void DrawFrequencyBands()
	{
		Color standardColor = GUI.color;
		for(int i = 0 ; i < detector.subBands.Count ; i++)
		{
			FrequencyBand band = detector.subBands[i];
			int colorIndex = i % settings.bandColors.Length;
			GUI.color = settings.bandColors[colorIndex];

			GUI.color = Color.Lerp(GUI.color, Color.clear, 0.5f);
			Rect spectrumRect = rightRect;
			float lower = FrequencyToGuiCoord(band.lowerFrequency);
			float higher = FrequencyToGuiCoord(band.higherFrequency);
			
			spectrumRect.y = lower;
			spectrumRect.height = higher - lower;
			
			GUI.DrawTexture(spectrumRect, EditorGUIUtility.whiteTexture);

		}

		GUI.color = standardColor;
	}

	float FrequencyToGuiCoord(float frequency)
	{
		float interpolation = Mathf.Clamp01( frequency / (currentClip.frequency * 0.5f) );

		return rightRect.y + rightRect.height - interpolation  * rightRect.height;
	}

	void StartAnalysis()
	{	
		spectogramTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
		currentXCoord = 0;
		textureUpdateCount = 0;
		fft = new FFT(fftSampleRate, currentClip.frequency);
	}

	void DoAnalysis()
	{
		if(currentClip == null)
			return;
		if(currentXCoord < textureWidth)
		{
			int sample = (int)((float) currentXCoord / textureWidth * currentClip.samples);
			float[] samples = StereoToMonoData( sample, fftSampleRate);
			float[] spectrum = fft.GetSpectrum(samples);

			Color[] pixels = new Color[textureHeight];

			for(int i = 0 ; i < pixels.Length ; i++)
			{
				int spectrumBucket = (int) ((float)i / pixels.Length * spectrum.Length);
				pixels[i] = GetSpectrumColor(spectrum[spectrumBucket]);
			}

			spectogramTexture.SetPixels(currentXCoord, 0, 1, textureHeight, pixels);

			textureUpdateCount++;
			currentXCoord++;

			if(textureUpdateCount >= textureUpdateThreshold)
			{
				spectogramTexture.Apply();
				textureUpdateCount = 0;
				this.Repaint();
			}

		}
		else
		{
			spectogramTexture.Apply();
		}
	}

	Color GetSpectrumColor(float value)
	{
		if(settings != null)
			return settings.spectogramGradient.Evaluate(value);
		else
			return Color.Lerp(Color.blue, Color.red, value);
	}

	float[] StereoToMonoData(int sampleIndex, int amount)
	{
		float[] data = new float[amount * currentClip.channels];
		currentClip.GetData(data, sampleIndex);

		if(currentClip.channels == 2)
		{
			float[] data2 = new float[amount];
			for(int i = 0, k = 0 ; k < amount * currentClip.channels ; i++, k += currentClip.channels)
			{
				data2[i] = data[k] + data[k + 1];
			}
			
			return data2;
		}
		else
			return data;
	}	

	void EndChangeCheck(System.Action callback)
	{
		if(EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(detector, "Change in Beat Detector");
			callback();
			EditorUtility.SetDirty(detector);
		}
	}

}
