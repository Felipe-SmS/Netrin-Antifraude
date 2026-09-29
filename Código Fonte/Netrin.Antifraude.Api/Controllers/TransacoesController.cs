namespace Netrin.Antifraude.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Netrin.Antifraude.Api.Autenticacao;
    using Netrin.Antifraude.Application.Commands.Transacoes;
    using Netrin.Antifraude.Core.Dtos;
    using MediatR;
    using Netrin.Antifraude.Application.Queries.Transacoes;
    using Netrin.Antifraude.Application.Mapeamentos;

    [ApiController]
    [Route("api/transacoes")]
    [Authorize(AuthenticationSchemes = BasicAuthHandler.Esquema)]
    public class TransacoesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly TransacaoQuery _query;

        public TransacoesController(IMediator mediator, TransacaoQuery query)
        {
            _mediator = mediator;
            _query = query;
        }

        [HttpPost]
        public async Task<ActionResult<TransacaoDto>> Receber(ReceberNovaTransacao command, CancellationToken cancelamento)
        {
            try
            {
                var transacao = await _mediator.Send(new ReceberTransacaoCommand { Transacao = command }, cancelamento);
                return transacao.Avaliacao is null ? Accepted(transacao) : Ok(transacao);
            }
            catch (ConflitoIdempotenciaException erro)
            {
                return Conflict(new { mensagem = erro.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TransacaoDto>> Obter(int id)
        {
            var transacao = await _query.ObterPeloIdAsync(id);
            return transacao is null ? NotFound() : Ok(transacao.ParaDto());
        }
    }
}
