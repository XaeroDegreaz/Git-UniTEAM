using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using LibGit2Sharp;

namespace UniTEAM {
	public class HistoryWindow {
		private Vector2 scroll;
		private Vector2 commitMessageScroll = new Vector2();
		private string commitMessage = string.Empty;
		public Rect rect;
		public Rect commitMessageRect;
		private int commitsToShow;

		public void draw( Console console, int id ) {
			GUILayout.BeginHorizontal();
			GUILayout.Label( "# Commits (0 for all)", GUILayout.Width( 150 ) );
			commitsToShow = EditorGUILayout.IntField( commitsToShow, GUILayout.Width( 50 ) );
			GUILayout.EndHorizontal();

			scroll = GUILayout.BeginScrollView( scroll );

			foreach ( Commit commit in ( commitsToShow > 0 ) ? console.repo.Commits.Take( commitsToShow ) : console.repo.Commits ) {
				try {
					console.getUpdateItem( commit, commit.Parents.First(), rect, onCommitSelected );
				} catch {
					console.getUpdateItem( commit, commit, rect, onCommitSelected );
				}
			}

			GUILayout.EndScrollView();
		}

		public void commitMessageWindow( int id ) {
			GUIStyle skin = new GUIStyle( GUI.skin.textArea );
			skin.normal.background = GUI.skin.label.normal.background;

			commitMessageScroll = GUILayout.BeginScrollView( commitMessageScroll );
			GUILayout.TextArea( commitMessage, skin );
			GUILayout.EndScrollView();
		}

		private void onCommitSelected( Commit commit ) {
			commitMessage = commit.Message;
		}
	}
}
