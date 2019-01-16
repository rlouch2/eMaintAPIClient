using System;
using System.Collections.Generic;
using System.Text;

namespace eMaintAPI
{
	class eMaintGetRecords
	{
		public string table { get; set; }
		public string columns { get; set; }
		public string filter { get; set; }
		public List<eMaintSortBy> sortBy { get; set; }
		public int pageNumber { get; set; }
		public int pageSize { get; set; }

		/// <summary>
		/// Holding space for the eMaint return JSON before beginning processing
		/// </summary>
		/// <param name="in_table">Table Name</param>
		/// <param name="in_columns"></param>
		/// <param name="in_filter"></param>
		/// <param name="in_sortBy"></param>
		/// <param name="in_pageNumber"></param>
		/// <param name="in_pageSize"></param>
		public eMaintGetRecords(string in_table, string in_columns, string in_filter, string in_sortBy, int in_pageNumber, int in_pageSize)
		{
			table = in_table;
			columns = in_columns;
			filter = in_filter;
			sortBy = new List<eMaintSortBy>();
			foreach (string sort in in_sortBy.Split(','))
			{
				string[] order = sort.Split(' ');
				string field = order[0];
				string dir = (order.Length > 1) ? order[1] : "ASC";
				sortBy.Add(new eMaintSortBy(field, dir));
			}
			pageNumber = in_pageNumber;
			pageSize = in_pageSize;
		}
	}


	class eMaintSortBy
	{
		public string field { get; set; }
		public string dir { get; set; }

		public eMaintSortBy(string field, string dir)
		{
			this.field = field;
			this.dir = dir;
		}
	}
}