using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using LibGit2Sharp;
using System.Linq;

namespace UniTEAM {
	public class ChangesetViewWindow {
		public Rect rect;
		private Vector2 scroll = Vector2.zero;
		private Dictionary<string, bool> checkboxValues = new Dictionary<string, bool>();
		private Dictionary<string, bool> foldoutValues = new Dictionary<string, bool>();
		private TreeView treeView;
		private List<string> pathNodes = new List<string>();
		public TreeChanges changes;
		private GUIStyle statusStyle;
		private GUIStyle highlightStyle;
		private GUIStyle noStyle;

		private Texture2D highlightTexture;
		private Texture2D noTexture;

		public ChangesetViewWindow() {
			highlightTexture = UncommitedChangesWindow.getGenericTexture( 1, 1, new Color( 71f / 255f, 71f / 255f, 71f / 255f ) );
			noTexture = UncommitedChangesWindow.getGenericTexture( 1, 1, new Color( 46f / 255f, 46f / 255f, 46f / 255f ) );

			statusStyle = new GUIStyle( "Label" );
			statusStyle.alignment = TextAnchor.LowerRight;

			highlightStyle = new GUIStyle( "Label" );
			highlightStyle.normal.background = highlightTexture;

			noStyle = new GUIStyle( "Label" );
			noStyle.normal.background = noTexture;

			treeView = new TreeView();
		}

		public void reset( TreeChanges newChanges, Console console ) {
			changes = newChanges;
			treeView = new TreeView();
		}

		public void reset( System.Exception e ) {
			GUILayout.Label( "This commit has no parent to compare to, perhaps it's deleted, or is very root?" );
			GUILayout.Label( "Exception: " + e );
		}

		public void draw( Console console, int i ) {
			pathNodes.Clear();
			treeView.nodes.Clear();

			scroll = GUILayout.BeginScrollView( scroll );

			if ( changes != null ) {
				foreach ( TreeEntryChanges change in changes ) {
					buildTreeView( change );
				}
			}

			drawTreeView();

			GUILayout.EndScrollView();
		}

		private void drawTreeView() {
			//# Loop through each node (folder)
			foreach ( KeyValuePair<string, TreeViewNode> treeViewNode in treeView.nodes ) {
				//# Add a foldup entry if there isn't already one
				if ( !foldoutValues.ContainsKey( treeViewNode.Value.name ) ) {
					foldoutValues.Add( treeViewNode.Value.name, true );
				}

				EditorGUILayout.BeginHorizontal( highlightStyle );

				foldoutValues[ treeViewNode.Value.name ] = EditorGUILayout.Foldout( foldoutValues[ treeViewNode.Value.name ], treeViewNode.Value.name );

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

					GUILayout.Label( treeViewItem.name );

					GUILayout.Label( "[" + treeViewItem.status + "]", statusStyle );

					//# Button for launching a diff instance.
					if ( GUILayout.Button( "Diff", GUILayout.Width( 50 ) ) ) {
						Diff.init( treeViewItem.patchDiff );
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
	}
}
