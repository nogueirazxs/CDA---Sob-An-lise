using UnityEngine;
using UnityEngine.UI;

public class ControladorLivro : MonoBehaviour
{
    [Header("Componentes do Livro")]
    public Image imagemLivro;
    public Sprite[] paginas;

    [Header("Botões de Navegação")]
    public GameObject botaoVoltar;
    public GameObject botaoAvancar;

    private int paginaAtual = 0;

    // Mantém o livro aberto exatamente na última página que o jogador estava lendo
    void OnEnable()
    {
        AtualizarPagina();
    }

    public void AvancarPagina()
    {
        if (paginaAtual < paginas.Length - 1)
        {
            paginaAtual++;
            AtualizarPagina();
        }
    }

    public void VoltarPagina()
    {
        if (paginaAtual > 0)
        {
            paginaAtual--;
            AtualizarPagina();
        }
    }

    // Troca a imagem da página atual e esconde as setas quando chega nas pontas do livro
    void AtualizarPagina()
    {
        if (paginas.Length == 0) return;

        imagemLivro.sprite = paginas[paginaAtual];

        if (botaoVoltar != null)
        {
            botaoVoltar.SetActive(paginaAtual > 0);
        }

        if (botaoAvancar != null)
        {
            botaoAvancar.SetActive(paginaAtual < paginas.Length - 1);
        }
    }
}