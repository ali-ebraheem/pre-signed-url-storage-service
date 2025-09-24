var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure gRPC client to use the HTTP/2 port
builder.Services.AddGrpcClient<StorageService.Grpc.StorageService.StorageServiceClient>(options =>
{
    options.Address = new Uri("http://storageservice:5002"); // Use port 5002 for gRPC
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Remove HTTPS redirection for Docker
// app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();

app.Run();