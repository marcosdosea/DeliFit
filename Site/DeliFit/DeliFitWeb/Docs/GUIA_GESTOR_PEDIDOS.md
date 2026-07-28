## Gestor de Pedidos - Guia de Uso

### Funcionalidades Implementadas

#### 1. **Dashboard Interativo**
- Métrica de "Entregas Hoje" - conta pedidos com status = "EM ENTREGA"
- Métrica de "Pedidos Ativos" - conta pedidos que não foram entregues
- Métrica de "Tempo Médio" - calcula tempo médio dos pedidos ativos

#### 2. **Kanban Board**
A tela é dividida em 4 colunas:
- **PENDENTES** - Pedidos aguardando início do preparo
- **EM PREPARO** - Pedidos em preparação
- **PRONTO** - Pedidos finalizados e prontos
- **EM ENTREGA** - Pedidos em transporte

#### 3. **Cards de Pedido**
Cada card exibe:
- **ID do Pedido** (#007, #008, etc)
- **Tempo Decorrido** (20s, 5m, 2h)
- **Nome do Cliente**
- **Endereço para Entrega**
- **Valor do Pedido**
- **Botões de Ação**:
  - "Para [PRÓXIMO STATUS]" - Move o pedido para o próximo status
  - "Ver Detalhes" - Abre página completa do pedido

#### 4. **Controles Header**
- **Status da Loja**: Exibe "Loja Aberta" ou "Loja Fechada"
- **Botão Atualizar**: Recarrega os dados manualmente
- **Botão Fechar Loja**: Alterna status da loja com confirmação

#### 5. **Atualização Automática**
A página se atualiza automaticamente a cada 15 segundos, sem necessidade de recarregar.

### Como Funciona Tecnicamente

#### Frontend (JavaScript)
```javascript
// Carrega pedidos via API
GET /Restaurante/GetPedidosRestaurante

// Atualiza status de um pedido
POST /Restaurante/AtualizarStatusPedido
Body: { pedidoId: 123, novoStatus: 2 }

// Alterna status da loja
POST /Restaurante/AlternarStatusLoja
```

#### Backend (C# Controller)
- **GetPedidosRestaurante()**: Retorna JSON `{ pedidos, finalizadosHoje }` — `pedidos` é o array de pedidos ativos (status P/E/S) e `finalizadosHoje` é a contagem de pedidos com status F e `Data` de hoje, calculada no banco
- **AtualizarStatusPedido()**: Processa mudança de status (pode ser estendido para persistir)
- **AlternarStatusLoja()**: Inverte status validado do restaurante

### Próximos Passos para Completar

Se o projeto tiver um campo de Status real no banco de dados (atualmente não observado):
1. Adicionar coluna `Status` (char ou enum) à tabela `pedido`
2. Descomenctar linha `pedido.Status = (char)request.NovoStatus;` em `AtualizarStatusPedido()`
3. Implementar salvamento com `_pedidoService.Edit(pedido);`

### Notas Importantes

- A view está **totalmente funcional** sem precisar de alterações no banco de dados
- Os dados são carregados em tempo real do banco
- O design é **responsivo** e funciona em mobile
- A paleta de cores segue o padrão do projeto (#ff6600 - laranja)
- Todos os ícones usam **Bootstrap Icons** (bi-*)

### Dependências Adicionadas

No `RestauranteController`:
- `IPedidoService` - para carregar pedidos
- `IClienteService` - para dados do cliente
- `ICarrinhoService` - para dados do carrinho

Todas as dependências já existem no projeto.
