using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using LibGit2Sharp;
using LibGit2Sharp.Core;
using LibGit2Sharp.Handlers;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UniTEAM {

	public class UpdatesOnServerWindow {

		private static Vector2 scroll;
		public static Rect rect;

		public static void draw( int id ) {
			scroll = GUILayout.BeginScrollView( scroll );

			foreach ( Commit commit in Console.repo.Commits.QueryBy( new Filter { Since = Console.branch.TrackedBranch, Until = Console.branch.Tip } ) ) {
				Console.getUpdateItem( commit, commit.Parents.First(), rect );
			}

			GUILayout.EndScrollView();
		}
	}
}
