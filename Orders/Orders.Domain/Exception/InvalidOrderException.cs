namespace Orders.Exception;

public class InvalidOrderException(string message) : System.Exception(message);