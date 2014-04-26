using UnityEngine;
using System.Collections;

public class Message : MonoBehaviour {
	
	public Object[] message;
	
	public Object[] GetMessage()
	{
		Destroy(gameObject);
		
		return message;
	}
	
	public static void CreateMessage(Object[] message, string name)
	{
		GameObject obj = (GameObject)Instantiate(new GameObject(name));
		
		obj.AddComponent<Message>();
		obj.GetComponent<Message>().message = message;
		
		DontDestroyOnLoad(obj);
	}
	
	public static void CreateMessage(Object message, string name)
	{
		GameObject obj = (GameObject)Instantiate(new GameObject(name));
		
		obj.AddComponent<Message>();
		obj.GetComponent<Message>().message = new Object[1];
		obj.GetComponent<Message>().message[0] = message;
		
		DontDestroyOnLoad(obj);
	}
}
