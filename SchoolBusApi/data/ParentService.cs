using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using SchoolBusApi.Models;
using System.Data;

namespace SchoolBusApi.data
{
    public class ParentService : DbContext
    {
        public string ConnectionString { get; set; }
        public ParentService(DbContextOptions<ParentService> options) : base(options)
        {
            ConnectionString = options.FindExtension<SqlServerOptionsExtension>().ConnectionString;
        }

        public DbSet<parent> parent { get; set; }
        public DbSet<users> users { get; set; }
        public DbSet<bill> bill { get; set; }

        public async Task<List<users>> getallusers() => await users.ToListAsync();
        public async Task<List<parent>> getallparent() => await parent.ToListAsync();


        public async Task UpdateBill(response res, Guid bill_id, string transaction_id)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string query = "update bill set transaction_id = " + "'" + transaction_id + " '" + ", paid_time= " + "'" + date + "'" + ", is_pay = 1" + "   where bill_id=" + "'" + bill_id + "'";
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
        public async Task loginv2(response res, string username, string pass)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("ParentLogin", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@username", username));
                command.Parameters.Add(new SqlParameter("@pass", pass));
                var reader = await command.ExecuteReaderAsync();
                Console.WriteLine("this is first");
                if (reader.Read())
                {
                    Console.WriteLine("this is second");

                    string result = reader.GetInt32(0).ToString();

                    Console.WriteLine("this is res " + result);
                    if (result == "0")
                    {
                        res.code = 0;
                        res.msg = "Invalid phone or password";

                    }
                    else
                    {
                        res.code = reader.GetInt32(0);
                        res.msg = "Login success";
                    }

                }
            }
        }

        public async Task<string> GetUnpaidBill(int parent_id)
        {
            string query = "select b.bill_id, b.create_time, b.end_time,st.student_name,b.cost from bill b join students st on b.student_id = st.student_id where b.parent_id= " + parent_id + "for json path";
            string query2 = "select b.bill_id, b.create_time, b.end_time,b.days_used,st.student_name,b.stop_id,cast(b.cost as decimal(7,1)) as cost from bill b join students st on b.student_id = st.student_id where  b.is_pay = 0 and b.parent_id= " + parent_id + " for json path";
            string temp = "[]";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query2, connection);

                var reader = await command.ExecuteReaderAsync();
                if (reader.Read())
                {

                    string result = reader.GetString(0);
                    return result;
                    Console.WriteLine("this is res " + result);

                }
                else return temp;
            }
        }

        public async Task<string> GetPaidBill(int parent_id)
        {
            string temp = "[]";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetPaidBill", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@parent_id", parent_id));
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    string jsonResult = result.ToString();
                    return jsonResult;
                    // process the JSON result here
                }
                else
                {
                    return temp;
                }

            }
        }






        public async Task TestTransaction(response res)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Perform database operations within the transaction
                        using (SqlCommand command = new SqlCommand("UPDATE parents SET name = 'test' WHERE phone = '0932043965'", connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }
                        using (SqlCommand command = new SqlCommand("UPDATE students SET student_name = 'test' WHERE student_id = 'n19dccn017'", connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }
                        //      var parent = await parents.FirstOrDefaultAsync(x => x.phone == "0932043965");
                        transaction.Commit();
                        res.code = 1;
                        res.msg = "ok";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Transaction rolled back due to an exception.");
                        transaction.Rollback();
                        res.code = 0;
                        res.msg = ex.Message;
                        throw;
                    }
                }
            }

        }


    }
}
