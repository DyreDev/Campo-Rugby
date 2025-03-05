using Godot;
using System.Collections.Generic;

public partial class Campo : Node2D
{
    // Ajuste para posição horizontal
    const int LARGURA = 114;  // O campo agora é mais largo
    const int ALTURA = 72;    // E menos alto
    const int TAMANHO_CELULA = 32; // Pixels por célula

    private Dictionary<Vector2, List<string>> propriedadesCelulas = new Dictionary<Vector2, List<string>>();
    private Camera2D camera;
    private const float VelocidadeMovimento = 500f;

    public PackedScene playerScene;

    // Chamado quando o nó entra na cena
    public override void _Ready()
    {
        base._Ready();
        CriarGrid();
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
}
