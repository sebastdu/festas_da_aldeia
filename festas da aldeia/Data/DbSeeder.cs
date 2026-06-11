using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;

namespace festas_da_aldeia.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAllAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Seed Roles
            string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Falha ao criar a Role Admin: {errors}");
                }
            }

            // 2. Seed Admin User
            string adminEmail = "admin@festas.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser != null)
            {
                // Garantir que a password atualizada (com maiúscula) é aplicada recriando o utilizador
                await userManager.DeleteAsync(adminUser);
                adminUser = null;
            }
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var createResult = await userManager.CreateAsync(adminUser, "123Qwe##");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
                else
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Falha ao criar o utilizador administrador: {errors}");
                }
            }

            // 3. Seed Locais (Locations)
            // Usamos verificação específica de nome para permitir que corra mesmo se o utilizador já inseriu dados manuais
            if (!await context.Locais.AnyAsync(l => l.Nome == "Recinto Principal (Praça do Município)"))
            {
                var locais = new List<Local>
                {
                    new Local
                    {
                        Nome = "Recinto Principal (Praça do Município)",
                        Descricao = "Palco principal das festas, localizado no centro histórico, onde ocorrem os grandes concertos.",
                        Outside = true,
                        Coordenadas = "39.6030,-8.4164"
                    },
                    new Local
                    {
                        Nome = "Palco Tradição (Jardim do Coreto)",
                        Descricao = "Espaço dedicado à música popular, desgarradas e folclore, rodeado de tasquinhas típicas.",
                        Outside = true,
                        Coordenadas = "39.6025,-8.4124"
                    },
                    new Local
                    {
                        Nome = "Auditório da Aldeia (Centro Cultural)",
                        Descricao = "Auditório fechado ideal para exposições de artesanato, debates e concertos filarmónicos mais íntimos.",
                        Outside = false,
                        Coordenadas = "39.6015,-8.4025"
                    }
                };
                await context.Locais.AddRangeAsync(locais);
                await context.SaveChangesAsync();
            }

            // 4. Seed Artistas (Artists)
            if (!await context.Artistas.AnyAsync(a => a.Nome == "Quim Barreiros"))
            {
                var artistas = new List<Artista>
                {
                    new Artista
                    {
                        Nome = "Quim Barreiros",
                        Biografia = "O mestre da música popular e brejeira portuguesa, famoso pelo acordeão e pelas suas letras de duplo sentido.",
                        Contacto = "912345678",
                        LinkFotoPerfil = "https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4"
                    },
                    new Artista
                    {
                        Nome = "Ana Malhoa",
                        Biografia = "Cantora e performer de referência em Portugal, traz um espetáculo eletrizante repleto de pop e ritmos tropicais.",
                        Contacto = "961234567",
                        LinkFotoPerfil = "https://images.unsplash.com/photo-1494790108377-be9c29b29330"
                    },
                    new Artista
                    {
                        Nome = "Rancho Folclórico da Região",
                        Biografia = "Fundado há mais de 40 anos, preserva as danças, trajes e cantares típicos dos nossos antepassados rurais.",
                        Contacto = "249123456",
                        LinkFotoPerfil = "https://images.unsplash.com/photo-1482440308425-276ad0f28b19"
                    }
                };
                await context.Artistas.AddRangeAsync(artistas);
                await context.SaveChangesAsync();
            }

            // 5. Seed Eventos (Events)
            if (!await context.Eventos.AnyAsync(e => e.Nome == "Grande Concerto de Abertura"))
            {
                // Vamos buscar os locais inseridos para ter IDs válidos
                var recintoPrincipal = await context.Locais.FirstOrDefaultAsync(l => l.Nome.Contains("Recinto Principal"));
                var palcoTradicao = await context.Locais.FirstOrDefaultAsync(l => l.Nome.Contains("Palco Tradição"));

                if (recintoPrincipal != null && palcoTradicao != null)
                {
                    var eventos = new List<Evento>
                    {
                        new Evento
                        {
                            Nome = "Grande Concerto de Abertura",
                            Descricao = "Cerimónia de inauguração oficial das festas seguida de espetáculo musical.",
                            DataInicio = DateTime.Today.AddDays(1).AddHours(21).AddMinutes(30), // Amanhã às 21h30
                            DataFim = DateTime.Today.AddDays(1).AddHours(23).AddMinutes(30),
                            Patrocinador = "Super Bock",
                            IdLocal = recintoPrincipal.IdLocal
                        },
                        new Evento
                        {
                            Nome = "Grande Arraial Popular",
                            Descricao = "Baile de verão com os melhores êxitos populares e muita animação pela noite dentro.",
                            DataInicio = DateTime.Today.AddDays(2).AddHours(21).AddMinutes(0), // Depois de amanhã às 21h00
                            DataFim = DateTime.Today.AddDays(3).AddHours(2).AddMinutes(0), // Termina no dia seguinte às 2h00
                            Patrocinador = "Licor Beirão",
                            IdLocal = recintoPrincipal.IdLocal
                        },
                        new Evento
                        {
                            Nome = "Tarde de Cantares e Tradição",
                            Descricao = "Demonstrações etnográficas, ranchos locais e feira de gastronomia.",
                            DataInicio = DateTime.Today.AddDays(3).AddHours(15).AddMinutes(0), // Daqui a 3 dias às 15h00
                            DataFim = DateTime.Today.AddDays(3).AddHours(19).AddMinutes(0),
                            Patrocinador = "Adega Cooperativa Local",
                            IdLocal = palcoTradicao.IdLocal
                        }
                    };
                    await context.Eventos.AddRangeAsync(eventos);
                    await context.SaveChangesAsync();
                }
            }

            // 6. Seed Cartazes (Lineup)
            // Verificamos se existem cartazes para os eventos seeded para evitar duplicações
            var concertoAbertura = await context.Eventos.FirstOrDefaultAsync(e => e.Nome.Contains("Concerto de Abertura"));
            var arraialPopular = await context.Eventos.FirstOrDefaultAsync(e => e.Nome.Contains("Arraial Popular"));
            var tardeTradicao = await context.Eventos.FirstOrDefaultAsync(e => e.Nome.Contains("Tarde de Cantares"));

            if (concertoAbertura != null && arraialPopular != null && tardeTradicao != null)
            {
                bool hasLineup = await context.Cartazes.AnyAsync(c => c.IdEvento == concertoAbertura.IdEvento || c.IdEvento == arraialPopular.IdEvento);
                if (!hasLineup)
                {
                    var quimBarreiros = await context.Artistas.FirstOrDefaultAsync(a => a.Nome.Contains("Quim Barreiros"));
                    var anaMalhoa = await context.Artistas.FirstOrDefaultAsync(a => a.Nome.Contains("Ana Malhoa"));
                    var rancho = await context.Artistas.FirstOrDefaultAsync(a => a.Nome.Contains("Rancho"));

                    if (quimBarreiros != null && anaMalhoa != null && rancho != null)
                    {
                        var cartazes = new List<Cartaz>
                        {
                            new Cartaz
                            {
                                IdEvento = concertoAbertura.IdEvento,
                                IdArtista = anaMalhoa.IdArtista,
                                DataHoraAtuacao = concertoAbertura.DataInicio.AddMinutes(15), // Começa 15 min após início do evento
                                DuracaoMinutos = 90
                            },
                            new Cartaz
                            {
                                IdEvento = arraialPopular.IdEvento,
                                IdArtista = quimBarreiros.IdArtista,
                                DataHoraAtuacao = arraialPopular.DataInicio.AddHours(1), // Começa 1h após início do arraial
                                DuracaoMinutos = 120
                            },
                            new Cartaz
                            {
                                IdEvento = tardeTradicao.IdEvento,
                                IdArtista = rancho.IdArtista,
                                DataHoraAtuacao = tardeTradicao.DataInicio.AddMinutes(30),
                                DuracaoMinutos = 90
                            }
                        };
                        await context.Cartazes.AddRangeAsync(cartazes);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
