using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using SchoolBusApi.Models;
using System.Data;

namespace SchoolBusApi.data
{
    public class StaffService : DbContext
    {
        public string ConnectionString { get; set; }
        public StaffService(DbContextOptions<StaffService> options) : base(options)
        {
            ConnectionString = options.FindExtension<SqlServerOptionsExtension>().ConnectionString;
        }

        public DbSet<parent> parent { get; set; }
        public DbSet<students> students { get; set; }
        public DbSet<users> users { get; set; }
        public DbSet<staff> staff { get; set; }
        public DbSet<schedule> schedule { get; set; }
        public DbSet<CarStop> CarStop { get; set; }
        public DbSet<ServiceRegistry> serviceRegistry { get; set; }
        public DbSet<car> car { get; set; }
        public async Task StaffLogin(response res, users item)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("StaffLogin", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@username", item.username));
                command.Parameters.Add(new SqlParameter("@pass", item.pass));
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
                        int role = reader.GetInt32(1);
                        if (role == 1)
                        {
                            res.msg = "admin";
                        }
                        else if (role == 2)
                        {
                            res.msg = "dbowner";
                        }
                        else res.msg = "accountant";
                        res.code = reader.GetInt32(0);
                    }
                }
            }
        }



        public async Task CreateParent(response res, string username, string pass, string name, string email, string phone)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SqlCommand command2 = new SqlCommand("CreateParent", connection, transaction);
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.Add(new SqlParameter("@username", username));
                        command2.Parameters.Add(new SqlParameter("@pass", pass));
                        command2.Parameters.Add(new SqlParameter("@name", name));
                        command2.Parameters.Add(new SqlParameter("@email", email));
                        command2.Parameters.Add(new SqlParameter("@phone", phone));

                        command2.ExecuteNonQuery();

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



        public async Task CreateStudent(response res, string student_id, string student_name, int parent_id)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SqlCommand command2 = new SqlCommand("CreateStudent", connection, transaction);
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.Add(new SqlParameter("@student_id", student_id));
                        command2.Parameters.Add(new SqlParameter("@student_name", student_name));
                        command2.Parameters.Add(new SqlParameter("@parent_id", parent_id));


                        command2.ExecuteNonQuery();

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


        public async Task CreateDriver(response res, string username, string pass, string name, string phone)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SqlCommand command2 = new SqlCommand("CreateDriver", connection, transaction);
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.Add(new SqlParameter("@username", username));
                        command2.Parameters.Add(new SqlParameter("@pass", pass));
                        command2.Parameters.Add(new SqlParameter("@name", name));
                        command2.Parameters.Add(new SqlParameter("@phone", phone));



                        command2.ExecuteNonQuery();

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


        public async Task CreateCar(response res, string CarId, string serial, int driver_id, int slot)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SqlCommand command2 = new SqlCommand("CreateCar", connection, transaction);
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.Add(new SqlParameter("@id", CarId));
                        command2.Parameters.Add(new SqlParameter("@serial", serial));
                        command2.Parameters.Add(new SqlParameter("@driver_id", driver_id));
                        command2.Parameters.Add(new SqlParameter("@slot", slot));



                        command2.ExecuteNonQuery();

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
        public async Task UpdateCarChangeDriver(response res, string CarId, int driver_id)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "update car set driver_id = " + "'" + driver_id + "'" + "  where id=" + "'" + CarId + "'";
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

        public async Task CreateCarStop(response res, CarStop item)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SqlCommand command2 = new SqlCommand("CreateCarStop", connection, transaction);
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.Add(new SqlParameter("@stop_id", item.stop_id));
                        command2.Parameters.Add(new SqlParameter("@stop_name", item.stop_name));
                        command2.Parameters.Add(new SqlParameter("@latitude", item.latitude));
                        command2.Parameters.Add(new SqlParameter("@longtitude", item.longitude));
                        command2.Parameters.Add(new SqlParameter("@number", item.number));
                        command2.Parameters.Add(new SqlParameter("@street", item.street));
                        command2.Parameters.Add(new SqlParameter("@ward", item.ward));
                        command2.Parameters.Add(new SqlParameter("@district", item.district));
                        command2.Parameters.Add(new SqlParameter("@city", item.city));
                        command2.Parameters.Add(new SqlParameter("@range_from_school", item.range_from_school));
                        command2.Parameters.Add(new SqlParameter("@cost", item.cost));
                        command2.Parameters.Add(new SqlParameter("@staff_id", item.staff_id));




                        command2.ExecuteNonQuery();
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


        public async Task CreateSchedule(response res, schedulev2 item)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SqlCommand command2 = new SqlCommand("CreateSchedule", connection, transaction);
                        command2.CommandType = CommandType.StoredProcedure;
                        command2.Parameters.Add(new SqlParameter("@car_id", item.car_id));
                        command2.Parameters.Add(new SqlParameter("@stop_id", item.stop_id));
                        command2.Parameters.Add(new SqlParameter("@pickup_time", item.pickup_time));
                        command2.Parameters.Add(new SqlParameter("@drop_time", item.drop_time));
                        command2.Parameters.Add(new SqlParameter("@staff_id", item.staff_id));
                        command2.ExecuteNonQuery();
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
        public async Task<string> GetAllCarInOneStop(string stop_id)
        {
            string query = "select c.id,c.slot, c.remain_slot,c.driver_id from car c join schedule sh on c.id = sh.car_id where sh.stop_id = " + "'" + stop_id + "'" + "for json path";
            string temp = "[]";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

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

        public async Task<string> GetAllCarNotInSchedule()
        {
            string query = "SELECT car.id,car.remain_slot,car.driver_id FROM car WHERE car.id NOT IN (SELECT schedule.car_id FROM schedule) for json path";
            string temp = "[]";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

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



        public async Task<string> GetStudentInOneStop(string stop_id)
        {
            string query = "select s.student_id, s.registry_id from students s inner join ServiceRegistry sr on s.registry_id = sr.id where sr.stop_id = " + "'" + stop_id + "'" + "for json path";
            string temp = "[]";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

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
        public async Task<string> GetStudentInOneStopNotInBus(string stop_id)
        {
            string query = "select s.student_id, s.registry_id from students s inner join ServiceRegistry sr on s.registry_id = sr.id where sr.stop_id = " + "'" + stop_id + "'" + "and sr.car_id is null " + "for json path";
            string temp = "[]";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

                var reader = await command.ExecuteReaderAsync();
                if (reader.Read())
                {

                    string result = reader.GetString(0);
                    return result;
                    Console.WriteLine("this is res " + result);

                    return result;
                }
                else return temp;
            }
        }
        public async Task<string> GetStudentInOneCar(string car_id)
        {
            string query = "select s.* from students s join ServiceRegistry sr on s.registry_id = sr.id where car_id = " + "'" + car_id + "'" + "for json auto";
            string temp = "[]";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);

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
        public async Task AsignStudentToBus(response res, students[] arr, string car_id, string staff_id)
        {
            int count = arr.Length;
            Console.WriteLine(count);
            /* foreach (var item in arr)

                 string query = "update ServviceRegistry set car_id = " + "'" + car_id + "'" + ", staff_id = " + "'" + staff_id + "'" + " where id = " + "'" + item.registry_id + "'";*/
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in arr)
                        {
                            string query = "update ServiceRegistry set car_id = " + "'" + car_id + "'" + ", staff_id = " + "'" + staff_id + "'" + " where id = " + "'" + item.registry_id + "'";
                            Console.WriteLine(query);
                            SqlCommand command = new SqlCommand(query, connection, transaction);
                            command.ExecuteNonQuery();
                        }

                        string sqlcmd3 = "update car set remain_slot = remain_slot -" + count + " where id= " + "'" + car_id + "'";
                        Console.WriteLine(sqlcmd3);
                        using (SqlCommand cmd = new SqlCommand(sqlcmd3, connection, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }

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
