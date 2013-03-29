using LibGit2Sharp;

namespace UniTEAM {
	public class CommitItem {
		private System.DateTimeOffset d;
		private string hour;
		private string minute;
		private string second;

		public string commitMessage;
		public string dateString;

		public CommitItem( Commit commit ) {
			d = commit.Author.When;
			//commitMessage = commit.Message.Split( "\r\n".ToCharArray() )[ 0 ];
			commitMessage = commit.MessageShort;
			hour = ( d.Hour.ToString().Length == 1 ) ? "0" + d.Hour : d.Hour.ToString();
			minute = ( d.Minute.ToString().Length == 1 ) ? "0" + d.Minute : d.Minute.ToString();
			second = ( d.Second.ToString().Length == 1 ) ? "0" + d.Second : d.Second.ToString();
			dateString = d.Month + "/" + d.Day + "/" + d.Year + " " + hour + ":" + minute + ":" + second;
		}
	}
}
