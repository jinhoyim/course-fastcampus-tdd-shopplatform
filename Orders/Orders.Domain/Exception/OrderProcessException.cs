namespace Orders.Domain.Exception;

public class OrderProcessException(string message) : System.Exception(message);