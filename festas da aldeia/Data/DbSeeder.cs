using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using festas_da_aldeia.Models;

namespace festas_da_aldeia.Data
{
    /// <summary>
    /// Classe auxiliar encarregue de semear (seed) dados iniciais na base de dados.
    /// Cria as funções (roles) de segurança padrão, bem como utilizadores administrativos
    /// e dados estáticos de teste para locais, artistas, eventos e atuações (cartaz).
    /// </summary>
    public static class DbSeeder
    {
        /// <summary>
        /// Popula a base de dados com as funções e utilizadores iniciais, além de dados estáticos
        /// de locais, artistas e atuações de teste necessárias para a demonstração e desenvolvimento do portal.
        /// </summary>
        /// <param name="context">O contexto de base de dados da aplicação.</param>
        /// <param name="userManager">O gestor de identidades de utilizadores.</param>
        /// <param name="roleManager">O gestor de funções/roles de segurança.</param>
        /// <returns>Uma tarefa assíncrona que representa o processo de seeding.</returns>
        public static async Task SeedAllAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Seed Roles
            string adminRole = "Admin";
            string clienteRole = "Cliente";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Falha ao criar a Role Admin: {errors}");
                }
            }

            if (!await roleManager.RoleExistsAsync(clienteRole))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(clienteRole));
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Falha ao criar a Role Cliente: {errors}");
                }
            }

            // 2. Seed Admin User
            string adminEmail = "admin@festas.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser != null)
            {
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
                var createResult = await userManager.CreateAsync(adminUser, "Password123!");
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

            // Seed Cliente User
            string clienteEmail = "cliente@festas.com";
            var clienteUser = await userManager.FindByEmailAsync(clienteEmail);
            if (clienteUser != null)
            {
                await userManager.DeleteAsync(clienteUser);
                clienteUser = null;
            }
            if (clienteUser == null)
            {
                clienteUser = new IdentityUser
                {
                    UserName = clienteEmail,
                    Email = clienteEmail,
                    EmailConfirmed = true
                };
                var createResult = await userManager.CreateAsync(clienteUser, "Password123!");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(clienteUser, clienteRole);
                }
                else
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Falha ao criar o utilizador cliente: {errors}");
                }
            }

            // 3. Seed Locais (Locations)
            var todosLocais = new List<Local>
            {
                new()
                {
                    Nome = "Recinto Principal (Praça do Município)",
                    Descricao = "Palco principal das festas, localizado no centro histórico, onde ocorrem os grandes concertos.",
                    Outside = true,
                    Coordenadas = "39.6030,-8.4164"
                },
                new()
                {
                    Nome = "Palco Tradição (Jardim do Coreto)",
                    Descricao = "Espaço dedicado à música popular, desgarradas e folclore, rodeado de tasquinhas típicas.",
                    Outside = true,
                    Coordenadas = "39.6025,-8.4124"
                },
                new()
                {
                    Nome = "Auditório da Aldeia (Centro Cultural)",
                    Descricao = "Auditório fechado ideal para exposições de artesanato, debates e concertos filarmónicos mais íntimos.",
                    Outside = false,
                    Coordenadas = "39.6015,-8.4025"
                },
                new()
                {
                    Nome = "Ferreira do Zêzere",
                    Descricao = "Praça central da vila, habitual recinto das Festas do Concelho e Feira de São Pedro em agosto.",
                    Outside = true,
                    Coordenadas = "39.6953,-8.3147"
                },
                new()
                {
                    Nome = "Serra",
                    Descricao = "Largo da festa na Serra, conhecido pelas Festas em Honra de Nossa Senhora da Purificação.",
                    Outside = true,
                    Coordenadas = "39.6195,-8.3102"
                },
                new()
                {
                    Nome = "Chão das Maias",
                    Descricao = "Recinto das festas de São Bartolomeu, com espaço para grandes jantares comunitários.",
                    Outside = true,
                    Coordenadas = "39.6051,-8.3058"
                },
                new()
                {
                    Nome = "Pedreira",
                    Descricao = "Largo principal da Pedreira, famoso pelos arraiais de verão e leilões de oferendas.",
                    Outside = true,
                    Coordenadas = "39.6352,-8.4051"
                },
                new()
                {
                    Nome = "Casais",
                    Descricao = "Recinto de festas da paróquia de Casais, com grande afluência em julho.",
                    Outside = true,
                    Coordenadas = "39.6504,-8.4103"
                },
                new()
                {
                    Nome = "Junceira",
                    Descricao = "Adro da Igreja da Junceira, palco do evento Junceira com Tradições e arraiais populares.",
                    Outside = true,
                    Coordenadas = "39.6158,-8.3304"
                },
                new()
                {
                    Nome = "Valdonas",
                    Descricao = "Espaço de festividades de Valdonas, nos arredores de Tomar, conhecido pelas noites de baile.",
                    Outside = true,
                    Coordenadas = "39.6102,-8.3905"
                },
                new()
                {
                    Nome = "Águas Belas",
                    Descricao = "Largo da freguesia, animado pelas festas de verão junto à bacia do Zêzere.",
                    Outside = true,
                    Coordenadas = "39.7051,-8.2809"
                },
                new()
                {
                    Nome = "Alqueidão",
                    Descricao = "Recinto da Associação Recreativa do Alqueidão, focado em petiscos e música ao vivo.",
                    Outside = true,
                    Coordenadas = "39.6305,-8.3201"
                },
                new()
                {
                    Nome = "Cabeças",
                    Descricao = "Largo da aldeia das Cabeças, habituado aos festejos dos Santos Populares.",
                    Outside = true,
                    Coordenadas = "39.5608,-8.3802"
                },
                new()
                {
                    Nome = "Vialonga",
                    Descricao = "Espaço central de Vialonga, com tradição de arraial bairrista no início de julho.",
                    Outside = true,
                    Coordenadas = "39.5902,-8.4157"
                },
                new()
                {
                    Nome = "Barreiras",
                    Descricao = "Recinto de festejos intimistas e tradicionais da aldeia de Barreiras.",
                    Outside = true,
                    Coordenadas = "39.6451,-8.3503"
                },
                new()
                {
                    Nome = "Fonte Dom João",
                    Descricao = "Espaço exterior do Centro Recreativo, palco de tasquinhas e convívio em julho.",
                    Outside = true,
                    Coordenadas = "39.6109,-8.3207"
                },
                new()
                {
                    Nome = "Poço Redondo",
                    Descricao = "Largo das Festas do Divino Espírito Santo, com jogos populares e baile.",
                    Outside = true,
                    Coordenadas = "39.5854,-8.3401"
                },
                new()
                {
                    Nome = "Carqueijal",
                    Descricao = "Recinto da associação local, com quermesse e festa dedicada aos emigrantes.",
                    Outside = true,
                    Coordenadas = "39.6406,-8.2904"
                },
                new()
                {
                    Nome = "Bairradinha",
                    Descricao = "Zona de romarias tradicionais junto à albufeira do Castelo de Bode.",
                    Outside = true,
                    Coordenadas = "39.6752,-8.2856"
                },
                new()
                {
                    Nome = "Cem Soldos",
                    Descricao = "Ruas e largos da aldeia, recinto integral do Festival Bons Sons.",
                    Outside = true,
                    Coordenadas = "39.5858,-8.4552"
                },
                new()
                {
                    Nome = "Asseiceira",
                    Descricao = "Largo da Igreja da Asseiceira, local das festas anuais e romarias de julho.",
                    Outside = true,
                    Coordenadas = "39.5167,-8.3969"
                },
                new()
                {
                    Nome = "Paialvo",
                    Descricao = "Recinto das festas de agosto de Paialvo, com forte componente gastronómica.",
                    Outside = true,
                    Coordenadas = "39.5534,-8.4578"
                },
                new()
                {
                    Nome = "Santa Cita",
                    Descricao = "Largo principal de Santa Cita, conhecido pelos festejos de fim de verão em setembro.",
                    Outside = true,
                    Coordenadas = "39.5381,-8.4125"
                },
                new()
                {
                    Nome = "Terreiro do Paço (Recinto das Festas de Lisboa)",
                    Descricao = "Largo monumental à beira do Tejo, palco dos grandes concertos de encerramento das Festas de Lisboa.",
                    Outside = true,
                    Coordenadas = "38.7075,-9.1364"
                },
                new()
                {
                    Nome = "Zona da Ribeira (Largo de São João do Porto)",
                    Descricao = "Ponto de encontro central do São João do Porto, com vista privilegiada para o rio Douro.",
                    Outside = true,
                    Coordenadas = "41.1406,-8.6111"
                },
                new()
                {
                    Nome = "Campo da Agonia (Recinto da Senhora da Agonia)",
                    Descricao = "Amplo recinto em Viana do Castelo onde se reúnem as festividades, desfiles e exposições da romaria.",
                    Outside = true,
                    Coordenadas = "41.6922,-8.8378"
                },
                new()
                {
                    Nome = "Praça da Canção (Recinto da Queima das Fitas)",
                    Descricao = "O famoso recinto da Queima das Fitas de Coimbra, situado na margem esquerda do rio Mondego.",
                    Outside = true,
                    Coordenadas = "40.2014,-8.4318"
                },
                new()
                {
                    Nome = "Campo de Viriato (Recinto da Feira de São Mateus)",
                    Descricao = "O mítico recinto que acolhe anualmente a secular Feira de São Mateus em Viseu.",
                    Outside = true,
                    Coordenadas = "40.6657,-7.9142"
                },
                new()
                {
                    Nome = "Avenida Central (Recinto de São João de Braga)",
                    Descricao = "A principal artéria bracarense, decorada e animada durante os festejos de São João.",
                    Outside = true,
                    Coordenadas = "41.5511,-8.4225"
                },
                new()
                {
                    Nome = "Largo de São Francisco (Feira de Santa Iria)",
                    Descricao = "Espaço multiusos em Faro que acolhe a tradicional e antiga Feira de Santa Iria.",
                    Outside = true,
                    Coordenadas = "37.0134,-7.9304"
                },
                new()
                {
                    Nome = "Avenida Arriaga (Festa da Flor)",
                    Descricao = "Placa central da Avenida Arriaga no Funchal, palco de exposições e do tapete floral.",
                    Outside = true,
                    Coordenadas = "32.6489,-16.9111"
                },
                new()
                {
                    Nome = "Campo de São Francisco (Festas do Senhor Santo Cristo)",
                    Descricao = "Grande largo em Ponta Delgada onde se realizam as celebrações profanas e de culto ao Senhor Santo Cristo.",
                    Outside = true,
                    Coordenadas = "37.7381,-25.6744"
                },
                new()
                {
                    Nome = "Rossio de São Brás (Feira de São João de Évora)",
                    Descricao = "O histórico largo eborense que se enche de luzes e comércio durante a Feira de São João.",
                    Outside = true,
                    Coordenadas = "38.5678,-7.9089"
                },

                // --- Novos locais espalhados pelo país (capitais de distrito) ---
                new()
                {
                    Nome = "Cidadela de Bragança (Recinto da Feira das Cantarinhas)",
                    Descricao = "Espaço histórico junto ao castelo bragançano, palco da centenária feira de artesanato em barro e da animação transmontana.",
                    Outside = true,
                    Coordenadas = "41.8064,-6.7567"
                },
                new()
                {
                    Nome = "Parque do Corgo (Recinto das Festas de São Pedro)",
                    Descricao = "Amplo parque urbano à beira do rio Corgo, principal recinto das festas e feiras de Vila Real.",
                    Outside = true,
                    Coordenadas = "41.3006,-7.7441"
                },
                new()
                {
                    Nome = "Rossio da Sé (Recinto das Feiras Francas)",
                    Descricao = "Praça emblemática junto à Sé da Guarda, palco das tradicionais Feiras Francas, as mais antigas de Portugal.",
                    Outside = true,
                    Coordenadas = "40.5374,-7.2683"
                },
                new()
                {
                    Nome = "Alameda da Liberdade (Recinto da Feira de Maio)",
                    Descricao = "Alameda arborizada no centro albicastrense, ponto de encontro da feira popular e dos concertos de maio.",
                    Outside = true,
                    Coordenadas = "39.8222,-7.4912"
                },
                new()
                {
                    Nome = "Rossio de Portalegre (Recinto da Feira de São Tiago)",
                    Descricao = "Largo central da cidade alentejana, anfitrião da tradicional Feira de São Tiago e das suas exposições regionais.",
                    Outside = true,
                    Coordenadas = "39.2967,-7.4281"
                },
                new()
                {
                    Nome = "Parque de Exposições de Beja (Recinto da Ovibeja)",
                    Descricao = "Maior recinto ferial do Alentejo, onde se realiza anualmente a Ovibeja, feira agropecuária e cultural de referência nacional.",
                    Outside = true,
                    Coordenadas = "38.0151,-7.8650"
                },
                new()
                {
                    Nome = "Avenida Luísa Todi (Recinto das Festas de Santiago)",
                    Descricao = "Avenida ribeirinha setubalense engalanada para as Festas de Santiago, com arraiais, desfiles e muita animação sardinheira.",
                    Outside = true,
                    Coordenadas = "38.5244,-8.8882"
                },
                new()
                {
                    Nome = "Parque da Feira do Ribatejo (Recinto da Feira Nacional de Agricultura)",
                    Descricao = "Vasto recinto ribatejano que acolhe a centenária Feira Nacional de Agricultura, com largadas de touros, concertos e exposições.",
                    Outside = true,
                    Coordenadas = "39.2362,-8.6862"
                },
                new()
                {
                    Nome = "Jardim Luís de Camões (Recinto das Feiras de Maio)",
                    Descricao = "Jardim central leiriense que recebe as animadas Feiras de Maio, com diversões, artesanato e grandes concertos.",
                    Outside = true,
                    Coordenadas = "39.7437,-8.8071"
                },
                new()
                {
                    Nome = "Cais da Fonte Nova (Recinto das Festas de São Gonçalinho)",
                    Descricao = "Espaço junto à Ria de Aveiro, palco das Festas de São Gonçalinho com o tradicional lançamento de cavacas do alto da capela.",
                    Outside = true,
                    Coordenadas = "40.6412,-8.6538"
                },

                // --- Novas aldeias/freguesias da região de Tomar ---
                new()
                {
                    Nome = "Olalhas",
                    Descricao = "Largo da freguesia de Olalhas, animado pelas festas anuais em honra do padroeiro local.",
                    Outside = true,
                    Coordenadas = "39.5453,-8.3661"
                },
                new()
                {
                    Nome = "Beselga",
                    Descricao = "Recinto da Beselga, conhecido pelo arraial de verão e pelas tasquinhas comunitárias.",
                    Outside = true,
                    Coordenadas = "39.5912,-8.3550"
                },
                new()
                {
                    Nome = "Madalena (Tomar)",
                    Descricao = "Largo da Madalena, ponto de encontro das festas paroquiais com procissão e baile popular.",
                    Outside = true,
                    Coordenadas = "39.6203,-8.3704"
                },
                new()
                {
                    Nome = "Sabacheira",
                    Descricao = "Recinto da Sabacheira, palco do tradicional arraial de São Pedro com petiscos e fogueira.",
                    Outside = true,
                    Coordenadas = "39.5701,-8.4302"
                },
                new()
                {
                    Nome = "Além da Ribeira",
                    Descricao = "Espaço ribeirinho de Além da Ribeira, animado pelas noites de fado e convívio popular de verão.",
                    Outside = true,
                    Coordenadas = "39.6087,-8.4203"
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

            if (locaisParaInserir.Count > 0)
            {
                await context.Locais.AddRangeAsync(locaisParaInserir);
                await context.SaveChangesAsync();
            }

            // 4. Seed Artistas (Artists)
            var todosArtistas = new List<Artista>
            {
                new()
                {
                    Nome = "Quim Barreiros",
                    Biografia = "O mestre da música popular e brejeira portuguesa, famoso pelo acordeão e pelas suas letras de duplo sentido.",
                    Contacto = "912345678",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4"
                },
                new()
                {
                    Nome = "Ana Malhoa",
                    Biografia = "Cantora e performer de referência em Portugal, traz um espetáculo eletrizante repleto de pop e ritmos tropicais.",
                    Contacto = "961234567",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1494790108377-be9c29b29330"
                },
                new()
                {
                    Nome = "Rancho Folclórico da Região",
                    Biografia = "Fundado há mais de 40 anos, preserva as danças, trajes e cantares típicos dos nossos antepassados rurais.",
                    Contacto = "249123456",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1482440308425-276ad0f28b19"
                },
                new()
                {
                    Nome = "Toy",
                    Biografia = "Ícone da música popular portuguesa, conhecido por grandes êxitos de cariz romântico e festivo como 'Coração não tem idade'.",
                    Contacto = "919999999",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1501386761578-eac5c94b800a"
                },
                new()
                {
                    Nome = "Rosinha",
                    Biografia = "Cantora popular com concertos animados e cheios de humor, conhecida pelas suas canções com acordeão e refrões divertidos.",
                    Contacto = "928888888",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1516450360452-9312f5e86fc7"
                },
                new()
                {
                    Nome = "Xutos & Pontapés",
                    Biografia = "A maior banda de rock em Portugal, com uma carreira de mais de 40 anos repleta de hinos intemporais.",
                    Contacto = "937777777",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1470225620780-dba8ba36b745"
                },
                new()
                {
                    Nome = "David Antunes & The Midnight Band",
                    Biografia = "Banda de pop-rock e entretenimento carismática, conhecida pelas suas performances enérgicas e interativas.",
                    Contacto = "916666666",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1506157786151-b8491531f063"
                },
                new()
                {
                    Nome = "Gisela João",
                    Biografia = "Uma das vozes mais marcantes do fado contemporâneo, trazendo uma intensidade emocional e frescura ao fado tradicional.",
                    Contacto = "965555555",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1534528741775-53994a69daeb"
                },
                new()
                {
                    Nome = "Miguel Araújo",
                    Biografia = "Músico, compositor e cantor português, autor de algumas das canções mais conhecidas da pop e folk nacional.",
                    Contacto = "914444444",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1507679799987-c73779587ccf"
                },
                new()
                {
                    Nome = "Bonga",
                    Biografia = "Embaixador da música angolana e do semba, com uma voz inconfundível que contagia qualquer recinto de dança.",
                    Contacto = "923333333",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1484755560693-a4074577af3a"
                },
                new()
                {
                    Nome = "The Gift",
                    Biografia = "Banda pioneira do pop/indie eletrónico em Portugal, famosa pelas atuações teatrais da vocalista Sónia Tavares.",
                    Contacto = "962222222",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1514320291840-2e0a9bf2a9ae"
                },
                new()
                {
                    Nome = "Kura",
                    Biografia = "Um dos DJs e produtores de música eletrónica portugueses mais reconhecidos internacionalmente.",
                    Contacto = "911111111",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1470229722913-7c0e2dbbafd3"
                },
                new()
                {
                    Nome = "Augusto Canário & Amigos",
                    Biografia = "Grupo de música tradicional e popular portuguesa, célebre pelas desgarradas e cantares ao desafio.",
                    Contacto = "929292929",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1511192336575-5a79af67a629"
                },
                new()
                {
                    Nome = "Bandas Filarmónicas Reunidas",
                    Biografia = "Agrupamento de músicos locais dedicados a manter vivas as marchas, hinos e clássicos filarmónicos nas festas da região.",
                    Contacto = "249999999",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1465847899084-d164df4dedc6"
                },
                new()
                {
                    Nome = "Tony Carreira",
                    Biografia = "O cantor romântico de referência nacional, com uma carreira de enorme sucesso repleta de grandes concertos.",
                    Contacto = "912223344",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4"
                },
                new()
                {
                    Nome = "Mariza",
                    Biografia = "Uma das maiores vozes do fado e embaixadora da música portuguesa a nível internacional.",
                    Contacto = "961122334",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1534528741775-53994a69daeb"
                },
                new()
                {
                    Nome = "Rui Veloso",
                    Biografia = "O pai do rock português, compositor de clássicos eternos e hinos da música nacional.",
                    Contacto = "931112223",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1470225620780-dba8ba36b745"
                },
                new()
                {
                    Nome = "Emanuel",
                    Biografia = "Ícone e pioneiro da música popular de dança em Portugal, com espetáculos sempre muito animados.",
                    Contacto = "913334445",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1501386761578-eac5c94b800a"
                },
                new()
                {
                    Nome = "Calema",
                    Biografia = "Dupla de irmãos que conquistou o público português com a sua mistura única de pop, kizomba e ritmos africanos.",
                    Contacto = "962233445",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1514320291840-2e0a9bf2a9ae"
                },
                new()
                {
                    Nome = "Pedro Abrunhosa",
                    Biografia = "Grande compositor, letrista e pianista da música pop-rock portuguesa, célebre pelas suas atuações magnéticas.",
                    Contacto = "919876543",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1506157786151-b8491531f063"
                },
                new()
                {
                    Nome = "Jorge Palma",
                    Biografia = "Exímio compositor, pianista e cantautor, autor de várias canções incontornáveis da música urbana nacional.",
                    Contacto = "929876543",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1507679799987-c73779587ccf"
                },
                new()
                {
                    Nome = "D.A.M.A",
                    Biografia = "Grupo pop nacional de enorme sucesso composto por Kasha, Miguel Coimbra e Miguel Cristovinho.",
                    Contacto = "939876543",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1482440308425-276ad0f28b19"
                },

                // --- Novos artistas (mais géneros e mais variedade para os cartazes) ---
                new()
                {
                    Nome = "Camané",
                    Biografia = "Um dos maiores fadistas da atualidade, reconhecido pela pureza vocal e pela fidelidade à tradição do fado de Lisboa.",
                    Contacto = "917001122",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f"
                },
                new()
                {
                    Nome = "Carminho",
                    Biografia = "Fadista de projeção internacional, conhecida por levar o fado contemporâneo aos maiores palcos do mundo.",
                    Contacto = "917002233",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1534528741775-53994a69daeb"
                },
                new()
                {
                    Nome = "David Carreira",
                    Biografia = "Cantor de pop urbano e R&B, conhecido pelos espetáculos cheios de energia e coreografias marcantes.",
                    Contacto = "917003344",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1506157786151-b8491531f063"
                },
                new()
                {
                    Nome = "Diogo Piçarra",
                    Biografia = "Cantor e compositor algarvio, autor de baladas românticas que se tornaram hinos da rádio nacional.",
                    Contacto = "917004455",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1501386761578-eac5c94b800a"
                },
                new()
                {
                    Nome = "Aurea",
                    Biografia = "Cantora de soul e pop com uma voz marcante, destacada por interpretações intensas e cheias de sentimento.",
                    Contacto = "917005566",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1516450360452-9312f5e86fc7"
                },
                new()
                {
                    Nome = "Agir",
                    Biografia = "Artista de reggae e pop português, conhecido pelas letras positivas e pelo ambiente descontraído dos seus concertos.",
                    Contacto = "917006677",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1470229722913-7c0e2dbbafd3"
                },
                new()
                {
                    Nome = "Moonspell",
                    Biografia = "Banda portuguesa de metal gótico com reconhecimento internacional, célebre pelos espetáculos intensos e teatrais.",
                    Contacto = "917007788",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1470225620780-dba8ba36b745"
                },
                new()
                {
                    Nome = "GNR",
                    Biografia = "Uma das bandas de rock mais influentes de Portugal, com décadas de carreira e êxitos incontornáveis.",
                    Contacto = "917008899",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1470225620780-dba8ba36b745"
                },
                new()
                {
                    Nome = "Black Mamba",
                    Biografia = "Banda de rock energética, conhecida pelos diretos vibrantes e pela forte ligação com o público mais jovem.",
                    Contacto = "917009900",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1511192336575-5a79af67a629"
                },
                new()
                {
                    Nome = "Sam the Kid",
                    Biografia = "Pioneiro e referência incontornável do hip-hop português, conhecido pelas letras incisivas e pela produção cuidada.",
                    Contacto = "917010011",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1484755560693-a4074577af3a"
                },
                new()
                {
                    Nome = "Plutonio",
                    Biografia = "Um dos nomes mais relevantes do rap português atual, com forte presença nos palcos e nas plataformas digitais.",
                    Contacto = "917011122",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1465847899084-d164df4dedc6"
                },
                new()
                {
                    Nome = "Carlão",
                    Biografia = "Músico que cruza o hip-hop com o pop e o rock, conhecido por uma carreira eclética e por letras muito pessoais.",
                    Contacto = "917012233",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1507679799987-c73779587ccf"
                },
                new()
                {
                    Nome = "Marante",
                    Biografia = "Cantor de música popular portuguesa, conhecido pela concertina e pelos espetáculos animados de arraial.",
                    Contacto = "917013344",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1511671782779-c97d3d27a1d4"
                },
                new()
                {
                    Nome = "Heidi",
                    Biografia = "Cantora de música popular e pimba, com espetáculos animados e muito próximos do público.",
                    Contacto = "917014455",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1494790108377-be9c29b29330"
                },
                new()
                {
                    Nome = "Marco Paulo",
                    Biografia = "Veterano da música popular portuguesa, com uma carreira de décadas repleta de êxitos de arraial.",
                    Contacto = "917015566",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1501386761578-eac5c94b800a"
                },
                new()
                {
                    Nome = "Pauliteiros de Miranda",
                    Biografia = "Grupo tradicional de dança de pauliteiros de Trás-os-Montes, património vivo da cultura mirandesa.",
                    Contacto = "917016677",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1482440308425-276ad0f28b19"
                },
                new()
                {
                    Nome = "Fernando Daniel",
                    Biografia = "Cantor revelado em concurso televisivo, hoje uma das vozes pop mais ouvidas do país.",
                    Contacto = "917017788",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1514320291840-2e0a9bf2a9ae"
                },
                new()
                {
                    Nome = "Anselmo Ralph",
                    Biografia = "Cantor de R&B e pop com raízes angolanas, conhecido pela voz potente e pelos espetáculos de grande produção.",
                    Contacto = "917018899",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1506157786151-b8491531f063"
                },
                new()
                {
                    Nome = "DJ Vibe",
                    Biografia = "Um dos pioneiros e nomes mais respeitados da música eletrónica portuguesa, com atuações em festivais por todo o país.",
                    Contacto = "917019900",
                    LinkFotoPerfil = "https://images.unsplash.com/photo-1470229722913-7c0e2dbbafd3"
                },
                new()
                {
                    Nome = "Coro de Vozes do Ribatejo",
                    Biografia = "Agrupamento coral ribatejano dedicado à preservação dos cantares e tradições populares da região.",
                    Contacto = "917020011",
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

            if (artistasParaInserir.Count > 0)
            {
                await context.Artistas.AddRangeAsync(artistasParaInserir);
                await context.SaveChangesAsync();
            }

            // 5. Seed Eventos (Events)
            var definicoesEventos = new List<(string NomeLocal, Evento Evento)>
            {
                ("Recinto Principal", new()
                {
                    Nome = "Grande Concerto de Abertura",
                    Descricao = "Cerimónia de inauguração oficial das festas seguida de espetáculo musical.",
                    DataInicio = DateTime.Today.AddDays(1).AddHours(21).AddMinutes(30),
                    DataFim = DateTime.Today.AddDays(1).AddHours(23).AddMinutes(30),
                    Patrocinador = "Super Bock"
                }),
                ("Recinto Principal", new()
                {
                    Nome = "Grande Arraial Popular",
                    Descricao = "Baile de verão com os melhores êxitos populares e muita animação pela noite dentro.",
                    DataInicio = DateTime.Today.AddDays(2).AddHours(21).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(3).AddHours(2).AddMinutes(0),
                    Patrocinador = "Licor Beirão"
                }),
                ("Palco Tradição", new()
                {
                    Nome = "Tarde de Cantares e Tradição",
                    Descricao = "Demonstrações etnográficas, ranchos locais e feira de gastronomia.",
                    DataInicio = DateTime.Today.AddDays(3).AddHours(15).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(3).AddHours(19).AddMinutes(0),
                    Patrocinador = "Adega Cooperativa Local"
                }),
                ("Ferreira do Zêzere", new()
                {
                    Nome = "Festas do Concelho de Ferreira do Zêzere",
                    Descricao = "Praça central animada pelas festas do concelho, concertos e feira gastronómica.",
                    DataInicio = DateTime.Today.AddDays(4).AddHours(18).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(5).AddHours(2).AddMinutes(0),
                    Patrocinador = "Delta Cafés"
                }),
                ("Serra", new()
                {
                    Nome = "Festas em Honra de Nossa Senhora da Purificação",
                    Descricao = "Tradicional arraial com fogaças, leilões e noites de baile.",
                    DataInicio = DateTime.Today.AddDays(5).AddHours(20).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(6).AddHours(3).AddMinutes(0),
                    Patrocinador = "Cerveja Sagres"
                }),
                ("Chão das Maias", new()
                {
                    Nome = "Grandes Jantares de São Bartolomeu",
                    Descricao = "Evento comunitário gastronómico com pratos típicos da região e música popular.",
                    DataInicio = DateTime.Today.AddDays(6).AddHours(19).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(6).AddHours(23).AddMinutes(30),
                    Patrocinador = "Vinhos do Tejo"
                }),
                ("Pedreira", new()
                {
                    Nome = "Arraial de Verão da Pedreira",
                    Descricao = "Tradicional arraial de verão com quermesse, música ao vivo e leilão de oferendas.",
                    DataInicio = DateTime.Today.AddDays(7).AddHours(21).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(8).AddHours(4).AddMinutes(0),
                    Patrocinador = "Intermarché"
                }),
                ("Casais", new()
                {
                    Nome = "Festas de Julho de Casais",
                    Descricao = "Festividades da paróquia com convívios, procissão e bailes noturnos.",
                    DataInicio = DateTime.Today.AddDays(8).AddHours(17).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(9).AddHours(1).AddMinutes(0),
                    Patrocinador = "Crédito Agrícola"
                }),
                ("Junceira", new()
                {
                    Nome = "Junceira com Tradições",
                    Descricao = "Mostra de tradições locais, folclore regional e doces tradicionais.",
                    DataInicio = DateTime.Today.AddDays(9).AddHours(15).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(9).AddHours(23).AddMinutes(0),
                    Patrocinador = "Fanta"
                }),
                ("Valdonas", new()
                {
                    Nome = "Noite de Baile de Valdonas",
                    Descricao = "Grande baile popular animado por artistas locais nos arredores de Tomar.",
                    DataInicio = DateTime.Today.AddDays(10).AddHours(22).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(11).AddHours(4).AddMinutes(0),
                    Patrocinador = "Rádio local"
                }),
                ("Águas Belas", new()
                {
                    Nome = "Festas de Verão de Águas Belas",
                    Descricao = "Arraial animado junto à bacia do Zêzere com petiscos e muita dança.",
                    DataInicio = DateTime.Today.AddDays(11).AddHours(19).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(12).AddHours(2).AddMinutes(0),
                    Patrocinador = "Adega de Tomar"
                }),
                ("Alqueidão", new()
                {
                    Nome = "Noite Recreativa do Alqueidão",
                    Descricao = "Evento focado em petiscos tradicionais, música ao vivo e convívio.",
                    DataInicio = DateTime.Today.AddDays(12).AddHours(20).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(13).AddHours(1).AddMinutes(0),
                    Patrocinador = "Mini Preço"
                }),
                ("Cabeças", new()
                {
                    Nome = "Festas de Santos Populares de Cabeças",
                    Descricao = "Marchas populares, sardinhada e baile dedicado ao São João.",
                    DataInicio = DateTime.Today.AddDays(13).AddHours(20).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(14).AddHours(3).AddMinutes(0),
                    Patrocinador = "Coca-Cola"
                }),
                ("Vialonga", new()
                {
                    Nome = "Arraial Bairrista de Vialonga",
                    Descricao = "Convívio de bairro com fados, sardinhada e animação infantil.",
                    DataInicio = DateTime.Today.AddDays(14).AddHours(18).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(14).AddHours(23).AddMinutes(59),
                    Patrocinador = "Padaria Central"
                }),
                ("Barreiras", new()
                {
                    Nome = "Festa Tradicional de Barreiras",
                    Descricao = "Festejos religiosos e tradicionais de cariz intimista.",
                    DataInicio = DateTime.Today.AddDays(15).AddHours(11).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(15).AddHours(23).AddMinutes(0),
                    Patrocinador = "Talho da Aldeia"
                }),
                ("Fonte Dom João", new()
                {
                    Nome = "Tasquinhas de Fonte Dom João",
                    Descricao = "Mostra gastronómica regional e convívio popular ao ar livre.",
                    DataInicio = DateTime.Today.AddDays(16).AddHours(19).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(17).AddHours(0).AddMinutes(30),
                    Patrocinador = "Azeite Fátima"
                }),
                ("Poço Redondo", new()
                {
                    Nome = "Festas do Divino Espírito Santo",
                    Descricao = "Celebrações tradicionais, jogos populares e feira de artesanato.",
                    DataInicio = DateTime.Today.AddDays(17).AddHours(10).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(18).AddHours(22).AddMinutes(0),
                    Patrocinador = "Junta de Freguesia"
                }),
                ("Carqueijal", new()
                {
                    Nome = "Festa dos Emigrantes do Carqueijal",
                    Descricao = "Grande almoço comunitário e tarde cultural dedicada aos emigrantes da aldeia.",
                    DataInicio = DateTime.Today.AddDays(18).AddHours(12).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(18).AddHours(21).AddMinutes(0),
                    Patrocinador = "Banco BPI"
                }),
                ("Bairradinha", new()
                {
                    Nome = "Romaria Tradicional da Bairradinha",
                    Descricao = "Romaria tradicional à beira da albufeira do Castelo de Bode com piquenique comunitário.",
                    DataInicio = DateTime.Today.AddDays(19).AddHours(9).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(19).AddHours(18).AddMinutes(0),
                    Patrocinador = "Pingo Doce"
                }),
                ("Cem Soldos", new()
                {
                    Nome = "Festival Bons Sons",
                    Descricao = "Festival de música portuguesa de referência nacional, espalhado por toda a aldeia de Cem Soldos.",
                    DataInicio = DateTime.Today.AddDays(20).AddHours(16).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(24).AddHours(23).AddMinutes(59),
                    Patrocinador = "Caixa Geral de Depósitos"
                }),
                ("Asseiceira", new()
                {
                    Nome = "Romaria Anual de Asseiceira",
                    Descricao = "Festa anual e romarias religiosas com missa, procissão e arraial de encerramento.",
                    DataInicio = DateTime.Today.AddDays(25).AddHours(10).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(25).AddHours(23).AddMinutes(0),
                    Patrocinador = "Águas do Luso"
                }),
                ("Paialvo", new()
                {
                    Nome = "Festas de Agosto de Paialvo",
                    Descricao = "Tradicional festa com gastronomia forte, sopa de pedras e música popular.",
                    DataInicio = DateTime.Today.AddDays(26).AddHours(19).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(27).AddHours(2).AddMinutes(0),
                    Patrocinador = "Supermercado local"
                }),
                ("Santa Cita", new()
                {
                    Nome = "Festejos de Santa Cita",
                    Descricao = "Arraial e convívio popular celebrando o encerramento das festividades de verão.",
                    DataInicio = DateTime.Today.AddDays(28).AddHours(21).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(29).AddHours(3).AddMinutes(0),
                    Patrocinador = "Adega Regional"
                }),
                ("Terreiro do Paço", new()
                {
                    Nome = "Festas de Santo António de Lisboa",
                    Descricao = "A grande celebração da capital com as marchas populares, casamentos de Santo António e muita sardinha assada.",
                    DataInicio = DateTime.Today.AddDays(30).AddHours(18).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(31).AddHours(6).AddMinutes(0),
                    Patrocinador = "Sagres"
                }),
                ("Zona da Ribeira", new()
                {
                    Nome = "Festas de São João do Porto",
                    Descricao = "A noite mais longa do ano na Invicta, marcada pelos balões de ar quente, martelinhos e o fogo-de-artifício no Douro.",
                    DataInicio = DateTime.Today.AddDays(31).AddHours(19).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(32).AddHours(5).AddMinutes(0),
                    Patrocinador = "Super Bock"
                }),
                ("Campo da Agonia", new()
                {
                    Nome = "Romaria de Nossa Senhora da Agonia",
                    Descricao = "A grandiosa romaria minhota com o seu desfile da mordomia, tapetes floridos e o tradicional traje à vianesa.",
                    DataInicio = DateTime.Today.AddDays(32).AddHours(10).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(34).AddHours(23).AddMinutes(59),
                    Patrocinador = "Junta de Turismo"
                }),
                ("Campo de Viriato", new()
                {
                    Nome = "Feira de São Mateus",
                    Descricao = "A feira popular mais antiga da Península Ibérica, cheia de expositores, diversão, farturas e grandes concertos.",
                    DataInicio = DateTime.Today.AddDays(35).AddHours(17).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(38).AddHours(23).AddMinutes(59),
                    Patrocinador = "Montepio"
                }),
                ("Praça da Canção", new()
                {
                    Nome = "Queima das Fitas de Coimbra",
                    Descricao = "A histórica e emblemática festa dos estudantes universitários conimbricenses, repleta de tradição e grandes noites académicas.",
                    DataInicio = DateTime.Today.AddDays(39).AddHours(22).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(42).AddHours(6).AddMinutes(0),
                    Patrocinador = "Delta Cafés"
                }),
                ("Avenida Arriaga", new()
                {
                    Nome = "Festa da Flor da Madeira",
                    Descricao = "Homenagem à primavera com cortejos de carros alegóricos florais, tapetes de pétalas e o terno Muro da Esperança.",
                    DataInicio = DateTime.Today.AddDays(43).AddHours(10).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(45).AddHours(20).AddMinutes(0),
                    Patrocinador = "Governo Regional"
                }),
                ("Campo de São Francisco", new()
                {
                    Nome = "Festas do Senhor Santo Cristo dos Milagres",
                    Descricao = "A segunda maior festividade religiosa do país, enchendo Ponta Delgada de fiéis e belíssimos tapetes de flores.",
                    DataInicio = DateTime.Today.AddDays(46).AddHours(11).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(48).AddHours(23).AddMinutes(0),
                    Patrocinador = "SATA"
                }),

                // --- Eventos para locais que ainda não tinham festa associada ---
                ("Avenida Central", new()
                {
                    Nome = "Festas de São João de Braga",
                    Descricao = "A icónica festa minhota com os tradicionais martelinhos de plástico, alho-porro, balões coloridos e arraial pela madrugada.",
                    DataInicio = DateTime.Today.AddDays(49).AddHours(20).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(50).AddHours(5).AddMinutes(0),
                    Patrocinador = "Super Bock"
                }),
                ("Largo de São Francisco", new()
                {
                    Nome = "Feira de Santa Iria",
                    Descricao = "Uma das feiras mais antigas do Algarve, com diversões, artesanato regional e animação musical em pleno centro de Faro.",
                    DataInicio = DateTime.Today.AddDays(51).AddHours(17).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(53).AddHours(23).AddMinutes(59),
                    Patrocinador = "Câmara Municipal de Faro"
                }),
                ("Rossio de São Brás", new()
                {
                    Nome = "Feira de São João de Évora",
                    Descricao = "Tradicional feira alentejana com exposições, gastronomia regional e noites de concertos no histórico Rossio de São Brás.",
                    DataInicio = DateTime.Today.AddDays(54).AddHours(18).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(56).AddHours(23).AddMinutes(59),
                    Patrocinador = "Vinhos do Alentejo"
                }),
                ("Auditório da Aldeia", new()
                {
                    Nome = "Noite de Fado e Música Erudita",
                    Descricao = "Espetáculo intimista no auditório fechado, dedicado ao fado e à música erudita portuguesa.",
                    DataInicio = DateTime.Today.AddDays(57).AddHours(21).AddMinutes(30),
                    DataFim = DateTime.Today.AddDays(57).AddHours(23).AddMinutes(30),
                    Patrocinador = "Câmara Municipal"
                }),

                // --- Novos eventos nacionais (capitais de distrito) ---
                ("Cidadela de Bragança", new()
                {
                    Nome = "Feira das Cantarinhas",
                    Descricao = "Centenária feira de artesanato em barro junto ao castelo, acompanhada de folclore transmontano e mostra de produtos regionais.",
                    DataInicio = DateTime.Today.AddDays(58).AddHours(10).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(60).AddHours(22).AddMinutes(0),
                    Patrocinador = "Câmara Municipal de Bragança"
                }),
                ("Parque do Corgo", new()
                {
                    Nome = "Festas de São Pedro de Vila Real",
                    Descricao = "Programação de concertos, feira popular e fogo-de-artifício junto ao rio Corgo, em honra do padroeiro da cidade.",
                    DataInicio = DateTime.Today.AddDays(61).AddHours(19).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(63).AddHours(2).AddMinutes(0),
                    Patrocinador = "EDP"
                }),
                ("Rossio da Sé", new()
                {
                    Nome = "Feiras Francas da Guarda",
                    Descricao = "A mais antiga feira de Portugal, com origem medieval, reunindo artesanato, gastronomia e animação musical na Sé da Guarda.",
                    DataInicio = DateTime.Today.AddDays(64).AddHours(10).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(66).AddHours(22).AddMinutes(0),
                    Patrocinador = "Turismo Centro de Portugal"
                }),
                ("Alameda da Liberdade", new()
                {
                    Nome = "Feira de Maio de Castelo Branco",
                    Descricao = "Feira popular albicastrense com diversões, exposições regionais e concertos ao ar livre na Alameda da Liberdade.",
                    DataInicio = DateTime.Today.AddDays(67).AddHours(18).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(68).AddHours(23).AddMinutes(59),
                    Patrocinador = "Continente"
                }),
                ("Rossio de Portalegre", new()
                {
                    Nome = "Feira de São Tiago de Portalegre",
                    Descricao = "Feira tradicional alto-alentejana com exposições de artesanato, tapeçaria local e animação cultural.",
                    DataInicio = DateTime.Today.AddDays(69).AddHours(17).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(70).AddHours(23).AddMinutes(0),
                    Patrocinador = "Câmara Municipal de Portalegre"
                }),
                ("Parque de Exposições de Beja", new()
                {
                    Nome = "Ovibeja",
                    Descricao = "A maior feira agropecuária, comercial e cultural do Alentejo, com exposições de gado, gastronomia e grandes concertos.",
                    DataInicio = DateTime.Today.AddDays(71).AddHours(10).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(75).AddHours(23).AddMinutes(59),
                    Patrocinador = "Crédito Agrícola"
                }),
                ("Avenida Luísa Todi", new()
                {
                    Nome = "Festas de Santiago de Setúbal",
                    Descricao = "Festas em honra do padroeiro da cidade, com arraiais, sardinhada e animação na avenida ribeirinha.",
                    DataInicio = DateTime.Today.AddDays(76).AddHours(19).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(78).AddHours(3).AddMinutes(0),
                    Patrocinador = "Sagres"
                }),
                ("Parque da Feira do Ribatejo", new()
                {
                    Nome = "Feira Nacional de Agricultura",
                    Descricao = "Histórico certame ribatejano dedicado à agricultura e pecuária, com largadas de touros, fado e grandes espetáculos musicais.",
                    DataInicio = DateTime.Today.AddDays(79).AddHours(10).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(84).AddHours(23).AddMinutes(59),
                    Patrocinador = "Banco Montepio"
                }),
                ("Jardim Luís de Camões", new()
                {
                    Nome = "Feiras de Maio de Leiria",
                    Descricao = "Feira popular leiriense com diversões, artesanato e concertos no centro da cidade.",
                    DataInicio = DateTime.Today.AddDays(85).AddHours(18).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(87).AddHours(23).AddMinutes(59),
                    Patrocinador = "Worten"
                }),
                ("Cais da Fonte Nova", new()
                {
                    Nome = "Festas de São Gonçalinho",
                    Descricao = "Festa aveirense marcada pelo tradicional lançamento de cavacas e pães do alto da capela do santo casamenteiro.",
                    DataInicio = DateTime.Today.AddDays(88).AddHours(11).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(89).AddHours(22).AddMinutes(0),
                    Patrocinador = "Câmara Municipal de Aveiro"
                }),

                // --- Novos eventos nas aldeias da região de Tomar ---
                ("Olalhas", new()
                {
                    Nome = "Festas de Olalhas",
                    Descricao = "Festejos anuais em honra do padroeiro local, com procissão, arraial e baile popular.",
                    DataInicio = DateTime.Today.AddDays(90).AddHours(20).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(91).AddHours(2).AddMinutes(0),
                    Patrocinador = "Junta de Freguesia"
                }),
                ("Beselga", new()
                {
                    Nome = "Arraial de Verão da Beselga",
                    Descricao = "Arraial comunitário com tasquinhas, leilão de oferendas e muita animação musical.",
                    DataInicio = DateTime.Today.AddDays(92).AddHours(21).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(93).AddHours(3).AddMinutes(0),
                    Patrocinador = "Adega Cooperativa Local"
                }),
                ("Madalena", new()
                {
                    Nome = "Festas da Madalena",
                    Descricao = "Festejos paroquiais com procissão, baile popular e fogo-de-artifício.",
                    DataInicio = DateTime.Today.AddDays(94).AddHours(19).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(94).AddHours(23).AddMinutes(59),
                    Patrocinador = "Padaria Central"
                }),
                ("Sabacheira", new()
                {
                    Nome = "Arraial de São Pedro da Sabacheira",
                    Descricao = "Tradicional arraial de São Pedro com fogueira, petiscos e baile até de madrugada.",
                    DataInicio = DateTime.Today.AddDays(95).AddHours(21).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(96).AddHours(2).AddMinutes(0),
                    Patrocinador = "Talho da Aldeia"
                }),
                ("Além da Ribeira", new()
                {
                    Nome = "Noites de Fado de Além da Ribeira",
                    Descricao = "Serão ribeirinho dedicado ao fado e à música tradicional portuguesa, junto ao rio.",
                    DataInicio = DateTime.Today.AddDays(97).AddHours(21).AddMinutes(0),
                    DataFim = DateTime.Today.AddDays(97).AddHours(23).AddMinutes(59),
                    Patrocinador = "Câmara Municipal"
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

            if (eventosParaInserir.Count > 0)
            {
                await context.Eventos.AddRangeAsync(eventosParaInserir);
                await context.SaveChangesAsync();
            }

            // 6. Seed Cartazes (Lineup)
            var specs = new List<(string EventoQuery, string ArtistaQuery, TimeSpan Offset, int Duracao)>
            {
                ("Grande Concerto de Abertura", "Ana Malhoa", TimeSpan.FromMinutes(15), 90),
                ("Grande Concerto de Abertura", "David Carreira", TimeSpan.Zero, 15),
                ("Grande Arraial Popular", "Quim Barreiros", TimeSpan.FromHours(1), 120),
                ("Grande Arraial Popular", "Marco Paulo", TimeSpan.FromHours(3), 90),
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
                ("Festival Bons Sons", "Kura", TimeSpan.FromDays(3).Add(TimeSpan.FromHours(7)), 120),
                ("Festival Bons Sons", "Carlão", TimeSpan.FromDays(4).Add(TimeSpan.FromHours(5)), 90),
                ("Festas de Santo António de Lisboa", "Tony Carreira", TimeSpan.FromHours(4), 120),
                ("Festas de Santo António de Lisboa", "GNR", TimeSpan.FromHours(6), 90),
                ("Festas de Santo António de Lisboa", "Mariza", TimeSpan.FromHours(8), 90),
                ("Festas de São João do Porto", "Rui Veloso", TimeSpan.FromHours(3), 120),
                ("Romaria de Nossa Senhora da Agonia", "Emanuel", TimeSpan.FromHours(8), 90),
                ("Romaria de Nossa Senhora da Agonia", "Marante", TimeSpan.FromDays(1).Add(TimeSpan.FromHours(9)), 90),
                ("Feira de São Mateus", "Calema", TimeSpan.FromHours(5), 90),
                ("Feira de São Mateus", "Pedro Abrunhosa", TimeSpan.FromDays(1).Add(TimeSpan.FromHours(4)), 90),
                ("Feira de São Mateus", "Diogo Piçarra", TimeSpan.FromDays(2).Add(TimeSpan.FromHours(5)), 90),
                ("Queima das Fitas de Coimbra", "D.A.M.A", TimeSpan.FromHours(3), 90),
                ("Festa da Flor da Madeira", "Jorge Palma", TimeSpan.FromHours(2), 90),
                ("Festas do Senhor Santo Cristo dos Milagres", "Mariza", TimeSpan.FromHours(4), 90),
                ("Festas do Senhor Santo Cristo dos Milagres", "Anselmo Ralph", TimeSpan.FromDays(1).Add(TimeSpan.FromHours(5)), 90),

                // --- Cartazes dos novos eventos (locais que já existiam mas sem evento) ---
                ("Festas de São João de Braga", "GNR", TimeSpan.FromHours(1), 90),
                ("Festas de São João de Braga", "Marante", TimeSpan.FromHours(3), 60),
                ("Feira de Santa Iria", "Anselmo Ralph", TimeSpan.FromDays(1).Add(TimeSpan.FromHours(4)), 90),
                ("Feira de Santa Iria", "DJ Vibe", TimeSpan.FromDays(2).Add(TimeSpan.FromHours(5)), 90),
                ("Feira de São João de Évora", "Coro de Vozes do Ribatejo", TimeSpan.FromDays(1).Add(TimeSpan.FromHours(3)), 75),
                ("Feira de São João de Évora", "Carlão", TimeSpan.FromDays(2).Add(TimeSpan.FromHours(4)), 90),
                ("Noite de Fado e Música Erudita", "Camané", TimeSpan.FromMinutes(15), 90),

                // --- Cartazes dos novos eventos nacionais ---
                ("Feira das Cantarinhas", "Pauliteiros de Miranda", TimeSpan.FromHours(6), 45),
                ("Feira das Cantarinhas", "Carminho", TimeSpan.FromDays(1).Add(TimeSpan.FromHours(8)), 75),
                ("Festas de São Pedro de Vila Real", "Fernando Daniel", TimeSpan.FromHours(2), 90),
                ("Festas de São Pedro de Vila Real", "Marco Paulo", TimeSpan.FromDays(1).Add(TimeSpan.FromHours(3)), 120),
                ("Feiras Francas da Guarda", "Heidi", TimeSpan.FromHours(9), 90),
                ("Feiras Francas da Guarda", "Black Mamba", TimeSpan.FromDays(1).Add(TimeSpan.FromHours(10)), 90),
                ("Feira de Maio de Castelo Branco", "David Carreira", TimeSpan.FromHours(3), 90),
                ("Feira de São Tiago de Portalegre", "Diogo Piçarra", TimeSpan.FromHours(4), 90),
                ("Ovibeja", "Aurea", TimeSpan.FromHours(10), 90),
                ("Ovibeja", "Agir", TimeSpan.FromDays(1).Add(TimeSpan.FromHours(11)), 90),
                ("Ovibeja", "Sam the Kid", TimeSpan.FromDays(2).Add(TimeSpan.FromHours(11)), 75),
                ("Ovibeja", "Plutonio", TimeSpan.FromDays(3).Add(TimeSpan.FromHours(11)), 75),
                ("Festas de Santiago de Setúbal", "Moonspell", TimeSpan.FromHours(3), 90),
                ("Festas de Santiago de Setúbal", "Carminho", TimeSpan.FromDays(1).Add(TimeSpan.FromHours(4)), 75),
                ("Feira Nacional de Agricultura", "Camané", TimeSpan.FromHours(11), 90),
                ("Feira Nacional de Agricultura", "GNR", TimeSpan.FromDays(1).Add(TimeSpan.FromHours(11)), 90),
                ("Feira Nacional de Agricultura", "Heidi", TimeSpan.FromDays(2).Add(TimeSpan.FromHours(11)), 90),
                ("Feira Nacional de Agricultura", "Marco Paulo", TimeSpan.FromDays(3).Add(TimeSpan.FromHours(11)), 120),
                ("Feiras de Maio de Leiria", "Diogo Piçarra", TimeSpan.FromHours(3), 90),
                ("Feiras de Maio de Leiria", "Black Mamba", TimeSpan.FromDays(1).Add(TimeSpan.FromHours(4)), 90),
                ("Festas de São Gonçalinho", "Coro de Vozes do Ribatejo", TimeSpan.FromHours(9), 60),

                // --- Cartazes das novas aldeias da região de Tomar ---
                ("Festas de Olalhas", "Marante", TimeSpan.FromHours(1), 90),
                ("Arraial de Verão da Beselga", "Heidi", TimeSpan.FromHours(1), 90),
                ("Festas da Madalena", "Rancho Folclórico", TimeSpan.FromHours(2), 90),
                ("Arraial de São Pedro da Sabacheira", "Marco Paulo", TimeSpan.FromHours(1), 90),
                ("Noites de Fado de Além da Ribeira", "Camané", TimeSpan.FromMinutes(30), 90)
            };

            var cartazesParaInserir = new List<Cartaz>();

            foreach (var (eventoQuery, artistaQuery, offset, duracao) in specs)
            {
                var evento = await context.Eventos.FirstOrDefaultAsync(e => e.Nome.Contains(eventoQuery));
                var artista = await context.Artistas.FirstOrDefaultAsync(a => a.Nome.Contains(artistaQuery));

                if (evento != null && artista != null)
                {
                    bool existeAtuacao = await context.Cartazes.AnyAsync(c => c.IdEvento == evento.IdEvento && c.IdArtista == artista.IdArtista);
                    if (!existeAtuacao)
                    {
                        var dataAtuacao = evento.DataInicio.Add(offset);

                        if (dataAtuacao >= evento.DataInicio && dataAtuacao < evento.DataFim)
                        {
                            cartazesParaInserir.Add(new()
                            {
                                IdEvento = evento.IdEvento,
                                IdArtista = artista.IdArtista,
                                DataHoraAtuacao = dataAtuacao,
                                DuracaoMinutos = duracao
                            });
                        }
                    }
                }
            }

            if (cartazesParaInserir.Count > 0)
            {
                await context.Cartazes.AddRangeAsync(cartazesParaInserir);
                await context.SaveChangesAsync();
            }
        }
    }
}
