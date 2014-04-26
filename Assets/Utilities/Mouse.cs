using UnityEngine;
using System.Collections;

public class Mouse{

	public static bool MouseMoved()
	{
		return (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0);
	}
}
