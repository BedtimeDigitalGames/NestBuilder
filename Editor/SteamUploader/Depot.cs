using System;
using System.Text;

namespace BedtimeCore.SteamUploader
{
	internal class Depot
	{
		public Depot(int depotID, string localPath, string depotPath, string[] fileExclusions = null)
		{
			DepotID = depotID;
			LocalPath = localPath;
			DepotPath = depotPath;
			FileExclusions = fileExclusions ?? Array.Empty<string>();
		}

		public int DepotID { get; set; }

		public string LocalPath { get; set; }

		public string DepotPath { get; set; }

		public bool Recursive { get; set; } = true;
        
        public string[] FileExclusions { get; set; }

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine($"{Quote(DepotID)}");
			sb.AppendLine("{");
			sb.AppendLine($"{Quote("FileMapping")}");
			sb.AppendLine("{");
			sb.AppendLine($"{Quote("LocalPath")} {Quote(LocalPath)}");
			sb.AppendLine($"{Quote("DepotPath")} {Quote(DepotPath)}");
			sb.AppendLine($"{Quote("recursive")} {Quote(Recursive ? 1 : 0)}");
			sb.AppendLine("}");
            
            foreach (var exclusion in FileExclusions)
            {
                sb.AppendLine($"{Quote("FileExclusion")} {Quote(exclusion)}");
            }
            
			sb.AppendLine("}");
			
			return sb.ToString();
		}
        
        private static string Quote(object obj) => VDF.Quote(obj);
	}
}