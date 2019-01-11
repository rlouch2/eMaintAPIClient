using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMaintAPI
{
	class eMaintResult
	{
		public string valid
		{
			get; set;
		}
		public string message { get; set; }

		public object data { get; set; }
	}
}
