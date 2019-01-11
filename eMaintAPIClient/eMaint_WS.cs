using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eMaintAPI
{
	public class eMaint_WS
	{
		private string baseURL { get; set; }

		//public string wsURL { get; set; }
		public HttpStatusCode LastStatus { get; set; }
		public string LastResponse { get; set; }
		public string LastRequest { get; set; }
		public string LastURL { get; private set; }
		public string eMaint_XT_UserAgent { get; set; }
		public string eMaint_Authenticate { get; set; }
		public string emaint_URL { get; set; }
		//public string TableName { get; set; }

		public eMaint_WS(string in_eMaint_URL)
		{
			Uri url = new Uri(in_eMaint_URL);

			emaint_URL = url.Scheme + "://" + url.Host + "/wc.dll?x3~api~&q={MethodName}";
		}

		public eMaint_WS(string in_eMaint_URL, string in_eMaint_XT_UserAgent, string in_eMaint_Authenticate)
		: this(in_eMaint_URL)
		{
			this.eMaint_XT_UserAgent = in_eMaint_XT_UserAgent;
			this.eMaint_Authenticate = in_eMaint_Authenticate;
		}

		#region Select
		public dynamic[] SelectAll(string tableName, string[] columns = null, string[] orderBy = null, int pageSize = 500, int pageIndex = 1)
		{
			if (tableName == null)
				throw new ArgumentNullException("tableName");

			//this.TableName = tableName;

			if (columns == null)
				columns = new string[1] { "" };

			if (orderBy == null)
				orderBy = new string[1] { "" };


			string eMaintDataArray = "starting array";
			List<dynamic> results = new List<dynamic>();

			while (eMaintDataArray != "[]")
			{
				eMaintGetRecords records = new eMaintGetRecords(tableName, string.Join(",", columns), "", string.Join(",", orderBy), pageIndex, pageSize);
				string json_Records = JsonConvert.SerializeObject(records);

				results.AddRange(PerformSelect(tableName, json_Records, out eMaintDataArray));
				pageIndex += 1;
			}

			return results.ToArray();
		}

		public dynamic[] Select(string tableName, ICriteria criteria, string[] columns, string[] orderBy = null, int pageSize = 500, int pageIndex = 1)
		{
			if (tableName == null)
				throw new ArgumentNullException("tableName");

			if (criteria != null && columns == null)
				throw new ArgumentNullException("columns");

			//this.TableName = tableName;

			if (columns == null)
				columns = new string[1] { "" };

			if (orderBy == null)
				orderBy = new string[1] { "" };

			//if (criteria.GetType().Name == "Criteria")
			//	criteria = new CriteriaGroup(Relationship.And, criteria);
			
			//CriteriaGroup group = new CriteriaGroup(Relationship.And, criteria);

			string eMaintDataArray = "starting array";
			List<dynamic> results = new List<dynamic>();

			while (eMaintDataArray != "[]")
			{
				eMaintGetRecords records = new eMaintGetRecords(tableName, string.Join(",", columns), criteria.ToJSON(), string.Join(",", orderBy), pageIndex, pageSize);
				string json_Records = JsonConvert.SerializeObject(records);

				results.AddRange(PerformSelect(tableName, json_Records, out eMaintDataArray));
				pageIndex += 1;
			}

			return results.ToArray();
		}


		public List<dynamic> PerformSelect(string tableName, string jsonRequest, out string dataReturn)
		{
			HttpStatusCode statusCode = PerformRequest(emaint_URL, "GetAnyData", System.Net.Http.HttpMethod.Post, jsonRequest, out string result);

			if (statusCode == HttpStatusCode.OK)
			{
				string json = result.ToString();
				eMaintResult maintResult = JsonConvert.DeserializeObject<eMaintResult>(json);
				if (maintResult.message == "")
				{
					JObject jobject = JObject.Parse(json);
					JArray data = (JArray)jobject["data"];
					dataReturn = maintResult.data.ToString();
					return data.Select(x => new JsonApiObject(x, tableName)).ToList<dynamic>();
				}
			}

			dataReturn = "[]";
			return new List<dynamic>();
		}
		#endregion

		#region Update
		/// <summary>
		/// Updates a given record in the database. 
		/// </summary>
		/// <param name="record">The record to be updated, will be turned into a payload for the JSON string</param>
		/// <param name="RecordID">Required since eMaint doens't have standard named ID columns for the database</param>
		/// <returns></returns>
		public bool Update(JsonApiObject record, string RecordID)
		{
			if (record == null) throw new ArgumentNullException("record");

			eMaintRecord UpdateRecord = new eMaintRecord(record, record.Table, RecordID);

			HttpStatusCode statusCode = PerformRequest(emaint_URL, "Record", System.Net.Http.HttpMethod.Post, UpdateRecord.ToJSON(), out string result);

			if (statusCode == HttpStatusCode.OK && result != "")
			{
				return true;
			}
			else
				return false;
		}
		#endregion

		#region CreateDefault
		/// <summary>
		/// Returns an default blank row for a given table. This assumes you know all the default values to feed in.
		/// </summary>
		/// <param name="tableName">Name of table to create a blank row for</param>
		/// <returns>A blank record</returns>
		public dynamic CreateDefault(string tableName)
		{
			if (tableName == null)
				throw new ArgumentNullException("tableName");

			List<dynamic> results = new List<dynamic>();

			eMaintGetRecords records = new eMaintGetRecords(tableName, "", "", "", 1, 1);
			string json_Records = JsonConvert.SerializeObject(records);

			results.AddRange(PerformSelect(tableName, json_Records, out string eMaintDataArray));
			JsonApiObject result = results[0];

			foreach (string col in ((JsonApiObject)result).GetDynamicMemberNames())
			{
				result.Update(col, "");
			}

			return result;
		}

		#endregion

		#region Create
		public bool Create(JsonApiObject record)
		{
			if (record == null) throw new ArgumentNullException("record");

			eMaintRecord CreateRecord = new eMaintRecord(record, record.Table);

			HttpStatusCode statusCode = PerformRequest(emaint_URL, "Record", System.Net.Http.HttpMethod.Put, CreateRecord.ToJSON(), out string result);

			if (statusCode == HttpStatusCode.OK && result != "")
			{
				return true;
			}
			else
				return false;
		}
		#endregion

		#region Delete
		public bool Delete(JsonApiObject record, string recordID)
		{
			if (record == null) throw new ArgumentNullException("record");
			if (recordID == null) throw new ArgumentNullException("recordID");

			eMaintRecord DeleteRecord = new eMaintRecord(record.Table, recordID, eMaintRecord.Action.Delete);

			HttpStatusCode statusCode = PerformRequest(emaint_URL, "Record", System.Net.Http.HttpMethod.Delete, DeleteRecord.ToJSON(), out string result);

			if (statusCode == HttpStatusCode.OK && result != "")
			{
				return true;
			}
			else
				return false;
		}
		#endregion

		#region RestoreRecord
		public bool Restore(JsonApiObject record, string recordID)
		{
			if (record == null) throw new ArgumentNullException("record");

			eMaintRecord RestoreRecord = new eMaintRecord(record, record.Table, recordID, eMaintRecord.Action.Restore);

			HttpStatusCode statusCode = PerformRequest(emaint_URL, "Record", System.Net.Http.HttpMethod.Delete, RestoreRecord.ToJSON(), out string result);

			if (statusCode == HttpStatusCode.OK && result != "")
			{
				return true;
			}
			else
				return false;
		}
		#endregion

		#region PrivateHelpers
		private HttpStatusCode PerformRequest(string url, string methodName, System.Net.Http.HttpMethod requestMethod, string postJSON, out string result)
		{
			result = null;
			HttpStatusCode status = HttpStatusCode.BadRequest;
			string wsURL = url;
			wsURL = wsURL.Replace("{MethodName}", methodName);

			Uri requestURI = new Uri(wsURL);


			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestURI);
			req.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;

			SetEMaintAuthHeader(req, eMaint_XT_UserAgent, eMaint_Authenticate);

			//Set request method
			req.Method = requestMethod.ToString();

			//Set content types
			req.ContentType = "application/json";
			req.Accept = "application/json";

			byte[] byteArray = Encoding.UTF8.GetBytes(postJSON);
			req.ContentLength = byteArray.Length;

			Stream stream = req.GetRequestStream();
			stream.Write(byteArray, 0, byteArray.Length);
			stream.Close();

			try
			{
				HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
				using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
				{
					string RespStream = sr.ReadToEnd();
					result = RespStream;
					//result = JsonConvert.DeserializeObject<eMaintResult>(RespStream);
					status = resp.StatusCode;
				}
			}
			catch (WebException ex)
			{
				using (var exStream = ex.Response.GetResponseStream())
				using (var exReader = new StreamReader(exStream))
				{
					//result = exReader.ReadToEnd();
					result = null;
					status = HttpStatusCode.BadRequest;
				}
			}
			catch (Exception ex)
			{
				LastResponse = "Web service call failed with - " + ex;
			}

			this.LastURL = wsURL;
			this.LastStatus = status;
			this.LastRequest = postJSON;
			this.LastResponse = result;

			return status;
		}
		#endregion

		private void SetEMaintAuthHeader(WebRequest request, string XT_UserAgent, string Authenticate)
		{
			//String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + userPassword));
			//request.Headers.Add("Authorization", "Basic " + encoded);
			request.Headers.Add("XT-UserAgent", XT_UserAgent);
			request.Headers.Add("Authenticate", Authenticate);
			request.Headers.Add("DataFormat", "JSON");
		}

	}
}
