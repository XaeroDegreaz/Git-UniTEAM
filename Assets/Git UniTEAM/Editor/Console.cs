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
		private float windowPadding = 5;

		public Rect overviewWindowRect;
		public Rect updatesOnServerWindowRect;
		public Rect uncommitedChangesWindowRect;
		public Rect localStashedCommitsWindowRect;
		
		public Vector2 overviewWindowScroll;
		public Vector2 updatesOnServerWindowScroll;
		public Vector2 uncommitedChangesWindowScroll;
		public Vector2 localStashedCommitsWindowScroll;
		
		[MenuItem("Team/Git UniTEAM")]
		static void init() {
			EditorWindow.GetWindow(typeof(Console), false, "UniTEAM");
		}
		
		void OnEnable() {
			repo = new Repository( Directory.GetCurrentDirectory() );
			repo.Fetch( "origin" );

			branch = repo.Head;

			Repaint();
		}
		
		void Update () {

		}
		
		void OnGUI() {
			float windowWidth = (position.width / 2) - windowPadding;
			overviewWindowRect = new Rect(windowPadding, 30, windowWidth, 500);
			
			updatesOnServerWindowRect = new Rect(
				overviewWindowRect.x + overviewWindowRect.width + windowPadding, 
				overviewWindowRect.y, 
				windowWidth - windowPadding, 
				( overviewWindowRect.height / 2 ) - windowPadding
			);
			
			localStashedCommitsWindowRect = new Rect(updatesOnServerWindowRect.x, updatesOnServerWindowRect.y + updatesOnServerWindowRect.height + (windowPadding * 2), windowWidth - windowPadding, updatesOnServerWindowRect.height);

			GUILayout.BeginHorizontal();
			GUILayout.Button("Overview");
			GUILayout.Button("Update");
			GUILayout.Button("Commit");
			GUILayout.EndHorizontal();
			
			BeginWindows();
			GUILayout.Window(0, overviewWindowRect, windowOverview, "Overview");
			GUILayout.Window(1, updatesOnServerWindowRect, windowUpdatesOnServer, "Updates on Server");
			GUILayout.Window(2, localStashedCommitsWindowRect, windowLocalStashedCommits, "Local Commit Stash");
			EndWindows();			
		}
		
		private void windowOverview(int id) {
			GUILayout.Label( "Repository: "+repo.Info.WorkingDirectory );
			GUILayout.Label( "Remote: " + repo.Remotes["origin"].Url );
			GUILayout.Label( "Commits Ahead: "+ repo.Head.AheadBy );
			GUILayout.Label( "Commits Behind: " + repo.Head.BehindBy );

			foreach ( Commit commit in repo.Commits.QueryBy( new Filter { Since = branch.TrackedBranch, Until = branch.Tip }) ) {
				CommitItem item = new CommitItem( commit );

				GUILayout.Label( item.dateString );
			}
		}
		
		private void windowUpdatesOnServer(int id) {
			updatesOnServerWindowScroll = GUILayout.BeginScrollView( updatesOnServerWindowScroll );

			foreach ( Commit commit in repo.Commits.QueryBy( new Filter { Since = branch.TrackedBranch, Until = branch.Tip } ) ) {
				getUpdateItem( commit, updatesOnServerWindowRect );
			}

			GUILayout.EndScrollView();
		}

		private void windowUncommitedChanges( int id ) {
			GUILayout.Label( "Uhh.." );
		}

		private void windowLocalStashedCommits( int id ) {
			localStashedCommitsWindowScroll = GUILayout.BeginScrollView( localStashedCommitsWindowScroll );

			foreach ( Commit commit in repo.Commits.QueryBy( new Filter { Since = branch.Tip, Until = branch.TrackedBranch } ) ) {
				getUpdateItem( commit, localStashedCommitsWindowRect );
			}

			GUILayout.EndScrollView();
		}
		
		void getUpdateItem(Commit commit, Rect windowRect) {				
			CommitItem item = new CommitItem( commit );

			float horizontalWidth = ( windowRect.width ) - ( windowPadding * 2 ) - 25;
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
