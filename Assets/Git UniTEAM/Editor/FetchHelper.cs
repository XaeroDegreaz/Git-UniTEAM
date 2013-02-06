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
		public static void RemoteFetch( ref Remote remote, ref Credentials creds, Console console ) {
			try {
				remote.Fetch( TagFetchMode.Auto,
					console.OnProgress,
					console.OnCompletion,
					console.OnUpdateTips,
					console.OnTransferProgress,
					credentials: creds
				);
			} catch ( System.Exception e ) {
				Debug.Log( e );
			}
		}
	}
}
