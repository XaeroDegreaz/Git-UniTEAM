using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using iGUI;

//Always add iGUI prefix to your custom controls
public class iGUIMyTextfield: iGUICustomFormElement {
	//Matches the event name with 'on' prefix and first letter capital
	public iGUIAction[] onValueChange=new iGUIAction[0];
		
	//Mathces the event name with 'Callback' prefix
	public iGUIEventCallback valueChangeCallback = null;
	
	public string value="";
	
	public GUIStyle style;
	
	string newValue;
	
	//You have to declare events here
	public override List<string> getEventNames (){
		base.getEventNames();
		eventNames.Add("ValueChange");
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
		//This method searches for a style with first parameter as name.
		//If can't found, searches for the second parameter 
		//If still not found returns an empty GUIStyle
		style = getCopyStyle("myTextfield", "textField");		
	}
	
	//This method is called after the initialization of element. Mainly is used for initialization of action lists.
	protected override void afterInit (){
		initActions(onValueChange);
	}
	
	
	protected override void customDraw (){
		//This line is needed only for textfields and windows
		iGUIRoot.useLayout=true;
		newValue = GUI.TextField(fieldRect, value, style);
		
		if(newValue!=value){
			value=newValue;
			triggerEvent("ValueChange");
		}
	}
}
