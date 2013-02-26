using System;
using UnityEngine;
using LibGit2Sharp;
using System.Linq;

namespace UniTEAM {
	public class LocalStashedCommitsWindow {
		public Rect rect;
		private Vector2 scroll = Vector2.zero;
		public bool isPushing = false;
		private bool doesRequireFetch = false;

		public void draw( Console console, int i ) {
			scroll = GUILayout.BeginScrollView( scroll );

			GUI.enabled = !isPushing;

			if ( isPushing ) {
				doesRequireFetch = true;
			} else if ( !isPushing && doesRequireFetch ) {
				//# Trigger a fetch
				doesRequireFetch = false;
				console.fetch();
			}

			foreach ( Commit commit in console.repo.Commits.QueryBy( new Filter {
				Since = console.branch.Tip, Until = console.branch.TrackedBranch
			} ) ) {
				console.getUpdateItem( commit, commit.Parents.First(), rect, onCommitSelected );
			}

			GUI.enabled = true;

			GUILayout.EndScrollView();
		}

		private void onCommitSelected( Commit commit ) {}
	}
}
