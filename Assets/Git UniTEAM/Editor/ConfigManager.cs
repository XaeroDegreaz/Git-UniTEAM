using LibGit2Sharp;
using UnityEngine;
using System.IO;

namespace UniTEAM {
	public class ConfigManager {
		public string username = string.Empty;
		public string password = string.Empty;
		private string dataPath;

		public ConfigManager( Console console ) {
			dataPath = Application.dataPath;
			loadConfig( console );
		}

		private void loadConfig( Console console ) {
			try {
				StreamReader reader = new StreamReader( dataPath + "\\Plugins\\git-uniteam-config.txt" );
				username = reader.ReadLine().Trim();
				password = reader.ReadLine().Trim();
				reader.Close();

				console.credentials = new Credentials();
				console.credentials.Username = username.Trim();
				console.credentials.Password = password.Trim();
			} catch ( System.Exception e ) {
				Debug.Log( e );
			}
		}

		public void saveConfig( Console console ) {
			FileInfo info = null;

			UnityThreadHelper.CreateThread( () => {
				try {
					info = new FileInfo( dataPath + "\\Plugins\\git-uniteam-config.txt" );
					info.Delete();
				} catch ( System.Exception e ) {
					Debug.Log( e );
				}

				try {
					StreamWriter writer = new StreamWriter( dataPath + "\\Plugins\\git-uniteam-config.txt" );

					writer.WriteLine( username.Trim() );
					writer.WriteLine( password.Trim() );

					writer.Close();

					console.credentials = new Credentials();
					console.credentials.Username = username.Trim();
					console.credentials.Password = password.Trim();
				} catch ( System.Exception e ) {
					Debug.Log( e );
				}
			} );
		}
	}
}
