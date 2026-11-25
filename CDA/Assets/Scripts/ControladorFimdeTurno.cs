using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorFimdeTurno : MonoBehaviour
{
    [Header("Componentes de Tela e Áudio")]
    public TextMeshProUGUI textoTela;
    public AudioSource audioSource;
    public AudioClip somTecla;

    [Header("Configurações de Tempo e Cena")]
    public float velocidadeDigitacao = 0.06f;
    public float tempoLeituraFrase1 = 2.5f;
    public float tempoLeituraFrase2 = 3.5f;
    public string nomeCenaMenu = "Menu";

    void Start()
    {
        // Garante que o overlay global de diálogo fique oculto ao entrar na tela final
        if (TelaPretaDialogo.instancia != null && TelaPretaDialogo.instancia.telaPreta != null)
        {
            TelaPretaDialogo.instancia.telaPreta.SetActive(false);
        }

        StartCoroutine(SequenciaMensagensFinais());
    }

    // Orquestra a exibição das frases finais com pausas dramáticas antes de voltar ao menu
    IEnumerator SequenciaMensagensFinais()
    {
        textoTela.text = "";
        textoTela.alpha = 1f;

        yield return new WaitForSeconds(1.0f);

        yield return StartCoroutine(DigitarTexto("O TURNO ACABOU!"));
        yield return new WaitForSeconds(tempoLeituraFrase1);

        yield return StartCoroutine(FadeTexto(1f, 0f, 0.8f));
        textoTela.text = "";
        textoTela.alpha = 1f;

        yield return new WaitForSeconds(0.6f);

        yield return StartCoroutine(DigitarTexto("DIA 3?..."));
        yield return new WaitForSeconds(1.2f);
        yield return StartCoroutine(AcrescentarTexto(" OU NÃO?"));

        yield return new WaitForSeconds(tempoLeituraFrase2);

        yield return StartCoroutine(FadeTexto(1f, 0f, 1.0f));
        yield return new WaitForSeconds(0.4f);

        SceneManager.LoadScene(nomeCenaMenu);
    }

    // Escreve a frase letra por letra disparando o som de tecla
    IEnumerator DigitarTexto(string frase)
    {
        textoTela.text = "";

        foreach (char letra in frase)
        {
            textoTela.text += letra;

            if (audioSource != null && somTecla != null && letra != ' ')
            {
                audioSource.PlayOneShot(somTecla, 0.4f);
            }

            yield return new WaitForSeconds(velocidadeDigitacao);
        }
    }

    // Continua escrevendo na mesma linha com um ritmo mais lento para criar suspense
    IEnumerator AcrescentarTexto(string complemento)
    {
        foreach (char letra in complemento)
        {
            textoTela.text += letra;

            if (audioSource != null && somTecla != null && letra != ' ')
            {
                audioSource.PlayOneShot(somTecla, 0.4f);
            }

            yield return new WaitForSeconds(velocidadeDigitacao * 1.4f);
        }
    }

    // Suaviza a transparência do texto para entrada ou saída de cena
    IEnumerator FadeTexto(float alfaInicial, float alfaFinal, float duracao)
    {
        float tempo = 0f;

        while (tempo < duracao)
        {
            tempo += Time.deltaTime;
            textoTela.alpha = Mathf.Lerp(alfaInicial, alfaFinal, tempo / duracao);
            yield return null;
        }

        textoTela.alpha = alfaFinal;
    }
}