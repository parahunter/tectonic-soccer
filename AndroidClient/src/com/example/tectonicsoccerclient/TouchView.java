package com.example.tectonicsoccerclient;

import android.content.Context;
import android.util.DisplayMetrics;
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
		
		DisplayMetrics metrics = context.getResources().getDisplayMetrics();
		
		screenWidth = metrics.widthPixels;
		screenHeight = metrics.heightPixels;
	}
	
	@Override
	public boolean onTouchEvent(MotionEvent e) {
	    // MotionEvent reports input details from the touch screen
	    // and other input controls. In this case, you are only
	    // interested in events where the touch position changed.
		
		touches = new Touch[e.getPointerCount()];
		
		for(int i = 0 ; i < e.getPointerCount() ; i++)
		{
			float x = e.getX(i) / screenWidth;
			float y = e.getY(i) / screenHeight;
			
			//switched to match with pitch
			touches[i] = new Touch(y, x);   
		}
	    
		int action = e.getActionMasked();
		
		if(action == MotionEvent.ACTION_DOWN)
			udpSender.NewTouch(0);
		
		if(action == MotionEvent.ACTION_POINTER_DOWN)	
			udpSender.NewTouch( e.getActionIndex() );
		
		if(action == MotionEvent.ACTION_UP && e.getPointerCount() == 1)
		{
			udpSender.setTouches(null);
			return true;
		}
		
	    Log.v("touch", touches[0].x + "");
		
	    udpSender.setTouches(touches);
	    
	    return true;
	}
}
