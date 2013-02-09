using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using LibGit2Sharp;
using LibGit2Sharp.Core;
using LibGit2Sharp.Handlers;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UniTEAM {
	public class UncommitedChangesWindow {

		
		private static List<string> pathNodes = new List<string>();
		private static Dictionary<string, bool> checkboxValues = new Dictionary<string, bool>();
		private static Dictionary<string, bool> foldoutValues = new Dictionary<string, bool>();
		private static GUIStyle statusStyle;
		private static GUIStyle highlightStyle;
		private static GUIStyle noStyle;

		private Texture2D highlightTexture;
		private Texture2D noTexture;

		public static TreeChanges changes;
		public static Rect rect;
		public static Vector2 scroll;
		public static string commitText = string.Empty;

		public UncommitedChangesWindow(  ) {

			highlightTexture = getGenericTexture( 1, 1, new Color( 71f / 255f, 71f / 255f, 71f / 255f ) );
			noTexture = getGenericTexture( 1, 1, new Color( 46f / 255f, 46f / 255f, 46f / 255f ) );

			statusStyle = new GUIStyle( "Label" );
			statusStyle.alignment = TextAnchor.LowerRight;

			highlightStyle = new GUIStyle( "Label" );
			highlightStyle.normal.background = highlightTexture;

			noStyle = new GUIStyle( "Label" );
			noStyle.normal.background = noTexture;
		}

		public static void draw( int i ) {

			bool highlight = true;
			pathNodes.Clear();

			scroll = GUILayout.BeginScrollView( scroll );

			if ( changes == null ) {
				GUILayout.Label( "Loading..." );
				GUILayout.EndScrollView();
				return;
			}

			foreach ( TreeEntryChanges change in changes ) {
				recurseToAssetFolder( change, ref highlight );
			}

			GUI.enabled = true;
			GUILayout.EndScrollView();

			GUILayout.Label( "Commit message:" );
			commitText = GUILayout.TextArea( commitText );
			if ( GUILayout.Button( "Commit Changes" ) ) {
				Signature signature = new Signature( "Jerome Doby", "xaerodegreaz@gmail.com", System.DateTimeOffset.Now );

				//# Stage everything
				string[] stage = new string[checkboxValues.Count];

				i = 0;
				foreach ( KeyValuePair<string, bool> pair in checkboxValues ) {
					if ( pair.Value ) {
						stage[ i ] = pair.Key;
						i++;
					}
				}

				stage = stage.Where( x => !string.IsNullOrEmpty( x ) ).ToArray();

				if ( stage.Length == 0 ) {
					Console.currentError = "You cannot commit without staged items.";
					Console.currentErrorLocation = rect;
				}else if(commitText.Equals( string.Empty )) {
					Console.currentError = "Please enter a commit message.";
					Console.currentErrorLocation = rect;
				} else {
					Console.repo.Index.Stage( stage );
					Console.repo.Commit( commitText, signature );
					FetchHelper.RemoteFetch( Console.instance.remote, Console.instance.credentials, Console.instance );

					checkboxValues.Clear();
					foldoutValues.Clear();
				}

				commitText = string.Empty;
			}
			
		}

		private static void recurseToAssetFolder( TreeEntryChanges change, ref bool highlight) {
			int spacing = 20;
			bool iterationIsDir = false;
			string[] pathArray = change.Path.Split( "\\".ToCharArray() );

			for ( int i = 0; i < pathArray.Length; i++ ) {

				if ( pathNodes.Contains( pathArray[ i ] ) || !GUI.enabled) {
					continue;
				}

				highlight = !highlight;

				EditorGUILayout.BeginHorizontal( ( highlight ) ? highlightStyle : noStyle );

				//# This must be a directory...
				if ( i < pathArray.Length - 1 ) {
					pathNodes.Add( pathArray[ i ] );

					if ( !foldoutValues.ContainsKey( pathArray[ i ] ) ) {
						foldoutValues.Add( pathArray[ i ], true );
					}
					
					iterationIsDir = true;
				} else {
					iterationIsDir = false;
				}

				if ( !checkboxValues.ContainsKey( change.Path ) ) {
					checkboxValues.Add( change.Path, true );
				}

				GUILayout.Space( i * spacing );

				if ( !iterationIsDir ) {
					checkboxValues[ change.Path ] = GUILayout.Toggle( checkboxValues[ change.Path ], pathArray[ i ] );
					GUILayout.Label( "[" + change.Status + "]", statusStyle );

					if ( GUILayout.Button( "Diff", GUILayout.Width( 50 ) ) ) {
						Diff.init(change.Patch);
					}

				} else {
					foldoutValues[ pathArray[ i ] ] = EditorGUILayout.Foldout( foldoutValues[ pathArray[ i ] ], pathArray[ i ] );
					GUI.enabled = foldoutValues[ pathArray[ i ] ];
				}

				EditorGUILayout.EndHorizontal();
			}
		}

		public static Texture2D getGenericTexture( int width, int height, Color col ) {
			Color[] pix = new Color[ width * height ];

			for ( int i = 0; i < pix.Length; i++ ) {
				pix[ i ] = col;
			}

			Texture2D result = new Texture2D( width, height );

			result.SetPixels( pix );
			result.Apply();

			result.hideFlags = HideFlags.HideAndDontSave;
			result.hideFlags ^= HideFlags.NotEditable;

			return result;
		}
	}

}

