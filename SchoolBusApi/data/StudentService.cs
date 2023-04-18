using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SchoolBusApi.Models;
using System.Data;

namespace SchoolBusApi.data
{
    public class StudentService : DbContext
    {
        public string ConnectionString { get; set; }
        public StudentService(DbContextOptions<StudentService> options) : base(options)
        {
            ConnectionString = options.FindExtension<SqlServerOptionsExtension>().ConnectionString;
        }

        public DbSet<students> students { get; set; }
        public DbSet<car> bus { get; set; }
        public DbSet<CarStop> CarStop { get; set; }
        public DbSet<ServiceRegistry> ServiceRegistry { get; set; }



        public async Task<List<CarStop>> GetAllStop() => await CarStop.ToListAsync();
        public async Task<List<students>> FindChild(int parent_id) => await students.Where(x => x.parent_id == parent_id).ToListAsync();

        public async Task<String> GetStudentSchedule(int parent_id)
        {
            string temp = "[]";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("FindChildRegistry", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@parent_id", parent_id));
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

        public async Task<String> FindChildRegistry(int parent_id)
        {
            string temp = "[]";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("FindChildRegistry", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@parent_id", parent_id));
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
        public async Task<String> FindChildNotRegistry(int parent_id)
        {
            string temp = "[]";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("FindChildNotRegistry", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@parent_id", parent_id));
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


        public async Task CarServiceRegistry(response res, int parent_id, string student_id, string stop)
        {
            var student = await students.Where(x => x.student_id == student_id).FirstOrDefaultAsync();
            string guid = student.registry_id.ToString();
            Console.WriteLine(guid);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {


                        string sqlcmd1 = "update ServiceRegistry set stop_id = " + "'" + stop + " '" + ", is_cancel = 0" + "   where id=" + "'" + guid + "'";
                        Console.WriteLine(sqlcmd1);
                        using (SqlCommand cmd = new SqlCommand(sqlcmd1, connection, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        string sqlcmd2 = "update CarStop set student_count = student_count + 1 where stop_id = " + "'" + stop + "'";
                        using (SqlCommand cmd = new SqlCommand(sqlcmd2, connection, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }


                        /*  string sqlcmd3 = "insert into bill (parent_id,student_id,stop_id) values (" + parent_id + "," + "'" + student_id + "'" + "," + "'" + stop + "'" + ")";
                          using (SqlCommand cmd = new SqlCommand(sqlcmd3, connection, transaction))
                          {
                              cmd.ExecuteNonQuery();
                          }*/

                        SqlCommand command2 = new SqlCommand("insertbill", connection, transaction);
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.Add(new SqlParameter("@parent_id", parent_id));
                        command2.Parameters.Add(new SqlParameter("@student_id", student_id));
                        command2.Parameters.Add(new SqlParameter("@stop", stop));
                        command2.ExecuteNonQuery();
                        transaction.Commit();
                        res.code = 1;
                        res.msg = "ok";

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Transaction rolled back due to an exception.");
                        transaction.Rollback();
                        res.code = 0;
                        res.msg = e.Message;
                    }

                }
            }
        }

        public async Task CancelRegistry(response res, string student_id)
        {
            var student = await students.Where(x => x.student_id == student_id).FirstOrDefaultAsync();
            Guid guid = student.registry_id;
            var tempregistry = await ServiceRegistry.Where(x => x.id == guid).FirstOrDefaultAsync();
            string car_id = tempregistry.car_id;
            string stop = tempregistry.stop_id;
            Console.WriteLine(guid.ToString());
            Console.WriteLine(car_id);
            Console.WriteLine(stop);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {

                        string sqlcmd1 = "update ServiceRegistry set stop_id = NULL ,is_cancel = 1 , car_id = NULL" + " where id = " + "'" + guid + "'";
                        Console.WriteLine(sqlcmd1);
                        using (SqlCommand cmd = new SqlCommand(sqlcmd1, connection, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        string sqlcmd2 = "update bill set is_cancel = 1 where student_id = " + "'" + student_id + "'";
                        using (SqlCommand cmd = new SqlCommand(sqlcmd2, connection, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        string sqlcmd3 = "update car set remain_slot = remain_slot+1 where id= " + "'" + car_id + "'";
                        using (SqlCommand cmd = new SqlCommand(sqlcmd3, connection, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        string sqlcmd4 = "update CarStop set student_count = student_count - 1 where stop_id = " + "'" + stop + "'";
                        using (SqlCommand cmd = new SqlCommand(sqlcmd4, connection, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        res.code = 1;
                        res.msg = "ok";

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Transaction rolled back due to an exception.");
                        transaction.Rollback();
                        res.code = 0;
                        res.msg = e.Message;
                    }

                }
            }
        }

        public async Task UpdateRegistry(response res, string student_id, string stop)
        {
            var student = await students.Where(x => x.student_id == student_id).FirstOrDefaultAsync();
            string guid = student.registry_id.ToString();
            var tempregistry = await ServiceRegistry.Where(x => x.id == student.registry_id).FirstOrDefaultAsync();
            if (tempregistry.is_cancel == true)
            {
                res.code = 0;
                res.msg = "hoc sinh da huy dang ki ko dc update";
                return;
            }

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {


                        string sqlcmd1 = "update ServiceRegistry set stop_id = " + "'" + stop + " '" + "   where id=" + "'" + guid + "'";
                        Console.WriteLine(sqlcmd1);
                        using (SqlCommand cmd = new SqlCommand(sqlcmd1, connection, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }


                        SqlCommand command2 = new SqlCommand("UpdateBill", connection, transaction);
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.Add(new SqlParameter("@student_id", student_id));
                        command2.Parameters.Add(new SqlParameter("@stop_id", stop));
                        command2.ExecuteNonQuery();


                        transaction.Commit();
                        res.code = 1;
                        res.msg = "ok";

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Transaction rolled back due to an exception.");
                        transaction.Rollback();
                        res.code = 0;
                        res.msg = e.Message;
                    }

                }
            }
        }




    }

}
