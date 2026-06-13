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
            // Definimos a lista completa de locais (originais e novos)
            var todosLocais = new List<Local>
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
                },
                new Local
                {
                    Nome = "Ferreira do Zêzere",
                    Descricao = "Praça central da vila, habitual recinto das Festas do Concelho e Feira de São Pedro em agosto.",
                    Outside = true,
                    Coordenadas = "39.6953,-8.3147"
                },
                new Local
                {
                    Nome = "Serra",
                    Descricao = "Largo da festa na Serra, conhecido pelas Festas em Honra de Nossa Senhora da Purificação.",
                    Outside = true,
                    Coordenadas = "39.6195,-8.3102"
                },
                new Local
                {
                    Nome = "Chão das Maias",
                    Descricao = "Recinto das festas de São Bartolomeu, com espaço para grandes jantares comunitários.",
                    Outside = true,
                    Coordenadas = "39.6051,-8.3058"
                },
                new Local
                {
                    Nome = "Pedreira",
                    Descricao = "Largo principal da Pedreira, famoso pelos arraiais de verão e leilões de oferendas.",
                    Outside = true,
                    Coordenadas = "39.6352,-8.4051"
                },
                new Local
                {
                    Nome = "Casais",
                    Descricao = "Recinto de festas da paróquia de Casais, com grande afluência em julho.",
                    Outside = true,
                    Coordenadas = "39.6504,-8.4103"
                },
                new Local
                {
                    Nome = "Junceira",
                    Descricao = "Adro da Igreja da Junceira, palco do evento Junceira com Tradições e arraiais populares.",
                    Outside = true,
                    Coordenadas = "39.6158,-8.3304"
                },
                new Local
                {
                    Nome = "Valdonas",
                    Descricao = "Espaço de festividades de Valdonas, nos arredores de Tomar, conhecido pelas noites de baile.",
                    Outside = true,
                    Coordenadas = "39.6102,-8.3905"
                },
                new Local
                {
                    Nome = "Águas Belas",
                    Descricao = "Largo da freguesia, animado pelas festas de verão junto à bacia do Zêzere.",
                    Outside = true,
                    Coordenadas = "39.7051,-8.2809"
                },
                new Local
                {
                    Nome = "Alqueidão",
                    Descricao = "Recinto da Associação Recreativa do Alqueidão, focado em petiscos e música ao vivo.",
                    Outside = true,
                    Coordenadas = "39.6305,-8.3201"
                },
                new Local
                {
                    Nome = "Cabeças",
                    Descricao = "Largo da aldeia das Cabeças, habituado aos festejos dos Santos Populares.",
                    Outside = true,
                    Coordenadas = "39.5608,-8.3802"
                },
                new Local
                {
                    Nome = "Vialonga",
                    Descricao = "Espaço central de Vialonga, com tradição de arraial bairrista no início de julho.",
                    Outside = true,
                    Coordenadas = "39.5902,-8.4157"
                },
                new Local
                {
                    Nome = "Barreiras",
                    Descricao = "Recinto de festejos intimistas e tradicionais da aldeia de Barreiras.",
                    Outside = true,
                    Coordenadas = "39.6451,-8.3503"
                },
                new Local
                {
                    Nome = "Fonte Dom João",
                    Descricao = "Espaço exterior do Centro Recreativo, palco de tasquinhas e convívio em julho.",
                    Outside = true,
                    Coordenadas = "39.6109,-8.3207"
                },
                new Local
                {
                    Nome = "Poço Redondo",
                    Descricao = "Largo das Festas do Divino Espírito Santo, com jogos populares e baile.",
                    Outside = true,
                    Coordenadas = "39.5854,-8.3401"
                },
                new Local
                {
                    Nome = "Carqueijal",
                    Descricao = "Recinto da associação local, com quermesse e festa dedicada aos emigrantes.",
                    Outside = true,
                    Coordenadas = "39.6406,-8.2904"
                },
                new Local
                {
                    Nome = "Bairradinha",
                    Descricao = "Zona de romarias tradicionais junto à albufeira do Castelo de Bode.",
                    Outside = true,
                    Coordenadas = "39.6752,-8.2856"
                },
                new Local
                {
                    Nome = "Cem Soldos",
                    Descricao = "Ruas e largos da aldeia, recinto integral do Festival Bons Sons.",
                    Outside = true,
                    Coordenadas = "39.5858,-8.4552"
                },
                new Local
                {
                    Nome = "Asseiceira",
                    Descricao = "Largo da Igreja da Asseiceira, local das festas anuais e romarias de julho.",
                    Outside = true,
                    Coordenadas = "39.5167,-8.3969"
                },
                new Local
                {
                    Nome = "Paialvo",
                    Descricao = "Recinto das festas de agosto de Paialvo, com forte componente gastronómica.",
                    Outside = true,
                    Coordenadas = "39.5534,-8.4578"
                },
                new Local
                {
                    Nome = "Santa Cita",
                    Descricao = "Largo principal de Santa Cita, conhecido pelos festejos de fim de verão em setembro.",
                    Outside = true,
                    Coordenadas = "39.5381,-8.4125"
                }
            };

            var locaisParaInserir = new List<Local>();
            foreach (var local in todosLocais)
            {
                if (!await context.Locais.AnyAsync(l => l.Nome == local.Nome))
                {
                    locaisParaInserir.Add(local);
                }
            }

            if (locaisParaInserir.Any())
            {
                await context.Locais.AddRangeAsync(locaisParaInserir);
                await context.SaveChangesAsync();
            }

            // 4. Seed Artistas (Artists)
            var todosArtistas = new List<Artista>
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
                },
                new Artista
                {
                    Nome = "Toy",
                    Biografia = "Ícone da música popular portuguesa, conhecido por grandes êxitos de cariz romântico e festivo como 'Coração não tem idade'.",
                    Contacto = "919999999",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1501386761578-eac5c94b800a"
                },
                new Artista
                {
                    Nome = "Rosinha",
                    Biografia = "Cantora popular com concertos animados e cheios de humor, conhecida pelas suas canções com acordeão e refrões divertidos.",
                    Contacto = "928888888",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1516450360452-9312f5e86fc7"
                },
                new Artista
                {
                    Nome = "Xutos & Pontapés",
                    Biografia = "A maior banda de rock em Portugal, com uma carreira de mais de 40 anos repleta de hinos intemporais.",
                    Contacto = "937777777",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1470225620780-dba8ba36b745"
                },
                new Artista
                {
                    Nome = "David Antunes & The Midnight Band",
                    Biografia = "Banda de pop-rock e entretenimento carismática, conhecida pelas suas performances enérgicas e interativas.",
                    Contacto = "916666666",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1506157786151-b8491531f063"
                },
                new Artista
                {
                    Nome = "Gisela João",
                    Biografia = "Uma das vozes mais marcantes do fado contemporâneo, trazendo uma intensidade emocional e frescura ao fado tradicional.",
                    Contacto = "965555555",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1534528741775-53994a69daeb"
                },
                new Artista
                {
                    Nome = "Miguel Araújo",
                    Biografia = "Músico, compositor e cantor português, autor de algumas das canções mais conhecidas da pop e folk nacional.",
                    Contacto = "914444444",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1507679799987-c73779587ccf"
                },
                new Artista
                {
                    Nome = "Bonga",
                    Biografia = "Embaixador da música angolana e do semba, com uma voz inconfundível que contagia qualquer recinto de dança.",
                    Contacto = "923333333",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1484755560693-a4074577af3a"
                },
                new Artista
                {
                    Nome = "The Gift",
                    Biografia = "Banda pioneira do pop/indie eletrónico em Portugal, famosa pelas atuações teatrais da vocalista Sónia Tavares.",
                    Contacto = "962222222",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1514320291840-2e0a9bf2a9ae"
                },
                new Artista
                {
                    Nome = "Kura",
                    Biografia = "Um dos DJs e produtores de música eletrónica portugueses mais reconhecidos internacionalmente.",
                    Contacto = "911111111",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1470229722913-7c0e2dbbafd3"
                },
                new Artista
                {
                    Nome = "Augusto Canário & Amigos",
                    Biografia = "Grupo de música tradicional e popular portuguesa, célebre pelas desgarradas e cantares ao desafio.",
                    Contacto = "929292929",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1511192336575-5a79af67a629"
                },
                new Artista
                {
                    Nome = "Bandas Filarmónicas Reunidas",
                    Biografia = "Agrupamento de músicos locais dedicados a manter vivas as marchas, hinos e clássicos filarmónicos nas festas da região.",
                    Contacto = "249999999",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1465847899084-d164df4dedc6"
                }
            };

            var artistasParaInserir = new List<Artista>();
            foreach (var artista in todosArtistas)
            {
                if (!await context.Artistas.AnyAsync(a => a.Nome == artista.Nome))
                {
                    artistasParaInserir.Add(artista);
                }
            }

            if (artistasParaInserir.Any())
            {
                await context.Artistas.AddRangeAsync(artistasParaInserir);
                await context.SaveChangesAsync();
            }

            // 5. Seed Eventos (Events)
            var definicoesEventos = new List<(string NomeLocal, Evento Evento)>
            {
                ("Recinto Principal", new Evento
                {
                    Nome = "Grande Concerto de Abertura",
                    Descricao = "Cerimónia de inauguração oficial das festas seguida de espetáculo musical.",
                    DataInicio = DateTime.Today.AddDays(1).AddHours(21).AddMinutes(30),
                    DataFim = DateTime.Today.AddDays(1).AddHours(23).AddMinutes(30),
                    Patrocinador = "Super Bock"
                }),
                ("Recinto Principal", new Evento
                {
                    Nome = "Grande Arraial Popular",
                    Descricao = "Baile de verão com os melhores êxitos populares e muita animação pela noite dentro.",
                    DataInicio = DateTime.Today.AddDays(2).AddHours(21).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(3).AddHours(2).AddMinutes(0),
                    Patrocinador = "Licor Beirão"
                }),
                ("Palco Tradição", new Evento
                {
                    Nome = "Tarde de Cantares e Tradição",
                    Descricao = "Demonstrações etnográficas, ranchos locais e feira de gastronomia.",
                    DataInicio = DateTime.Today.AddDays(3).AddHours(15).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(3).AddHours(19).AddMinutes(0),
                    Patrocinador = "Adega Cooperativa Local"
                }),
                ("Ferreira do Zêzere", new Evento
                {
                    Nome = "Festas do Concelho de Ferreira do Zêzere",
                    Descricao = "Praça central animada pelas festas do concelho, concertos e feira gastronómica.",
                    DataInicio = DateTime.Today.AddDays(4).AddHours(18).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(5).AddHours(2).AddMinutes(0),
                    Patrocinador = "Delta Cafés"
                }),
                ("Serra", new Evento
                {
                    Nome = "Festas em Honra de Nossa Senhora da Purificação",
                    Descricao = "Tradicional arraial com fogaças, leilões e noites de baile.",
                    DataInicio = DateTime.Today.AddDays(5).AddHours(20).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(6).AddHours(3).AddMinutes(0),
                    Patrocinador = "Cerveja Sagres"
                }),
                ("Chão das Maias", new Evento
                {
                    Nome = "Grandes Jantares de São Bartolomeu",
                    Descricao = "Evento comunitário gastronómico com pratos típicos da região e música popular.",
                    DataInicio = DateTime.Today.AddDays(6).AddHours(19).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(6).AddHours(23).AddMinutes(30),
                    Patrocinador = "Vinhos do Tejo"
                }),
                ("Pedreira", new Evento
                {
                    Nome = "Arraial de Verão da Pedreira",
                    Descricao = "Tradicional arraial de verão com quermesse, música ao vivo e leilão de oferendas.",
                    DataInicio = DateTime.Today.AddDays(7).AddHours(21).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(8).AddHours(4).AddMinutes(0),
                    Patrocinador = "Intermarché"
                }),
                ("Casais", new Evento
                {
                    Nome = "Festas de Julho de Casais",
                    Descricao = "Festividades da paróquia com convívios, procissão e bailes noturnos.",
                    DataInicio = DateTime.Today.AddDays(8).AddHours(17).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(9).AddHours(1).AddMinutes(0),
                    Patrocinador = "Crédito Agrícola"
                }),
                ("Junceira", new Evento
                {
                    Nome = "Junceira com Tradições",
                    Descricao = "Mostra de tradições locais, folclore regional e doces tradicionais.",
                    DataInicio = DateTime.Today.AddDays(9).AddHours(15).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(9).AddHours(23).AddMinutes(0),
                    Patrocinador = "Fanta"
                }),
                ("Valdonas", new Evento
                {
                    Nome = "Noite de Baile de Valdonas",
                    Descricao = "Grande baile popular animado por artistas locais nos arredores de Tomar.",
                    DataInicio = DateTime.Today.AddDays(10).AddHours(22).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(11).AddHours(4).AddMinutes(0),
                    Patrocinador = "Rádio local"
                }),
                ("Águas Belas", new Evento
                {
                    Nome = "Festas de Verão de Águas Belas",
                    Descricao = "Arraial animado junto à bacia do Zêzere com petiscos e muita dança.",
                    DataInicio = DateTime.Today.AddDays(11).AddHours(19).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(12).AddHours(2).AddMinutes(0),
                    Patrocinador = "Adega de Tomar"
                }),
                ("Alqueidão", new Evento
                {
                    Nome = "Noite Recreativa do Alqueidão",
                    Descricao = "Evento focado em petiscos tradicionais, música ao vivo e convívio.",
                    DataInicio = DateTime.Today.AddDays(12).AddHours(20).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(13).AddHours(1).AddMinutes(0),
                    Patrocinador = "Mini Preço"
                }),
                ("Cabeças", new Evento
                {
                    Nome = "Festas de Santos Populares de Cabeças",
                    Descricao = "Marchas populares, sardinhada e baile dedicado ao São João.",
                    DataInicio = DateTime.Today.AddDays(13).AddHours(20).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(14).AddHours(3).AddMinutes(0),
                    Patrocinador = "Coca-Cola"
                }),
                ("Vialonga", new Evento
                {
                    Nome = "Arraial Bairrista de Vialonga",
                    Descricao = "Convívio de bairro com fados, sardinhada e animação infantil.",
                    DataInicio = DateTime.Today.AddDays(14).AddHours(18).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(14).AddHours(23).AddMinutes(59),
                    Patrocinador = "Padaria Central"
                }),
                ("Barreiras", new Evento
                {
                    Nome = "Festa Tradicional de Barreiras",
                    Descricao = "Festejos religiosos e tradicionais de cariz intimista.",
                    DataInicio = DateTime.Today.AddDays(15).AddHours(11).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(15).AddHours(23).AddMinutes(0),
                    Patrocinador = "Talho da Aldeia"
                }),
                ("Fonte Dom João", new Evento
                {
                    Nome = "Tasquinhas de Fonte Dom João",
                    Descricao = "Mostra gastronómica regional e convívio popular ao ar livre.",
                    DataInicio = DateTime.Today.AddDays(16).AddHours(19).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(17).AddHours(0).AddMinutes(30),
                    Patrocinador = "Azeite Fátima"
                }),
                ("Poço Redondo", new Evento
                {
                    Nome = "Festas do Divino Espírito Santo",
                    Descricao = "Celebrações tradicionais, jogos populares e feira de artesanato.",
                    DataInicio = DateTime.Today.AddDays(17).AddHours(10).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(18).AddHours(22).AddMinutes(0),
                    Patrocinador = "Junta de Freguesia"
                }),
                ("Carqueijal", new Evento
                {
                    Nome = "Festa dos Emigrantes do Carqueijal",
                    Descricao = "Grande almoço comunitário e tarde cultural dedicada aos emigrantes da aldeia.",
                    DataInicio = DateTime.Today.AddDays(18).AddHours(12).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(18).AddHours(21).AddMinutes(0),
                    Patrocinador = "Banco BPI"
                }),
                ("Bairradinha", new Evento
                {
                    Nome = "Romaria Tradicional da Bairradinha",
                    Descricao = "Romaria tradicional à beira da albufeira do Castelo de Bode com piquenique comunitário.",
                    DataInicio = DateTime.Today.AddDays(19).AddHours(9).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(19).AddHours(18).AddMinutes(0),
                    Patrocinador = "Pingo Doce"
                }),
                ("Cem Soldos", new Evento
                {
                    Nome = "Festival Bons Sons",
                    Descricao = "Festival de música portuguesa de referência nacional, espalhado por toda a aldeia de Cem Soldos.",
                    DataInicio = DateTime.Today.AddDays(20).AddHours(16).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(24).AddHours(23).AddMinutes(59),
                    Patrocinador = "Caixa Geral de Depósitos"
                }),
                ("Asseiceira", new Evento
                {
                    Nome = "Romaria Anual de Asseiceira",
                    Descricao = "Festa anual e romarias religiosas com missa, procissão e arraial de encerramento.",
                    DataInicio = DateTime.Today.AddDays(25).AddHours(10).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(25).AddHours(23).AddMinutes(0),
                    Patrocinador = "Águas do Luso"
                }),
                ("Paialvo", new Evento
                {
                    Nome = "Festas de Agosto de Paialvo",
                    Descricao = "Tradicional festa com gastronomia forte, sopa de pedras e música popular.",
                    DataInicio = DateTime.Today.AddDays(26).AddHours(19).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(27).AddHours(2).AddMinutes(0),
                    Patrocinador = "Supermercado local"
                }),
                ("Santa Cita", new Evento
                {
                    Nome = "Festejos de Santa Cita",
                    Descricao = "Arraial e convívio popular celebrando o encerramento das festividades de verão.",
                    DataInicio = DateTime.Today.AddDays(28).AddHours(21).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(29).AddHours(3).AddMinutes(0),
                    Patrocinador = "Adega Regional"
                })
            };

            var eventosParaInserir = new List<Evento>();
            foreach (var item in definicoesEventos)
            {
                if (!await context.Eventos.AnyAsync(e => e.Nome == item.Evento.Nome))
                {
                    var local = await context.Locais.FirstOrDefaultAsync(l => l.Nome.Contains(item.NomeLocal));
                    if (local != null)
                    {
                        item.Evento.IdLocal = local.IdLocal;
                        eventosParaInserir.Add(item.Evento);
                    }
                }
            }

            if (eventosParaInserir.Any())
            {
                await context.Eventos.AddRangeAsync(eventosParaInserir);
                await context.SaveChangesAsync();
            }

            // 6. Seed Cartazes (Lineup)
            var specs = new List<(string EventoQuery, string ArtistaQuery, TimeSpan Offset, int Duracao)>
            {
                ("Grande Concerto de Abertura", "Ana Malhoa", TimeSpan.FromMinutes(15), 90),
                ("Grande Arraial Popular", "Quim Barreiros", TimeSpan.FromHours(1), 120),
                ("Tarde de Cantares e Tradição", "Rancho Folclórico", TimeSpan.FromMinutes(30), 90),
                ("Festas do Concelho de Ferreira do Zêzere", "Toy", TimeSpan.FromHours(1.5), 90),
                ("Festas em Honra de Nossa Senhora da Purificação", "Rosinha", TimeSpan.FromHours(1), 90),
                ("Grandes Jantares de São Bartolomeu", "Augusto Canário & Amigos", TimeSpan.FromMinutes(30), 120),
                ("Arraial de Verão da Pedreira", "Toy", TimeSpan.FromHours(1), 90),
                ("Festas de Julho de Casais", "Quim Barreiros", TimeSpan.FromHours(1.5), 120),
                ("Junceira com Tradições", "Rancho Folclórico", TimeSpan.FromMinutes(45), 90),
                ("Noite de Baile de Valdonas", "Toy", TimeSpan.FromHours(1), 120),
                ("Festas de Verão de Águas Belas", "Rosinha", TimeSpan.FromHours(1), 90),
                ("Noite Recreativa do Alqueidão", "David Antunes & The Midnight Band", TimeSpan.FromHours(1), 120),
                ("Festas de Santos Populares de Cabeças", "Quim Barreiros", TimeSpan.FromHours(1.5), 120),
                ("Arraial Bairrista de Vialonga", "Augusto Canário & Amigos", TimeSpan.FromHours(1), 120),
                ("Festa Tradicional de Barreiras", "Bandas Filarmónicas Reunidas", TimeSpan.FromMinutes(30), 180),
                ("Tasquinhas de Fonte Dom João", "Rancho Folclórico", TimeSpan.FromMinutes(30), 120),
                ("Festas do Divino Espírito Santo", "Bandas Filarmónicas Reunidas", TimeSpan.FromHours(1), 120),
                ("Festa dos Emigrantes do Carqueijal", "Toy", TimeSpan.FromHours(1), 120),
                ("Romaria Tradicional da Bairradinha", "Bandas Filarmónicas Reunidas", TimeSpan.FromHours(1), 120),
                ("Romaria Anual de Asseiceira", "Bandas Filarmónicas Reunidas", TimeSpan.FromHours(2), 180),
                ("Festas de Agosto de Paialvo", "David Antunes & The Midnight Band", TimeSpan.FromHours(1.5), 120),
                ("Festejos de Santa Cita", "Ana Malhoa", TimeSpan.FromHours(1), 90),
                
                // Festival Bons Sons (multi-dia)
                ("Festival Bons Sons", "Miguel Araújo", TimeSpan.FromHours(4), 90),
                ("Festival Bons Sons", "Gisela João", TimeSpan.FromDays(1).Add(TimeSpan.FromHours(5)), 75),
                ("Festival Bons Sons", "The Gift", TimeSpan.FromDays(2).Add(TimeSpan.FromHours(6.5)), 90),
                ("Festival Bons Sons", "Bonga", TimeSpan.FromDays(3).Add(TimeSpan.FromHours(3.5)), 90),
                ("Festival Bons Sons", "Kura", TimeSpan.FromDays(3).Add(TimeSpan.FromHours(7)), 120)
            };

            var cartazesParaInserir = new List<Cartaz>();

            foreach (var spec in specs)
            {
                var evento = await context.Eventos.FirstOrDefaultAsync(e => e.Nome.Contains(spec.EventoQuery));
                var artista = await context.Artistas.FirstOrDefaultAsync(a => a.Nome.Contains(spec.ArtistaQuery));

                if (evento != null && artista != null)
                {
                    bool existeAtuacao = await context.Cartazes.AnyAsync(c => c.IdEvento == evento.IdEvento && c.IdArtista == artista.IdArtista);
                    if (!existeAtuacao)
                    {
                        var dataAtuacao = evento.DataInicio.Add(spec.Offset);
                        
                        if (dataAtuacao >= evento.DataInicio && dataAtuacao < evento.DataFim)
                        {
                            cartazesParaInserir.Add(new Cartaz
                            {
                                IdEvento = evento.IdEvento,
                                IdArtista = artista.IdArtista,
                                DataHoraAtuacao = dataAtuacao,
                                DuracaoMinutos = spec.Duracao
                            });
                        }
                    }
                }
            }

            if (cartazesParaInserir.Any())
            {
                await context.Cartazes.AddRangeAsync(cartazesParaInserir);
                await context.SaveChangesAsync();
            }
        }
    }
}
