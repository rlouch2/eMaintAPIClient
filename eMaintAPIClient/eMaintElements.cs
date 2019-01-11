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
		public string sortBy { get; set; }
		public int pageNumber { get; set; }
		public int pageSize { get; set; }

		/// <summary>
		/// Holding space for the eMaint return JSON before beginning processing
		/// </summary>
		/// <param name="in_table"></param>
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
			sortBy = in_sortBy;
			pageNumber = in_pageNumber;
			pageSize = in_pageSize;
		}
	}
}