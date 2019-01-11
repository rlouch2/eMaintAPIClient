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
	public class JsonApiObject : System.Dynamic.DynamicObject
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

		//public void Add(string name, string value = null)
		//{
		//	if (!((JObject)m_token).ContainsKey(name))
		//	{
		//		((JObject)m_token).Add(name, new JToken.)
		//	}
		//}


		public void Update(string name, string value = null)
		{
			if (((JObject)m_token).ContainsKey(name))
			{
				m_token[name] = value;
			}
		}

		/// <summary>
		/// Gets an Xml fragment of all of the fields that have been changed since they were loaded, including in sub-tables
		/// </summary>
		//internal XElement ReduceToChanges()
		//{
		//	XElement result = new XElement(m_element.Name);
		//	RecursiveLookForChanges(result, m_element);
		//	return result;
		//}

		//internal JToken ReduceToChanges()
		//{
		//	//JToken result = new JObject(m_token);
		//	List<JProperty> removeProperties = new List<JProperty>();
		//	foreach (JProperty prop in ((JObject)m_token).Properties())
		//	{
		//		if (prop.Value == m_OriginalToken[prop.Name])
		//		{
		//			removeProperties.Add(prop);
		//			//prop.Remove();
		//		}
		//	}

		//	foreach (JProperty prop in removeProperties)
		//		prop.Remove();

		//	return m_token;
		//}

		/// <summary>
		/// Clears the changed flag from all elements so we can start tracking again
		/// </summary>
		//internal void ClearChanges()
		//{
		//	RecursiveClearChanges(m_element);
		//}

		//private void RecursiveClearChanges(XElement start)
		//{
		//	foreach (XElement element in start.Elements())
		//	{
		//		if (element.Attribute("changed") != null)
		//		{
		//			element.Attribute("changed").Remove();
		//		}
		//		else if (element.HasElements)
		//		{
		//			RecursiveClearChanges(element);
		//		}
		//	}
		//}

		//private void RecursiveLookForChanges(XElement result, XElement start)
		//{
		//	foreach (XElement element in start.Elements())
		//	{
		//		if (element.Attribute("changed") != null)
		//		{
		//			result.Add(new XElement(element.Name, element.Value));
		//		}
		//		else if (element.HasElements)
		//		{
		//			XElement child = new XElement(element.Name);
		//			RecursiveLookForChanges(child, element);
		//			if (child.HasElements)
		//			{
		//				// if its a sub-table, we need to attach the ID as an attribute, so the service knows which record to update
		//				child.Add(new XAttribute(child.Name + "ID", element.Elements(child.Name + "ID").First().Value));
		//				result.Add(child);
		//			}
		//		}
		//	}
		//}



		#endregion Methods
	}

}
