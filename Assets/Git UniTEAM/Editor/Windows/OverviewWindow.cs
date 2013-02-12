using System;
using UnityEngine;
using UnityEditor;
using LibGit2Sharp;
using LibGit2Sharp.Core;
using LibGit2Sharp.Handlers;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UniTEAM {
	public static class OverviewWindow {
		public static string selectedRemote;
		public static bool isSelecting;
		public static Rect rect;

		public static void draw(Console console, int id ) {
			GUILayout.Label( "Repository: " + console.repo.Info.WorkingDirectory );

			//GUILayout.BeginHorizontal();
			//GUILayout.Label( "Remote: " + Console.repo.Remotes[ "origin" ].Url );
			//getRemoteList();
			//GUILayout.EndHorizontal();

			GUILayout.Label( "Current branch: " + console.branch.Name );

			GUILayout.Label( "SSL Credentials" );
			GUILayout.Label( "Username" );
			console.credentials.Username = GUILayout.TextField( console.credentials.Username );
			GUILayout.Label( "Password" );
			console.credentials.Password = GUILayout.TextField( console.credentials.Password );

		}

		static void getRemoteList(Console console) {
			GUILayout.BeginVertical( "Box" );
			if ( GUILayout.Button( selectedRemote ) ) {
				isSelecting = !isSelecting;
			}

			if ( isSelecting ) {
				int i = 0;
				foreach ( Remote b in console.repo.Network.Remotes ) {
					if ( GUI.Button( new Rect(0, 30 + (i*30), 20, 20), b.Name  ) ) {
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
