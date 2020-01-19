using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StartWarsContract;
using static StartWarsContract.MillenniumFalconService;

namespace MillenniumFalconClient.Services
{
    public class ClientService
    {
        private readonly CancellationTokenSource _tokenSource;
        private readonly MillenniumFalconServiceClient _client;
        public ClientService(
            MillenniumFalconServiceClient client
        )
        {
            _tokenSource = new CancellationTokenSource();
            _client = client;
        }
        public async Task AutoControlClientService()
        {
            var request = new AutoControlRequest() { Status = StatusEnum.Active, DestinationSystem = SpaceSystemEnum.Hoth };
            try
            {
                using (var call = _client.AutoControl(request))
                {
                    Console.WriteLine("Start AutoControl");
                    while (await call.ResponseStream.MoveNext(_tokenSource.Token))
                    {
                        Console.WriteLine($"Speed = {call.ResponseStream.Current.Speed.ToString()}, Distance = {call.ResponseStream.Current.Distance.ToString()}");
                    }
                    Console.WriteLine("End AutoControl");
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task OpenLightClientService()
        {
            try
            {
                var request = new OpenLightSpeedRequest() { Power = 100, Distance = 5000 };
                var response = await _client.OpenLightSpeedAsync(request);

                Console.WriteLine("Start OpenLight");
                if (response.Status == StatusEnum.Active)
                    Console.WriteLine("Light speed is ACTIVE.");
                else
                    Console.WriteLine("Light speed is PASSIVE.");
                Console.WriteLine("End OpenLight");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task OpenRadarScanningClientService()
        {
            try
            {
                using (var call = _client.OpenRadarScanning())
                {
                    for (int i = 0; i < 20; i++)
                    {
                        var random = new Random();
                        var request = new OpenRadarScanningRequest()
                        {
                            Distance = random.Next(0, 500),
                            ObjectSize = (ObjectSizeEnum)Enum.Parse(typeof(ObjectSizeEnum), random.Next(0, 2).ToString()),
                            ObjectStatus = (ObjectEnum)Enum.Parse(typeof(ObjectEnum), random.Next(0, 2).ToString()),
                        };
                        await call.RequestStream.WriteAsync(request);
                        await Task.Delay(100);
                        Console.WriteLine($"Scaning Object = Size - {request.ObjectSize} Status - {request.ObjectStatus} Distance - {request.Distance}");
                    }
                    await call.RequestStream.CompleteAsync();

                    var response = await call;
                    Console.WriteLine($"Alarm = {response.Alert}");
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task RadioControlClientService()
        {
            try
            {
                using (var call = _client.RadioControl())
                {
                    var responseReaderTask = Task.Run(async () =>
                    {
                        while (await call.ResponseStream.MoveNext())
                        {
                            var response = call.ResponseStream.Current;
                            Console.WriteLine($"{response.Message} system - {response.System}");
                        }
                    });

                    var requestModel = new RadioControlRequest()
                    {
                        System = SpaceSystemEnum.Naboo
                    };
                    for (int i = 0; i < 5; i++)
                    {
                        requestModel.Message = $"Request Message Count={i}";
                        await Task.Delay(100);
                        await call.RequestStream.WriteAsync(requestModel);
                    }
                    await call.RequestStream.CompleteAsync();
                    await responseReaderTask;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}