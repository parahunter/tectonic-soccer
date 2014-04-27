using UnityEngine;
using System.Collections;

public class ScoreCounter : MonoBehaviour {

	public int team = 0;
	private int ScoreCounter;

	void onGoal()
	{
		if(GameController.instance.lastScorer == team)
		{
			renderer.material.mainTexture = transform;
		}
	}

	// Use this for initialization
	void Start () {
		GameController.instance.onGoal += onGoal;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
