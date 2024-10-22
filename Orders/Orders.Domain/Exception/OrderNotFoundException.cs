namespace Orders.Domain.Exception;

public class OrderNotFoundException(string message) : System.Exception(message);