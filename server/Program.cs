using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Greet;
using Grpc.Core;
using Prime;
using Sum;

namespace server
{
    internal class Program
    {
        //Defining the port number
        const int customPort = 50051;

        static void Main(string[] args)
        {
            Server server = null;

            try
            {
                server = new Server()
                {
                    //Tells the server when the client is calling the greet function it will call the implementation
                    //Similar to binding in web api
                    Services =
                    {
                        GreetingService.BindService(new GreetingServiceImpl()),
                        AdditionService.BindService(new AdditionServerImpt()),
                        PrimeNumberService.BindService(new  PrimeNumberServiceImpl())
                    },



                    //Setting the port
                    Ports = { new ServerPort("localhost", customPort, ServerCredentials.Insecure) }

                };

                server.Start();
                Console.WriteLine("The server is listening on the port: " + customPort);
                Console.ReadLine();
            }
            catch (IOException e)
            {
                Console.WriteLine($"The server failed to start {e.Message}");
                throw;
            }
            finally
            {
                if (server != null)
                {
                    server.ShutdownAsync().Wait();
                }
            }

        }
    }
}
