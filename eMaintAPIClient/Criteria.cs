using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace eMaintAPI
{
	public enum Operator
	{
		Equals,
		NotEquals,
		StartsWith,
		NotStartsWith,
		EndsWith,
		NotEndsWith,
		Contains,
		NotContains,
		In,
		NotIn,
		GreaterThan,
		GreaterThanOrEqualTo,
		LessThan,
		LessThanOrEqualTo
	}

	public enum Relationship
	{
		And,
		Or
	}


	public class Criteria : ICriteria
	{
		public string field { get; set; }
		public string @operator { get; set; }
		public string entryas { get; set; }
		public string value { get; set; }

		private Operator Operator { get; set; }

		private Dictionary<string, string> ops = new Dictionary<string, string> {
			{ "Equals", "eq" }
			, { "NotEquals", "neq" }
			, {"StartsWith", "startswith" }
			, {"GreaterThan", "gt"}
			, {"LessThan","lt" }
			, {"GreaterThanOrEqualTo", "gte" }
			, {"LessThanOrEqualTo", "lte" }
			, {"NotStartsWith","notstartswith" }
			, {"In", "containany"}
			, {"Contains", "contains"}
			, {"EndsWith", "endswith"}
		};

		public static Criteria Equals(string field, string value)
		{
			return new Criteria(field, Operator.Equals, value);
		}

		public Criteria(string field, Operator op, string value)
		{
			this.field = field;
			this.@operator = ops[op.ToString()];
			this.value = value;
		}

		public Criteria(string field, string value)
			: this(field, Operator.Equals, value)
		{
		}

		public Criteria(string field, Operator op, string value, string entryas)
		{
			this.field = field;
			this.@operator = op.ToString();
			this.value = value;
			this.entryas = entryas;
		}


		public string ToJSON()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.NullValueHandling = NullValueHandling.Ignore;

			return JsonConvert.SerializeObject(this, settings);
		}
	}

	public class CriteriaGroup : ICriteria
	{
		public string logic { get; set; }
		//public Relationship Relationship { get; set; }
		public List<ICriteria> filters { get; private set; }
		public string ToJSON()
		{
			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.NullValueHandling = NullValueHandling.Ignore;

			string json = JsonConvert.SerializeObject(this, settings);
			//json = json.TrimStart('{');
			//json = json.TrimEnd('}');
			return json;
		}

		public CriteriaGroup(Relationship relationship, params ICriteria[] criteria)
		{
			this.filters = new List<ICriteria>();
			//this.Relationship = relationship;
			this.logic = relationship.ToString();
			this.filters.AddRange(criteria);
		}
	}

	public interface ICriteria
	{
		string ToJSON();
	}
}
