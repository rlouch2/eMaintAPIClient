using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace eMaintAPI
{
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

		public eMaintRecord(dynamic payload, string tableName, string id = null, Action? action = null)
		{
			this.payload = payload;
			this.id = id;
			this.table = tableName;
			this.action = (action != null) ? action.ToString() : null;
		}

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
