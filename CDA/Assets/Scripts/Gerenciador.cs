using UnityEngine;

public class Gerenciador : MonoBehaviour
{
    [Header("Fluxo Inicial e Tutorial")]
    public Dialogo dialogoInicio;
    public TutorialGuiado tutorial;
    public bool iniciou = false;

    [Header("Efeitos Sonoros de Cliques")]
    public AudioSource somCliques;
    public AudioClip cliqueMapa, cliqueAceitar, cliqueNegar, cliqueCaixaEnergia, cliqueLivro;

    [Header("Ambiente e Música de Fundo")]
    public AudioSource musicaFundo;
    [Range(0f, 1f)] public float volumeNormalFundo = 0.18f;
    [Range(0f, 1f)] public float volumeDuranteDialogo = 0.04f;

    private bool dialogoComecou = false;

    void Start()
    {
        if (musicaFundo != null)
        {
            musicaFundo.volume = volumeNormalFundo;
        }

        // Dispara o diálogo inicial na tela preta assim que a cena carrega
        if (TelaPretaDialogo.instancia != null)
        {
            TelaPretaDialogo.instancia.acabou = false;
            TelaPretaDialogo.instancia.ComecarDialogo(dialogoInicio);
            dialogoComecou = true;
        }
    }

    void Update()
    {
        // Libera o tutorial guiado assim que o diálogo de abertura termina
        if (dialogoComecou && TelaPretaDialogo.instancia != null && TelaPretaDialogo.instancia.acabou && !iniciou)
        {
            iniciou = true;
            tutorial.IniciarTutorial();
        }

        // Suaviza o volume da música de fundo sempre que a tela de diálogo estiver aberta
        if (musicaFundo != null && musicaFundo.isPlaying)
        {
            bool emDialogo = (TelaPretaDialogo.instancia != null && TelaPretaDialogo.instancia.telaPreta != null && TelaPretaDialogo.instancia.telaPreta.activeSelf);

            float alvo = emDialogo ? volumeDuranteDialogo : volumeNormalFundo;
            musicaFundo.volume = Mathf.Lerp(musicaFundo.volume, alvo, Time.deltaTime * 5f);
        }
    }

    // Métodos chamados diretamente pelos botões da interface para tocar cada clique
    public void CliqueMapa() => somCliques.PlayOneShot(cliqueMapa);
    public void CliqueAceitar() => somCliques.PlayOneShot(cliqueAceitar);
    public void CliqueNegar() => somCliques.PlayOneShot(cliqueNegar);
    public void CliqueCaixaEnergia() => somCliques.PlayOneShot(cliqueCaixaEnergia);
    public void CliqueLivro() => somCliques.PlayOneShot(cliqueLivro);
}