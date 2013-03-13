using System.Linq;
using UnityEngine;
using UnityEditor;
using LibGit2Sharp;
using System.Collections.Generic;

namespace UniTEAM {
	public class UncommitedChangesWindow {
		private List<string> pathNodes = new List<string>();
		private Dictionary<string, bool> checkboxValues = new Dictionary<string, bool>();
		private Dictionary<string, bool> foldoutValues = new Dictionary<string, bool>();
		private GUIStyle statusStyle;
		private GUIStyle highlightStyle;
		private GUIStyle noStyle;
		private GUIStyle buttonStyle;

		private Texture2D highlightTexture;
		private Texture2D noTexture;

		public TreeChanges changes;
		public IEnumerable<string> untracked;
		public Rect rect;
		private Vector2 scroll;
		private string commitText = string.Empty;
		private TreeView treeView;

		public UncommitedChangesWindow() {
			highlightTexture = getGenericTexture( 1, 1, new Color( 71f / 255f, 71f / 255f, 71f / 255f ) );
			noTexture = getGenericTexture( 1, 1, new Color( 46f / 255f, 46f / 255f, 46f / 255f ) );

			statusStyle = new GUIStyle( "Label" );
			statusStyle.alignment = TextAnchor.LowerRight;

			highlightStyle = new GUIStyle( "Label" );
			highlightStyle.normal.background = highlightTexture;
			highlightStyle.padding.bottom = 2;

			noStyle = new GUIStyle( "Label" );
			noStyle.normal.background = noTexture;

			buttonStyle = new GUIStyle( GUI.skin.button );
			//buttonStyle.padding.bottom = 20;
		}

		public void reset( TreeChanges newChanges, Console console ) {
			changes = newChanges;
			untracked = console.repo.Index.RetrieveStatus().Untracked;
			treeView = new TreeView();
		}

		public void draw( Console console, int i ) {

			scroll = GUILayout.BeginScrollView( scroll );

			try {
				pathNodes.Clear();
				treeView.nodes.Clear();

				changes = changes ?? console.repo.Diff.Compare();

				foreach ( TreeEntryChanges change in changes ) {
					buildTreeView( change );
				}

				foreach ( string untrackedFile in untracked ) {
					buildTreeView( untrackedFile );
				}

				drawTreeView( console );
			}
			catch {}

			GUILayout.EndScrollView();

			GUILayout.Label( "Commit message:" );
			commitText = GUILayout.TextArea( commitText );
			if ( GUILayout.Button( "Commit Changes" ) ) {
				Signature signature = new Signature( "Jerome Doby", "xaerodegreaz@gmail.com", System.DateTimeOffset.Now );

				//# Stage everything
				string[] stage = new string[ checkboxValues.Count ];

				i = 0;
				foreach ( KeyValuePair<string, bool> pair in checkboxValues ) {
					if ( pair.Value ) {
						stage[ i ] = pair.Key;
						i++;
					}
				}

				stage = stage.Where( x => !string.IsNullOrEmpty( x ) ).ToArray();

				if ( stage.Length == 0 ) {
					console.currentError = "You cannot commit without staged items.";
					console.currentErrorLocation = rect;
				} else if ( commitText.Equals( string.Empty ) ) {
					console.currentError = "Please enter a commit message.";
					console.currentErrorLocation = rect;
				} else {
					console.repo.Index.Stage( stage );
					console.repo.Commit( commitText, signature );

					checkboxValues.Clear();
					foldoutValues.Clear();

					console.fetch();
				}

				commitText = string.Empty;
			}
		}

		private void drawTreeView( Console console ) {
			//# Loop through each node (folder)
			foreach ( KeyValuePair<string, TreeViewNode> treeViewNode in treeView.nodes ) {
				//# Add a foldup entry if there isn't already one
				if ( !foldoutValues.ContainsKey( treeViewNode.Value.name ) ) {
					foldoutValues.Add( treeViewNode.Value.name, true );
				}

				EditorGUILayout.BeginHorizontal( highlightStyle );

				foldoutValues[ treeViewNode.Value.name ] = EditorGUILayout.Foldout( foldoutValues[ treeViewNode.Value.name ], treeViewNode.Value.name );

				if ( treeViewNode.Value.items.All( delegate( TreeViewItem item ) { return item.status.Equals( "Untracked" ); } ) ) {
					if ( GUILayout.Button( "Ignore", buttonStyle, GUILayout.Width( 50 ) ) ) {
						console.repo.Ignore.AddPermanentRules( new string[] {
							treeViewNode.Value.name
						} );
					}
				}

				EditorGUILayout.EndHorizontal();

				//# If the foldup is folded, then just continue to the next iteration and don't show children
				if ( !foldoutValues[ treeViewNode.Value.name ] ) {
					continue;
				}

				//# Loop through each actual file in this node
				foreach ( TreeViewItem treeViewItem in treeViewNode.Value.items ) {
					//# Add checkbox entry if not already there. Untracked files are unchecked by default.
					if ( !checkboxValues.ContainsKey( treeViewItem.path ) ) {
						checkboxValues.Add( treeViewItem.path, !treeViewItem.status.Equals( "Untracked" ) );
					}

					EditorGUILayout.BeginHorizontal( noStyle );
					GUILayout.Space( 15f );

					checkboxValues[ treeViewItem.path ] = GUILayout.Toggle( checkboxValues[ treeViewItem.path ], treeViewItem.name );

					GUILayout.Label( "[" + treeViewItem.status + "]", statusStyle );

					//# Button for launching a diff instance.
					if ( GUILayout.Button( "Diff", GUILayout.Width( 50 ) ) ) {
						Diff.init( treeViewItem.patchDiff );
					}

					if ( treeViewItem.status.Equals( "Untracked" ) ) {
						if ( GUILayout.Button( "Ignore", GUILayout.Width( 50 ) ) ) {
							Debug.Log( console.repo.Info.Path );
							console.repo.Ignore.AddPermanentRules( new string[] {
								treeViewItem.path
							} );
							console.Repaint();
						}
					}

					EditorGUILayout.EndHorizontal();
				}
			}
		}

		private void buildTreeView( TreeEntryChanges change ) {
			int index = change.Path.LastIndexOf( "\\" );
			string folder = ( index >= 0 ) ? change.Path.Substring( 0, index ) : "\\";

			TreeViewNode node = new TreeViewNode( folder.Trim() );
			TreeViewItem item = new TreeViewItem( change );

			treeView.tryAdd( node ).tryAdd( item );
		}

		private void buildTreeView( string change ) {
			int index = change.LastIndexOf( "\\" );
			string folder = ( index >= 0 ) ? change.Substring( 0, index ) : "\\";

			TreeViewNode node = new TreeViewNode( folder.Trim() );
			TreeViewItem item = new TreeViewItem( change );

			treeView.tryAdd( node ).tryAdd( item );
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
