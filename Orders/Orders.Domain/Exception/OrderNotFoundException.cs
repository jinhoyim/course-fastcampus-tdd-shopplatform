namespace Orders.Exception;

public class OrderNotFoundException(string message) : System.Exception(message);