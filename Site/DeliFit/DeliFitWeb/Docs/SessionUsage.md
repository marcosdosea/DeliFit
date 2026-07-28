# Documentação de Uso da Sessão no DeliFit

## Visão Geral

O sistema DeliFit agora possui um gerenciamento de sessão que armazena informações importantes do usuário logado para otimizar o desempenho e reduzir consultas ao banco de dados.

## Configuração

### 1. SessionHelper
Localização: `DeliFitWeb\Helpers\SessionHelper.cs`

Este helper fornece métodos de extensão para facilitar o acesso aos dados da sessão:

```csharp
// Cliente
HttpContext.Session.SetClienteId(clienteId);
var clienteId = HttpContext.Session.GetClienteId();

// Restaurante
HttpContext.Session.SetRestauranteId(restauranteId);
var restauranteId = HttpContext.Session.GetRestauranteId();

// Email do usuário
HttpContext.Session.SetUserEmail(email);
var email = HttpContext.Session.GetUserEmail();

// Role do usuário
HttpContext.Session.SetUserRole(role);
var role = HttpContext.Session.GetUserRole();

// Limpar todos os dados
HttpContext.Session.ClearUserData();
```

### 2. Armazenamento Automático no Login
Os dados do usuário são armazenados automaticamente durante o login em:
`DeliFitWeb\Areas\Identity\Pages\Account\Login.cshtml.cs`

- **Admin**: Armazena role "Admin"
- **GerenteRestaurante**: Armazena role "GerenteRestaurante" e ID do restaurante
- **Cliente**: Armazena role "Cliente" e ID do cliente

### 3. Limpeza Automática no Logout
Os dados da sessão são limpos automaticamente durante o logout em:
`DeliFitWeb\Areas\Identity\Pages\Account\Logout.cshtml.cs`

## Exemplos de Uso nos Controllers

### ClienteController

```csharp
// Método auxiliar privado para obter ID do cliente logado
private uint? GetClienteIdLogado()
{
    // Tenta buscar da sessão
    var clienteId = HttpContext.Session.GetClienteId();
    
    if (!clienteId.HasValue)
    {
        // Se não estiver na sessão, busca pelo email
        var userEmail = _userManager.GetUserName(User);
        var cliente = _clienteService.GetByEmail(userEmail);
        
        if (cliente != null)
        {
            // Armazena na sessão para próximas requisições
            HttpContext.Session.SetClienteId(cliente.Id);
            clienteId = cliente.Id;
        }
    }
    
    return clienteId;
}

// Uso em actions
public ActionResult MeuPerfil()
{
    var clienteId = GetClienteIdLogado();
    if (!clienteId.HasValue)
        return NotFound();
        
    var cliente = _clienteService.Get(clienteId.Value);
    return View(cliente);
}
```

### RestauranteController

```csharp
// Método auxiliar privado para obter ID do restaurante logado
private uint? GetRestauranteIdLogado()
{
    var restauranteId = HttpContext.Session.GetRestauranteId();
    
    if (!restauranteId.HasValue)
    {
        var userEmail = User.Identity?.Name;
        if (!string.IsNullOrEmpty(userEmail))
        {
            var restaurante = _restauranteService.GetByEmail(userEmail);
            
            if (restaurante != null)
            {
                HttpContext.Session.SetRestauranteId(restaurante.Id);
                restauranteId = restaurante.Id;
            }
        }
    }
    
    return restauranteId;
}
```

## Casos de Uso Adicionais

### Armazenar Carrinho de Compras (Exemplo)

```csharp
// No CarrinhoController
public class CarrinhoItem
{
    public uint ItemId { get; set; }
    public int Quantidade { get; set; }
    public decimal Preco { get; set; }
}

// Adicionar item ao carrinho
public IActionResult AdicionarItem(uint itemId, int quantidade)
{
    var carrinho = HttpContext.Session.GetObject<List<CarrinhoItem>>("Carrinho") 
                   ?? new List<CarrinhoItem>();
    
    carrinho.Add(new CarrinhoItem 
    { 
        ItemId = itemId, 
        Quantidade = quantidade 
    });
    
    HttpContext.Session.SetObject("Carrinho", carrinho);
    
    return RedirectToAction("Index");
}

// Obter carrinho
public IActionResult VerCarrinho()
{
    var carrinho = HttpContext.Session.GetObject<List<CarrinhoItem>>("Carrinho");
    return View(carrinho);
}
```

### Armazenar Preferências de Filtro

```csharp
// Armazenar filtros de busca
public IActionResult AplicarFiltro(string categoria, decimal precoMax)
{
    HttpContext.Session.SetString("FiltroCategoriaAtual", categoria);
    HttpContext.Session.SetInt32("FiltroPrecoMax", (int)(precoMax * 100));
    
    return RedirectToAction("Index");
}

// Recuperar filtros
public IActionResult Index()
{
    var categoria = HttpContext.Session.GetString("FiltroCategoriaAtual");
    var precoMaxCents = HttpContext.Session.GetInt32("FiltroPrecoMax");
    var precoMax = precoMaxCents.HasValue ? precoMaxCents.Value / 100m : 0;
    
    // Aplicar filtros na consulta...
}
```

## Benefícios

1. **Performance**: Reduz consultas ao banco de dados
2. **Simplicidade**: API fácil de usar com métodos de extensão
3. **Segurança**: Dados armazenados no servidor, não no cliente
4. **Automático**: Login/Logout gerenciam a sessão automaticamente

## Notas Importantes

- As sessões expiram após 60 minutos de inatividade (configurado no Program.cs)
- Os dados são armazenados em memória no servidor
- Para ambientes com múltiplos servidores, considere usar cache distribuído (Redis, SQL Server, etc.)
- Sempre verifique se o valor retornado é nulo antes de usar
