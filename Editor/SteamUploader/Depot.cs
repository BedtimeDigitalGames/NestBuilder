using System.Text;

namespace BedtimeCore.SteamUploader
{
	internal class Depot
	{
		public Depot(int depotID, string localPath, string depotPath)
		{
			DepotID = depotID;
			LocalPath = localPath;
			DepotPath = depotPath;
		}

		public int DepotID { get; set; }

		public string LocalPath { get; set; }

		public string DepotPath { get; set; }

		public bool Recursive { get; set; } = true;

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"\"{DepotID}\"");
			sb.AppendLine("{");
			sb.AppendLine("\"FileMapping\"");
			sb.AppendLine("{");
			sb.AppendLine($"\"LocalPath\" \"{LocalPath}\"");
			sb.AppendLine($"\"DepotPath\" \"{DepotPath}\"");
			sb.AppendLine($"\"recursive\" \"{(Recursive ? "1" : "0") }\"");
			sb.AppendLine("}");
			sb.AppendLine("}");
			
			return sb.ToString();
		}
	}
}