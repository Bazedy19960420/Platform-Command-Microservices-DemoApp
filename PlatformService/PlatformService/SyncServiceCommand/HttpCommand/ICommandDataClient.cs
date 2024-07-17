using PlatformService.Dtos;

namespace PlatformService.SyncServiceCommand.HttpCommand
{
    public interface ICommandDataClient
    {
        public Task SendPlatformToCommand(PlatformReadDto platform);
    }
}
