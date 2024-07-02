using System.Text;

namespace BedtimeCore.SteamUploader
{
	internal class VDF
	{
		public int AppID { get; set; }
		public string Description { get; set; }
		public string ContentRoot { get; set; }
		public string BuildOutput { get; set; }
		public bool Preview { get; set; }
		public Depot Depot { get; set; }
		
		public string SetLiveBranch { get; set; }
		
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"\"AppBuild\"");
			sb.AppendLine("{");
			sb.AppendLine($"\"AppID\" \"{AppID}\"");
			sb.AppendLine($"\"Preview\" \"{(Preview ? 1 : 0)}\"");
			sb.AppendLine($"\"Desc\" \"{Description}\"");
			sb.AppendLine($"\"ContentRoot\" \"{ContentRoot}\"");
			if(!string.IsNullOrEmpty(SetLiveBranch))
			{
				sb.AppendLine($"\"SetLive\" \"{SetLiveBranch}\"");
			}
			sb.AppendLine($"\"BuildOutput\" \"{BuildOutput}\"");
				
			sb.AppendLine($"\"Depots\"");
			sb.AppendLine("{");
			sb.AppendLine(Depot.ToString());
			sb.AppendLine("}");
			sb.AppendLine("}");
			
			return sb.ToString();
		}
	}
}