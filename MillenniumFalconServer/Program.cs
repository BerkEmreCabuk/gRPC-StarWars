using System;
using Grpc.Core;
using MillenniumFalconServer.Services;
using StartWarsContract;

namespace MillenniumFalconServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server gRPCServer = new Server
            {
                Services = { MillenniumFalconService.BindService(new ServerService()) },
                Ports = { new ServerPort("localhost", 50051, ServerCredentials.Insecure) }
            };

            gRPCServer.Start();
            Console.ReadLine();
            gRPCServer.ShutdownAsync().Wait();
        }
    }
}
