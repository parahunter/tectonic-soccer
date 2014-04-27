using UnityEngine;
using System.Collections;

public class ScoreCounter : MonoBehaviour {

	public int team = 0;

	void onGoal()
	{
		if(GameController.instance.lastScorer == team)
		{
			renderer.material.mainTexture = ((ScoreBoardScript)(transform.parent.gameObject.GetComponent ("ScoreBoardScript"))).goalTextures[GameController.instance.playerGoals[team]];
		}
	}

	void onFinish()
	{
		renderer.material.mainTexture = ((ScoreBoardScript)(transform.parent.gameObject.GetComponent ("ScoreBoardScript"))).goalTextures[0];
	}
	
	// Use this for initialization
	void Start () 
	{
		GameController.instance.onScored += onGoal;
		GameController.instance.onFinished += onFinish;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
