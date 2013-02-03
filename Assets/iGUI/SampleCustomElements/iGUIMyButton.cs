using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using iGUI;

//Always add iGUI prefix to your custom elements
public class iGUIMyButton : iGUICustomElement {
	//Matches the event name with 'on' prefix and first letter capital
	public iGUIAction[] onClick=new iGUIAction[0];
	
	//Mathces the event name with 'Callback' prefix
	public iGUIEventCallback clickCallback = null;
	
	public GUIStyle style;
	
	//You have to declare events here
	public override List<string> getEventNames (){
		base.getEventNames();
		eventNames.Add("Click");
		return eventNames;
	}
	
	//This method is called repeatedly in editor mode but once at start in runtime.
	//For example this method fixes the height of textfield if the corresponding style has a fixedHeight value
	public override void refreshStyle(){
		if(style!=null && style.fixedHeight>0)
			positionAndSize.height=style.fixedHeight;
	}
	
	//This method is called only once  at editor mode when the element is created.
	public override void onCreate (){
		base.onCreate();
		style = getCopyStyle("button");		
	}
	
	//This method is called after the initialization of element. Mainly is used for initialization of action lists.
	protected override void afterInit (){
		initActions(onClick);
	}
	
	
	protected override void customDraw (){
		if(GUI.Button(rect, label)){
			triggerEvent("Click");
		}
	}
}
