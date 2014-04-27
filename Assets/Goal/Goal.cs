using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Goal : MonoBehaviour 
{
	public int playerId = 0;
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag( "Ball" ) )
		{
			GameController.instance.OnGoal(playerId);
		}
	}
}
