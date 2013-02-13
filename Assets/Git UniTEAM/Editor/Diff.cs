using UnityEngine;
using UnityEditor;

namespace UniTEAM {

	public class Diff : EditorWindow {
		private static string patch;
		private Vector2 rect;

		public static void init( string diffPatch ) {
			patch = diffPatch;
			GetWindow<Diff>( "UniTEAM -- DiffffffFFFFFFFFFFFFFFFFFFFF", typeof(Console) );
		}


		private void OnGUI() {
			try {
				rect = GUILayout.BeginScrollView( rect );
				string[] arr = patch.Split( "\n".ToCharArray() );
				Texture2D oldBG = GUI.skin.label.normal.background;

				foreach ( string s in arr ) {
					if ( s.StartsWith( "-" ) ) {
						GUI.skin.label.normal.background = UncommitedChangesWindow.getGenericTexture( 1, 1, new Color( Color.red.r, Color.red.g, Color.red.b, 0.25f ) );
					} else if ( s.StartsWith( "+" ) ) {
						GUI.skin.label.normal.background = UncommitedChangesWindow.getGenericTexture( 1, 1, new Color( Color.green.r, Color.green.g, Color.green.b, 0.25f ) );
					} else {
						GUI.skin.label.normal.background = oldBG;
					}

					GUILayout.Label( s );
				}
				GUILayout.EndScrollView();
			}catch{}
		}
	}

}

