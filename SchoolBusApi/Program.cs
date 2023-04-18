using Microsoft.EntityFrameworkCore;
using SchoolBusApi.data;
using SchoolBusApi.Models;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddConsole();
});
builder.Services.AddCors();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ParentService>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<StudentService>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<RouteService>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<BusService>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<StaffService>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient();

builder.Services.AddDbContext<PayPalService>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseCors(policy =>
    policy.AllowAnyHeader()
          .AllowAnyMethod()
          .AllowAnyOrigin());


// Configure the HTTP request pipeline.
app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    app.UseCors("corsapp");
    app.UseCors(policy =>
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin());
}
if (!app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
    app.UseCors("corsapp");
    app.UseCors(policy =>
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin());
}
app.UseHttpsRedirection();


/*app.MapGet("/parent/getall", async (ParentService db) =>
{
    return await db.parents.ToListAsync();
});



app.MapPost("/parent/createparents", async (ParentService sv, parents parent) =>
{
    response res = new response();
    await sv.Create(res, parent);
    return res;
});

app.MapPost("testTransaction", async (ParentService sv) =>
{
    response res = new response();
    await sv.TestTransaction(res);
    return res;
});*/

/*app.MapPost("/parent/loginv2", async (ParentService sv, parents item) =>
{
    response res = new response();
    await sv.loginv2(res, item.phone, item.pass);
    return res;
});*/

/*app.MapPost("/parent/updatestudent", async (StudentService sv, students item) =>
{
    response res = new response();
    await sv.UpdateStudent(res, item);
    return res;
});*/
/*app.MapPost("/parent/findchild", async (ParentService sv, parents item) =>
{
    response res = new response();
    await sv.Update(res, item);
    return res;
});
*/

/*app.MapPost("/parent/createstudent", async (StudentService sv, students item) =>
{
    response res = new response();
    await sv.CreateStudent(res, item);
    return res;
});*/
/*app.MapPost("/student/getallbyphone", async (StudentService sv, string phone) =>
{
    response res = new response();
    var list = await sv.GetAllStudentByPhone(phone);
    list[0].route_id += "1";
    return list;
});*/
app.MapPost("/parent/login", async (ParentService sv, users item) =>
{
    response res = new response();
    await sv.loginv2(res, item.username, item.pass);
    return res;
});



app.MapGet("/parent/findchildnotregistry", async (StudentService sv, int parent_id) =>
{
    return await sv.FindChildNotRegistry(parent_id);
});


app.MapGet("/parent/findchildregistry", async (StudentService sv, int parent_id) =>
{
    return await sv.FindChildRegistry(parent_id);
});



app.MapGet("getallstop", async (StudentService sv) =>
{
    return await sv.GetAllStop();
});

app.MapPost("CarServiceRegistry", async (StudentService sv, int parent_id, string student_id, string stop_id) =>
{
    response res = new response();
    await sv.CarServiceRegistry(res, parent_id, student_id, stop_id);
    return res;
});

app.MapPost("CancelService", async (StudentService sv, string student_id) =>
{
    response res = new response();
    await sv.CancelRegistry(res, student_id);
    return res;
});
app.MapPost("UpdateService", async (StudentService sv, string student_id, string stop) =>
{
    response res = new response();
    await sv.UpdateRegistry(res, student_id, stop);
    return res;
});

app.MapGet("getunpaidbill", async (ParentService sv, int parent_id) =>
{
    // return await sv.bill.Where(x => x.parent_id == parent_id && x.is_pay == false).ToListAsync();
    return await sv.GetUnpaidBill(parent_id);
});
app.MapGet("getpaidbill", async (ParentService sv, int parent_id) =>
{
    //return await sv.bill.Where(x => x.parent_id == parent_id && x.is_pay == true).ToListAsync();
    return await sv.GetPaidBill(parent_id);
});


/*app.MapPost("test", async (StudentService sv, students[] arr, string bus) =>
{
    await sv.Test(arr, bus);
    return 0;
});*/

app.MapPost("StaffLogin", async (StaffService sv, users item) =>
{
    response res = new response();
    await sv.StaffLogin(res, item);
    return res;
});
app.MapPost("CreateParent", async (StaffService sv, string username, string pass, string name, string email, string phone) =>
{
    response res = new response();
    await sv.CreateParent(res, username, pass, name, email, phone);
    return res;
});
app.MapPost("CreateStudent", async (StaffService sv, string student_id, string student_name, int parent_id) =>
{
    response res = new response();
    await sv.CreateStudent(res, student_id, student_name, parent_id);
    return res;
});

app.MapPost("CreateDriver", async (StaffService sv, string username, string pass, string name, string phone) =>
{
    response res = new response();
    await sv.CreateDriver(res, username, pass, name, phone);
    return res;
});
app.MapPost("CreateCar", async (StaffService sv, string CarId, string serial, int driver_id, int slot) =>
{
    response res = new response();
    await sv.CreateCar(res, CarId, serial, driver_id, slot);
    return res;
});

app.MapPost("UpdateCarChangeDriver", async (StaffService sv, string CarId, int driver_id) =>
{
    response res = new response();
    await sv.UpdateCarChangeDriver(res, CarId, driver_id);
    return res;
});

app.MapPost("CreateCarStop", async (StaffService sv, CarStop item) =>
{
    response res = new response();
    await sv.CreateCarStop(res, item);
    return res;
});

app.MapGet("GetCarStopsHaveStudents", async (StaffService sv) =>
{
    return await sv.CarStop.Where(x => x.student_count > 0).ToListAsync();
});

app.MapGet("GetStudentInOneStop", async (StaffService sv, string stop_id) =>
{

    return await sv.GetStudentInOneStop(stop_id);
});

app.MapGet("GetStudentInOneStopNotInBus", async (StaffService sv, string stop_id) =>
{

    return await sv.GetStudentInOneStopNotInBus(stop_id);
});
app.MapGet("GetStudentInOneCar", async (StaffService sv, string car_id) =>
{

    return await sv.GetStudentInOneCar(car_id);
});

app.MapGet("GetAllCarInOneStop", async (StaffService sv, string stop_id) =>
{

    return await sv.GetAllCarInOneStop(stop_id);
});
app.MapGet("GetAllCar", async (StaffService sv) =>
{
    return await sv.car.ToListAsync();
});
app.MapGet("GetAllCarNotInSchedule", async (StaffService sv) =>
{
    return await sv.GetAllCarNotInSchedule();
});

app.MapPost("CreateSchedule", async (StaffService sv, schedulev2 item) =>
{

    response res = new response();
    await sv.CreateSchedule(res, item);
    return res;
});
app.MapPost("AsignStudentToBus", async (StaffService sv, students[] arr, string car_id, string staff_id) =>
{
    response res = new response();
    await sv.AsignStudentToBus(res, arr, car_id, staff_id);
    return res;
});

app.MapPost("updatebill", async (ParentService sv, Guid bill_id, string transaction_id) =>
{
    response res = new response();
    await sv.UpdateBill(res, bill_id, transaction_id);
    return res;
});
app.MapPost("CreateOrderAndReturnLink", async (PayPalService sv, string bill_id, double value) =>
{
    response res = new response();
    return await sv.Test(res, bill_id, value);
});


app.MapPost("CapturePayment", async (PayPalService sv, string order_id, string staff_id) =>
{
    response res = new response();
    await sv.CapturePayment(res, order_id, staff_id);
    return res;
});








app.Run();

