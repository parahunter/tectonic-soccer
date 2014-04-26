using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheeseMessageTest : MonoBehaviour 
{
	void Awake()
	{
		if( CheeseMessage.GetMessage() != null)
		{
			CheeseMessage message = CheeseMessage.GetMessage();
			
			print ("The cheese on today's menu is " + message.content.cheeseType.ToString() );
		}
		else 
		{
			print ( "Sorry there's no cheese in this scene. Press space to try another" );	
		}
	}
	
	void Update()
	{
		if( Input.GetKeyDown( KeyCode.Space) )
		{
			LovelyCheese lovelyCheese = new LovelyCheese();
		
			CheeseMessage.CreateMessage( lovelyCheese );
			
			Application.LoadLevel( Application.loadedLevel );
		}
	}
	

}
