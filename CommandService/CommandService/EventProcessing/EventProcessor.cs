using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessing
{
    public class EventProcessor: IEventProcessor
    {
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(IServiceScopeFactory scopeFactory,IMapper mapper)
        {
            _mapper = mapper;
            _scopeFactory = scopeFactory;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                addPlatform(message);
                break;
                default:
                System.Console.WriteLine("---->Couldnt Add");
                break;
            }
        }
        private EventType DetermineEvent(string message)
        {
           var eventType = JsonSerializer.Deserialize<GenericEventDto>(message);
           if(eventType.Event == "PlatformPublished")
           {
                return EventType.PlatformPublished;
           }
           else
           {
            return EventType.underTermined;
           }
        }
        private void addPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    if(!repo.ExternalPlatformExist(plat.ExternalId))
                    {
                        System.Console.WriteLine("Platform Added");
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                    }
                    else
                    {
                        System.Console.WriteLine("Platform already Exists");
                    }

                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"===> doesnot Published{ex.Message}");
                }
            }
        }
    }
    enum EventType{
        PlatformPublished,
        underTermined
    }
}