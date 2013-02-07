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
	public class UncommitedChangesWindow {

		public static Rect rect;
		public static Vector2 scroll;
		public static List<string> pathNodes = new List<string>();
		public static TreeChanges changes;
		public static GUIStyle statusStyle;
		public static GUIStyle highlightStyle;
		private static GUIStyle noStyle;

		private static Texture2D highlightTexture = getGenericTexture( 1, 1, new Color( 71f / 255f, 71f / 255f, 71f / 255f ) );
		private static Texture2D noTexture = getGenericTexture( 1, 1, new Color( 46f / 255f, 46f / 255f, 46f / 255f ) );

		public static void draw( int i ) {
			bool highlight = true;

			statusStyle = new GUIStyle( GUI.skin.label );
			statusStyle.alignment = TextAnchor.LowerRight;

			highlightStyle = new GUIStyle( GUI.skin.box );
			highlightStyle.normal.background = highlightTexture;

			noStyle = new GUIStyle( GUI.skin.box );
			noStyle.normal.background = noTexture;

			pathNodes.Clear();

			scroll = GUILayout.BeginScrollView( scroll );
			foreach ( TreeEntryChanges change in changes ) {
				recurseToAssetFolder( change, ref highlight );
			}
			GUILayout.EndScrollView();

			GUI.skin.label.padding = new RectOffset( 0, 0, 0, 0 );
		}

		private static void recurseToAssetFolder( TreeEntryChanges change, ref bool highlight) {
			int spacing = 20;
			string[] pathArray = change.Path.Split( "\\".ToCharArray() );

			for ( int i = 0; i < pathArray.Length; i++ ) {

				if ( pathNodes.Contains( pathArray[ i ] ) ) {
					
					continue;
				}

				highlight = !highlight;
	
				//# This must be a directory...
				if ( i < pathArray.Length - 1 ) {
					pathNodes.Add( pathArray[i] );
				}

				GUI.skin.label.padding = new RectOffset( i * spacing, 0, 0, 0 );

				GUILayout.BeginHorizontal( (highlight) ? highlightStyle : noStyle );

				string child = ( i > 0 ) ? "^  " : "";

				GUILayout.Label( child + pathArray[ i ] );
				GUILayout.Label("[" + change.Status + "]", statusStyle);
				GUILayout.EndHorizontal();
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

			return result;
		}
	}

}

