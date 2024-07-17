using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICommandRepo _commandRepo;

        public CommandController(IMapper mapper, ICommandRepo commandRepo)
        {
            _mapper = mapper;
            _commandRepo = commandRepo;
        }
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands(int platformId)
        {
            if (!_commandRepo.PlatformExist(platformId))
            {
                return NotFound();
            }
            var commands = _commandRepo.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<CommandReadDto>(commands));
        }
        [HttpGet("{commandId}", Name = "GetCommand")]
        public ActionResult<CommandReadDto> GetCommand(int platformId, int commandId)
        {
            if (!_commandRepo.PlatformExist(platformId))
            {
                return NotFound();
            }
            var command = _commandRepo.GetCommand(platformId, commandId);
            return Ok(_mapper.Map<CommandReadDto>(command));
        }
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(int platformId, CommandCreateDto commandCreateDto)
        {
            if (_commandRepo.PlatformExist(platformId))
            {
                return NotFound();
            }
            var command = _mapper.Map<Command>(commandCreateDto);
            _commandRepo.CreateCommand(platformId, command);
            _commandRepo.SaveChanges();
            var commandreturned = _mapper.Map<CommandReadDto>(command);
            return CreatedAtRoute(nameof(GetCommand), new { platformId = platformId, commandId = command.Id }, commandreturned);
        }
    }
}
