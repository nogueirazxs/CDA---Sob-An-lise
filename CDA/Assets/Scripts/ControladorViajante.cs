using System.Collections;
using UnityEngine;

public class ControladorViajante : MonoBehaviour
{
    [Header("Componentes Visuais e Sonoros")]
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

    // Prepara o viajante na janela e ativa o comportamento oculto caso seja uma anomalia
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

    // Alterna rapidamente para a aparência anômala em intervalos aleatórios para dar pistas sutis
    IEnumerator RotinaGlitchAnomalia()
    {
        while (ativo)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));

            spriteRenderer.sprite = dadosAtuais.spriteAnomalo;
            if (somGlitch != null)
            {
                audioSource.clip = somGlitch;
                audioSource.Play();
            }

            yield return new WaitForSeconds(0.12f);

            spriteRenderer.sprite = dadosAtuais.spriteNormal;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    // Avança a criatura na direção da tela com áudio alto quando o jogador comete um erro fatal
    public IEnumerator ExecutarJumpscare()
    {
        ativo = false;
        StopAllCoroutines();

        spriteRenderer.sprite = dadosAtuais.spriteAnomalo;
        spriteRenderer.sortingOrder = 10;
        if (somJumpscare != null)
        {
            audioSource.PlayOneShot(somJumpscare);
        }

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