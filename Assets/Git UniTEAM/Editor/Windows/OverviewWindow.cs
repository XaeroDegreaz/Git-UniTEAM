using UnityEngine;
using LibGit2Sharp;

namespace UniTEAM {
	public class OverviewWindow {
		public string selectedRemote;
		public bool isSelecting;
		public Rect rect;

		public void draw( Console console, int id ) {
			GUILayout.Label( "Repository: " + console.repo.Info.WorkingDirectory );
			GUILayout.Label( "Current branch: " + console.branch.Name );

			GUILayout.Label( "" );

			GUILayout.Label( "HTTP/HTTPS Credentials" );
			GUILayout.Label( "Username (if required)" );
			console.configManager.username = GUILayout.TextField( console.configManager.username );
			GUILayout.Label( "Password (if required)" );
			console.configManager.password = GUILayout.PasswordField( console.configManager.password, "*".ToCharArray()[ 0 ] );

			if ( GUILayout.Button( "Save" ) ) {
				console.configManager.saveConfig( console );
			}
		}

		private void getRemoteList( Console console ) {
			GUILayout.BeginVertical( "Box" );
			if ( GUILayout.Button( selectedRemote ) ) {
				isSelecting = !isSelecting;
			}

			if ( isSelecting ) {
				int i = 0;
				foreach ( Remote b in console.repo.Network.Remotes ) {
					if ( GUI.Button( new Rect( 0, 30 + ( i * 30 ), 20, 20 ), b.Name ) ) {
						selectedRemote = b.Name;
						isSelecting = false;
					}

					i++;
				}
			}
			GUILayout.EndHorizontal();
		}
	}
}
