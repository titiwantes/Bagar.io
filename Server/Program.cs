namespace BagarIo;

class Program
{
    static void Main(string[] args)
    {
        Server server = new Server("ws://0.0.0.0:8181");
        server.Start();

        Console.WriteLine("Press Enter to stop the server.");
        Console.ReadLine();
    }
}
