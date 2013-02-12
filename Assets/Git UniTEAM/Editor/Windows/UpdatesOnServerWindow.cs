using UnityEngine;
using System.Linq;
using LibGit2Sharp;

namespace UniTEAM {

	public class UpdatesOnServerWindow {

		private static Vector2 scroll;
		public static Rect rect;

		public static void draw( Console console, int id ) {
			scroll = GUILayout.BeginScrollView( scroll );

			if ( console.commitsOnServer.Any() ) {
				foreach ( Commit commit in console.commitsOnServer ) {
					Console.getUpdateItem( commit, commit.Parents.First(), rect );
				}
			}

			GUILayout.EndScrollView();
		}
	}
}
