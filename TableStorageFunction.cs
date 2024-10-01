using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace TableStorageFunction
{
    public class TableStorageFunction
    {
        // Declare a private TableClient object to interact with Azure Table Storage
        private readonly TableClient _tableClient;
        private readonly ILogger<TableStorageFunction> _logger; // Add a logger field

        public TableStorageFunction(ILogger<TableStorageFunction> logger)
        {
            _logger = logger; // Initialize the logger
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient("Product");
            _tableClient.CreateIfNotExists();
        }

        [Function("AddProductFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("AddProductFunction processed a request for a product");

            // Read the request body to get the product data
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Product product = JsonConvert.DeserializeObject<Product>(requestBody);

            // Check if the product was deserialized correctly
            if (product == null)
            {
                return new BadRequestObjectResult("Invalid product data.");
            }

            // Add product to the table storage
            await _tableClient.AddEntityAsync(product); // Use the instance's TableClient

            string responseMessage = $"Product {product.Name} added successfully.";
            return new OkObjectResult(responseMessage);
        }
    }

    // Define the Product class implementing ITableEntity
    public class Product : ITableEntity
    {
        

        // Ensure the property names match what you are setting in the controller

        public int Product_Id { get; set; }
        public string Name { get; set; }
       
        public double Price { get; set; }
        public string Category { get; set; }
        public string Artist { get; set; }
        public string Size { get; set; }
        public string ImageUrl { get; set; }  // Add this if you want to store the image URL

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }



}


