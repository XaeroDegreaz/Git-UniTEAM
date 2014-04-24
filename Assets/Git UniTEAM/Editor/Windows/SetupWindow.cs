using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using LibGit2Sharp;
using System.Linq;

namespace UniTEAM {
	public class SetupWindow : EditorWindow {
		private List<string> progressInfo;
		private const float SETUP_REDRAW_FREQUENCY = .1f;
		private float nextSetupRedraw = -1;
		public SetupState setupState;
		private string gitURL = "";
		private string username = "";
		private string password = "";
		private GUIStyle wordWrapLabelStyle;
		private GUIStyle textAreaStyle;
		private string tempDir;
		private string workingDir;
		private Vector2 scroll = new Vector2( 0, 0 );
		public static Console cns;
		private bool hasClosedConsole = false;
		private bool hasWorkError = false;

		public enum SetupState {
			nothing,
			clone,
			initialize,
			working,
			complete
		}

		void OnEnable() {
			progressInfo = new List<string>();

			workingDir = new DirectoryInfo( Application.dataPath ).Parent.FullName;
			tempDir = workingDir + Path.DirectorySeparatorChar + "uniteamtempclonedir";

			//# Hack to delete temp dir if it exists...
			try {
				Directory.Delete( tempDir, true );
			} catch { }
		}

		void Update() {
			if ( setupState == SetupState.working ) {
				if ( Time.realtimeSinceStartup >= nextSetupRedraw ) {
					Repaint();
					nextSetupRedraw = Time.realtimeSinceStartup + SETUP_REDRAW_FREQUENCY;
				}
			}
		}

		private void OnGUI() {
			if ( !hasClosedConsole ) {
				hasClosedConsole = true;
				cns.Close();
				cns = null;
			}

			if ( wordWrapLabelStyle == null ) {
				wordWrapLabelStyle = new GUIStyle( GUI.skin.label );
				textAreaStyle = new GUIStyle( GUI.skin.textArea );
				wordWrapLabelStyle.wordWrap = true;
			}

			switch ( setupState ) {
				case SetupState.nothing:
					GUILayout.Label( "This Unity project is not currently under version control." );
					GUILayout.Label( "" );
					GUILayout.Label( "Would you like to 'Clone' a remote repository, or 'Initialize' your current project for Git version control?" );
					GUILayout.Label( "" );

					GUILayout.BeginHorizontal();

					GUI.enabled = !( setupState == SetupState.clone );
					if ( GUILayout.Button( "Clone" ) ) {
						setupState = SetupState.clone;
					}

					GUI.enabled = false;
					if ( GUILayout.Button( "Initialize [NOT YET IMPLEMENTED]" ) ) {
						setupState = SetupState.initialize;
					}

					GUI.enabled = true;
					GUILayout.EndHorizontal();
					break;
				case SetupState.clone:
					drawGetRemoteCredentials();
					GUILayout.Label( "" );
					GUILayout.Label( "WARNING!" );
					GUILayout.Label( "1 ) Any conflicting files in this project directory [" + tempDir + "] will be OVERWRITTEN by the ones one the server. " +
					                 "Back them up if you value them.", wordWrapLabelStyle, GUILayout.Width( 400 ) );
					GUILayout.Label( "" );
					GUILayout.Label( "2 ) If the remote repository has 'Git UniTEAM' under version control, the clone will fail since the .dll " +
					                 "files are currently in use by Unity; you'll have to close Unity, and clone the repository using another tool, " +
					                 "such as TortoiseGit. Data loss may occur if you do not heed / investigate this warning!", wordWrapLabelStyle, GUILayout.Width( 400 ) );
					GUILayout.Label( "" );
					if ( GUILayout.Button( "I understand; Start Clone" ) ) {
						setupState = SetupState.working;
						EditorApplication.LockReloadAssemblies();
						clone();
					}
					if ( GUILayout.Button( "Cancel" ) ) {
						setupState = SetupState.nothing;
					}
					break;
				case SetupState.initialize:
					break;
				case SetupState.working:
					GUILayout.Label( "Work log:" );

					try {
						scroll = GUILayout.BeginScrollView( ( hasWorkError ) ? scroll : new Vector2( 0, int.MaxValue ), textAreaStyle, GUILayout.Height( 200 ) );

						EditorGUILayout.TextArea( string.Join( "\r\n", progressInfo.ToArray() ) );
					}catch{}
					
					GUILayout.EndScrollView();

					if ( hasWorkError ) {
						if ( GUILayout.Button( "Retry failed operation." ) ) {
							progressInfo.Clear();
							hasWorkError = false;
							Directory.Delete( tempDir, true );
							EditorApplication.LockReloadAssemblies();
							clone();
						}
					}
					break;
				case SetupState.complete:
					EditorApplication.UnlockReloadAssemblies();

					GUILayout.Label( "All work complete. Please click the close button." );
					if ( GUILayout.Button( "Close, and restart console." ) ) {
						Console.init();
						this.Close();
					}
					break;
			}
		}

		private void clone(int attempt = 0) {
			progressInfo.Add( "Standby; negotiating connection to remote repository..." );
			scroll.y = int.MaxValue;

			UnityThreadHelper.CreateThread( () => {
				Credentials credentials = new Credentials();
				credentials.Username = username;
				credentials.Password = password;

				try {
					using ( var repo = Repository.Clone(
						gitURL,
						tempDir,
						false, true,
						onTransferProgress, onCheckoutProgress,
						null, credentials ) ) {

						progressInfo.Add( "Clone complete!" );
					}

					moveFiles();
					setupState = SetupState.complete;
				}
				catch ( LibGit2SharpException e ) {
					progressInfo.Add( "Clone failed [PLUGIN] ==> " + e );
					scroll.y = int.MaxValue;

					if ( attempt == 0 ) {
						clone(1);
					}

					hasWorkError = true;
				}
				catch ( System.Exception e ) {
					progressInfo.Add( "Clone Failed [OTHER] ==> " + e );
					scroll.y = int.MaxValue;
					hasWorkError = true;
				}
				finally {
					try {
						Directory.Delete( tempDir, true );
					}
					catch {}
				}
			} );
		}

		private void moveFiles() {

			progressInfo.Add( @"*** All operations complete! Please restart the Git UniTEAM console. ***" );
			scroll.y = int.MaxValue;

			foreach ( string dir in Directory.GetDirectories( tempDir, "*", SearchOption.AllDirectories ) ) {
				Directory.CreateDirectory( dir.Replace( tempDir, workingDir ) );
			}

			foreach ( string file in Directory.GetFiles( tempDir, "*.*", SearchOption.AllDirectories ) ) {

				if ( File.Exists( file.Replace( tempDir, workingDir ) ) ) {
					File.Delete( file.Replace( tempDir, workingDir ) );
				}

				File.Move( file, file.Replace( tempDir, workingDir ) );
			}
		}

		private void drawGetRemoteCredentials() {
			GUILayout.Label( "" );
			GUILayout.Label( "EX: https://www.example.com/my-repository.git" );
			GUILayout.Label( "NOTE: https://USERNAME@www.example.com/my-repository.git type URLs will not work!" );
			GUILayout.BeginHorizontal();
			GUILayout.Label( "HTTPS Git URL",GUILayout.Width( 100 ) );
			gitURL = EditorGUILayout.TextField(gitURL, GUILayout.Width( 350 ) );
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label( "Username",GUILayout.Width( 100 ) );
			username = EditorGUILayout.TextField( username, GUILayout.Width( 150 ) );
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label( "Password", GUILayout.Width( 100 ) );
			password = EditorGUILayout.PasswordField( password, GUILayout.Width( 150 ) );
			GUILayout.EndHorizontal();
		}

		private void onCheckoutProgress( string path, int completedSteps, int totalSteps ) {
			progressInfo.Add( "Checking out: " + completedSteps + "/" + totalSteps + " => " + path );
			scroll.y = int.MaxValue;
		}

		private void onTransferProgress( TransferProgress progress ) {
			progressInfo.Add( "Received bytes: " + progress.ReceivedBytes );
			scroll.y = int.MaxValue;
		}
	}
}
