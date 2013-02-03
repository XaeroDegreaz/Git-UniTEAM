using UnityEngine;
using UnityEditor;
using GitSharp;
using System.Collections;

namespace UniTEAM {
	public class Console : EditorWindow {
	
		[MenuItem("Team/Git UniTEAM")]
		static void init() {
			EditorWindow.GetWindow(typeof(Console), false, "UniTEAM");
		}
		
		void OnEnable() {
			Debug.LogWarning("Git UniTEAM loaded: "+System.DateTime.Now+" -> Git: "+Git.Version);
			
			Repository repo = new Repository("/");
			Debug.Log(repo.Directory);
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
