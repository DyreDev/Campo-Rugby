using Godot;
using System;
using System.Collections.Generic;

public partial class Jogador : Node2D
{
    // Propriedades e variáveis existentes
    public string Nome { get; set; }
    public int PosX { get; set; }
    public int PosY { get; set; }
    private int tamanho_celula;
    public string Time { get; set; } // "A" ou "B"
    public int Numero { get; set; } // Número do jogador
    public bool TemBola { get; set; }
    public bool EmBreakdown { get; set; }
    public int Forca { get; private set; } // Força do jogador para disputas

    private ColorRect visual;
    private Label labelNumero;
    private bool selecionado = false;

    // Variáveis para controle de arraste com o mouse
    private bool arrastando = false;
    private Vector2 offsetMouse = Vector2.Zero;

    private List<Button> ButtonList = new List<Button>(); // Lista de botões

    // Chamado quando o nó entra na cena
    public override void _Ready()
    {
        // Criar uma representação visual do jogador
        visual = new ColorRect();
        visual.Size = new Vector2(tamanho_celula - 4, tamanho_celula - 4);
        visual.Position = new Vector2(-visual.Size.X / 2, -visual.Size.Y / 2);
        visual.Color = Time == "A" ? new Color(1, 0, 0) : new Color(0, 0, 1);

        // Adicionar um label para o número do jogador
        labelNumero = new Label();
        labelNumero.Text = Numero.ToString();
        labelNumero.HorizontalAlignment = HorizontalAlignment.Center;
        labelNumero.VerticalAlignment = VerticalAlignment.Center;
        labelNumero.Position = new Vector2(-tamanho_celula / 2, -tamanho_celula / 2);
        labelNumero.Size = new Vector2(tamanho_celula, tamanho_celula);
        labelNumero.Modulate = new Color(1, 1, 1); // Texto branco

        AddChild(visual);
        AddChild(labelNumero);

        AtualizarPosicaoVisual();

        // Exemplo de inicialização e adição de botões
        Button button1 = new Button();
        button1.Text = "Botão 1";
        ButtonList.Add(button1);

        Button button2 = new Button();
        button2.Text = "Botão 2";
        ButtonList.Add(button2);

        // Exemplo de adição de botões à cena
        AddChild(button1);
        AddChild(button2);
    }

    // Inicializar o jogador
    public void Inicializar(int x, int y, int tamanhoCelula, string time, int numero)
    {
        PosX = x;
        PosY = y;
        tamanho_celula = tamanhoCelula;
        Time = time;
        Numero = numero;
        Forca = new Random().Next(70, 100); // Valor aleatório de força entre 70 e 99
        TemBola = false;
        EmBreakdown = false;

        // Configurar aparência baseada no time
        if (Time == "A")
        {
            if (visual != null) visual.Color = new Color(1, 0, 0); // Vermelho para time A
        }
        else
        {
            if (visual != null) visual.Color = new Color(0, 0, 1); // Azul para time B
        }

        if (labelNumero != null) labelNumero.Text = Numero.ToString();
    }

    // Mover o jogador para uma nova posição
    public void MoverPara(int novoX, int novoY)
    {
        if (EmBreakdown) return; // Não pode se mover durante um breakdown

        PosX = novoX;
        PosY = novoY;
        AtualizarPosicaoVisual();
    }

    // Atualizar a posição visual baseada nas coordenadas do grid
    private void AtualizarPosicaoVisual()
    {
        Position = new Vector2(PosX * tamanho_celula + tamanho_celula / 2,
                              PosY * tamanho_celula + tamanho_celula / 2);
    }

    // Métodos de seleção
    public void Selecionar()
    {
        selecionado = true;
        if (visual != null) visual.Scale = new Vector2(1.2f, 1.2f); // Aumentar o tamanho
    }

    public void Desselecionar()
    {
        selecionado = false;
        if (visual != null) visual.Scale = new Vector2(1.0f, 1.0f);
    }

    // Verificar se está adjacente a outro jogador
    public bool EstaAdjacente(Jogador outro)
    {
        int distX = Math.Abs(PosX - outro.PosX);
        int distY = Math.Abs(PosY - outro.PosY);

        return (distX <= 1 && distY <= 1) && !(distX == 0 && distY == 0);
    }

    // Tentar fazer tackle em outro jogador
    public bool TentarTackle(Jogador adversario)
    {
        if (EmBreakdown || adversario.EmBreakdown) return false;

        if (EstaAdjacente(adversario) && adversario.TemBola)
        {
            return true; // Pode iniciar o breakdown
        }

        return false;
    }

    // Atualizar estado visual quando tem a bola
    public void AtualizarVisualComBola()
    {
        if (TemBola)
        {
            if (visual != null)
            {
                var strokeRect = new ColorRect();
                strokeRect.Size = visual.Size + new Vector2(4, 4);
                strokeRect.Position = visual.Position - new Vector2(2, 2);
                strokeRect.Color = new Color(1, 1, 0); // Amarelo
                AddChild(strokeRect);
                MoveChild(strokeRect, 0); // Colocar atrás do visual principal
            }
        }
    }

    // Função chamada a cada quadro (processa eventos e atualizações)
    /*public override void _Process(float delta)
    {
        base._Process(delta);

        // Aqui você pode adicionar o código que deseja atualizar a cada quadro, por exemplo:
        if (EmBreakdown)
        {
            // Lógica que deve ser executada enquanto o jogador está no breakdown
        }

        // Exemplo: Movimentação do jogador ou outras ações baseadas no delta
        // Mover o jogador com base na entrada ou outras lógicas que você queira atualizar
        // Exemplo fictício de movimento:
        if (selecionado)
        {
            MoverPara(PosX + 1, PosY); // Exemplo de movimento
        }

        // Atualizar a visualização a cada quadro
        AtualizarPosicaoVisual();
    }

    // Função chamada para processar eventos de entrada (como cliques)
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == (int)ButtonList.Left) // Se o botão pressionado for o esquerdo
            {
                if (mouseEvent.Pressed)
                {
                    // Quando o mouse é pressionado, verificamos se o jogador foi clicado
                    if (visual.GetRect().HasPoint(visual.ToLocal(GetGlobalMousePosition())))
                    {
                        arrastando = true;
                        // Calcula a diferença (offset) entre a posição do mouse e a posição do jogador
                        offsetMouse = Position - GetGlobalMousePosition();
                    }
                }
                else
                {
                    // Quando o mouse é solto, o arrasto é finalizado
                    arrastando = false;
                }
            }
        }
    }*/
}
