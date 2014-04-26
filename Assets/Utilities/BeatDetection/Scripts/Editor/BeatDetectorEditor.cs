using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor( typeof(BeatDetector) )]
public class BeatDetectorEditor : Editor
{
	BeatDetector detector;
	void OnEnable()
	{
		detector = (BeatDetector)target;
	}

	public override void OnInspectorGUI()
	{
		DrawBasicSettings();
		DrawFrequencyBands();
	}

	void DrawFrequencyBands()
	{
		DrawHeader("Beat Frequency Bands");
		
		int indexToDelete = -1;
		
		for(int i = 0 ; i < detector.subBands.Count ; i++)
		{
			FrequencyBand band = detector.subBands[i];
			EditorGUILayout.BeginHorizontal();
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
			
			EditorGUILayout.EndHorizontal();
		}
		
		if(indexToDelete >= 0)
		{	
			Undo.RecordObject(detector, "Deleted a band in the beat dector");
			detector.subBands.RemoveAt(indexToDelete);
		}
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Add BeatBand"))
			detector.subBands.Add( new FrequencyBand() );
		if(GUILayout.Button("Beat Band Editor"))
			SpectogramWindow.OpenWindow(detector);

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
	}

	void DrawBasicSettings()
	{
		DrawHeader("Basic Settings");
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Sample rate");
		EditorGUI.BeginChangeCheck();
		BeatDetector.SampleRate sampleRate = (BeatDetector.SampleRate) EditorGUILayout.EnumPopup(detector.sampleRate);
		EndChangeCheck( () =>
	    {
			detector.sampleRate = sampleRate;
		});
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Beat Threshold");
		EditorGUI.BeginChangeCheck();
		float beatThreshold = EditorGUILayout.FloatField(detector.beatThreshold);
		EndChangeCheck( () =>
		               {
			detector.beatThreshold = beatThreshold;
		});
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Beat Cutoff Threshold");
		EditorGUI.BeginChangeCheck();
		float beatCutoffThreshold = EditorGUILayout.FloatField(detector.beatCutoffThreshold);
		EndChangeCheck( () =>
		               {
			detector.beatCutoffThreshold = beatCutoffThreshold;
		});
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Threshold Window");
		EditorGUI.BeginChangeCheck();
		int thresholdRadius = EditorGUILayout.IntField(detector.thresholdRadius);
		EndChangeCheck( () =>
		               {
			detector.thresholdRadius = thresholdRadius;
		});
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("BPM window size");
		EditorGUI.BeginChangeCheck();
		float bpmWindowSize = EditorGUILayout.FloatField(detector.bpmWindowSize);
		EndChangeCheck( () =>
		               {
			detector.bpmWindowSize = bpmWindowSize;
		});
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Cycles per frame");
		EditorGUI.BeginChangeCheck();
		int analyticsCyclesPerFrame = EditorGUILayout.IntField(detector.analyticsCyclesPerFrame);
		EndChangeCheck( () =>
		               {
			detector.analyticsCyclesPerFrame = analyticsCyclesPerFrame;
		});
		EditorGUILayout.EndHorizontal();
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

	void DrawHeader(string text)
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(text, EditorStyles.boldLabel);
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
	}
	
}
