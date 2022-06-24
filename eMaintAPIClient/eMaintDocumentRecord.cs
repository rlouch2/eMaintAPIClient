using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace eMaintAPI
{
	public class eMaintDocumentRecord
	{
		public string filename { get; set; }
		public string description { get; set; }
		public string folder { get; set; }
		public string data { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename">Document file name</param>
		/// <param name="description">what is this document</param>
		/// <param name="folder">where will this document go</param>
		/// <param name="data">Base64 string of data</param>
		public eMaintDocumentRecord(string filename, string description, string folder, string data)
		{
			this.filename = filename;
			this.description = description;
			this.folder = folder;
			this.data = data;
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
