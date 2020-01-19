using System.Threading.Tasks;
using StartWarsContract;
using Grpc.Core;
using System;
using System.Collections.Generic;

namespace MillenniumFalconServer.Services
{
    public class ServerService : MillenniumFalconService.MillenniumFalconServiceBase
    {
        public ServerService()
        {
        }
        public override async Task AutoControl(AutoControlRequest request, IServerStreamWriter<AutoControlResponse> responseStream, ServerCallContext context)
        {
            var response = new AutoControlResponse() { Distance = 0, Speed = 0 };
            try
            {
                if (request.Status == StatusEnum.Active)
                {
                    switch (request.DestinationSystem)
                    {
                        case SpaceSystemEnum.Alderaan:
                            response.Speed = 1;
                            response.Distance = 50;
                            break;
                        case SpaceSystemEnum.Endor:
                            response.Speed = 2;
                            response.Distance = 60;
                            break;
                        case SpaceSystemEnum.Hoth:
                            response.Speed = 4;
                            response.Distance = 80;
                            break;
                        default:
                            response.Speed = 5;
                            response.Distance = 500;
                            break;
                    }
                    while (response.Distance > 0)
                    {
                        response.Distance -= response.Speed;
                        await Task.Delay(100);
                        await responseStream.WriteAsync(response);
                    }
                }
                else
                {
                    await responseStream.WriteAsync(response);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public override async Task<OpenLightSpeedResponse> OpenLightSpeed(OpenLightSpeedRequest request, ServerCallContext context)
        {
            try
            {
                var response = new OpenLightSpeedResponse();
                if (request.Distance > 100 && request.Power > 0)
                    response.Status = StatusEnum.Active;
                else
                    response.Status = StatusEnum.Passive;
                await Task.Delay(100);
                return response;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async override Task<OpenRadarScanningResponse> OpenRadarScanning(IAsyncStreamReader<OpenRadarScanningRequest> requestStream, ServerCallContext context)
        {
            try
            {
                var request = new OpenRadarScanningResponse() { Alert = AlertEnum.Normal };
                var enemyCount = 0;
                var unknownCount = 0;
                while (await requestStream.MoveNext())
                {
                    if (requestStream.Current.ObjectStatus == ObjectEnum.Enemy)
                    {
                        enemyCount += 1;
                    }
                    else if ((requestStream.Current.ObjectSize == ObjectSizeEnum.Big ||
                    requestStream.Current.ObjectSize == ObjectSizeEnum.Medium) &&
                    requestStream.Current.ObjectStatus == ObjectEnum.Unknown)
                    {
                        unknownCount += 1;
                    }
                }
                if (enemyCount > 5 || unknownCount > 10)
                    request.Alert = AlertEnum.EnemyAtack;
                return request;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async override Task RadioControl(IAsyncStreamReader<RadioControlRequest> requestStream, IServerStreamWriter<RadioControlResponse> responseStream, ServerCallContext context)
        {
            try
            {
                while (await requestStream.MoveNext())
                {
                    var request = requestStream.Current;
                    Console.WriteLine($"{request.Message} system - {request.System}");
                    var random = new Random();

                    var responseModel = new RadioControlResponse();
                    for (int i = 0; i < 5; i++)
                    {
                        responseModel.System = (SpaceSystemEnum)Enum.Parse(typeof(SpaceSystemEnum), random.Next(0, 6).ToString());
                        responseModel.Message = $"Response Message Count={i}";
                        await Task.Delay(100);
                        await responseStream.WriteAsync(responseModel);
                    }
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