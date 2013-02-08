using System;
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
		public static Repository repo;
		public Remote remote;
		public static Branch branch;
		public Credentials credentials;
		private static float windowPadding = 5f;
		private float nextRefetch = 30f;
		private float refetchFrequency = 30f;

		public Vector2 overviewWindowScroll;
		public Vector2 updatesOnServerWindowScroll;
		public Vector2 uncommitedChangesWindowScroll;
		public Vector2 localStashedCommitsWindowScroll;

		public UncommitedChangesWindow uncommitedChangesWindow;

		private string selectedRemote;
		private bool isSelecting = false;

		[MenuItem( "Team/Git UniTEAM" )]
		static void init() {
			//CreateInstance<Console>();
			EditorWindow.GetWindow( typeof( UniTEAM.Console ), false, "UniTEAM" );
		}

		public static Console instance;
		void OnEnable() {
			instance = this;
			credentials = new Credentials();
			credentials.Username = "xaerodegreaz";
			credentials.Password = "!!11OBywan";

			repo = new Repository( Directory.GetCurrentDirectory() );
			remote = repo.Remotes[ "origin" ];

			OverviewWindow.selectedRemote = remote.Name;

			fetch();
		}

		public void fetch() {
			FetchHelper.isFetchComplete = false;
			FetchHelper.RemoteFetch( ref remote, ref credentials, this );
			UncommitedChangesWindow.reset( repo.Diff.Compare() );

			nextRefetch = Time.realtimeSinceStartup + refetchFrequency;
			branch = repo.Head;

			Repaint();
		}

		void Update() {
			if ( FetchHelper.isFetchComplete ) {
				if ( Time.realtimeSinceStartup >= nextRefetch ) {
					fetch();
				}
			}
		}

		void OnGUI() {
			//# Create new instances so we can instantia the guiskin stuff once and only once
			//# reducing the amount of function calls during ongui
			if ( uncommitedChangesWindow == null ) {
				uncommitedChangesWindow = new UncommitedChangesWindow();
			}

			fixWindowRects();

			GUILayout.BeginHorizontal();
			GUILayout.Button( "Overview" );

			if ( GUILayout.Button( "Update" ) ) {
				FetchHelper.RemoteFetch( ref remote, ref credentials, this );
			}


			if ( GUILayout.Button( "Commit" ) ) {
				
			}

			GUILayout.EndHorizontal();

			BeginWindows();
			GUILayout.Window( 0, OverviewWindow.rect, OverviewWindow.draw, "Overview" );
			GUILayout.Window( 1, UncommitedChangesWindow.rect, UncommitedChangesWindow.draw, "Uncommited Changes" );
			GUILayout.Window( 2, UpdatesOnServerWindow.rect, UpdatesOnServerWindow.draw,
							  "Updates on Server [Commits Behind: " + repo.Head.BehindBy + "]" );
			GUILayout.Window( 3, LocalStashedCommitsWindow.rect, LocalStashedCommitsWindow.draw,
							  "Local Commit Stash [Commits Ahead: " + repo.Head.AheadBy + "]" );
			EndWindows();
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

		public static void getUpdateItem( Commit commit, Rect windowRect ) {
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