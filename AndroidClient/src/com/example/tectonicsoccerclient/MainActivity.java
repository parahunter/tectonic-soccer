package com.example.tectonicsoccerclient;

import java.io.IOException;
import java.net.InetAddress;

import android.R;
import android.support.v4.app.Fragment;
import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.os.Build;

public class MainActivity extends Activity
{
	TouchView touchView;
	
	UdpSender udpSender;
	
	String hostname = "192.168.0.30";
	int udpServerPort = 40005;
	
    @Override
    protected void onCreate(Bundle savedInstanceState) 
    {
    	try {
			StartNetworkThread();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    	
    	touchView = new TouchView(this, udpSender);
    	
        super.onCreate(savedInstanceState);
        setContentView(touchView);        
    }
    
    private void StartNetworkThread() throws IOException
    {	
    	InetAddress serverAddress = InetAddress.getByName(hostname);
	    
        udpSender = new UdpSender(serverAddress, udpServerPort);
    	Thread udpSendThread = new Thread(udpSender);
    	
        Log.v("thread", "starting thread");
		
		udpSendThread.start();
		Log.v("thread", "thread start finished");	
    }
}

