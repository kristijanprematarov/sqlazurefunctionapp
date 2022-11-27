using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using sqlfunction.Models;
using System.Data;

namespace sqlfunction
{
    public static class AddProduct
    {
        private static SqlConnection GetConnection()
        {
            string connectionString = "Server=tcp:kprsqlappserver.database.windows.net,1433;Initial Catalog=sqlappdb;Persist Security Info=False;User ID=krisprematarov;Password=KRISTIJANazure1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            return new SqlConnection(connectionString);
        }

        [FunctionName("AddProduct")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            SqlConnection sqlConnection = GetConnection();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Product product = JsonConvert.DeserializeObject<Product>(requestBody);

            string cmdText = $"INSERT INTO Products(ProductID, ProductName, Quantity) VALUES (@param1,@param2,@param3)";

            sqlConnection.Open();

            using (SqlCommand cmd = new SqlCommand(cmdText, sqlConnection))
            {
                cmd.Parameters.Add("@param1", SqlDbType.Int).Value = product.ProductID;
                cmd.Parameters.Add("@param2", SqlDbType.VarChar, 1000).Value = product.ProductName;
                cmd.Parameters.Add("@param3", SqlDbType.Int).Value = product.Quantity;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }

            return new OkObjectResult($"Product added");
        }
    }
}
