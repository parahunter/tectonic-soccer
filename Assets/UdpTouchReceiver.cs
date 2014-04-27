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
	
	public class TouchData
	{
		public Vector2 position;
		public bool newTouch;
	}
	
	TouchData[] touches;

	public PitchTectonics pitch;
		
	public TouchData[] GetTouches()
	{
//		lock(touches)
		{
			return touches;
		}
	}
	
	void Start()
	{
		try
		{
			udpRemoteIpEndPoint = new IPEndPoint( IPAddress.Any, port );	
					
			client = new UdpClient(udpRemoteIpEndPoint);
			
			//client.Client.ExclusiveAddressUse = false;
			//client.Client.ReceiveTimeout = 1000;
		}
		catch( SocketException e)
		{
			print (e);
			
			if(client != null)
				client.Close();
		}
		
		BeginRecieve();
	}
	
	void OnDrawGizmos()
	{
		if(touches != null)
		{
			foreach(TouchData touch in GetTouches())
			{
				Vector2 pos = touch.position;
				Vector3 center = new Vector3(pos.x, 0, pos.y) * 10;
				Gizmos.DrawWireSphere(center, 1f);
			}
		}
	}
	
	bool recieving = false;
	bool reading = false;
	
	void Update()
	{
		if(touches == null)
			return;
		
		if(recieving)
			return;
			
		{
			reading = true;
			foreach(TouchData touch in touches)
			{
				if(touch.newTouch == true)
				{
					pitch.AddTectonics( touch.position );
					print (touch.position);
					
					touch.newTouch = false;	
				}
			}
			
			reading = false;
		}
	}
		
 	void BeginRecieve()
	{
	//	lock(client)
		{
			UdpState state = new UdpState();
			state.endPoint = udpRemoteIpEndPoint;
			state.client = client;
		
			client.BeginReceive(new AsyncCallback(OnReceive), state);
		}
	}
	
	const int packetLength = 9;
	void OnReceive(IAsyncResult result)
	{
		while(reading)
		{
			
		}
		
	//	lock(client)
		{
			UdpState state = (UdpState) result.AsyncState;
			UdpClient c = state.client;
			IPEndPoint e = state.endPoint;
		
			Byte[] data = c.EndReceive( result, ref e);
		
			TouchData[] newTouchData = new TouchData[ data.Length / packetLength];
				
			for(int i = 0 ; i < newTouchData.Length ; i++)
			{
				float x = 1 - EndianBitConverter.Big.ToSingle( data, i * packetLength);
				float y = EndianBitConverter.Big.ToSingle( data, i * packetLength + 4);
				
				TouchData touchData = new TouchData();
				touchData.position = new Vector2(x, y);
				touchData.newTouch = data[i * packetLength + 8] == 1 ? true : false;
				
				if(touchData.newTouch)
				{ 
				//	pitch.AddTectonics( touchData.position );
					print ("boom");
				}
				
				newTouchData[i] = touchData;
			}
			
			recieving = true;
//			lock(touches)
			{
				touches = newTouchData;
			}
									
			BeginRecieve();	
			
			recieving = false;
		}			
	}
	
	void OnDestroy()
	{
		if(client != null)
		{	
			lock(client)
			{
				client.Close();
			}
		}	
	}
}
