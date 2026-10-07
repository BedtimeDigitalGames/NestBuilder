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
            
			sb.AppendLine($"{Quote("AppBuild")}");
			sb.AppendLine("{");
            sb.AppendLine($"{Quote("AppID")} {Quote(AppID)}");
			sb.AppendLine($"{Quote("Preview")} {Quote(Preview ? 1 : 0)}");
			sb.AppendLine($"{Quote("Desc")} {Quote(Description)}");
			sb.AppendLine($"{Quote("ContentRoot")} {Quote(ContentRoot)}");

			if(!string.IsNullOrEmpty(SetLiveBranch))
			{
				sb.AppendLine($"{Quote("SetLive")} {Quote(SetLiveBranch)}");
			}
			sb.AppendLine($"{Quote("BuildOutput")} {Quote(BuildOutput)}");
				
			sb.AppendLine($"{Quote("Depots")}");
			sb.AppendLine("{");
			sb.AppendLine(Depot.ToString());
			sb.AppendLine("}");
			sb.AppendLine("}");
			
			return sb.ToString();
		}

        internal static string Quote(object value)
        {
            return $"\"{value}\"";
        }
	}
}