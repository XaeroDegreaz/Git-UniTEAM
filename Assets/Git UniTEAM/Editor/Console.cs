using UnityEngine;
using UnityEditor;
using GitSharp;
using System.Collections;
using System.IO;

namespace UniTEAM {
	public class Console : EditorWindow {
		
		private string lastCommitMessage;
		private Repository repo;
		
		public float windowPadding = 5;
		public Rect overviewRect;
		public Rect updatesRect;
		public Rect changesRect;
		
		public GitSharp.Tree currentTree;
		
		[MenuItem("Team/Git UniTEAM")]
		static void init() {
			EditorWindow.GetWindow(typeof(Console), false, "UniTEAM");
		}
		
		void OnEnable() {
			//Debug.LogWarning(" Git UniTEAM loaded: "+System.DateTime.Now+" -> Git: "+Git.Version);
			
			repo = new Repository(Directory.GetCurrentDirectory());
			currentTree = repo.Head.CurrentCommit.Tree;
			
			lastCommitMessage = new Commit(repo, "HEAD^").Message;
			
			Repaint();			
		}
		
		// Update is called once per frame
		void Update () {
			float windowWidth = (position.width / 2) - windowPadding;
			overviewRect = new Rect(windowPadding, 30, windowWidth, 500);
			updatesRect = new Rect(overviewRect.x + overviewRect.width + windowPadding, overviewRect.y, windowWidth - windowPadding, 250);
			changesRect = new Rect(updatesRect.x,updatesRect.y + updatesRect.height + windowPadding, windowWidth - windowPadding, 250);
		}
		
		void OnGUI() {
			GUILayout.BeginHorizontal();
			GUILayout.Button("Overview");
			GUILayout.Button("Update");
			GUILayout.Button("Commit");
			GUILayout.EndHorizontal();
			
			BeginWindows();
			GUILayout.Window(0, overviewRect, getOverviewWindow, "Overview");
			GUILayout.Window(1, updatesRect, getUpdatesWindow, "Updates on Server");
			GUILayout.Window(2, changesRect, getLocalChangesWindow, "Local Changes");
			EndWindows();			
		}
		
		void getOverviewWindow(int id) {
			//GUILayout.BeginVertical();
				
			foreach(GitSharp.Tree subTree in currentTree.Trees) {
				GUILayout.Label(subTree.Path);	
			}
			
			//GUILayout.EndVertical();
		}
		
		void getUpdatesWindow(int id) {
			foreach(Commit commit in repo.Head.CurrentCommit.Ancestors) {
				
				string commitMessage = commit.Message.Split("\r\n".ToCharArray())[0];
				
				System.DateTimeOffset d = commit.CommitDate;
				string dateString = d.Month+"/"+d.Day+"/"+d.Year+" "+d.Hour+":"+d.Minute+":"+d.Second;
				
				GUILayout.BeginHorizontal();
				
				GUILayout.Label(commitMessage);
				GUILayout.Label(commit.Author.Name);
				GUILayout.Label(dateString);
				
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				
				GUILayout.Label(commitMessage);
				GUILayout.Label(commit.Author.Name);
				GUILayout.Label(dateString);
				
				GUILayout.EndHorizontal();
			}
		}
		
		void getLocalChangesWindow(int id) {
			GUILayout.Label("Uhh..");
		}
	}
}
