using System.Collections.Generic;
using System.IO;
using LibGit2Sharp;
using UnityEngine;

namespace UniTEAM {
	public class TreeView {
		public SortedDictionary<string, TreeViewNode> nodes = new SortedDictionary<string, TreeViewNode>();

		public TreeViewNode tryAdd( TreeViewNode node ) {
			try {
				nodes.Add( node.name, node );
				return node;
			} catch {
				return nodes[ node.name ];
			}
		}
	}

	public class TreeViewNode {
		public string name;
		public List<TreeViewItem> items = new List<TreeViewItem>();

		public TreeViewNode( string name ) {
			this.name = name;
		}

		public void tryAdd( TreeViewItem item ) {
			try {
				items.Add( item );
			} catch ( System.Exception e ) {
				Debug.Log( e );
			}
		}
	}

	public class TreeViewItem {
		public string name;
		public string path;
		public string status;
		public string patchDiff;

		public TreeViewItem( TreeEntryChanges change ) {
			name = new FileInfo( change.Path ).Name;
			path = change.Path;
			status = change.Status.ToString();
			patchDiff = change.Patch;
		}

		public TreeViewItem( string path ) {
			this.path = path;
			name = new FileInfo( path ).Name;
			status = "Untracked";
			patchDiff = "New file";
		}
	}
}
