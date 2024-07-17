using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncServiceCommand.HttpCommand;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformRepo _platformRepo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;
        public PlatformController(IMessageBusClient messageBusClient, IPlatformRepo platformRepo, IMapper mapper, ICommandDataClient commandDataClient)
        {
            _mapper = mapper;
            _platformRepo = platformRepo;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }
        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
        {
            var platformItems = _platformRepo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }
        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platformItem = _platformRepo.GetPlatformById(id);
            if (platformItem == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PlatformReadDto>(platformItem));
        }
        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform([FromBody] PlatformCreateDto plat)
        {
            var platformMapper = _mapper.Map<Platform>(plat);
            _platformRepo.CreatePlatform(platformMapper);
            _platformRepo.SaveChanges();

            var platformReturned = _mapper.Map<PlatformReadDto>(platformMapper);
            try
            {
                Console.WriteLine($"===>Send The Message syncoronsely");
                await _commandDataClient.SendPlatformToCommand(platformReturned);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldnt Send {ex}");
            }
            try
            {
                var platformPublished = _mapper.Map<PlatformPublishedDto>(platformReturned);
                platformPublished.Event = "PlatformPublished";
                _messageBusClient.PublishNewPlatform(platformPublished);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldnt send Asynconoursly{ex.Message}");
            }

            return CreatedAtRoute("GetPlatformById", new { Id = platformMapper.Id }, platformReturned);
        }
    }
}
