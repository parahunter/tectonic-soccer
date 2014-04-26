using UnityEngine;
using UnityEditor;
using System.Collections;

public static class AlignTransformation
{
	[MenuItem("Utilities/Align/Rotation &r")]
	public static void AllignRotation () 
	{
		foreach(Transform transform in Selection.transforms)
		{	
			transform.rotation = Selection.activeTransform.rotation;
		}	
	}
	
	[MenuItem("Utilities/Align/X Rotation &x")]
	public static void AllignRotationX () 
	{
		foreach(Transform transform in Selection.transforms)
		{
			Vector3 rot = transform.rotation.eulerAngles;		
			rot.x = Selection.activeTransform.eulerAngles.x;
			
			transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
		}
	}
	
	[MenuItem("Utilities/Align/Y Rotation &y")]
	public static void AllignRotationY () 
	{
		foreach(Transform transform in Selection.transforms)
		{
			Vector3 rot = transform.rotation.eulerAngles;		
			rot.y = Selection.activeTransform.eulerAngles.y;

			transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
		}
	}
	
	[MenuItem("Utilities/Align/Z Rotation &z")]
	public static void AllignRotationZ () 
	{
		foreach(Transform transform in Selection.transforms)
		{
			Vector3 rot = transform.rotation.eulerAngles;		
			rot.z = Selection.activeTransform.eulerAngles.z;

			transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
		}
	}
	
	[MenuItem("Utilities/Align/Position &p")]
	public static void AllignPositionX () 
	{
		foreach(Transform transform in Selection.transforms)
		{
			transform.position = Selection.activeTransform.position;
		}
	}
	
	[MenuItem("Utilities/Align/X Position #&x")]
	public static void AllignPosition () 
	{
		foreach(Transform transform in Selection.transforms)
		{
			Vector3 pos = transform.position;		
			pos.x = Selection.activeTransform.position.x;

			transform.position = pos;
		}
	}
	
	[MenuItem("Utilities/Align/Y Position #&y")]
	public static void AllignPositionY () 
	{
		foreach(Transform transform in Selection.transforms)
		{
			Vector3 pos = transform.position;		
			pos.y = Selection.activeTransform.position.y;

			transform.position = pos;
		}
	}
	
	[MenuItem("Utilities/Align/Z Position #&z")]
	public static void AllignPositionZ () 
	{
		foreach(Transform transform in Selection.transforms)
		{
			Vector3 pos = transform.position;		
			pos.z = Selection.activeTransform.position.z;

			transform.position = pos;
		}
	}
}
