
using Azure.Data.Tables;
using Microsoft.Azure. Functions. Worker;
using Microsoft.Azure. Functions. Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

public class TableStorageFunction
{
	// Declare a private TableClient object to interact with Azure Table Storage
	private readonly TableClient _tableClient;

	public TableStorageFunction()
	{
		// Get the Azure Storage connection string from environment variables
		string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
		
		// Create a service client to interact with the Table Service in the Azure Storage account
		var serviceClient = new TableServiceClient(connectionString);
		
		// Initialize the TableClient to interact with a specific table
		_tableClient = serviceClient.GetTableClient("Product");
		
		// Create the table if it doesn't already exist 
		_tableClient.CreateIfNotExists();
		
		// Function that handles HTTP POST requests to store data in Table Storage
	}

	[Function("StoreToTable")]
	public async Task Run([HttpTrigger(AuthorizationLevel. Function, "post")] HttpRequestData req, FunctionContext executionContext)
	{
		// Get a logger instance to log information about the function's execution
		var Logger = executionContext.GetLogger("StoreToTable");
	
		// Log that the function is starting the process of storing data to Table Storage
		Logger.LogInformation("Storing data to Azure Table Storage...");
		
		// Create a new TableEntity with a specified Partitionkey and Rowkey
		var entity = new TableEntity("PartitionKey", "Rowkey")
		{
			{"PropertyName", "PropertyValue" } // Add custom properties to the entity
		};

		// Add the entity to the Azure Table Storage asynchronously
		await _tableClient.AddEntityAsync(entity);
		var response = req. CreateResponse(HttpStatusCode.OK);
		await response.WriteStringAsync("Data stored successfully.");

	}

}

