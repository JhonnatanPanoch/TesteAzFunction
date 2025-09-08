using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;


namespace TesteAzFunction
{
    public class Function2
    {
        private readonly ILogger<Function2> _logger;

        public Function2(ILogger<Function2> logger)
        {
            _logger = logger;
        }

        [Function("Function2")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "getLote")] HttpRequest req,
            ILogger log)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string id = req.Query["id"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            id = id ?? data?.id;


            string connectionString = "Server=localhost;Port=5432;Database=teste;User Id=postgres;Password=postgres;";

            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            var query = $"SELECT * FROM Lote WHERE id = {id}";
            var result = connection.QueryFirstOrDefault(query);
            connection.Close();


            return new OkObjectResult(result ?? "Lote não processado ou não existente!");
        }
    }
}
