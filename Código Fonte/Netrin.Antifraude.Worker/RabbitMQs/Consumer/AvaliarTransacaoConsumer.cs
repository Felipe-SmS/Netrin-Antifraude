namespace Netrin.Antifraude.Worker.RabbitMQs.Consumer
{
    using MassTransit;
    using MediatR;
    using Netrin.Antifraude.Application.Commands.Transacoes;
    using Netrin.Antifraude.Core.RabbitMq.Domain.Entities;

    public class AvaliarTransacaoConsumer : IConsumer<TransacaoRecebida>
    {
        private readonly IMediator _mediator;

        public AvaliarTransacaoConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<TransacaoRecebida> context)
        {
            if (context.Message.TransacaoId <= 0)
                throw new ArgumentException("O ID da transação é obrigatório.");

            await _mediator.Send(new AvaliarTransacaoCommand
            {
                TransacaoId = context.Message.TransacaoId
            }, context.CancellationToken);
        }
    }
}
