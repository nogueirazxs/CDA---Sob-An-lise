using System.Collections;
using UnityEngine;

public class ControladorViajante : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public AudioSource audioSource;
    public AudioClip somGlitch;
    public AudioClip somJumpscare;

    private DadosViajante dadosAtuais;
    private bool ehAnomalia;
    private bool ativo = false;
    private Vector3 escalaOriginal;

    void Awake()
    {
        escalaOriginal = transform.localScale;
    }

    public void ConfigurarViajante(DadosViajante dados, bool anomalia)
    {
        dadosAtuais = dados;
        ehAnomalia = anomalia;
        ativo = true;

        transform.localScale = escalaOriginal;
        spriteRenderer.sprite = dados.spriteNormal;
        spriteRenderer.sortingOrder = -1;
        gameObject.SetActive(true);

        StopAllCoroutines();
        if (ehAnomalia)
        {
            StartCoroutine(RotinaGlitchAnomalia());
        }
    }

    // Faz a anomalia trocar para o sprite bizarro muito rápido de tempos em tempos!
    IEnumerator RotinaGlitchAnomalia()
    {
        while (ativo)
        {
            // Espera um tempo aleatório entre 3 e 7 segundos
            yield return new WaitForSeconds(Random.Range(3f, 7f));

            // Pisca para o sprite anômalo por apenas 0.12 segundos!
            spriteRenderer.sprite = dadosAtuais.spriteAnomalo;
            if (somGlitch != null)
            {
                audioSource.clip = somGlitch;
                audioSource.Play();
            } 

            yield return new WaitForSeconds(0.12f);

            // Volta ao normal como se nada tivesse acontecido
            spriteRenderer.sprite = dadosAtuais.spriteNormal;
            if (audioSource.isPlaying) audioSource.Stop();
        }
    }

    // Efeito FODA de Jumpscare quando o jogador aprova uma anomalia ou perde
    public IEnumerator ExecutarJumpscare()
    {
        ativo = false;
        StopAllCoroutines();

        spriteRenderer.sprite = dadosAtuais.spriteAnomalo;
        spriteRenderer.sortingOrder = 10;
        if (somJumpscare != null) audioSource.PlayOneShot(somJumpscare);

        // Aumenta o tamanho bruscamente na direção da tela e treme
        float tempo = 0f;
        while (tempo < 0.6f)
        {
            tempo += Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, escalaOriginal * 2.8f, Time.deltaTime * 15f);
            transform.localPosition += (Vector3)Random.insideUnitCircle * 0.15f;
            yield return null;
        }
    }

    public void Esconder()
    {
        ativo = false;
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}