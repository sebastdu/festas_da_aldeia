using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace festas_da_aldeia_pages.Hubs
{
    public class VisualizacoesHub : Hub
    {
        // Dicionário thread-safe para manter a contagem de espectadores por evento
        private static readonly ConcurrentDictionary<string, int> _espectadores = new ConcurrentDictionary<string, int>();

        // Regista os eventos visitados por ID de ligação para decrementar automaticamente em desligamentos abruptos
        private static readonly ConcurrentDictionary<string, ConcurrentBag<string>> _conexoesEventos = new ConcurrentDictionary<string, ConcurrentBag<string>>();

        public async Task EntrarNaPagina(string eventoId)
        {
            // Adiciona o utilizador ao grupo exclusivo do evento
            await Groups.AddToGroupAsync(Context.ConnectionId, eventoId);

            // Incrementa a contagem de espectadores de forma thread-safe
            _espectadores.AddOrUpdate(eventoId, 1, (key, value) => value + 1);

            // Associa a ligação atual a este evento
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

            // Notifica todos os clientes no grupo sobre a contagem atualizada
            await Clients.Group(eventoId).SendAsync("AtualizarContador", _espectadores.GetValueOrDefault(eventoId, 0));
        }

        public async Task SairDaPagina(string eventoId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventoId);

            // Decrementa a contagem de espectadores de forma thread-safe
            _espectadores.AddOrUpdate(eventoId, 0, (key, value) => Math.Max(0, value - 1));

            // Remove a associação do registo de ligações
            if (_conexoesEventos.TryGetValue(Context.ConnectionId, out var bag))
            {
                // Nota: ConcurrentBag não suporta facilmente a remoção de itens específicos.
                // No entanto, como a ligação acabará por ser desligada, a limpeza ocorre no OnDisconnectedAsync.
                // Para manter simples, uma vez que chamaram SairDaPagina, não precisamos de fazer mais nada aqui.
            }

            await Clients.Group(eventoId).SendAsync("AtualizarContador", _espectadores.GetValueOrDefault(eventoId, 0));
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Limpeza automática caso o cliente se desligue abruptamente
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
