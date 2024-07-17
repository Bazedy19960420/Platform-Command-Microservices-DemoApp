using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _context;
        public CommandRepo(AppDbContext context)
        {
            _context = context;
        }
        public void CreatePlatform(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException();
            }
            _context.Platforms.Add(platform);
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (!PlatformExist(platformId))
            {
                throw new ArgumentNullException();
            }
            if (command == null)
            {
                throw new ArgumentNullException();
            }
            command.PlatformId = platformId;
            _context.Commands.Add(command);
        }



        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)
        {
            
            return _context.Commands.Where(c => c.PlatformId == platformId && c.Id == commandId).FirstOrDefault();
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return _context.Commands.Where(c => c.PlatformId == platformId).OrderBy(c => c.Platform.Name);
        }

        public bool PlatformExist(int platformId)
        {
            return _context.Platforms.Any(p => p.Id == platformId);

        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool ExternalPlatformExist(int ExternalId)
        {
            return _context.Platforms.Any(p => p.ExternalId == ExternalId);
        }
    }
}
