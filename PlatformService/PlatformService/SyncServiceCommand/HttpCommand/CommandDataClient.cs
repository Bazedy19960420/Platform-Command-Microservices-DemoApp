using PlatformService.Dtos;
using System.Text;
using System.Text.Json;

namespace PlatformService.SyncServiceCommand.HttpCommand
{
    public class CommandDataClient : ICommandDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public CommandDataClient(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async Task SendPlatformToCommand(PlatformReadDto platform)
        {
            var httpContent = new StringContent(
                    JsonSerializer.Serialize(platform),
                    Encoding.UTF8,
                    "application/json"
                    );
            var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Success Platform Command");
            }
            else
            {
                Console.WriteLine("Failed Platform");
            }
        }
    }
}
