using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Storage.Tables.Entities
{
	class CustomerUS : TableEntity
	{
		public string Name { get; set; }
		public string Email { get; set; }

		public CustomerUS(string name, string email)
		{
			Name = name;
			Email = email;
			PartitionKey = "US";
			RowKey = email;
		}

		public CustomerUS() { }
	}
}
