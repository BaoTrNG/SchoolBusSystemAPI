using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using SchoolBusApi.Models;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Newtonsoft.Json.Linq;
using System.Text.Json;


using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Data.SqlClient;

namespace SchoolBusApi.data
{
    public class PayPalService : DbContext
    {
        private string _clientId;
        private string _clientSecret;
        private string authString;
        private string encodedAuthString;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public string ConnectionString { get; set; }

        public class Link
        {
            public string href { get; set; }
            public string rel { get; set; }
            public string method { get; set; }
        }

        public class CheckoutOrderResponse
        {
            public string id { get; set; }
            public string status { get; set; }
            public Link[] links { get; set; }
        }




        public PayPalService(DbContextOptions<PayPalService> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            ConnectionString = options.FindExtension<SqlServerOptionsExtension>().ConnectionString;
            _httpContextAccessor = httpContextAccessor;

            _clientId = "AaBupsZOXI_54s2Bx3g3QZf7cJANqUbZOotwcSPuUhjXRbvto2gxdUzlLrEOOl0-p5f5UB0d3saYw1yW";
            _clientSecret = "EKiynJJ-iuES2bMYafx_-FuMaf_ADJuYU8nxr1uafBGvHWhcjuRQ1j08bxe1C49LjaATZx6b36aIWY5G";
            authString = _clientId + ":" + _clientSecret;
            encodedAuthString = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authString));
        }

        public async Task HandlePayPalWebhook()
        {
            var context = _httpContextAccessor.HttpContext;
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            try
            {
                var requestData = JsonSerializer.Deserialize<dynamic>(requestBody);

                // Log the request body for debugging purposes
                Console.WriteLine("Request body: " + requestBody);

                // Handle the PayPal webhook event
                // ...

                // Save changes to the database
                await SaveChangesAsync();

                // Return a success response to PayPal
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("Webhook received successfully");
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Error deserializing JSON: " + ex.Message);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Error deserializing JSON: " + ex.Message);
            }
        }

        /* public async Task<String> Test2(response res, string bill_id, double value)
         {
             string today = DateTime.Now.ToString("yyyy-MM-dd");
             DateTime today2 = DateTime.Today;
             DateTime futureDate = today2.AddDays(10);
             string futureDateString = futureDate.ToString("yyyy-MM-dd");
             try
             {
             }
             catch(Exception e)
             {
                 return 
             }
          }*/

        public async Task CapturePayment(response res, string order_id, string staff_id)
        {

            try
            {

                var client = new HttpClient();
                var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes(authString));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

                var requestBody = new FormUrlEncodedContent(new[]
               {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var requestUrl = "https://api-m.sandbox.paypal.com/v1/oauth2/token";
                var response = client.PostAsync(requestUrl, requestBody).Result;

                // Process response
                var responseBody = response.Content.ReadAsStringAsync().Result;

                var json = JObject.Parse(responseBody);
                var accessToken = (string)json["access_token"];

                Console.WriteLine(accessToken);

                /////////////////////////////////////Capture payment///////////////////////////////

                var client1 = new HttpClient();
                client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authHeaderValue);
                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "https://api-m.sandbox.paypal.com/v2/checkout/orders/" + order_id + "/capture";

                var response2 = await client.PostAsync(url, new StringContent("", Encoding.UTF8, "application/json"));

                //////////////////////////////////GetOrder Status//////////////////////////////////////
                var client2 = new HttpClient();
                client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authHeaderValue);
                client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                string url2 = "https://api-m.sandbox.paypal.com/v2/checkout/orders/" + order_id;

                var response3 = await client.GetAsync(url2);
                var responseContent2 = await response3.Content.ReadAsStringAsync();

                var jsonObject = JObject.Parse(responseContent2);
                var orderStatus = jsonObject["status"].ToString();

                if (orderStatus == "COMPLETED")
                {
                    var bill_id = jsonObject["purchase_units"][0]["items"][0]["name"].ToString();
                    string guidString = bill_id.ToString().Replace(" ", "");
                    Console.WriteLine("'" + guidString + "'");
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();
                        using (SqlTransaction transaction = connection.BeginTransaction())
                        {
                            try
                            {
                                string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                                string query = "update bill set transaction_id = " + "'" + order_id + " '" + ",staff_id=" + "'" + staff_id + "'" + ", paid_time= " + "'" + date + "'" + ", is_pay = 1" + "   where bill_id=" + "'" + guidString + "'";
                                Console.WriteLine(query);
                                using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                transaction.Commit();
                                res.code = 1;
                                res.msg = "ok";
                            }
                            catch (Exception e)
                            {
                                res.code = 0;
                                res.msg = e.Message;
                            }
                        }
                    }

                }
                else
                {
                    res.code = 2;
                    res.msg = "don hang chua duoc thanh toan";
                }
            }
            catch (Exception e)
            {

                res.code = 0;
                res.msg = e.Message;

            }
        }


        public async Task<String> Test(response res, string bill_id, double value)
        {

            value = value / 23000;
            value = Math.Round(value, 2);
            string item_value = value.ToString();

            try
            {


                var client = new HttpClient();
                var authHeaderValue = Convert.ToBase64String(Encoding.ASCII.GetBytes(authString));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

                var requestBody = new FormUrlEncodedContent(new[]
               {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var requestUrl = "https://api-m.sandbox.paypal.com/v1/oauth2/token";
                var response = client.PostAsync(requestUrl, requestBody).Result;

                // Process response
                var responseBody = response.Content.ReadAsStringAsync().Result;
                var json = JObject.Parse(responseBody);
                var accessToken = (string)json["access_token"];

                //Console.WriteLine(accessToken);


                var client1 = new HttpClient();
                client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authHeaderValue);
                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var payload = new
                {
                    intent = "CAPTURE",
                    purchase_units = new[]
     {
        new
        {
            items = new[]
            {
                new
                {
                    name = bill_id,
                    description = "Hoa Don Thanh Toan Tien Dich Vu Xe Dua Don",
                    quantity = "1",
                    unit_amount = new
                    {
                        currency_code = "USD",
                        value = item_value
                    }
                }
            },
            amount = new
            {
                currency_code = "USD",
                value = item_value,
                breakdown = new
                {
                    item_total = new
                    {
                        currency_code = "USD",
                        value = item_value
                    }
                }
            }
        }
    },
                    application_context = new
                    {
                        return_url = "https://makeagif.com/i/Ew-1kj",
                        cancel_url = "https://example.com/cancel"
                    }
                };

                // Serialize the payload to JSON
                var requestBodyJson = JsonSerializer.Serialize(payload);
                //  Console.WriteLine(requestBodyJson);
                var response2 = await client.PostAsync("https://api-m.sandbox.paypal.com/v2/checkout/orders", new StringContent(requestBodyJson, Encoding.UTF8, "application/json"));
                var responseContent = await response2.Content.ReadAsStringAsync();
                var responseJson = JsonSerializer.Deserialize<dynamic>(responseContent);

                var responsed = JsonSerializer.Deserialize<CheckoutOrderResponse>(responseJson);

                // Get the ID and href for the "approve" link



                //  Console.WriteLine(responsed.id);
                //  Console.WriteLine(responsed.links[1].href);

                string re = "{\"id\":" + "\"" + responsed.id + "\"," + "\"link\":" + "\"" + responsed.links[1].href + "\"}";
                return re;
            }
            catch (Exception e)
            {


                return "";
            }
        }


    }
}
