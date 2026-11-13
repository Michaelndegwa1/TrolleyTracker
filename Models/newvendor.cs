namespace TrolleyTracker.Models;
public record NewVendor(string VendorId, string VendorName, int PhoneNumber, string Location, DateOnly ContractStart, int DailyAmount);