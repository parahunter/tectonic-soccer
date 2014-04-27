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
	
	private int milisecondsPerPackage = 1000/60;
	
	private boolean[] newTouches = new boolean[4];
	
	public synchronized void NewTouch(int index)
	{
		this.newTouches[index] = true;
	}
	
	public synchronized void setTouches(Touch[] touches)
	{
		this.touches = touches;
	}
	
	public UdpSender ( InetAddress address, int port) 
	{
		serverPort = port;
		serverAddress = address;
		
		for(int i = 0 ; i < newTouches.length ; i++)
			newTouches[i] = false;
	}
	
	public void run() 
	{
		DatagramSocket socket;
		try {
			socket = new DatagramSocket();
		
			while(runThread == true) 
			{
			
			//Log.v("progress", "start of thread");
			try 
			{
				// Create UDP socket
	
				//OutputStream stream = new OutputStream();
				ByteArrayOutputStream stream = new ByteArrayOutputStream(0);
				DataOutputStream dataStream = new DataOutputStream(stream);
				
					if(touches != null)
					{
						stream.reset();
						
						for(int i = 0 ; i < touches.length ; i++)
						{
							dataStream.writeFloat(touches[i].x );
							dataStream.writeFloat(touches[i].y );
							
							if(newTouches[i])
							{
								dataStream.writeByte( 1);
								newTouches[i] = false;
							}
							else
								dataStream.writeByte(0);
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
					else
					{
						byte[] bytes = new byte[1];
					
						// Create the UDP packet with destination
						DatagramPacket packet = new DatagramPacket(bytes,bytes.length, serverAddress,serverPort);
					
						//Log.v("packet message", "trying to send packet");
						// Send of the packet
						socket.send(packet);
					}
						
					//suspend the thread for x miliseconds
					Thread.sleep(milisecondsPerPackage);
				}
			catch (Exception e) 
			{
				
				e.printStackTrace();
			}
		
		}
		} catch (SocketException e1) {
			// TODO Auto-generated catch block
			e1.printStackTrace();
		}
	}
}
