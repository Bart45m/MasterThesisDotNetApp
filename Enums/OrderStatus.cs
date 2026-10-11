namespace MasterThesisDotNetApp.Enums;

public enum OrderStatus
{
    Pending = 0,       // Oczekujące na płatność / realizację
    Processing = 1,    // W trakcie realizacji
    Shipped = 2,       // Wysłane
    Delivered = 3,     // Dostarczone
    Cancelled = 4      // Anulowane
}