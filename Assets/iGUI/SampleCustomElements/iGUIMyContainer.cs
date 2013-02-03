using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using iGUI;

//Always add iGUI prefix to your custom elements
public class iGUIMyContainer : iGUICustomContainer {	
	public GUIStyle style;
	
	//This method is called only once  at editor mode when the element is created.
	public override void onCreate (){
		base.onCreate();
		style = getCopyStyle("panel", "window");		
	}	
	
	protected override void beforeInitItems (){
		//Can be used to do something before initialization of items
	}
	
	protected override void afterInitItems (){
		//Can be used to do something after initialization of items		
	}
	
	protected override void beforeDrawItems (){
		GUI.Box(rect, label, style);
	}
	
	protected override void afterDrawItems (){
		//Can be used to do something after items drawn
	}
}
