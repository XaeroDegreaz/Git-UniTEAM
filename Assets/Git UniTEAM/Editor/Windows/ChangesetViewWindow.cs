using UnityEngine;
using LibGit2Sharp;
using System.Linq;

namespace UniTEAM {
	public class ChangesetViewWindow {

		public static Rect rect;
		private static Vector2 scroll = Vector2.zero;

		public static void draw( Console console, int i ) {
			scroll = GUILayout.BeginScrollView( scroll );

			GUILayout.EndScrollView();
		}
	}

}
