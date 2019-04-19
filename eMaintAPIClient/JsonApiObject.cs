using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml.Linq;

using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace eMaintAPI
{
	public class JsonApiObject : DynamicObject
	{
		#region Declarations

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private JToken m_token;

		#endregion Declarations

		#region Properties

		/// <summary>
		/// Gets the name of the source table for the record that this object represents
		/// </summary>
		public string Table { get; set; }

		public int ColumnCount
		{
			get
			{
				return ((JObject)m_token).Properties().Count();
			}
		}

		#endregion Properties

		#region Constructor

		internal JsonApiObject(JToken token, string tableName)
		{
			if (token == null) throw new ArgumentNullException("token");
			Table = tableName;
			m_token = token;
		}

		#endregion Constructor

		#region DynamicObject Overrides

		/// <summary>
		/// Determines whether the specified field exists in this object.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public bool HasField(string name)
		{
			return m_token.Parent[name].Any();
			//return m_element.Elements(name).Any();
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			int count = m_token[binder.Name].Parent.Count();

			if (count >= 1 && m_token[binder.Name].HasValues)
			{
				result = m_token[binder.Name].ToString();
				return true;
			}
			else if (count == 1)
			{
				result = m_token[binder.Name].ToString();
				return true;
			}

			return base.TryGetMember(binder, out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			m_token[binder.Name] = (string)value;
			return true;

			//return base.TrySetMember(binder, value);
		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			IList<string> keys = ((JObject)m_token).Properties().Select(p => p.Name).ToList();
			return keys;
		}

		#endregion DynamicObject Overrides

		private void UpdateValue(string name, object value)
		{
			m_token[name] = (string)value;
		}

		#region Methods

		public void Update(string name, string value = null)
		{
			if (((JObject)m_token).ContainsKey(name))
			{
				m_token[name] = value;
			}
		}

		#endregion Methods
	}
}
