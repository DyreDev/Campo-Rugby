using Godot;
using System;

public partial class Bola : Node2D
{
    public Jogador Portador { get; private set; }
    private int tamanho_celula;

    // Visual da bola
    private ColorRect visual;

    public override void _Ready()
    {
        // Criar representação visual da bola
        visual = new ColorRect();
        visual.Size = new Vector2(tamanho_celula / 2, tamanho_celula / 2);
        visual.Position = new Vector2(-visual.Size.X / 2, -visual.Size.Y / 2);
        visual.Color = new Color(0.8f, 0.6f, 0.2f); // Cor marrom/laranja para a bola
        AddChild(visual);
    }

    public void Inicializar(int tamanhoCelula)
    {
        tamanho_celula = tamanhoCelula;
    }

    public void TransferirBola(Jogador novoPortador)
    {
        if (Portador != null)
        {
            Portador.TemBola = false;
        }

        Portador = novoPortador;
        Portador.TemBola = true;

        // Atualizar posição da bola para o novo portador
        AtualizarPosicao();
    }

    public void AtualizarPosicao()
    {
        if (Portador != null)
        {
            // Posicionar a bola próxima ao portador
            Position = new Vector2(
                Portador.PosX * tamanho_celula + tamanho_celula / 2,
                Portador.PosY * tamanho_celula + tamanho_celula / 2
            );
        }
    }
}