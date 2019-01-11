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