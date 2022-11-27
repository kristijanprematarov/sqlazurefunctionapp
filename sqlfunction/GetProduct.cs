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
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

namespace sqlfunction
{
    public static class GetProduct
    {
        [FunctionName("GetProducts")]
        public static async Task<IActionResult> RunProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            SqlConnection sqlConnection = GetConnection();

            List<Product> _productList = new List<Product>();

            string cmdText = "SELECT ProductID, ProductName, Quantity FROM Products";

            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand(cmdText, sqlConnection);

            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            ProductID = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            Quantity = reader.GetInt32(2)
                        };

                        _productList.Add(product);
                    }

                    sqlConnection.Close();
                    return new OkObjectResult(_productList);
                }
            }
            catch (Exception)
            {
                var response = "No records found";
                sqlConnection.Close();
                return new OkObjectResult(response);
            }
        }

        private static SqlConnection GetConnection()
        {
            string connectionString = "Server=tcp:kprsqlappserver.database.windows.net,1433;Initial Catalog=sqlappdb;Persist Security Info=False;User ID=krisprematarov;Password=KRISTIJANazure1!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            return new SqlConnection(connectionString);
        }

        [FunctionName("GetProduct")]
        public static async Task<IActionResult> RunProduct(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            SqlConnection sqlConnection = GetConnection();

            int productId = Convert.ToInt32(req.Query["id"]);

            string cmdText = $"SELECT TOP(1) ProductID, ProductName, Quantity FROM Products WHERE ProductID={productId}";

            sqlConnection.Open();

            SqlCommand cmd = new SqlCommand(cmdText, sqlConnection);
            var product = new Product();

            try
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    product.ProductID = reader.GetInt32(0);
                    product.ProductName = reader.GetString(1);
                    product.Quantity = reader.GetInt32(2);

                    sqlConnection.Close();

                    return new OkObjectResult(product);
                }
            }
            catch (Exception)
            {
                var response = "No record found";
                sqlConnection.Close();
                return new OkObjectResult(response);
            }
        }
    }
}
