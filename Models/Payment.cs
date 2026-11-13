namespace TrolleyTracker.Models;

public class Payment
{
    public int Id { get; set; }
    public int VendorId { get; set; }
    public int Amount { get; set; }
    public string MpesaCode { get; set; } = ""; // e.g. TKD4H7XQ2P
    public DateTime PaidAt { get; set; }
}