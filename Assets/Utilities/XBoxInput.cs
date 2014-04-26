using UnityEngine;
using System.Collections;

class XBoxInput : MonoBehaviour
{

	static public Vector2 GetLeftAnalogInput(int player)
	{
		return new Vector2( Input.GetAxis("joystick " + player + " left analog x"),
			                Input.GetAxis("joystick " + player + " left analog y"));
	}
	
	static public Vector2 GetRightAnalogInput(int player)
	{
		return new Vector2( Input.GetAxis("joystick " + player + " right analog x"),
			                Input.GetAxis("joystick " + player + " right analog y"));
	}
	
	static public Vector2 GetDPadInput(int player)
	{
		return new Vector2( Input.GetAxis("joystick " + player + " dpad x"),
			                Input.GetAxis("joystick " + player + " dpad y"));
	}
	
	static public float GetTriggers(int player)
	{
		return Input.GetAxis("joystick " + player + " triggers");
	}
	
	static public bool GetButtonA(int player)
	{
		
		return Input.GetButton("joystick " + player + " button A");	
	}
	
	static public bool GetButtonADown(int player)
	{
		return Input.GetButtonDown("joystick " + player + " button A");	
	}
	
	static public bool GetButtonAUp(int player)
	{
		return Input.GetButtonUp ("joystick " + player + " button A");	
	}
	
	
	static public bool GetButtonBDown(int player)
	{
		return Input.GetButtonDown("joystick " + player + " button B");	
	}
	
	static public bool GetButtonXDown(int player)
	{
		return Input.GetButtonDown("joystick " + player + " button X");	
	}
	
	static public bool GetButtonYDown(int player)
	{
		return Input.GetButtonDown("joystick " + player + " button Y");	
	}
	
	static public bool GetButtonLDown(int player)
	{
		return Input.GetButtonDown("joystick " + player + " button L");	
	}
	
	static public bool GetButtonRDown(int player)
	{
		return Input.GetButtonDown("joystick " + player + " button R");	
	}
	
	static public bool GetButtonBackDown(int player)
	{
		return Input.GetButtonDown("joystick " + player + " button back");	
	}
	
	static public bool GetButtonHomeDown(int player)
	{
		return Input.GetButtonDown("joystick " + player + " button home");	
	}
	
	
	static public bool GetButton(int player)
	{
		return Input.GetButton("joystick " + player + " button A");	
	}
	
	static public bool GetButtonB(int player)
	{
		return Input.GetButton("joystick " + player + " button B");	
	}
	
	static public bool GetButtonX(int player)
	{
		return Input.GetButton("joystick " + player + " button X");	
	}
	
	static public bool GetButtonY(int player)
	{
		return Input.GetButton("joystick " + player + " button Y");	
	}
	
	static public bool GetButtonL(int player)
	{
		return Input.GetButton("joystick " + player + " button L");	
	}
	
	static public bool GetButtonR(int player)
	{
		return Input.GetButton("joystick " + player + " button R");	
	}
	
	static public bool GetButtonBack(int player)
	{
		return Input.GetButton("joystick " + player + " button back");	
	}
	
	static public bool GetButtonHome(int player)
	{
		return Input.GetButton("joystick " + player + " button home");	
	}
}