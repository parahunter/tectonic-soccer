using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SnapScript))]
public class EditorLevelSnap : Editor 
{
	public void OnEnable()
	{
		Transform currentTrans = ((SnapScript)target).transform;
		SnapScript snapScript = (SnapScript)target;
		
		//get position and get the integer value of x coordinate
		
		if(snapScript.SnapPosition)
		{
			Vector3 position = currentTrans.position;
		
			int coordinate = (int)position.x;
			
			//snap the coordinate
			position.x = SnapCoordinate(coordinate);
			
			//do the same for the y-coordinate
			coordinate = (int)position.y;
			position.y = SnapCoordinate(coordinate);
			
			position.z = 0; //make sure the object lies in the 0 z-plane
			
			//update position with snapped position
			currentTrans.position = position;
		}
		
		if(snapScript.SnapRotation)
		{
			//next make sure the rotation is snapped and reset for the x and y axis
			Vector3 rotation = currentTrans.rotation.eulerAngles;
			rotation.x = 0;
			rotation.y = 0;
			rotation.z = SnapRotation(rotation.z);
			
			//and save the new rotation
			currentTrans.rotation = Quaternion.Euler(rotation);
		}
	}
	
	public void OnSceneGUI() 
	{
		//if the mouse is released on a drag on the object gizmo
		if(Event.current.type == EventType.MouseUp)
		{
			
			
			//find the transform
			Transform currentTrans = ((SnapScript)target).transform;
			SnapScript snapScript = (SnapScript)target;
			
			if(snapScript.SnapPosition == true)
			{
				//get position and get the integer value of x coordinate
				Vector3 position = currentTrans.position;
				
				int coordinate = (int)position.x;
				
				//snap the coordinate
				position.x = SnapCoordinate(coordinate);
				
				//do the same for the y-coordinate
				coordinate = (int)position.y;
				position.y = SnapCoordinate(coordinate);
				
				position.z = 0; //make sure the object lies in the 0 z-plane
				
				//update position with snapped position
				currentTrans.position = position;
			}
			
			if(snapScript.SnapRotation == true)
			{
				//next make sure the rotation is snapped and reset for the x and y axis
				Vector3 rotation = currentTrans.rotation.eulerAngles;
				rotation.x = 0;
				rotation.y = 0;
				rotation.z = SnapRotation(rotation.z);
				
				//and save the new rotation
				currentTrans.rotation = Quaternion.Euler(rotation);
			}
		}
		else //draw the position and rotation the object will snap to when mouse is released
		{
			//find the transform
			Transform currentTrans = ((SnapScript)target).transform;
			
			//get position and get the integer value of x coordinate
			Vector3 position = currentTrans.position;
			
			int coordinate = (int)position.x;
			position.x = SnapCoordinate(coordinate);
			//snap the coordinate
			
			//do the same for the y-coordinate
			coordinate = (int)position.y;
			position.y = SnapCoordinate(coordinate);
			
			position.z = 0; //make sure the object lies in the 0 z-plane

			//draw crosshair at the position the object will snap to
			Handles.color = Color.magenta;
			Handles.DrawLine(ViewportVector(position, 50, Vector2.one),ViewportVector(position, 50, -Vector2.one));
			Handles.DrawLine(ViewportVector(position, 50, new Vector2(1,-1)),ViewportVector(position, 50, new Vector2(-1,1)));
			
			//next draw line that tells which direction it will snap to
			
			// get the snapped value of the current rotatation along z axis
			float zRotation = currentTrans.rotation.eulerAngles.z;
			zRotation = SnapRotation(zRotation);
			
			//get the direction that corresponds to this rotation
			Vector2 rotationDirection;
			switch((int)zRotation)
			{
			case 0:
				rotationDirection = new Vector2(1,0);
				break;
			case 90:
				rotationDirection = new Vector2(0,-1);
				break;
			case 180:
				rotationDirection = new Vector2(-1,0);
				break;
			case 270:
				rotationDirection = new Vector2(0,1);
				break;
			default:
				rotationDirection = new Vector2(1,1);
				Debug.LogError("Something has happened to the rotation snap script");
				break;
			}
			
			//draw the line
			Handles.DrawLine(position,ViewportVector(position, 200, rotationDirection));
		}
	}
	
	//returns a point in worldspace that is the result of taking point pos and adding
	//a vector that is pixels pixels long in the viewport and has the direction direction;
	private Vector3 ViewportVector(Vector3 pos, float pixels, Vector2 direction)
	{
		direction.Normalize();
		
		Vector2 pixelCords = HandleUtility.WorldToGUIPoint(pos);
		pixelCords += (direction*pixels);
		
		Ray ray = HandleUtility.GUIPointToWorldRay(pixelCords);
		
		float distanceToZPlane;
			
		new Plane(-Vector3.forward, Vector3.zero).Raycast(ray, out distanceToZPlane);
		
		
		
		//Debug.Log(pixelCords);
		
		return ray.GetPoint(distanceToZPlane);
	}
		
	//snaps a coordinate to the nearest that goes up in 5
	private int SnapCoordinate(int coordinate)
	{
		if(coordinate > 0)
	    {
			if(coordinate % 10 > 5)
			{
				
				if(coordinate % 5 > 2)
					coordinate += ( 5 -  coordinate % 5);
				else
					coordinate -= coordinate % 5;
			}
			else
			{
				if(coordinate % 5 <= 3)
					coordinate -= coordinate % 5;
				else
					coordinate += (5 - coordinate % 5);
			}
		}
		else
		{
			if(coordinate % 10 > -5)
			{
				if(coordinate % 5 <= -3)
					coordinate -= 5 + coordinate % 5;
				else
					coordinate -= coordinate % 5;
			}
			else
			{
				if(coordinate % 5 <= -3)
					coordinate -= 5 + coordinate % 5;
				else
					coordinate -= coordinate % 5;
			}
		}	
		
		return coordinate;
	}
	//snap a rotation to the nearest right angle
	private int SnapRotation(float rotation)
	{

		if(rotation < 45 || rotation > 315)
			return 0;
		else if(rotation > 45 && rotation < 135)
			return 90;
		else if(rotation > 135 && rotation < 225)
			return 180;
		else
			return 270;
		
		
	//	return rotation;
	}
}
