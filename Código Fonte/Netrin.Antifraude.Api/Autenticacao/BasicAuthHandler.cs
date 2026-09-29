namespace Netrin.Antifraude.Api.Autenticacao
{
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Encodings.Web;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Options;

    public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string Esquema = "Basic";
        private readonly IConfiguration _configuracao;

        public BasicAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> opcoes,
            ILoggerFactory logger, UrlEncoder encoder, IConfiguration configuracao)
            : base(opcoes, logger, encoder)
        {
            _configuracao = configuracao;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.NoResult());

            if (Request.Headers.Authorization.Count != 1
                || !AuthenticationHeaderValue.TryParse(Request.Headers.Authorization.ToString(), out var cabecalho)
                || !string.Equals(cabecalho.Scheme, Esquema, StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(cabecalho.Parameter))
            {
                return Task.FromResult(AuthenticateResult.Fail("Autenticação Basic inválida."));
            }

            string credenciais;
            try
            {
                credenciais = new UTF8Encoding(false, true).GetString(Convert.FromBase64String(cabecalho.Parameter));
            }
            catch (Exception erro) when (erro is FormatException or DecoderFallbackException)
            {
                return Task.FromResult(AuthenticateResult.Fail("Autenticação Basic inválida."));
            }

            var separador = credenciais.IndexOf(':');
            var usuario = _configuracao["BasicAuth:Usuario"];
            var senha = _configuracao["BasicAuth:Senha"];

            if (separador <= 0 || string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha))
                return Task.FromResult(AuthenticateResult.Fail("Credenciais inválidas."));

            var usuarioValido = Comparar(credenciais[..separador], usuario);
            var senhaValida = Comparar(credenciais[(separador + 1)..], senha);

            if (!usuarioValido || !senhaValida)
                return Task.FromResult(AuthenticateResult.Fail("Credenciais inválidas."));

            var identidade = new ClaimsIdentity([new Claim(ClaimTypes.Name, usuario)], Esquema);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identidade), Esquema);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.Headers.WWWAuthenticate = "Basic realm=\"Antifraude\", charset=\"UTF-8\"";
            return Task.CompletedTask;
        }

        private static bool Comparar(string recebido, string esperado)
        {
            return CryptographicOperations.FixedTimeEquals(
                SHA256.HashData(Encoding.UTF8.GetBytes(recebido)),
                SHA256.HashData(Encoding.UTF8.GetBytes(esperado)));
        }
    }
}
