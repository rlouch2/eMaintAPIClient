using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace eMaintAPI
{
	/// <summary>
	/// Holding space for the eMaint record data to be sne t
	/// </summary>
	class eMaintRecord
	{
		public string id { get; set; }

		public string table { get; set; }

		public dynamic payload { get; set; }

		public string action { get; set; }

		public enum Action
		{
			Delete,
			Restore
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="payload">Table row to be worked on</param>
		/// <param name="tableName">Table to be worked on</param>
		/// <param name="id">Row id to be worked on</param>
		/// <param name="action">Action to be completed if a DELETE/RESTORE operation</param>
		public eMaintRecord(dynamic payload, string tableName, string id = null, Action? action = null)
		{
			this.payload = payload;
			this.id = id;
			this.table = tableName;
			this.action = action?.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName">Table to be worked on</param>
		/// <param name="id">Row id to be worked on</param>
		/// <param name="action">Action to be completed if a DELETE/RESTORE operation</param>
		public eMaintRecord(string tableName, string id, Action action)
		{
			this.payload = payload;
			this.id = id;

			this.action = action.ToString();
		}


		public string ToJSON()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.NullValueHandling = NullValueHandling.Ignore;

			string json = JsonConvert.SerializeObject(this, settings);
			return json;
		}
	}
}
