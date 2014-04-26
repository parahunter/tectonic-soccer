using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Net.Sockets;
using System.Net;
using System;


public class UdpTouchReceiver : MonoBehaviour 
{
	public class UdpState
	{
		public IPEndPoint endPoint;
		public UdpClient client;
	}

	public int port = 40000;
	UdpClient client;	

	private IPEndPoint udpRemoteIpEndPoint;
	
	public Vector2[] touches;
	
	void Start()
	{
		udpRemoteIpEndPoint = new IPEndPoint( IPAddress.Any, port );	
		
		client = new UdpClient(udpRemoteIpEndPoint);

		BeginRecieve();
	}
	
	void OnDrawGizmos()
	{
		if(touches != null)
		{
			foreach(Vector2 touch in touches)
			{
				Vector3 center = new Vector3(touch.x, 0, touch.y) * 10;
				Gizmos.DrawWireSphere(center, 1f);
			}
		}
	}
	
 	void BeginRecieve()
	{
		UdpState state = new UdpState();
		state.endPoint = udpRemoteIpEndPoint;
		state.client = client;
		
		client.BeginReceive(new AsyncCallback(OnReceive), state);
	}
	
	void OnReceive(IAsyncResult result)
	{
		UdpState state = (UdpState) result.AsyncState;
		UdpClient c = state.client;
		IPEndPoint e = state.endPoint;
		
		Byte[] data = c.EndReceive( result, ref e);
				
		touches = new Vector2[ data.Length / ( 4 * 2)];
		
		for(int i = 0 ; i < touches.Length ; i++)
		{
			float x = EndianBitConverter.Big.ToSingle( data, (i*2) * 4);
			float y = EndianBitConverter.Big.ToSingle( data, (i*2) * 4 + 4);
			
			Vector2 touch = new Vector2(x, y);
			
			touches[i] = touch;
		}
				
		BeginRecieve();				
	}
}
