using System.Threading.Tasks;
using Grpc.Core;
using MillenniumFalconClient.Services;
using StartWarsContract;

namespace MillenniumFalconClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            var client = new MillenniumFalconService.MillenniumFalconServiceClient(channel);

            var request = new AutoControlRequest() { Status = StatusEnum.Active, DestinationSystem = SpaceSystemEnum.Hoth };
            ClientService service = new ClientService(client);
            await service.AutoControlClientService();
            await service.OpenLightClientService();
            await service.OpenRadarScanningClientService();
            await service.RadioControlClientService();
        }
    }
}
