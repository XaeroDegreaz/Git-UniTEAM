using UnityEngine;
using LibGit2Sharp;

namespace UniTEAM {
	public class FetchHelper {
		public static void RemoteFetch( Remote remote, Credentials creds, Console console ) {
			try {
				remote.Fetch( TagFetchMode.Auto,
				              OnProgress,
				              OnCompletion,
				              OnUpdateTips,
				              OnTransferProgress,
				              credentials: creds
					);
			} catch ( System.Exception e ) {
				Debug.Log( e );
			}
		}

		public static void OnTransferProgress( TransferProgress progress ) {
			Debug.LogWarning( "FetchHelper - OnTransferProgress => " + progress );
		}

		public static int OnUpdateTips( string referenceName, ObjectId oldId, ObjectId newId ) {
			Debug.LogWarning( "FetchHelper - OnUpdateTips => " + referenceName + " / " + oldId + " / " + newId );
			return 0;
		}

		public static int OnCompletion( RemoteCompletionType remoteCompletionType ) {
			Debug.LogWarning( "FetchHelper - OnCompletion => " + remoteCompletionType );
			return 0;
		}

		public static void OnProgress( string serverProgressOutput ) {
			Debug.LogWarning( "FetchHelper - OnProgress => " + serverProgressOutput );
		}
	}
}
