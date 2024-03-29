using ASPServerSignalR;
using ASPServerSignalR.DataStorage;
using ASPServerSignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMessageStorage, MessageStorage>();
builder.Services.AddSingleton<IMatchStorage, MatchStorage>();
builder.Services.AddSignalR(hubConfig => { hubConfig.EnableDetailedErrors = true; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHub<ChatHub>("/ChatHub");
app.MapHub<LobbyHub>("/LobbyHub");
app.UseAuthorization();

app.MapControllers();

app.Run();