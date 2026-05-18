using Godot;
using System;
using System.Collections.Generic;

public partial class Campo : Node2D
{
    // Ajuste para posição horizontal
    const int LARGURA = 114;  // O campo agora é mais largo
    const int ALTURA = 72;    // E menos alto
    const int TAMANHO_CELULA = 32; // Pixels por célula

    private Dictionary<Vector2, List<string>> propriedadesCelulas = new Dictionary<Vector2, List<string>>();
    private Camera2D camera;
    private const float VelocidadeMovimento = 800f;

    //public PackedScene playerScene;

    //private Bola bola;
    //private bool breakdownAtivo = false;
    //private Vector2 posicaoBreakdown;
    //private List<Jogador> jogadoresNoBreakdown = new List<Jogador>();

    //private Node2D indicadorBreakdown;

    //private List<Jogador> jogadores = new List<Jogador>(); // Lista para armazenar jogadores
    //private Jogador jogadorSelecionado = null; // Jogador atualmente selecionado


    // Chamado quando o nó entra na cena
    public override void _Ready()
    {
        base._Ready();
        CriarGrid();
        //CriarJogadores();
        //InicializarBola();
        camera = GetNode<Camera2D>("Camera2D");
    }

    // Criação do grid
    private void CriarGrid()
    {
        for (int x = 0; x < LARGURA; x++)
        {
            for (int y = 0; y < ALTURA; y++)
            {
                var posicao = new Vector2(x, y);
                propriedadesCelulas[posicao] = DeterminarPropriedades(x, y);

                // Criar o painel da célula
                var cell = new Panel
                {
                    CustomMinimumSize = new Vector2(TAMANHO_CELULA, TAMANHO_CELULA),
                    Position = new Vector2(x * TAMANHO_CELULA, y * TAMANHO_CELULA),
                    Modulate = CorCelula(propriedadesCelulas[posicao])
                };

                // Configurar a borda
                var style = new StyleBoxFlat();
                style.BorderWidthLeft = 1;
                style.BorderWidthTop = 1;
                style.BorderWidthRight = 1;
                style.BorderWidthBottom = 1;
                style.BorderColor = new Color(0, 0, 0, 0.2f);
                cell.AddThemeStyleboxOverride("panel", style);

                // Criar o rótulo para mostrar as coordenadas
                var label = new Label
                {
                    Text = $"{x}:{y}",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    CustomMinimumSize = new Vector2(TAMANHO_CELULA, TAMANHO_CELULA)
                };

                // Ajustar tamanho da fonte
                var fontSetting = new LabelSettings();
                fontSetting.FontSize = 8; // Tamanho da fonte pequeno para caber na célula
                label.LabelSettings = fontSetting;

                // Determinar a cor do texto com base na cor de fundo
                Color bgColor = CorCelula(propriedadesCelulas[posicao]);
                // Se a cor de fundo for escura, use texto branco
                if (bgColor.R + bgColor.G + bgColor.B < 1.5f)
                {
                    label.Modulate = new Color(1, 1, 1); // Texto branco
                }
                else
                {
                    label.Modulate = new Color(0, 0, 0); // Texto preto
                }

                // Adicionar o rótulo à célula
                cell.AddChild(label);

                // Adicionar a célula à cena
                AddChild(cell);
            }
        }
    }

    // Propriedades das celulas
    private List<string> DeterminarPropriedades(int x, int y)
    {
        var propriedades = new List<string>();

        // Perímetro
        if (y == 0 || y == ALTURA - 1 || x == 0 || x == LARGURA - 1)
            propriedades.Add("Perímetro");

        // Área de Jogo - corrigido
        if (x > 0 && x < LARGURA - 1 && y > 0 && y < ALTURA - 1)
            propriedades.Add("Área de Jogo");

        // In-Goal - vertical em relação ao campo horizontal
        if (x < 7 || x >= LARGURA - 7)
            propriedades.Add("In-Goal");

        // Zona 22m - vertical em relação ao campo horizontal
        if (x < 29 || x >= LARGURA - 29)
            propriedades.Add("Zona 22m");

        // Zona livre - vertical em relação ao campo horizontal
        if (x < 47 || x >= LARGURA - 47)
            propriedades.Add("Zona livre");

        // Zona 10m - vertical em relação ao campo horizontal
        if (x < 57 || x >= LARGURA - 57)
            propriedades.Add("Zona 10m");

        return propriedades;
    }

    // Cores das celulas
    private Color CorCelula(List<string> propriedades)
    {
        if (propriedades.Contains("Perímetro"))
            return new Color(0.5f, 0.5f, 0.5f); // GREY
        if (propriedades.Contains("In-Goal"))
            return new Color(1, 0, 0);          // RED
        if (propriedades.Contains("Zona 22m"))
            return new Color(0, 0, 1);          // BLUE
        if (propriedades.Contains("Zona livre"))
            return new Color(0, 1, 0);          // GREEN
        if (propriedades.Contains("Zona 10m"))
            return new Color(1, 1, 1);          // WHITE

        return new Color(1, 1, 1);              // WHITE
    }

    // Camera
    public override void _Process(double delta) 
    {
        float scroll = Input.GetAxis("ui_scroll_up", "ui_scroll_down");

        if (scroll != 0)
        {
            camera.Zoom *= 1 + scroll * 0.1f;
            camera.Zoom = new Vector2(Mathf.Clamp(camera.Zoom.X, 0.1f, 2f),
            Mathf.Clamp(camera.Zoom.Y, 0.1f, 2f));
        }

        Vector2 movimento = Vector2.Zero;

        if (Input.IsActionPressed("ui_right"))
            movimento.X += 1;
        if (Input.IsActionPressed("ui_left"))
            movimento.X -= 1;
        if (Input.IsActionPressed("ui_down"))
            movimento.Y += 1;
        if (Input.IsActionPressed("ui_up"))
            movimento.Y -= 1;

        camera.Position += movimento * VelocidadeMovimento * (float)delta;
    }

    //-----------------------------------------------------------------------------------------------------------

    // Método para inicializar a bola
    /*private void InicializarBola()
    {
        bola = new Bola();
        bola.Inicializar(TAMANHO_CELULA);
        AddChild(bola);

        // Dar a bola para um jogador inicial (por exemplo, o primeiro jogador do time A)
        if (jogadores.Count > 0)
        {
            var jogadorInicial = jogadores.Find(j => j.Time == "A" && j.Numero == 1);
            if (jogadorInicial != null)
            {
                bola.TransferirBola(jogadorInicial);
                jogadorInicial.AtualizarVisualComBola();
            }
        }
    }

    // Adicione uma chamada para InicializarBola() em _Ready() após CriarJogadores()

    // Modificar o método _Input para incluir tackles e breakdowns
    public override void _Input(InputEvent @event)
    {
        // Se um breakdown estiver ativo, não permitir seleção normal
        if (breakdownAtivo)
        {
            ManipularInputBreakdown(@event);
            return;
        }

        // Verificar se o usuário clicou com o mouse
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
        {
            // Converter a posição do mouse para coordenadas do grid
            Vector2 posicaoMouse = GetLocalMousePosition();
            int gridX = (int)(posicaoMouse.X / TAMANHO_CELULA);
            int gridY = (int)(posicaoMouse.Y / TAMANHO_CELULA);

            // Verificar se está dentro dos limites do campo
            if (gridX >= 0 && gridX < LARGURA && gridY >= 0 && gridY < ALTURA)
            {
                // Se já temos um jogador selecionado, verificar ação a tomar
                if (jogadorSelecionado != null)
                {
                    // Verificar se há um jogador na célula clicada
                    Jogador jogadorAlvo = null;
                    foreach (var jogador in jogadores)
                    {
                        if (jogador != jogadorSelecionado && jogador.PosX == gridX && jogador.PosY == gridY)
                        {
                            jogadorAlvo = jogador;
                            break;
                        }
                    }

                    if (jogadorAlvo != null)
                    {
                        // Se o jogador selecionado é de time diferente e está adjacente, tentar tackle
                        if (jogadorSelecionado.Time != jogadorAlvo.Time && jogadorSelecionado.EstaAdjacente(jogadorAlvo))
                        {
                            if (jogadorAlvo.TemBola && jogadorSelecionado.TentarTackle(jogadorAlvo))
                            {
                                IniciarBreakdown(jogadorSelecionado, jogadorAlvo);
                                jogadorSelecionado.Desselecionar();
                                jogadorSelecionado = null;
                                return;
                            }
                        }
                    }

                    // Verificar se a célula está vazia para movimento normal
                    bool celulaVazia = jogadorAlvo == null;

                    if (celulaVazia)
                    {
                        jogadorSelecionado.MoverPara(gridX, gridY);
                        if (jogadorSelecionado.TemBola)
                        {
                            bola.AtualizarPosicao();
                        }
                        jogadorSelecionado.Desselecionar();
                        jogadorSelecionado = null;
                    }
                }
                else
                {
                    // Verificar se há um jogador na posição clicada
                    foreach (var jogador in jogadores)
                    {
                        if (jogador.PosX == gridX && jogador.PosY == gridY)
                        {
                            jogadorSelecionado = jogador;
                            jogadorSelecionado.Selecionar();
                            break;
                        }
                    }
                }
            }
        }
    }

    // Método para iniciar um breakdown
    private void IniciarBreakdown(Jogador defensor, Jogador portadorBola)
    {
        breakdownAtivo = true;
        posicaoBreakdown = new Vector2(portadorBola.PosX, portadorBola.PosY);

        // Determinar jogadores no breakdown (3x3 ao redor do portador)
        jogadoresNoBreakdown.Clear();
        jogadoresNoBreakdown.Add(portadorBola);
        jogadoresNoBreakdown.Add(defensor);

        // Adicionar indicador visual
        CriarIndicadorBreakdown(portadorBola.PosX, portadorBola.PosY);

        // Adicionar outros jogadores que estão próximos
        foreach (var jogador in jogadores)
        {
            if (jogador != portadorBola && jogador != defensor)
            {
                int distX = Math.Abs(jogador.PosX - portadorBola.PosX);
                int distY = Math.Abs(jogador.PosY - portadorBola.PosY);

                if (distX <= 1 && distY <= 1)
                {
                    jogadoresNoBreakdown.Add(jogador);
                }
            }
        }

        // Marcar jogadores como em breakdown
        foreach (var jogador in jogadoresNoBreakdown)
        {
            jogador.EmBreakdown = true;
        }

        // Mostrar interface de breakdown
        MostrarInterfaceBreakdown();
    }

    // Método para mostrar a interface do breakdown
    private void MostrarInterfaceBreakdown()
    {
        // Interface simples para o breakdown
        var painel = new Panel();
        painel.Position = new Vector2(400, 300);
        painel.Size = new Vector2(300, 200);
        painel.Name = "PainelBreakdown";

        var titulo = new Label();
        titulo.Text = "BREAKDOWN!";
        titulo.Position = new Vector2(10, 10);
        titulo.Size = new Vector2(280, 30);
        painel.AddChild(titulo);

        // Criar botões de resolução
        var botaoTimeA = new Button();
        botaoTimeA.Text = "Time A vence";
        botaoTimeA.Position = new Vector2(10, 50);
        botaoTimeA.Size = new Vector2(130, 50);
        botaoTimeA.Pressed += () => ResolverBreakdown("A");
        painel.AddChild(botaoTimeA);

        var botaoTimeB = new Button();
        botaoTimeB.Text = "Time B vence";
        botaoTimeB.Position = new Vector2(150, 50);
        botaoTimeB.Size = new Vector2(130, 50);
        botaoTimeB.Pressed += () => ResolverBreakdown("B");
        painel.AddChild(botaoTimeB);

        // Botão para resolver automaticamente
        var botaoAuto = new Button();
        botaoAuto.Text = "Resolver Automaticamente";
        botaoAuto.Position = new Vector2(10, 110);
        botaoAuto.Size = new Vector2(280, 50);
        botaoAuto.Pressed += ResolverBreakdownAutomatico;
        painel.AddChild(botaoAuto);

        AddChild(painel);
    }

    // Método para manipular input durante o breakdown
    private void ManipularInputBreakdown(InputEvent @event)
    {
        // Durante o breakdown, podemos adicionar inputs específicos se necessário
    }

    // Resolver o breakdown com vencedor predeterminado
    private void ResolverBreakdown(string timeVencedor)
    {
        Jogador novoPortador = null;

        // Encontrar um jogador do time vencedor que está no breakdown
        foreach (var jogador in jogadoresNoBreakdown)
        {
            if (jogador.Time == timeVencedor)
            {
                novoPortador = jogador;
                break;
            }
        }

        if (novoPortador != null)
        {
            // Transferir a bola para o novo portador
            bola.TransferirBola(novoPortador);
            novoPortador.AtualizarVisualComBola();
        }

        FinalizarBreakdown();
    }

    // Resolver o breakdown automaticamente com base na força dos jogadores
    private void ResolverBreakdownAutomatico()
    {
        // Calcular força total por time
        int forcaTimeA = 0;
        int forcaTimeB = 0;
        int contadorTimeA = 0;
        int contadorTimeB = 0;

        foreach (var jogador in jogadoresNoBreakdown)
        {
            if (jogador.Time == "A")
            {
                forcaTimeA += jogador.Forca;
                contadorTimeA++;
            }
            else
            {
                forcaTimeB += jogador.Forca;
                contadorTimeB++;
            }
        }

        // Calcular médias ponderadas (mais jogadores = vantagem)
        float mediaA = contadorTimeA > 0 ? forcaTimeA * (1.0f + 0.1f * contadorTimeA) : 0;
        float mediaB = contadorTimeB > 0 ? forcaTimeB * (1.0f + 0.1f * contadorTimeB) : 0;

        // Adicionar fator aleatório (20%)
        float aleatorioA = (float)GD.RandRange(0.9f, 1.1f);
        float aleatorioB = (float)GD.RandRange(0.9f, 1.1f);

        mediaA *= aleatorioA;
        mediaB *= aleatorioB;

        // Determinar vencedor
        string timeVencedor = mediaA > mediaB ? "A" : "B";
        ResolverBreakdown(timeVencedor);
    }

    // Finalizar o breakdown
    private void FinalizarBreakdown()
    {
        // Remover interface de breakdown
        var painel = GetNode<Panel>("PainelBreakdown");
        if (painel != null)
        {
            painel.QueueFree();
        }

        // Remover indicador visual
        if (indicadorBreakdown != null)
        {
            indicadorBreakdown.QueueFree();
            indicadorBreakdown = null;
        }

        // Resetar estado dos jogadores
        foreach (var jogador in jogadoresNoBreakdown)
        {
            jogador.EmBreakdown = false;
        }

        jogadoresNoBreakdown.Clear();
        breakdownAtivo = false;
    }

    //-----------------------------------------------------------------------------------------------------------
    private void CriarIndicadorBreakdown(int posX, int posY)
    {
        indicadorBreakdown = new Node2D();

        // Criar uma área 3x3 destacada
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int cellX = posX + x;
                int cellY = posY + y;

                // Verificar se está dentro dos limites do campo
                if (cellX >= 0 && cellX < LARGURA && cellY >= 0 && cellY < ALTURA)
                {
                    var highlight = new ColorRect();
                    highlight.Size = new Vector2(TAMANHO_CELULA, TAMANHO_CELULA);
                    highlight.Position = new Vector2(cellX * TAMANHO_CELULA, cellY * TAMANHO_CELULA);
                    highlight.Color = new Color(1, 1, 0, 0.3f); // Amarelo translúcido
                    indicadorBreakdown.AddChild(highlight);
                }
            }
        }

        AddChild(indicadorBreakdown);
    
    //-----------------------------------------------------------------------------------------------------------

   private void CriarJogadores()
    {
        if (playerScene == null)
        {
            GD.PrintErr("playerScene não foi carregado corretamente.");
            return;
        }

        // Criar jogador atacante (com bola)
        Jogador atacante = (Jogador)playerScene.Instantiate();
        atacante.Nome = "Atacante";
        atacante.Position = new Vector2(10 * TAMANHO_CELULA, 36 * TAMANHO_CELULA);
        atacante.TemBola = true; // Começa com a bola
        jogadores.Add(atacante);
        AddChild(atacante);

        // Criar jogador defensor
        Jogador defensor = (Jogador)playerScene.Instantiate();
        defensor.Nome = "Defensor";
        defensor.Position = new Vector2(20 * TAMANHO_CELULA, 36 * TAMANHO_CELULA);
        jogadores.Add(defensor);
        AddChild(defensor);

        GD.Print("Jogadores criados.");
    }*/
}