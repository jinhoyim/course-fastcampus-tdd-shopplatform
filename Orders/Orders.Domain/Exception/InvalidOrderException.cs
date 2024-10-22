namespace Orders.Domain.Exception;

public class InvalidOrderException(string message) : System.Exception(message);