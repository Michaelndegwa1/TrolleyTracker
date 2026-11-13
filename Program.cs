using TrolleyTracker.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Sample vendors kept in memory for now (a database comes in Step 5)
var vendors = new List<Vendor>
{
    new Vendor { Id = 1, Code = "V01", Name = "John Kamau",   Phone = "0712345678", ContractStart = new DateOnly(2026, 12, 1) },
    new Vendor { Id = 2, Code = "V02", Name = "Mary Wanjiku", Phone = "0723456789", ContractStart = new DateOnly(2026, 12, 1) },
    new Vendor { Id = 3, Code = "V03", Name = "Peter Otieno", Phone = "0734567890", ContractStart = new DateOnly(2026, 12, 1) },
    new Vendor { Id = 4, Code = "V04", Name = "Alice Akinyi", Phone = "0745678901", ContractStart = new DateOnly(2026, 12, 1) },
};

// GET /vendors  -> list all vendors
app.MapGet("/vendors", () => vendors);

// GET /vendors/V01  -> one vendor by code
app.MapGet("/vendors/{code}", (string code) =>
{
    var vendor = vendors.FirstOrDefault(v => v.Code == code);
    return vendor is null ? Results.NotFound() : Results.Ok(vendor);
});

app.Run();