using System.ComponentModel;
using UnityEngine;
using UnityEditor;
using LibGit2Sharp;
using System.Collections.Generic;
using System.IO;

namespace UniTEAM {
	public class Console : EditorWindow {

		private string lastCommitMessage;
		private const float windowPadding = 5f;
		private float nextRefetch = -1;
		private const float refetchFrequency = 15f;

		public Vector2 overviewWindowScroll;
		public Vector2 updatesOnServerWindowScroll;
		public Vector2 uncommitedChangesWindowScroll;
		public Vector2 localStashedCommitsWindowScroll;

		public UncommitedChangesWindow uncommitedChangesWindow;
		public Repository repo;
		public Remote remote;
		public Branch branch;
		public Credentials credentials;
		public bool isFetchComplete = false;
		public bool isOkToPoll = false;
		public IEnumerable<Commit> commitsOnServer = new BindingList<Commit>();
		public IEnumerable<Commit> commitsInStash = new BindingList<Commit>();
		private bool isConsoleReady = false;

		[MenuItem( "Team/Git UniTEAM Console" )]
		static void init() {
			EditorWindow.GetWindow( typeof( Console ), false, "UniTEAM" );
		}

		void OnEnable() {
			credentials = new Credentials();
			credentials.Username = "xaerodegreaz";
			credentials.Password = "!!11OBywan";

			repo = new Repository( Directory.GetCurrentDirectory() );
			branch = repo.Head;
			remote = repo.Network.Remotes[ "origin" ];

			isConsoleReady = true;
		}

		void OnDisable() {
			isFetchComplete = false;
		}

		public void fetch() {
			commitsOnServer = repo.Commits.QueryBy( new Filter { Since = branch.TrackedBranch, Until = branch.Tip } );
			commitsInStash = repo.Commits.QueryBy( new Filter { Since = branch.Tip, Until = branch.TrackedBranch } );

			try {
				//isFetchComplete = false;

				FetchHelper.RemoteFetch( remote, credentials, this );

				if ( uncommitedChangesWindow != null ) {
					uncommitedChangesWindow.reset( repo.Diff.Compare(), this );
				}

				nextRefetch = Time.realtimeSinceStartup + refetchFrequency;

				Repaint();
			}
			catch ( System.Exception e) {
				Debug.Log( e );
			}
		}

		private void OnInspectorUpdate() {
			if ( !isConsoleReady ) {
				return;
			}

			if ( Time.realtimeSinceStartup >= nextRefetch ) {
				fetch();
			}
		}

		void OnGUI() {
			if ( !isConsoleReady ) {
				return;
			}

			try {
				//# Create new instances so we can instantia the guiskin stuff once and only once
				//# reducing the amount of function calls during ongui
				if ( uncommitedChangesWindow == null ) {
					uncommitedChangesWindow = new UncommitedChangesWindow();

					uncommitedChangesWindow.reset( repo.Diff.Compare(), this );
				}

				fixWindowRects();

				GUILayout.BeginHorizontal();
				GUILayout.Button( "Overview" );

				if ( GUILayout.Button( "Force Re-fetch (refresh): " ) ) {
					fetch();
				}

				GUI.enabled = !LocalStashedCommitsWindow.isPushing;
				if ( GUILayout.Button( ( !LocalStashedCommitsWindow.isPushing ) ? "Push Stashed Commits" : "Pushing, please wait..." ) ) {

					//# Don't send blank pushes....
					if ( repo.Head.AheadBy == 0 ) {
						return;
					}

					UnityThreadHelper.CreateThread( () => {
						LocalStashedCommitsWindow.isPushing = true;
						repo.Network.Push( remote, "refs/heads/master:refs/heads/master", OnPushStatusError, credentials );
						LocalStashedCommitsWindow.isPushing = false;
					} );
				}
				GUI.enabled = true;

				GUILayout.EndHorizontal();

				BeginWindows();
				if ( !currentError.Equals( string.Empty ) ) {
					GUILayout.Window( 4, currentErrorLocation, errorWindow, "Error:" );
				}
				else {
					GUILayout.Window( 0, OverviewWindow.rect, windowDelegate, "Overview" );
					GUILayout.Window( 1, UncommitedChangesWindow.rect, windowDelegate, "Uncommited Changes" );
					GUILayout.Window( 2, UpdatesOnServerWindow.rect, windowDelegate,
					                  "Updates on Server [Commits Behind: " + repo.Head.BehindBy + "]" );
					GUILayout.Window( 3, LocalStashedCommitsWindow.rect, windowDelegate,
					                  "Local Commit Stash [Commits Ahead: " + repo.Head.AheadBy + "]" );
				}

				EndWindows();
			}
			catch {}
		}

		//# Using this to pass the console reference. Trying not to leak stuff with static properties...
		private void windowDelegate( int id ) {
			switch ( id ) {
				case 0:
					OverviewWindow.draw( this, id );
					break;
				case 1:
					uncommitedChangesWindow.draw( this, id );
					break;
				case 2:
					UpdatesOnServerWindow.draw( this, id );
					break;
				case 3:
					LocalStashedCommitsWindow.draw( this, id );
					break;
			}
		}

		private void OnPushStatusError( PushStatusError pushStatusErrors ) {
			Debug.LogError( "Push errors: "+pushStatusErrors );
		}

		public static string currentError = string.Empty;
		public static Rect currentErrorLocation;

		private static void errorWindow( int i ) {
			GUILayout.Label( currentError );
			if ( GUI.Button( new Rect(0,currentErrorLocation.height - 20, currentErrorLocation.width, 20), "Close" ) ) {
				currentError = string.Empty;
			}
		}

		private void fixWindowRects() {
			float windowWidth = ( position.width / 2 ) - windowPadding;
			float windowHeight = ( position.height / 2.25f ) - windowPadding;

			OverviewWindow.rect = new Rect( windowPadding, 30, windowWidth, windowHeight );

			UncommitedChangesWindow.rect = new Rect(
				windowPadding,
				OverviewWindow.rect.y + OverviewWindow.rect.height + ( windowPadding * 2 ),
				windowWidth,
				windowHeight - windowPadding
			);

			UpdatesOnServerWindow.rect = new Rect(
				OverviewWindow.rect.x + OverviewWindow.rect.width + windowPadding,
				OverviewWindow.rect.y,
				windowWidth - windowPadding,
				windowHeight
			);

			LocalStashedCommitsWindow.rect = new Rect(
				UpdatesOnServerWindow.rect.x,
				UpdatesOnServerWindow.rect.y + UpdatesOnServerWindow.rect.height + ( windowPadding * 2 ),
				windowWidth - windowPadding,
				windowHeight - windowPadding
			);
		}

		public static void getUpdateItem(Commit commit, Commit lastCommit, Rect windowRect ) {

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
}