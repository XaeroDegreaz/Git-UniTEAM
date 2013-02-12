using System;
using UnityEngine;
using UnityEditor;
using LibGit2Sharp;
using System.Linq;
using LibGit2Sharp.Core;
using LibGit2Sharp.Handlers;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UniTEAM {
	public class LocalStashedCommitsWindow {

		public static Rect rect;
		private static Vector2 scroll = Vector2.zero;
		public static bool isPushing = false;
		private static bool doesRequireFetch = false;

		public static void draw(Console console, int i ) {

			scroll = GUILayout.BeginScrollView( scroll );

			GUI.enabled = !isPushing;

			if ( isPushing ) {
				doesRequireFetch = true;
			}else if ( !isPushing && doesRequireFetch ) {
				//# Trigger a fetch
				doesRequireFetch = false;
				console.fetch();
			}

			foreach ( Commit commit in console.repo.Commits.QueryBy( new Filter { Since = console.branch.Tip, Until = console.branch.TrackedBranch } ) ) {
				Console.getUpdateItem( commit, commit.Parents.First(), rect );
			}

			GUI.enabled = true;

			GUILayout.EndScrollView();
		
		}
	}

}
