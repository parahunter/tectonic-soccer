package com.example.tectonicsoccerclient;

import android.content.Context;
import android.util.Log;
import android.view.*;

public class TouchView extends View
{
	Touch[] touches;
	
	float screenWidth;
	float screenHeight;
	UdpSender udpSender;
	
	public TouchView(Context context, UdpSender udpSender) 
	{
		super(context);
		this.udpSender = udpSender;
		
		screenWidth = (float) context.getResources().getConfiguration().screenWidthDp;
		screenHeight = (float) context.getResources().getConfiguration().screenHeightDp;
	}
	
	@Override
	public boolean onTouchEvent(MotionEvent e) {
	    // MotionEvent reports input details from the touch screen
	    // and other input controls. In this case, you are only
	    // interested in events where the touch position changed.
		
		touches = new Touch[e.getPointerCount()];
		
		for(int i = 0 ; i < e.getPointerCount() ; i++)
		{
			float x = e.getX(i) / screenHeight;
			float y = e.getY(i) / screenWidth;
			
			touches[i] = new Touch(x,y);   
		}
	    
	    Log.v("touch", touches[0].x + "");
		
	    udpSender.setTouches(touches);
	    
	    return true;
	}
}
