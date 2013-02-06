using System;
using UnityEngine;
using UnityEditor;
using LibGit2Sharp;
using LibGit2Sharp.Core;
using LibGit2Sharp.Handlers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniTEAM;

namespace UniTEAM {
	public class FetchHelper {

		public static bool isFetchComplete = false;

		public static void RemoteFetch( ref Remote remote, ref Credentials creds, Console console ) {
			try {
				remote.Fetch( TagFetchMode.Auto,
					OnProgress,
					OnCompletion,
					OnUpdateTips,
					OnTransferProgress,
					credentials: creds
				);

				isFetchComplete = true;
			} catch ( System.Exception e ) {
				Debug.Log( e );
			}
		}

		public static void OnTransferProgress( TransferProgress progress ) {
			Debug.LogWarning( progress );
		}

		public static int OnUpdateTips( string referenceName, ObjectId oldId, ObjectId newId ) {
			isFetchComplete = true;

			Debug.LogWarning( referenceName + "/" + oldId + "/" + newId );

			return 0;
		}

		public static int OnCompletion( RemoteCompletionType remoteCompletionType ) {
			Debug.LogWarning( "Complete" );
			return 0;
		}

		public static void OnProgress( string serverProgressOutput ) {
			isFetchComplete = false;

			Debug.LogWarning( serverProgressOutput );
		}
	}
}