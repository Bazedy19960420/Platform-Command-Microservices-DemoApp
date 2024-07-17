using PlatformService.Dtos;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PlatformService.AsyncDataService
{
    public class MessageBusClient : IMessageBusClient

    {
        private readonly IConfiguration _configuartion;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public MessageBusClient(IConfiguration configuration)
        {
            _configuartion = configuration;
            var factory = new ConnectionFactory() { HostName = _configuartion["RabbitMQHost"], Port = int.Parse(_configuartion["RabbitMQPort"]) };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_Connection_ShutDown;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CouldntConnect To the MessageBus{ex.Message}");
            }
        }
        private void RabbitMQ_Connection_ShutDown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Connection ShutDown");
        }
        private void sendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
            Console.WriteLine($"We Have Sent {message}");
        }
        public void Dispose()
        {
            Console.WriteLine("Message Disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platform)
        {
            var message = JsonSerializer.Serialize(platform);
            if (_connection.IsOpen)
            {
                Console.WriteLine("====>Connection Is Open");
                sendMessage(message);
            }
            else
            {

                Console.WriteLine("====> Connection Closed");
            }
        }
    }
}
