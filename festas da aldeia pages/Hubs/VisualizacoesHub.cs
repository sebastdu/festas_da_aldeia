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
        /// <summary>
        /// Dicionário thread-safe que associa o ID de cada evento ao número de utilizadores
        /// que o estão a visualizar em simultâneo no presente momento.
        /// </summary>
        private static readonly ConcurrentDictionary<string, int> _espectadores = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// Dicionário thread-safe que rastreia os IDs dos eventos que cada ID de ligação ativa (ConnectionId)
        /// está a consultar. Essencial para efetuar a limpeza automática em caso de desligamento abrupto.
        /// </summary>
        private static readonly ConcurrentDictionary<string, ConcurrentBag<string>> _conexoesEventos = new ConcurrentDictionary<string, ConcurrentBag<string>>();

        /// <summary>
        /// Regista a entrada de um utilizador na página de visualização de um evento.
        /// Adiciona a ligação ao grupo do evento e incrementa o contador global de visualizações deste.
        /// </summary>
        /// <param name="eventoId">O identificador único do evento que está a ser acedido.</param>
        /// <returns>Uma tarefa assíncrona que representa a operação.</returns>
        public async Task EntrarNaPagina(string eventoId)
        {
            // Adiciona a ligação atual do utilizador ao grupo exclusivo de comunicações do evento
            await Groups.AddToGroupAsync(Context.ConnectionId, eventoId);

            // Incrementa de forma segura (thread-safe) o contador de visualizações do evento correspondente
            _espectadores.AddOrUpdate(eventoId, 1, (key, value) => value + 1);

            // Associa a ligação ativa a este evento para efeitos de limpeza futura
            _conexoesEventos.AddOrUpdate(
                Context.ConnectionId,
                new ConcurrentBag<string> { eventoId },
                (key, bag) =>
                {
                    if (!bag.Contains(eventoId))
                    {
                        bag.Add(eventoId);
                    }
                    return bag;
                }
            );

            // Envia o novo número acumulado de espectadores para todos os clientes subscritos no grupo do evento
            await Clients.Group(eventoId).SendAsync("AtualizarContador", _espectadores.GetValueOrDefault(eventoId, 0));
        }

        /// <summary>
        /// Regista a saída voluntária de um utilizador da página de visualização de um evento (ex: ao retroceder).
        /// Remove a ligação do grupo e decrementa o contador global do evento.
        /// </summary>
        /// <param name="eventoId">O identificador único do evento que deixou de ser visualizado.</param>
        /// <returns>Uma tarefa assíncrona que representa a operação.</returns>
        public async Task SairDaPagina(string eventoId)
        {
            // Remove a ligação atual do grupo de comunicações em tempo real do evento
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventoId);

            // Decrementa de forma segura o contador, garantindo que o valor nunca seja inferior a zero
            _espectadores.AddOrUpdate(eventoId, 0, (key, value) => Math.Max(0, value - 1));

            // Notifica todos os utilizadores restantes que permaneçam no grupo do evento sobre a nova contagem
            await Clients.Group(eventoId).SendAsync("AtualizarContador", _espectadores.GetValueOrDefault(eventoId, 0));
        }

        /// <summary>
        /// Interceta o desligamento físico de uma ligação cliente com o servidor SignalR.
        /// Garante que quaisquer contadores de visualizações ativos associados a esta ligação sejam decrementados.
        /// </summary>
        /// <param name="exception">A exceção que causou o desligamento, se aplicável; caso contrário, null.</param>
        /// <returns>Uma tarefa assíncrona que representa a operação de encerramento da ligação.</returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Limpa as associações e decrementa os contadores dos eventos que este cliente ainda estivesse a ver
            if (_conexoesEventos.TryRemove(Context.ConnectionId, out var eventos))
            {
                foreach (var eventoId in eventos)
                {
                    _espectadores.AddOrUpdate(eventoId, 0, (key, value) => Math.Max(0, value - 1));
                    await Clients.Group(eventoId).SendAsync("AtualizarContador", _espectadores.GetValueOrDefault(eventoId, 0));
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
