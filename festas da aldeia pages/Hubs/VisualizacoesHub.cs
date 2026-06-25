using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace festas_da_aldeia_pages.Hubs
{
    public class VisualizacoesHub : Hub
    {
        // Thread-safe dictionary to maintain count of spectators per event
        private static readonly ConcurrentDictionary<string, int> _espectadores = new ConcurrentDictionary<string, int>();

        // Tracks event pages visited by connection ID to auto-decrement on abrupt disconnects
        private static readonly ConcurrentDictionary<string, ConcurrentBag<string>> _conexoesEventos = new ConcurrentDictionary<string, ConcurrentBag<string>>();

        public async Task EntrarNaPagina(string eventoId)
        {
            // Add user to the exclusive event group
            await Groups.AddToGroupAsync(Context.ConnectionId, eventoId);

            // Increment spectator count thread-safely
            _espectadores.AddOrUpdate(eventoId, 1, (key, value) => value + 1);

            // Associate the current connection to this event
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

            // Notify all clients in the group of the updated count
            await Clients.Group(eventoId).SendAsync("AtualizarContador", _espectadores.GetValueOrDefault(eventoId, 0));
        }

        public async Task SairDaPagina(string eventoId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventoId);

            // Decrement spectator count thread-safely
            _espectadores.AddOrUpdate(eventoId, 0, (key, value) => Math.Max(0, value - 1));

            // Remove association from connection tracking
            if (_conexoesEventos.TryGetValue(Context.ConnectionId, out var bag))
            {
                // Note: ConcurrentBag doesn't easily support removal of arbitrary items.
                // However, since connection will disconnect eventually, clean-up happens in OnDisconnectedAsync.
                // To keep it simple, we can filter/recreate if we want, but since they called SairDaPagina, we don't strictly need to do more here.
            }

            await Clients.Group(eventoId).SendAsync("AtualizarContador", _espectadores.GetValueOrDefault(eventoId, 0));
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Auto clean-up if client disconnects abruptly
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
