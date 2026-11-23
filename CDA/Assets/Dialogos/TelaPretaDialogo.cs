using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TelaPretaDialogo : MonoBehaviour
{
    public static TelaPretaDialogo instancia; // Variável para acessar o script sem precisar arrastar a referência.

    [Header("Componentes da Tela")]
    public GameObject telaPreta;
    public TextMeshProUGUI textoNaTela;
    public AudioSource tocadorDeAudio;

    InputAction acaoAvancar;
    private Dialogo dialogoAtual;
    private int numeroDaFala = 0;
    private bool escrevendo = false;

    public bool acabou = false;

    void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject); // Não destrói o canvas quando passar de cena.
        }
        else
        {
            Destroy(gameObject);
        }

        acaoAvancar = InputSystem.actions.FindAction("Avancar");

        telaPreta.SetActive(false);
    }

    void Update()
    {
        // Se a tela preta não estiver aberta, não faz nada.
        if (telaPreta.activeSelf == false) return;

        if (acaoAvancar != null && acaoAvancar.WasPerformedThisFrame())
        {
            AvancarDialogo();
        }
    }

    // Metódo para começar um diálogo configurado e passado por referência.
    public void ComecarDialogo(Dialogo novoDialogo)
    {
        acabou = false;
        dialogoAtual = novoDialogo; // Pega o parâmetro para usar no script.
        numeroDaFala = 0;

        telaPreta.SetActive(true);
        MostrarFala();
    }

    void MostrarFala()
    {
        Fala falaAtual = dialogoAtual.falas[numeroDaFala]; // Pega o string da fala atual.

        tocadorDeAudio.Stop();
        if (falaAtual.audio != null)
        {
            tocadorDeAudio.clip = falaAtual.audio;
            tocadorDeAudio.Play();
        }

        StopAllCoroutines();
        StartCoroutine(EscreverAosPoucos(falaAtual.texto)); // Começa a escrever o texto aos poucos.
    }

    IEnumerator EscreverAosPoucos(string frase)
    {
        escrevendo = true;
        textoNaTela.text = "";

        foreach (char letra in frase)
        {
            textoNaTela.text += letra; // Aparece letra por letra em um tempo específico.
            yield return new WaitForSecondsRealtime(0.04f);
        }

        escrevendo = false;
    }

    void AvancarDialogo()
    {
        // Se ainda está digitando, completa a frase na hora.
        if (escrevendo == true)
        {
            StopAllCoroutines();
            textoNaTela.text = dialogoAtual.falas[numeroDaFala].texto; // Mostra a fala completa.
            escrevendo = false;
        }
        // Se já terminou de digitar, vai para a próxima fala.
        else
        {
            numeroDaFala++;

            if (numeroDaFala < dialogoAtual.falas.Count)
            {
                MostrarFala();
            }
            else
            {
                tocadorDeAudio.Stop();
                telaPreta.SetActive(false);
                acabou = true;
            }
        }
    }
}