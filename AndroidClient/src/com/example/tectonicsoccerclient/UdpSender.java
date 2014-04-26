package com.example.tectonicsoccerclient;

import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.io.*;

//import virtua.pad.AndroidVirtuaPadMain.clientState;

import android.util.Log;

public class UdpSender implements Runnable {
	
	private InetAddress serverAddress;
	
	private int serverPort = 40000;
	
	public boolean runThread = true;
	
	private Touch[] touches;
	
	private int milisecondsPerPackage = 1000/25;
	
	public synchronized void setTouches(Touch[] touches)
	{
		this.touches = touches;
	}
	
	public UdpSender ( InetAddress address, int port) 
	{
		serverPort = port;
		serverAddress = address;
	}
	
	public void run() 
	{
		//Log.v("progress", "start of thread");
		try {
			
			// Create UDP socket
			DatagramSocket socket = new DatagramSocket();
			//OutputStream stream = new OutputStream();
			ByteArrayOutputStream stream = new ByteArrayOutputStream(0);
			DataOutputStream dataStream = new DataOutputStream(stream);
			
			while(runThread == true) 
			{
				if(touches != null)
				{
					stream.reset();
					
					for(int i = 0 ; i < touches.length ; i++)
					{
						dataStream.writeFloat(touches[i].x );
						dataStream.writeFloat(touches[i].y );
											
					}
					
					dataStream.flush();
					
					byte[] bytes = stream.toByteArray();
					
					//Log.v("UDP", "x " + tempAccData[0] + " y " + tempAccData[1] + " z " + tempAccData[2]);
					
					// Create the UDP packet with destination
					DatagramPacket packet = new DatagramPacket(bytes,bytes.length, serverAddress,serverPort);
				
					//Log.v("packet message", "trying to send packet");
					// Send of the packet
					socket.send(packet);
				}
				
				//suspend the thread for x miliseconds
				Thread.sleep(milisecondsPerPackage);
			}
			
		}
		catch(SocketException e)
		{
			Log.e("udp error", e.toString());
		}
		catch (Exception e) 
		{
			Log.e("udp error", e.getMessage());
			StackTraceElement[] stackTrace= e.getStackTrace();
			for(StackTraceElement element : stackTrace )
				Log.e("udp error trace", element.toString());
			
			e.printStackTrace();
		}
	}
}
