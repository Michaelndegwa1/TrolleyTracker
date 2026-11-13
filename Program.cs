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
// ---------- PAYMENTS ----------
var payments = new List<Payment>();
var nextPaymentId = 1;

// POST /payments  -> record a payment
app.MapPost("/payments", (NewPayment input) =>
{
    var vendor = vendors.FirstOrDefault(v => v.Code == input.VendorCode);
    if (vendor is null)
        return Results.BadRequest($"No vendor with code {input.VendorCode}");

    // Stop the same M-Pesa payment being counted twice
    if (payments.Any(p => p.MpesaCode == input.MpesaCode))
        return Results.Conflict($"M-Pesa code {input.MpesaCode} was already recorded");

    var payment = new Payment
    {
        Id = nextPaymentId++,
        VendorId = vendor.Id,
        Amount = input.Amount,
        MpesaCode = input.MpesaCode,
        PaidAt = DateTime.Now
    };
    payments.Add(payment);

    return Results.Created($"/payments/{payment.Id}", payment);
});

// GET /payments  -> all payments
app.MapGet("/payments", () => payments);

// GET /today  -> who has paid today and who hasn't
app.MapGet("/today", () =>
{
    var today = DateOnly.FromDateTime(DateTime.Now);

    // Total paid today per vendor: { vendorId: total }
    var totals = payments
        .Where(p => DateOnly.FromDateTime(p.PaidAt) == today)
        .GroupBy(p => p.VendorId)
        .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

    var report = vendors
        .Where(v => v.IsActive)
        .Select(v =>
        {
            var paid = totals.GetValueOrDefault(v.Id); // 0 if they haven't paid
            var owed = Math.Max(0, v.DailyAmount - paid);
            return new
            {
                v.Code,
                v.Name,
                PaidToday = paid,
                Owed = owed,
                Status = owed == 0 ? "PAID" : (paid > 0 ? "PARTIAL" : "NOT PAID")
            };
        })
        .ToList();

    return new
    {
        Date = today,
        TotalCollected = report.Sum(r => r.PaidToday),
        Expected = vendors.Where(v => v.IsActive).Sum(v => v.DailyAmount),
        Vendors = report
    };
});
app.Run();