using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Data;
using festas_da_aldeia.Models;
using festas_da_aldeia.Models.Api;

namespace festas_da_aldeia.Controllers.ApiOperations
{
    /// <summary>
    /// Controlador da API REST para operações CRUD sobre Atuações (Cartaz).
    /// Gerido por endpoints HTTP.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCartazesController : ControllerBase
    {
        /// <summary>
        /// Contexto de acesso à base de dados.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância do controlador de API de Cartazes.
        /// </summary>
        /// <param name="context">O contexto da base de dados da aplicação.</param>
        public ApiCartazesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obter todos os cartazes (atuações)
        /// </summary>
        /// <returns>Lista de todos os cartazes com evento e artista</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartazDto>>> GetCartazes()
        {
            var cartazes = await _context.Cartazes
                .Include(c => c.Evento)
                    .ThenInclude(e => e.Local)
                .Include(c => c.Artista)
                .ToListAsync();

            var cartazesDto = cartazes.Select(c => new CartazDto
            {
                IdCartaz = c.IdCartaz,
                DataHoraAtuacao = c.DataHoraAtuacao,
                DuracaoMinutos = c.DuracaoMinutos,
                IdEvento = c.IdEvento,
                IdArtista = c.IdArtista,
                Evento = c.Evento != null ? new EventoDto
                {
                    IdEvento = c.Evento.IdEvento,
                    Nome = c.Evento.Nome,
                    Descricao = c.Evento.Descricao,
                    DataInicio = c.Evento.DataInicio,
                    DataFim = c.Evento.DataFim,
                    Patrocinador = c.Evento.Patrocinador,
                    IdLocal = c.Evento.IdLocal,
                    Local = c.Evento.Local != null ? new LocalDto
                    {
                        IdLocal = c.Evento.Local.IdLocal,
                        Nome = c.Evento.Local.Nome,
                        Descricao = c.Evento.Local.Descricao,
                        Outside = c.Evento.Local.Outside,
                        Coordenadas = c.Evento.Local.Coordenadas
                    } : null
                } : null,
                Artista = c.Artista != null ? new ArtistaDto
                {
                    IdArtista = c.Artista.IdArtista,
                    Nome = c.Artista.Nome,
                    Biografia = c.Artista.Biografia,
                    Contacto = c.Artista.Contacto,
                    LinkFotoPerfil = c.Artista.LinkFotoPerfil
                } : null
            }).ToList();

            return Ok(cartazesDto);
        }

        /// <summary>
        /// Obter um cartaz por ID
        /// </summary>
        /// <param name="id">ID do cartaz</param>
        /// <returns>O cartaz solicitado com evento e artista</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CartazDto>> GetCartaz(int id)
        {
            var cartaz = await _context.Cartazes
                .Include(c => c.Evento)
                    .ThenInclude(e => e.Local)
                .Include(c => c.Artista)
                .FirstOrDefaultAsync(c => c.IdCartaz == id);

            if (cartaz == null)
            {
                return NotFound(new { mensagem = "Cartaz não encontrado" });
            }

            var cartazDto = new CartazDto
            {
                IdCartaz = cartaz.IdCartaz,
                DataHoraAtuacao = cartaz.DataHoraAtuacao,
                DuracaoMinutos = cartaz.DuracaoMinutos,
                IdEvento = cartaz.IdEvento,
                IdArtista = cartaz.IdArtista,
                Evento = cartaz.Evento != null ? new EventoDto
                {
                    IdEvento = cartaz.Evento.IdEvento,
                    Nome = cartaz.Evento.Nome,
                    Descricao = cartaz.Evento.Descricao,
                    DataInicio = cartaz.Evento.DataInicio,
                    DataFim = cartaz.Evento.DataFim,
                    Patrocinador = cartaz.Evento.Patrocinador,
                    IdLocal = cartaz.Evento.IdLocal,
                    Local = cartaz.Evento.Local != null ? new LocalDto
                    {
                        IdLocal = cartaz.Evento.Local.IdLocal,
                        Nome = cartaz.Evento.Local.Nome,
                        Descricao = cartaz.Evento.Local.Descricao,
                        Outside = cartaz.Evento.Local.Outside,
                        Coordenadas = cartaz.Evento.Local.Coordenadas
                    } : null
                } : null,
                Artista = cartaz.Artista != null ? new ArtistaDto
                {
                    IdArtista = cartaz.Artista.IdArtista,
                    Nome = cartaz.Artista.Nome,
                    Biografia = cartaz.Artista.Biografia,
                    Contacto = cartaz.Artista.Contacto,
                    LinkFotoPerfil = cartaz.Artista.LinkFotoPerfil
                } : null
            };

            return Ok(cartazDto);
        }

        /// <summary>
        /// Obter todos os cartazes de um evento específico
        /// </summary>
        /// <param name="idEvento">ID do evento</param>
        /// <returns>Lista de cartazes do evento</returns>
        [HttpGet("evento/{idEvento}")]
        public async Task<ActionResult<IEnumerable<CartazDto>>> GetCartazesByEvento(int idEvento)
        {
            var cartazes = await _context.Cartazes
                .Where(c => c.IdEvento == idEvento)
                .Include(c => c.Artista)
                .ToListAsync();

            if (!cartazes.Any())
            {
                return NotFound(new { mensagem = "Nenhum cartaz encontrado para este evento" });
            }

            var cartazesDto = cartazes.Select(c => new CartazDto
            {
                IdCartaz = c.IdCartaz,
                DataHoraAtuacao = c.DataHoraAtuacao,
                DuracaoMinutos = c.DuracaoMinutos,
                IdEvento = c.IdEvento,
                IdArtista = c.IdArtista,
                Artista = c.Artista != null ? new ArtistaDto
                {
                    IdArtista = c.Artista.IdArtista,
                    Nome = c.Artista.Nome,
                    Biografia = c.Artista.Biografia,
                    Contacto = c.Artista.Contacto,
                    LinkFotoPerfil = c.Artista.LinkFotoPerfil
                } : null
            }).ToList();

            return Ok(cartazesDto);
        }

        /// <summary>
        /// Criar um novo cartaz (atuação)
        /// </summary>
        /// <param name="createDto">Dados do novo cartaz</param>
        /// <returns>O cartaz criado</returns>
        [HttpPost]
        public async Task<ActionResult<CartazDto>> PostCartaz(CartazCreateDto createDto)
        {
            // Validar se o evento existe
            var eventoExists = await _context.Eventos.AnyAsync(e => e.IdEvento == createDto.IdEvento);
            if (!eventoExists)
            {
                return BadRequest(new { mensagem = "O evento especificado não existe" });
            }

            // Validar se o artista existe
            var artistaExists = await _context.Artistas.AnyAsync(a => a.IdArtista == createDto.IdArtista);
            if (!artistaExists)
            {
                return BadRequest(new { mensagem = "O artista especificado não existe" });
            }

            // Validar se já existe uma atuação do mesmo artista no mesmo evento
            var cartazExists = await _context.Cartazes
                .FirstOrDefaultAsync(c => c.IdEvento == createDto.IdEvento && c.IdArtista == createDto.IdArtista);

            if (cartazExists != null)
            {
                return BadRequest(new { mensagem = "Este artista já tem uma atuação agendada neste evento" });
            }

            var cartaz = new Cartaz
            {
                DataHoraAtuacao = createDto.DataHoraAtuacao,
                DuracaoMinutos = createDto.DuracaoMinutos,
                IdEvento = createDto.IdEvento,
                IdArtista = createDto.IdArtista
            };

            _context.Cartazes.Add(cartaz);
            await _context.SaveChangesAsync();

            var cartazDto = new CartazDto
            {
                IdCartaz = cartaz.IdCartaz,
                DataHoraAtuacao = cartaz.DataHoraAtuacao,
                DuracaoMinutos = cartaz.DuracaoMinutos,
                IdEvento = cartaz.IdEvento,
                IdArtista = cartaz.IdArtista
            };

            return CreatedAtAction("GetCartaz", new { id = cartaz.IdCartaz }, cartazDto);
        }

        /// <summary>
        /// Atualizar um cartaz existente
        /// </summary>
        /// <param name="id">ID do cartaz a atualizar</param>
        /// <param name="updateDto">Novos dados do cartaz</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartaz(int id, CartazCreateDto updateDto)
        {
            var cartaz = await _context.Cartazes.FindAsync(id);

            if (cartaz == null)
            {
                return NotFound(new { mensagem = "Cartaz não encontrado" });
            }

            // Validar se o evento existe
            var eventoExists = await _context.Eventos.AnyAsync(e => e.IdEvento == updateDto.IdEvento);
            if (!eventoExists)
            {
                return BadRequest(new { mensagem = "O evento especificado não existe" });
            }

            // Validar se o artista existe
            var artistaExists = await _context.Artistas.AnyAsync(a => a.IdArtista == updateDto.IdArtista);
            if (!artistaExists)
            {
                return BadRequest(new { mensagem = "O artista especificado não existe" });
            }

            // Validar se outra atuação do mesmo artista já existe neste evento
            var cartazExists = await _context.Cartazes
                .FirstOrDefaultAsync(c => c.IdCartaz != id && 
                    c.IdEvento == updateDto.IdEvento && 
                    c.IdArtista == updateDto.IdArtista);

            if (cartazExists != null)
            {
                return BadRequest(new { mensagem = "Este artista já tem uma atuação agendada neste evento" });
            }

            cartaz.DataHoraAtuacao = updateDto.DataHoraAtuacao;
            cartaz.DuracaoMinutos = updateDto.DuracaoMinutos;
            cartaz.IdEvento = updateDto.IdEvento;
            cartaz.IdArtista = updateDto.IdArtista;

            _context.Entry(cartaz).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartazExists(id))
                {
                    return NotFound(new { mensagem = "Cartaz não encontrado" });
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Eliminar um cartaz
        /// </summary>
        /// <param name="id">ID do cartaz a eliminar</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartaz(int id)
        {
            var cartaz = await _context.Cartazes.FindAsync(id);

            if (cartaz == null)
            {
                return NotFound(new { mensagem = "Cartaz não encontrado" });
            }

            _context.Cartazes.Remove(cartaz);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartazExists(int id)
        {
            return _context.Cartazes.Any(c => c.IdCartaz == id);
        }
    }
}
