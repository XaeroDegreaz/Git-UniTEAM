using UnityEngine;
using UnityEditor;
using LibGit2Sharp;
using LibGit2Sharp.Core;
using LibGit2Sharp.Handlers;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UniTEAM {
	public class Console : EditorWindow {
		
		private string lastCommitMessage;
		private Repository repo;
		private Branch branch;
		private IEnumerable<FetchHead> heads;
		//private ArrayList heads;
		
		public float windowPadding = 5;
		public Rect overviewRect;
		public Rect updatesRect;
		public Rect changesRect;
		public Vector2 updatesRectScroll;
		
		[MenuItem("Team/Git UniTEAM")]
		static void init() {
			EditorWindow.GetWindow(typeof(Console), false, "UniTEAM");
		}
		
		void OnEnable() {
			//Debug.LogWarning(" Git UniTEAM loaded: "+System.DateTime.Now);

			repo = new Repository( Directory.GetCurrentDirectory() );
			repo.Fetch( "origin" );

			branch = repo.Head;

			Repaint();
		}
		
		// Update is called once per frame
		void Update () {
		}
		
		void OnGUI() {
			float windowWidth = (position.width / 2) - windowPadding;
			overviewRect = new Rect(windowPadding, 30, windowWidth, 500);
			
			updatesRect = new Rect(
				overviewRect.x + overviewRect.width + windowPadding, 
				overviewRect.y, 
				windowWidth - windowPadding, 
				( overviewRect.height / 2 ) - windowPadding
			);
			
			changesRect = new Rect(updatesRect.x, updatesRect.y + updatesRect.height + (windowPadding * 2), windowWidth - windowPadding, updatesRect.height);

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
			GUILayout.Label( "Repository: "+repo.Info.WorkingDirectory );
			GUILayout.Label( "Remote: " + repo.Remotes["origin"].Url );
			GUILayout.Label( "Commits Ahead: "+ repo.Head.AheadBy );
			GUILayout.Label( "Commits Behind: " + repo.Head.BehindBy );

			

			/*foreach ( Commit commit in repo.Commits.QueryBy( new Filter { Since = branch.TrackedBranch, Until = branch.Tip }) ) {
				CommitItem item = new CommitItem( commit );

				GUILayout.Label( item.dateString );
			}*/
		}
		
		void getUpdatesWindow(int id) {
			updatesRectScroll = GUILayout.BeginScrollView( updatesRectScroll );
			foreach ( Commit commit in repo.Commits ) {
				getUpdateItem( commit );
			}
			GUILayout.EndScrollView();
		}
		
		void getUpdateItem(Commit commit) {				
			CommitItem item = new CommitItem( commit );

			float horizontalWidth = ( updatesRect.width ) - ( windowPadding * 2 ) - 25;
			float halfWidth = ( horizontalWidth / 2 ) - ( windowPadding * 2 );
			float quarterWidth = ( horizontalWidth / 4 ) - ( windowPadding * 2 );

			Rect r = EditorGUILayout.BeginHorizontal( "Button", GUILayout.Width( horizontalWidth ) );

			if ( GUI.Button( r, GUIContent.none ) ) {
				Debug.Log( "Commit pressed" );
			}

			GUILayout.Label( item.commitMessage.Substring( 0, Mathf.Min( item.commitMessage.Length, 100 ) ), GUILayout.Width( halfWidth ) );
			GUILayout.Label( "\t\t" + commit.Author.Name, GUILayout.Width( quarterWidth ) );
			GUILayout.Label( item.dateString, GUILayout.Width( quarterWidth ) );
			
			GUILayout.EndHorizontal();
		}
		
		void getLocalChangesWindow(int id) {
			GUILayout.Label("Uhh..");
		}
	}

	public class CommitItem {
		private System.DateTimeOffset d;
		public string commitMessage;
		private string hour;
		private string minute;
		private string second;
		public string dateString; 

		public CommitItem( Commit commit ) {
			d = commit.Author.When;
			commitMessage = commit.Message.Split( "\r\n".ToCharArray() )[ 0 ];
			hour = ( d.Hour.ToString().Length == 1 ) ? "0" + d.Hour : d.Hour.ToString();
			minute = ( d.Minute.ToString().Length == 1 ) ? "0" + d.Minute : d.Minute.ToString();
			second = ( d.Second.ToString().Length == 1 ) ? "0" + d.Second : d.Second.ToString();
			dateString = d.Month + "/" + d.Day + "/" + d.Year + " " + hour + ":" + minute + ":" + second;
		}
	}
}
