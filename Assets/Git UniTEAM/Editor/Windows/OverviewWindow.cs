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

		public static void draw( int id ) {
			GUILayout.Label( "Repository: " + Console.repo.Info.WorkingDirectory );

			GUILayout.BeginHorizontal();
			GUILayout.Label( "Remote: " + Console.repo.Remotes[ "origin" ].Url );
			getRemoteList();
			GUILayout.EndHorizontal();

			GUILayout.Label( "Current branch: " + Console.branch.Name );
		}

		static void getRemoteList() {
			GUILayout.BeginVertical( "Box" );
			if ( GUILayout.Button( selectedRemote ) ) {
				isSelecting = !isSelecting;
			}

			if ( isSelecting ) {
				int i = 0;
				foreach ( Remote b in Console.repo.Remotes ) {
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
