using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace festas_da_aldeia_pages.Hubs
{
    /// <summary>
    /// Hub do SignalR encarregue de monitorizar e gerir em tempo real a presença
    /// de utilizadores nas páginas de detalhes dos eventos.
    /// Estabelece uma ligação bidirecional contínua entre o cliente (navegador) e o servidor.
    /// </summary>
    public class VisualizacoesHub : Hub
    {
        // Dicionário thread-safe que associa cada ConnectionId física ao tuplo (UserId, EventoId)
        private static readonly ConcurrentDictionary<string, (string UserId, string EventoId)> _conexoes = 
            new ConcurrentDictionary<string, (string UserId, string EventoId)>();

        /// <summary>
        /// Regista a entrada de um utilizador na página de visualização de um evento.
        /// Adiciona a ligação ao grupo do evento e recalcula o número de utilizadores únicos presentes.
        /// </summary>
        /// <param name="eventoId">O identificador único do evento que está a ser acedido.</param>
        /// <returns>Uma tarefa assíncrona que representa a operação.</returns>
        public async Task EntrarNaPagina(string eventoId)
        {
            // Adiciona a ligação atual do utilizador ao grupo exclusivo de comunicações do evento
            await Groups.AddToGroupAsync(Context.ConnectionId, eventoId);

            // Obtém o identificador único do utilizador autenticado
            string userId = Context.UserIdentifier ?? Context.User?.Identity?.Name ?? "Anonimo";

            // Regista a ligação ativa associada ao utilizador e ao evento
            _conexoes[Context.ConnectionId] = (userId, eventoId);

            // Recalcula e envia o número atualizado de utilizadores únicos na página
            await NotificarAtualizacaoGrupo(eventoId);
        }

        /// <summary>
        /// Regista a saída voluntária de um utilizador da página de visualização de um evento (ex: ao retroceder).
        /// Remove a ligação do grupo e delega a remoção e recálculo da contagem.
        /// </summary>
        /// <param name="eventoId">O identificador único do evento que deixou de ser visualizado.</param>
        /// <returns>Uma tarefa assíncrona que representa a operação.</returns>
        public async Task SairDaPagina(string eventoId)
        {
            // Remove a ligação atual do grupo de comunicações em tempo real do evento
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventoId);

            // Processa a saída e recalcula a contagem de forma segura
            await ProcessarSaida(Context.ConnectionId);
        }

        /// <summary>
        /// Interceta o desligamento físico de uma ligação cliente com o servidor SignalR.
        /// Garante a limpeza em segundo plano caso a ligação se perca sem chamada voluntária.
        /// </summary>
        /// <param name="exception">A exceção que causou o desligamento, se aplicável.</param>
        /// <returns>Uma tarefa assíncrona que representa a operação de encerramento da ligação.</returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Processa a saída e recalcula a contagem de forma segura
            await ProcessarSaida(Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Processa a remoção de uma ligação ativa e atualiza o contador dos utilizadores restantes.
        /// Centralizado para evitar duplo decremento em caso de sobreposição de chamadas.
        /// </summary>
        /// <param name="connectionId">O ID da ligação que se desligou ou saiu.</param>
        private async Task ProcessarSaida(string connectionId)
        {
            // TryRemove garante que apenas a primeira chamada (seja SairDaPagina ou OnDisconnectedAsync) executa a limpeza
            if (_conexoes.TryRemove(connectionId, out var info))
            {
                // Recalcula e notifica o grupo do evento correspondente
                await NotificarAtualizacaoGrupo(info.EventoId);
            }
        }

        /// <summary>
        /// Método auxiliar para calcular os utilizadores únicos e notificar o grupo do evento.
        /// </summary>
        /// <param name="eventoId">O ID do evento a atualizar.</param>
        private async Task NotificarAtualizacaoGrupo(string eventoId)
        {
            // Calcula dinamicamente a contagem de utilizadores únicos para o evento (agrupando por UserId)
            int totalEspectadoresUnicos = _conexoes.Values
                .Where(x => x.EventoId == eventoId)
                .Select(x => x.UserId)
                .Distinct()
                .Count();

            // Envia o novo valor para todos os membros subscritos no grupo do evento
            await Clients.Group(eventoId).SendAsync("AtualizarContador", totalEspectadoresUnicos);
        }
    }
}
