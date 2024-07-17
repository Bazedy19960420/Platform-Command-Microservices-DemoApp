using System.Text;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataService
{
    public class MessageBusSubscriper:BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private string _queueName;
        private IModel _channel;

        public MessageBusSubscriper(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            InitializeRabbitMQ();
            
        }
        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory(){HostName=_configuration["RabbitMQHost"],Port=int.Parse(_configuration["RabbitMQPort"])};
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange:"trigger",type:ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue:_queueName,exchange:"trigger",routingKey:"");
            System.Console.WriteLine("Lisiting On The Message Bus");
            _connection.ConnectionShutdown += RabbitMQShutDown; 
        }
        private void RabbitMQShutDown(object sender, ShutdownEventArgs e)
        {
            System.Console.WriteLine("---->Listining Shut Down");
        }
        public override void Dispose()
        {
            if(_channel.IsClosed)
            {
                _channel.Close();
                _connection.Close();
            }
            base.Dispose();
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer (_channel);
            consumer.Received += (ModuleHandle, e) => 
            {
                System.Console.WriteLine("--> Event Recieved");
                var body = e.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessor.ProcessEvent(notificationMessage);
            };
            _channel.BasicConsume(queue:_queueName,autoAck:true,consumer:consumer);
            return Task.CompletedTask;
        }
    }
}