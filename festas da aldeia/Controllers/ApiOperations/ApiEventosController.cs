using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Data;
using festas_da_aldeia.Models;
using festas_da_aldeia.Models.Api;

namespace festas_da_aldeia.Controllers.ApiOperations
{
    /// <summary>
    /// Controlador da API REST para operações CRUD sobre Eventos.
    /// Gerido por endpoints HTTP.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ApiEventosController : ControllerBase
    {
        /// <summary>
        /// Contexto de acesso à base de dados.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância do controlador de API de Eventos.
        /// </summary>
        /// <param name="context">O contexto da base de dados da aplicação.</param>
        public ApiEventosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obter todos os eventos
        /// </summary>
        /// <returns>Lista de todos os eventos com seus locais</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoDto>>> GetEventos()
        {
            var eventos = await _context.Eventos
                .Include(e => e.Local)
                .Include(e => e.Cartazes)
                    .ThenInclude(c => c.Artista)
                .ToListAsync();

            var eventosDto = eventos.Select(e => new EventoDto
            {
                IdEvento = e.IdEvento,
                Nome = e.Nome,
                Descricao = e.Descricao,
                DataInicio = e.DataInicio,
                DataFim = e.DataFim,
                Patrocinador = e.Patrocinador,
                IdLocal = e.IdLocal,
                Local = e.Local != null ? new LocalDto
                {
                    IdLocal = e.Local.IdLocal,
                    Nome = e.Local.Nome,
                    Descricao = e.Local.Descricao,
                    Outside = e.Local.Outside,
                    Coordenadas = e.Local.Coordenadas
                } : null,
                Cartazes = e.Cartazes.Select(c => new CartazDto
                {
                    IdCartaz = c.IdCartaz,
                    DataHoraAtuacao = c.DataHoraAtuacao,
                    DuracaoMinutos = c.DuracaoMinutos,
                    IdEvento = c.IdEvento,
                    IdArtista = c.IdArtista,
                    Artista = new ArtistaDto
                    {
                        IdArtista = c.Artista.IdArtista,
                        Nome = c.Artista.Nome,
                        Biografia = c.Artista.Biografia,
                        Contacto = c.Artista.Contacto,
                        LinkFotoPerfil = c.Artista.LinkFotoPerfil
                    }
                }).ToList()
            }).ToList();

            return Ok(eventosDto);
        }

        /// <summary>
        /// Obter um evento por ID com todos os seus dados relacionados
        /// </summary>
        /// <param name="id">ID do evento</param>
        /// <returns>O evento solicitado com local e artistas</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EventoDto>> GetEvento(int id)
        {
            var evento = await _context.Eventos
                .Include(e => e.Local)
                .Include(e => e.Cartazes)
                    .ThenInclude(c => c.Artista)
                .FirstOrDefaultAsync(e => e.IdEvento == id);

            if (evento == null)
            {
                return NotFound(new { mensagem = "Evento não encontrado" });
            }

            var eventoDto = new EventoDto
            {
                IdEvento = evento.IdEvento,
                Nome = evento.Nome,
                Descricao = evento.Descricao,
                DataInicio = evento.DataInicio,
                DataFim = evento.DataFim,
                Patrocinador = evento.Patrocinador,
                IdLocal = evento.IdLocal,
                Local = evento.Local != null ? new LocalDto
                {
                    IdLocal = evento.Local.IdLocal,
                    Nome = evento.Local.Nome,
                    Descricao = evento.Local.Descricao,
                    Outside = evento.Local.Outside,
                    Coordenadas = evento.Local.Coordenadas
                } : null,
                Cartazes = evento.Cartazes.Select(c => new CartazDto
                {
                    IdCartaz = c.IdCartaz,
                    DataHoraAtuacao = c.DataHoraAtuacao,
                    DuracaoMinutos = c.DuracaoMinutos,
                    IdEvento = c.IdEvento,
                    IdArtista = c.IdArtista,
                    Artista = new ArtistaDto
                    {
                        IdArtista = c.Artista.IdArtista,
                        Nome = c.Artista.Nome,
                        Biografia = c.Artista.Biografia,
                        Contacto = c.Artista.Contacto,
                        LinkFotoPerfil = c.Artista.LinkFotoPerfil
                    }
                }).ToList()
            };

            return Ok(eventoDto);
        }

        /// <summary>
        /// Criar um novo evento
        /// </summary>
        /// <param name="createDto">Dados do novo evento</param>
        /// <returns>O evento criado</returns>
        [HttpPost]
        public async Task<ActionResult<EventoDto>> PostEvento(EventoCreateDto createDto)
        {
            // Validar se o local existe
            var localExists = await _context.Locais.AnyAsync(l => l.IdLocal == createDto.IdLocal);
            if (!localExists)
            {
                return BadRequest(new { mensagem = "O local especificado não existe" });
            }

            // Validar se já existe um evento com o mesmo nome
            var eventoExists = await _context.Eventos
                .FirstOrDefaultAsync(e => e.Nome.Equals(createDto.Nome));

            if (eventoExists != null)
            {
                return BadRequest(new { mensagem = "Já existe um evento com este nome" });
            }

            // Validar datas
            if (createDto.DataInicio >= createDto.DataFim)
            {
                return BadRequest(new { mensagem = "A data de início deve ser anterior à data de fim" });
            }

            var evento = new Evento
            {
                Nome = createDto.Nome,
                Descricao = createDto.Descricao,
                DataInicio = createDto.DataInicio,
                DataFim = createDto.DataFim,
                Patrocinador = createDto.Patrocinador,
                IdLocal = createDto.IdLocal
            };

            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();

            var eventoDto = new EventoDto
            {
                IdEvento = evento.IdEvento,
                Nome = evento.Nome,
                Descricao = evento.Descricao,
                DataInicio = evento.DataInicio,
                DataFim = evento.DataFim,
                Patrocinador = evento.Patrocinador,
                IdLocal = evento.IdLocal,
                Cartazes = []
            };

            return CreatedAtAction("GetEvento", new { id = evento.IdEvento }, eventoDto);
        }

        /// <summary>
        /// Atualizar um evento existente
        /// </summary>
        /// <param name="id">ID do evento a atualizar</param>
        /// <param name="updateDto">Novos dados do evento</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvento(int id, EventoCreateDto updateDto)
        {
            var evento = await _context.Eventos.FindAsync(id);

            if (evento == null)
            {
                return NotFound(new { mensagem = "Evento não encontrado" });
            }

            // Validar se o local existe
            var localExists = await _context.Locais.AnyAsync(l => l.IdLocal == updateDto.IdLocal);
            if (!localExists)
            {
                return BadRequest(new { mensagem = "O local especificado não existe" });
            }

            // Validar duplicação de nome (se foi alterado)
            if (!evento.Nome.Equals(updateDto.Nome))
            {
                var eventoExists = await _context.Eventos
                    .FirstOrDefaultAsync(e => e.Nome.Equals(updateDto.Nome));

                if (eventoExists != null)
                {
                    return BadRequest(new { mensagem = "Já existe um evento com este nome" });
                }
            }

            // Validar datas
            if (updateDto.DataInicio >= updateDto.DataFim)
            {
                return BadRequest(new { mensagem = "A data de início deve ser anterior à data de fim" });
            }

            evento.Nome = updateDto.Nome;
            evento.Descricao = updateDto.Descricao;
            evento.DataInicio = updateDto.DataInicio;
            evento.DataFim = updateDto.DataFim;
            evento.Patrocinador = updateDto.Patrocinador;
            evento.IdLocal = updateDto.IdLocal;

            _context.Entry(evento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventoExists(id))
                {
                    return NotFound(new { mensagem = "Evento não encontrado" });
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Eliminar um evento
        /// </summary>
        /// <param name="id">ID do evento a eliminar</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);

            if (evento == null)
            {
                return NotFound(new { mensagem = "Evento não encontrado" });
            }

            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.IdEvento == id);
        }
    }
}
