using System.Text.Json;
using Confluent.Kafka;
using Kafka.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kafka.Consumer.Console
{
    public class ConsumerService : BackgroundService
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ConsumerConfig _consumerConfig;
        private readonly ILogger<ConsumerService> _logger;
        private readonly ParametersModel _parameter;

        public ConsumerService(ILogger<ConsumerService> logger)
        {
            _logger = logger;
            _parameter = new ParametersModel();
            _consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = _parameter.BootstrapServer,
                GroupId = _parameter.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Aguardando mensagens.");
            _consumer.Subscribe(_parameter.TopicName);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() =>
                {
                    var result = _consumer.Consume(stoppingToken);
                    var pessoa = JsonSerializer.Deserialize<Pessoa>(result.Message.Value);

                    _logger.LogInformation($"Grupo: {_parameter.GroupId} \r\nNome: {pessoa?.Nome} \r\nIdade: {pessoa?.Idade}");
                });
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _consumer.Close();
            _logger.LogInformation("Aplicação parou, conexão fechada.");
            return Task.CompletedTask;
        }
    }
}