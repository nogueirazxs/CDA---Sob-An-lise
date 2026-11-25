using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TelaPretaDialogo : MonoBehaviour
{
    public static TelaPretaDialogo instancia;

    [Header("Componentes da Tela")]
    public GameObject telaPreta;
    public TextMeshProUGUI textoNaTela;
    public AudioSource tocadorDeAudio;

    [Header("Estado do Diálogo")]
    public bool acabou = false;

    private InputAction acaoAvancar;
    private Dialogo dialogoAtual;
    private int numeroDaFala = 0;
    private bool escrevendo = false;

    void Awake()
    {
        // Garante uma instância única e mantém o canvas de diálogo ativo entre as cenas
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
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
        if (!telaPreta.activeSelf) return;

        if (acaoAvancar != null && acaoAvancar.WasPerformedThisFrame())
        {
            AvancarDialogo();
        }
    }

    // Inicia uma nova sequência de falas na tela preta
    public void ComecarDialogo(Dialogo novoDialogo)
    {
        acabou = false;
        dialogoAtual = novoDialogo;
        numeroDaFala = 0;

        telaPreta.SetActive(true);
        MostrarFala();
    }

    // Carrega o áudio e dispara a digitação da fala atual
    void MostrarFala()
    {
        Fala falaAtual = dialogoAtual.falas[numeroDaFala];

        tocadorDeAudio.Stop();
        if (falaAtual.audio != null)
        {
            tocadorDeAudio.clip = falaAtual.audio;
            tocadorDeAudio.Play();
        }

        StopAllCoroutines();
        StartCoroutine(EscreverAosPoucos(falaAtual.texto));
    }

    // Efeito máquina de escrever independente do Time.timeScale
    IEnumerator EscreverAosPoucos(string frase)
    {
        escrevendo = true;
        textoNaTela.text = "";

        foreach (char letra in frase)
        {
            textoNaTela.text += letra;
            yield return new WaitForSecondsRealtime(0.04f);
        }

        escrevendo = false;
    }

    // Completa o texto na hora se ainda estiver digitando ou pula para a próxima fala
    void AvancarDialogo()
    {
        if (escrevendo)
        {
            StopAllCoroutines();
            textoNaTela.text = dialogoAtual.falas[numeroDaFala].texto;
            escrevendo = false;
        }
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