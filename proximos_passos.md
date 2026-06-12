# Próximos Passos: Roadmap & Ideias de Desenvolvimento

Este documento serve como guia de planeamento para as próximas etapas do trabalho de Desenvolvimento Web, consolidando a divisão de responsabilidades, controlo de acessos e a implementação da funcionalidade sugerida de **Geolocalização por Raio de Distância**.

---

## 1. Controlo de Acesso (Segurança & Autenticação)
Para cumprir os requisitos do projeto, a aplicação deve distinguir entre utilizadores administradores e visitantes:

- [x] **Configuração do Identity**:
  - Garantir que o ASP.NET Core Identity está ativo.
  - Criar um mecanismo de *Seed* (população inicial da BD) para criar automaticamente a role `Admin` e uma conta de administrador inicial (`admin@festas.com` / `123Qwe##`), documentando as credenciais.
- [x] **Proteção de Rotas**:
  - Adicionar o atributo `[Authorize(Roles = "Admin")]` no topo das classes das páginas de criação, edição e eliminação (`Create.cshtml.cs`, `Edit.cshtml.cs`, `Delete.cshtml.cs`) de todas as entidades (*Artista, Cartaz, Evento, Local*).
- [x] **Ocultação de Interface**:
  - Utilizar `@if (User.IsInRole("Admin"))` nas listagens públicas para exibir os botões de ação (Editar/Eliminar) apenas a utilizadores autenticados e autorizados.
- [ ] **Design da Autenticação (Login/Registo)**:
  - Personalizar o visual das páginas de Login e Registo geradas pelo Identity, substituindo o layout básico do Bootstrap por um design moderno e elegante em harmonia com o tema RallyFestas.

---

## 2. Estrutura de Páginas Públicas (Utilizador Final)
As vistas devem ser intuitivas e focadas em quem quer ir às festas:

- [] **Página Inicial (Landing Page)**:
  - Banner dinâmico de boas-vindas com o nome e as datas das festas.
  - Destaques rápidos: "Hoje nas Festas" (eventos a decorrer nas próximas horas) e "Locais mais movimentados".
- [ ] **Detalhes Públicos**:
  - Vista detalhada do Evento com o alinhamento de artistas (Cartaz) cronológico.
  - Vista detalhada do Artista com biografia, contactos e agenda dele durante o festival.
  - Vista detalhada do Local com mapa e acessos.
- [ ] **Poster de Evento Opcional**:
  - Adicionar o campo `LinkPoster` à entidade `Evento`.
  - Exibir a imagem do poster se estiver preenchida; caso contrário, apresentar o cartão de evento padrão.


---

## 3. Funcionalidade Premium: Geolocalização de Eventos
A ideia sugerida pelo professor de obter a localização das pessoas para encontrar festas próximas é excelente e eleva o nível técnico do trabalho (usabilidade e inovação).

### Como Funciona a Implementação Técnica:

#### A. Obtenção da Localização do Utilizador (Frontend)
Utilizar a API de Geolocalização nativa do browser (HTML5 Geolocation API) via JavaScript para obter a latitude e longitude do utilizador após a sua permissão:
```javascript
if (navigator.geolocation) {
    navigator.geolocation.getCurrentPosition(successCallback, errorCallback);
}

function successCallback(position) {
    const userLat = position.coords.latitude;
    const userLng = position.coords.longitude;
    // Enviar estas coordenadas para o servidor ou processar no cliente
}
```

#### B. Cálculo da Distância (Backend ou Frontend)
Para calcular a distância entre o utilizador e cada local de festa, utiliza-se a **Fórmula de Haversine** (que calcula a distância em linha reta entre dois pontos numa esfera):

* **Fórmula de Haversine em C#**:
```csharp
public static double CalcularDistanciaKm(double lat1, double lon1, double lat2, double lon2)
{
    double R = 6371; // Raio da Terra em km
    double dLat = (lat2 - lat1) * Math.PI / 180.0;
    double dLon = (lon2 - lon1) * Math.PI / 180.0;
    
    double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
               Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
               Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
               
    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    return R * c;
}
```

#### C. Filtragem Dinâmica por Raio
1. Na página inicial ou na página de eventos, disponibilizar um seletor (ex: *slider* de 1 km a 50 km).
2. O utilizador ativa a localização e escolhe o raio pretendido (ex: "Mostrar festas num raio de 10 km").
3. A aplicação filtra os locais que possuem coordenadas no formato `latitude,longitude` e exibe apenas os eventos nesses recintos.

#### D. Visualização num Mapa de Festas
Integrar uma biblioteca de mapas leve e pública como o **Leaflet.js** (OpenStreetMap, 100% gratuita, sem necessidade de chaves de API pagas):
* Renderizar um mapa interativo onde:
  * Um marcador azul representa o **utilizador** (onde ele está).
  * Marcadores vermelhos/coloridos representam os **locais com eventos ativos**.
  * Clicar no marcador do local abre um balão com o nome do evento, local e botão para obter direções.
