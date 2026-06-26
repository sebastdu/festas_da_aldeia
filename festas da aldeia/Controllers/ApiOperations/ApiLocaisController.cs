using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Data;
using festas_da_aldeia.Models;
using festas_da_aldeia.Models.Api;

namespace festas_da_aldeia.Controllers.ApiOperations
{
    /// <summary>
    /// Controlador da API REST para operações CRUD sobre Locais.
    /// Gerido por endpoints HTTP.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ApiLocaisController : ControllerBase
    {
        /// <summary>
        /// Contexto de acesso à base de dados.
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância do controlador de API de Locais.
        /// </summary>
        /// <param name="context">O contexto da base de dados da aplicação.</param>
        public ApiLocaisController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obter todos os locais
        /// </summary>
        /// <returns>Lista de todos os locais</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocalDto>>> GetLocais()
        {
            var locais = await _context.Locais.ToListAsync();

            var locaisDto = locais.Select(l => new LocalDto
            {
                IdLocal = l.IdLocal,
                Nome = l.Nome,
                Descricao = l.Descricao,
                Outside = l.Outside,
                Coordenadas = l.Coordenadas
            }).ToList();

            return Ok(locaisDto);
        }

        /// <summary>
        /// Obter um local por ID
        /// </summary>
        /// <param name="id">ID do local</param>
        /// <returns>O local solicitado</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<LocalDto>> GetLocal(int id)
        {
            var local = await _context.Locais.FindAsync(id);

            if (local == null)
            {
                return NotFound(new { mensagem = "Local não encontrado" });
            }

            var localDto = new LocalDto
            {
                IdLocal = local.IdLocal,
                Nome = local.Nome,
                Descricao = local.Descricao,
                Outside = local.Outside,
                Coordenadas = local.Coordenadas
            };

            return Ok(localDto);
        }

        /// <summary>
        /// Criar um novo local
        /// </summary>
        /// <param name="createDto">Dados do novo local</param>
        /// <returns>O local criado</returns>
        [HttpPost]
        public async Task<ActionResult<LocalDto>> PostLocal(LocalCreateDto createDto)
        {
            // Validar se já existe um local com o mesmo nome
            var localExists = await _context.Locais
                .FirstOrDefaultAsync(l => l.Nome.Equals(createDto.Nome));

            if (localExists != null)
            {
                return BadRequest(new { mensagem = "Já existe um local com este nome" });
            }

            var local = new Local
            {
                Nome = createDto.Nome,
                Descricao = createDto.Descricao,
                Outside = createDto.Outside,
                Coordenadas = createDto.Coordenadas
            };

            _context.Locais.Add(local);
            await _context.SaveChangesAsync();

            var localDto = new LocalDto
            {
                IdLocal = local.IdLocal,
                Nome = local.Nome,
                Descricao = local.Descricao,
                Outside = local.Outside,
                Coordenadas = local.Coordenadas
            };

            return CreatedAtAction("GetLocal", new { id = local.IdLocal }, localDto);
        }

        /// <summary>
        /// Atualizar um local existente
        /// </summary>
        /// <param name="id">ID do local a atualizar</param>
        /// <param name="updateDto">Novos dados do local</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocal(int id, LocalCreateDto updateDto)
        {
            var local = await _context.Locais.FindAsync(id);

            if (local == null)
            {
                return NotFound(new { mensagem = "Local não encontrado" });
            }

            // Validar duplicação de nome (se foi alterado)
            if (!local.Nome.Equals(updateDto.Nome))
            {
                var localExists = await _context.Locais
                    .FirstOrDefaultAsync(l => l.Nome.Equals(updateDto.Nome));

                if (localExists != null)
                {
                    return BadRequest(new { mensagem = "Já existe um local com este nome" });
                }
            }

            local.Nome = updateDto.Nome;
            local.Descricao = updateDto.Descricao;
            local.Outside = updateDto.Outside;
            local.Coordenadas = updateDto.Coordenadas;

            _context.Entry(local).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocalExists(id))
                {
                    return NotFound(new { mensagem = "Local não encontrado" });
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Eliminar um local
        /// </summary>
        /// <param name="id">ID do local a eliminar</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocal(int id)
        {
            var local = await _context.Locais.FindAsync(id);

            if (local == null)
            {
                return NotFound(new { mensagem = "Local não encontrado" });
            }

            _context.Locais.Remove(local);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocalExists(int id)
        {
            return _context.Locais.Any(l => l.IdLocal == id);
        }
    }
}
