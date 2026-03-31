using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Data;
using festas_da_aldeia.Models;
using festas_da_aldeia.Models.Api;

namespace festas_da_aldeia.Controllers.ApiOperations
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiArtistasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiArtistasController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obter todos os artistas
        /// </summary>
        /// <returns>Lista de todos os artistas</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArtistaDto>>> GetArtistas()
        {
            var artistas = await _context.Artistas.ToListAsync();

            var artistasDto = artistas.Select(a => new ArtistaDto
            {
                IdArtista = a.IdArtista,
                Nome = a.Nome,
                Biografia = a.Biografia,
                Contacto = a.Contacto,
                LinkFotoPerfil = a.LinkFotoPerfil
            }).ToList();

            return Ok(artistasDto);
        }

        /// <summary>
        /// Obter um artista por ID
        /// </summary>
        /// <param name="id">ID do artista</param>
        /// <returns>O artista solicitado</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ArtistaDto>> GetArtista(int id)
        {
            var artista = await _context.Artistas.FindAsync(id);

            if (artista == null)
            {
                return NotFound(new { mensagem = "Artista não encontrado" });
            }

            var artistaDto = new ArtistaDto
            {
                IdArtista = artista.IdArtista,
                Nome = artista.Nome,
                Biografia = artista.Biografia,
                Contacto = artista.Contacto,
                LinkFotoPerfil = artista.LinkFotoPerfil
            };

            return Ok(artistaDto);
        }

        /// <summary>
        /// Criar um novo artista
        /// </summary>
        /// <param name="createDto">Dados do novo artista</param>
        /// <returns>O artista criado</returns>
        [HttpPost]
        public async Task<ActionResult<ArtistaDto>> PostArtista(ArtistaCreateDto createDto)
        {
            // Validar se já existe um artista com o mesmo nome
            var artistaExists = await _context.Artistas
                .FirstOrDefaultAsync(a => a.Nome.Equals(createDto.Nome));

            if (artistaExists != null)
            {
                return BadRequest(new { mensagem = "Já existe um artista com este nome" });
            }

            var artista = new Artista
            {
                Nome = createDto.Nome,
                Biografia = createDto.Biografia,
                Contacto = createDto.Contacto,
                LinkFotoPerfil = createDto.LinkFotoPerfil
            };

            _context.Artistas.Add(artista);
            await _context.SaveChangesAsync();

            var artistaDto = new ArtistaDto
            {
                IdArtista = artista.IdArtista,
                Nome = artista.Nome,
                Biografia = artista.Biografia,
                Contacto = artista.Contacto,
                LinkFotoPerfil = artista.LinkFotoPerfil
            };

            return CreatedAtAction("GetArtista", new { id = artista.IdArtista }, artistaDto);
        }

        /// <summary>
        /// Atualizar um artista existente
        /// </summary>
        /// <param name="id">ID do artista a atualizar</param>
        /// <param name="updateDto">Novos dados do artista</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArtista(int id, ArtistaCreateDto updateDto)
        {
            var artista = await _context.Artistas.FindAsync(id);

            if (artista == null)
            {
                return NotFound(new { mensagem = "Artista não encontrado" });
            }

            // Validar duplicação de nome (se foi alterado)
            if (!artista.Nome.Equals(updateDto.Nome))
            {
                var artistaExists = await _context.Artistas
                    .FirstOrDefaultAsync(a => a.Nome.Equals(updateDto.Nome));

                if (artistaExists != null)
                {
                    return BadRequest(new { mensagem = "Já existe um artista com este nome" });
                }
            }

            artista.Nome = updateDto.Nome;
            artista.Biografia = updateDto.Biografia;
            artista.Contacto = updateDto.Contacto;
            artista.LinkFotoPerfil = updateDto.LinkFotoPerfil;

            _context.Entry(artista).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtistaExists(id))
                {
                    return NotFound(new { mensagem = "Artista não encontrado" });
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Eliminar um artista
        /// </summary>
        /// <param name="id">ID do artista a eliminar</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArtista(int id)
        {
            var artista = await _context.Artistas.FindAsync(id);

            if (artista == null)
            {
                return NotFound(new { mensagem = "Artista não encontrado" });
            }

            _context.Artistas.Remove(artista);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArtistaExists(int id)
        {
            return _context.Artistas.Any(a => a.IdArtista == id);
        }
    }
}
