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
	public class LocalStashedCommitsWindow {

		public static Rect rect;
		public static Vector2 scroll;

		public static void draw( int i ) {
			scroll = GUILayout.BeginScrollView( scroll );

			foreach ( Commit commit in Console.repo.Commits.QueryBy( new Filter { Since = Console.branch.Tip, Until = Console.branch.TrackedBranch } ) ) {
				Console.getUpdateItem( commit, rect );
			}

			GUILayout.EndScrollView();
		
		}
	}

}
