using Azure.Storage.Tables.Entities;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Storage.Tables
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				#region Creating the Table Storage

				CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
					CloudConfigurationManager.GetSetting("StorageConnection"));

				var tableClient = storageAccount.CreateCloudTableClient();

				var table = tableClient.GetTableReference("customers");

				table.CreateIfNotExists();

				#endregion

				#region Inserting Entities to the Table Storage

				//CreateCustomer(table, new CustomerUS("Jorge Apodaca", "japodaca@bts.com.mx"));
				//CreateCustomer(table, new CustomerUS("Gerardo Luque", "gluque@bts.com.mx"));

				#endregion

				#region Getting one customer from Table Storage

				//Console.WriteLine("{0}", GetCustomer(table, "US", "japodaca@bts.com.mx").Name);

				#endregion

				#region Get all customers from Table Storage

				//GetAllCustomers(table);

				#endregion

				#region Update an existing customer in Table Storage

				//var customer = GetCustomer(table, "US", "japodaca@bts.com.mx");

				//customer.Name = "Jorge Mario";

				//UpdateCustomer(table, customer);

				//GetAllCustomers(table);

				#endregion

				#region Delete a customer from Table Storage

				//var customer = GetCustomer(table, "US", "gluque@bts.com.mx");

				//DeleteCustomer(table, customer);

				//GetAllCustomers(table);

				#endregion

				#region Execute Batch Commands to Table Operations

				//ExecuteBatchOperations(table);

				//GetAllCustomers(table);

				#endregion

				Console.ReadKey();

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Console.ReadKey();
			}
		}

		static void ExecuteBatchOperations(CloudTable table)
		{
			TableBatchOperation batch = new TableBatchOperation();

			var customer1 = new CustomerUS("Manuel", "mlopez@bts.com.mx");
			var customer2 = new CustomerUS("Raul", "rjimenez@bts.com.mx");
			var customer3 = new CustomerUS("Samantha", "srodriguez@bts.com.mx");

			batch.Insert(customer1);
			batch.Insert(customer2);
			batch.Insert(customer3);

			var customer = GetCustomer(table, "US", "japodaca@bts.com.mx");

			DeleteCustomer(table, customer);

			table.ExecuteBatch(batch);
		}

		static void UpdateCustomer(CloudTable table, CustomerUS customer)
		{
			TableOperation update = TableOperation.Replace(customer);

			table.Execute(update);
		}

		static void DeleteCustomer(CloudTable table, CustomerUS customer)
		{
			TableOperation delete = TableOperation.Delete(customer);

			table.Execute(delete);
		}

		static void CreateCustomer(CloudTable table, CustomerUS customer)
		{
			TableOperation insert = TableOperation.Insert(customer);

			table.Execute(insert);
		}

		static CustomerUS GetCustomer(CloudTable table, string partitionkey, string rowkey)
		{
			TableOperation retrieve = TableOperation.Retrieve<CustomerUS>(partitionkey, rowkey);
			var result = table.Execute(retrieve);
			return (CustomerUS)result.Result;
		}

		static void GetAllCustomers(CloudTable table)
		{
			TableQuery<CustomerUS> query = new TableQuery<CustomerUS>()
				.Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "US"));

			foreach (var item in table.ExecuteQuery(query))
			{
				Console.WriteLine("Name: {0}, Email: {1}", item.Name, item.Email);
			}
		}
	}
}
