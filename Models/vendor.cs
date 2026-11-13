namespace TrolleyTracker.Models;

public class Vendor
{
    public int Id { get; set; }
    public string Code { get; set; } = "";      // V01, V02... used as the M-Pesa account number
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public DateOnly ContractStart { get; set; }
    public int DailyAmount { get; set; } = 300; // KSh per day
    public bool IsActive { get; set; } = true;
    public string Location { get; set; } = "Kencom stage";
    // Calculated, not stored: contract ends 6 months after it starts
    public DateOnly ContractEnd => ContractStart.AddMonths(6);
}