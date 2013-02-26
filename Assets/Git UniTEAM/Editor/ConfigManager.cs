using LibGit2Sharp;
using UnityEngine;
using System.IO;

namespace UniTEAM {
	public class ConfigManager {
		public string username = string.Empty;
		public string password = string.Empty;
		public string explicitPathToRepository;
		private string dataPath;
		private string configPath;

		public ConfigManager( Console console ) {
			dataPath = Application.dataPath;
			configPath = dataPath + "\\Git UniTEAM\\Editor\\git-uniteam-config.txt";

			loadConfig( console );
		}

		private void loadConfig( Console console ) {
			try {
				StreamReader reader = new StreamReader( configPath );
				username = reader.ReadLine().Trim();
				password = reader.ReadLine().Trim();

				//# Test for real path defined...
				string readLine = reader.ReadLine();

				if ( readLine != null ) {
					explicitPathToRepository = readLine.Trim();
				}

				reader.Close();

				console.credentials = new Credentials();
				console.credentials.Username = username.Trim();
				console.credentials.Password = password.Trim();

			} catch ( System.Exception ) { }
		}

		public void saveConfig( Console console ) {
			FileInfo info = null;

			UnityThreadHelper.CreateThread( () => {
				try {
					info = new FileInfo( configPath );
					info.Delete();
				} catch ( System.Exception e ) {
					Debug.Log( e );
				}

				try {
					StreamWriter writer = new StreamWriter( configPath );

					writer.WriteLine( username.Trim() );
					writer.WriteLine( password.Trim() );

					if ( explicitPathToRepository != null ) {
						writer.WriteLine( explicitPathToRepository.Trim() );
					}

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
