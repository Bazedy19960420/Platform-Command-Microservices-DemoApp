using PlatformService.Dtos;

namespace PlatformService.AsyncDataService
{
    public interface IMessageBusClient
    {
        public void PublishNewPlatform(PlatformPublishedDto platform);
    }
}
