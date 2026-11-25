using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControladorIntroLogo : MonoBehaviour
{
    [Header("Referências da Logo e Áudio")]
    public Image imagemLogo;
    public AudioSource audioSource;
    public AudioClip somLogo;

    [Header("Tempos da Animação")]
    public float duracaoFadeIn = 1.5f;
    public float tempoVisivel = 2.0f;
    public float duracaoFadeOut = 1.0f;

    void Start()
    {
        StartCoroutine(SequenciaIntro());
    }

    // Controla o surgimento suave da logo do estúdio antes de abrir o menu principal
    IEnumerator SequenciaIntro()
    {
        Color cor = imagemLogo.color;
        cor.a = 0f;
        imagemLogo.color = cor;

        yield return new WaitForSeconds(0.3f);

        if (audioSource != null && somLogo != null)
        {
            audioSource.PlayOneShot(somLogo);
        }

        // Aparece gradualmente do transparente até ficar totalmente visível
        float tempo = 0f;
        while (tempo < duracaoFadeIn)
        {
            tempo += Time.deltaTime;
            cor.a = Mathf.Lerp(0f, 1f, tempo / duracaoFadeIn);
            imagemLogo.color = cor;
            yield return null;
        }
        cor.a = 1f;
        imagemLogo.color = cor;

        // Mantém a logo parada em destaque por alguns segundos
        yield return new WaitForSeconds(tempoVisivel);

        // Desaparece suavemente antes da troca de cena
        tempo = 0f;
        while (tempo < duracaoFadeOut)
        {
            tempo += Time.deltaTime;
            cor.a = Mathf.Lerp(1f, 0f, tempo / duracaoFadeOut);
            imagemLogo.color = cor;
            yield return null;
        }
        cor.a = 0f;
        imagemLogo.color = cor;

        SceneManager.LoadScene("Menu");
    }
}