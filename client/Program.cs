using Dummy;
using Greet;
using Grpc.Core;
using Sum;
using Prime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace client
{
    internal class Program
    {
        //This is the boiler plate code
        const string target = "127.0.0.1:50051";

        static async Task Main(string[] args)
        {
            //Use insecure as no authentication is taking place 
            Channel channel = new Channel(target,ChannelCredentials.Insecure);

            //ensures connectivity is established
            await channel.ConnectAsync().ContinueWith(t => {
                if (t.Status == TaskStatus.RanToCompletion) {
                    Console.WriteLine("The client connected successfully");
                }
            
            });

            //var client = new DummyService.DummyServiceClient(channel);

            //Send data over network
            var client = new GreetingService.GreetingServiceClient(channel);

            //Initialize the object to be passed
            var greeting = new Greeting()
            {
                FirstName = "Alex",
                LastName = "Bond"
            };

            /* This section used for Unary 

            //Create the request
            var request = new GreetingRequest() 
            {
                Greeting = greeting 
            };

            var response = client.Greet(request);

            Console.WriteLine(response.Result);
            */


            //This section is for Server Streaming
            // For server streaming I have made the main method async and added await function where necessary
            var request = new GreetManyTimesRequest() { Greeting = greeting };
            var response = client.GreetManyTimes(request);

            while (await response.ResponseStream.MoveNext())
            {
                //checking the current item in stream and print result
                Console.WriteLine(response.ResponseStream.Current.Result);
                await Task.Delay(200);//This is to slow down process to see in the console
            }

            //This section is for Client Streaming
            var LongRequest = new LongGreetRequest() { Greeting = greeting };
            var LongStream = client.LongGreet();

            foreach (var item in Enumerable.Range(1, 20))
            {
                await LongStream.RequestStream.WriteAsync(LongRequest);
            }

            await LongStream.RequestStream.CompleteAsync();

            var LongResponse = await LongStream.ResponseAsync;

            Console.WriteLine(LongResponse.Result);

            /************************************************************/

            var clientAddition = new AdditionService.AdditionServiceClient(channel);

            var addition = new ValuePackage()
            {
                ValueOne = 10,
                ValueTwo = 20
            };

            var requestAddition = new AdditionRequest()
            {
                ValuePackage = addition
            };

            var responseAddition = clientAddition.Sum(requestAddition);
            Console.WriteLine(responseAddition.Result);
            /************************************************************/

            var clientPrime = new PrimeNumberService.PrimeNumberServiceClient(channel);

            var requestPrime = new PrimeNumberDecompositionRequest()
            {

                Number = 120
            };

            var responsePrime = clientPrime.PrimeNumberDecomposition(requestPrime);

            while (await responsePrime.ResponseStream.MoveNext())
            {
                Console.WriteLine(responsePrime.ResponseStream.Current.PrimeFactor);
            }
            Console.WriteLine(responsePrime.ToString());

            /************************************************************/


            channel.ShutdownAsync().Wait();
            Console.ReadKey();

        }
    }
}
