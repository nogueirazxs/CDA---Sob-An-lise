using System.Collections;
using UnityEngine;

public class EventoEnergiaDia2 : MonoBehaviour
{
    [Header("Referências da Cabine e Fios")]
    public SpriteRenderer fundoCabine;
    public Color corEscura = new Color(0.25f, 0.25f, 0.3f, 1f);
    public GameObject botaoIrParaCaixa;
    public GerenciadorFios gerenciadorFios;

    [Header("Configuração de Tempo e Áudio")]
    public float tempoLimiteConserto = 25f;
    public AudioSource audioSource;
    public AudioClip somQuedaEnergia;
    public AudioClip somInvasaoAnomalia;

    [Header("Consequência (Se o tempo acabar)")]
    public GameObject painelSalaEnergia;
    public GameObject painelCaixaAberta;
    public GameObject visualAnomaliaInvasao;
    public GerenciadorTriagem gerenciadorTriagem;

    private bool energiaCaida = false;
    private float cronometro = 0f;

    void Update()
    {
        if (!energiaCaida) return;

        // Conta o tempo restante para religar os fios antes da invasão acontecer
        cronometro -= Time.deltaTime;

        if (cronometro <= 0f)
        {
            energiaCaida = false;
            StartCoroutine(SequenciaInvasaoNoEscuro());
        }
    }

    // Escurece a cabine, embaralha o puzzle novamente e libera o botão de acesso à caixa
    public void DispararQuedaDeEnergia()
    {
        if (energiaCaida) return;

        energiaCaida = true;
        cronometro = tempoLimiteConserto;

        if (gerenciadorFios != null)
        {
            gerenciadorFios.ResetarPuzzleCompleto();
        }

        if (fundoCabine != null)
        {
            fundoCabine.color = corEscura;
        }

        if (audioSource != null && somQuedaEnergia != null)
        {
            audioSource.PlayOneShot(somQuedaEnergia);
        }

        if (botaoIrParaCaixa != null)
        {
            botaoIrParaCaixa.SetActive(true);
        }
    }

    // Acende as luzes e retoma a fila se o conserto veio dessa queda de energia
    public void EnergiaRestauradaComSucesso()
    {
        // Evita conflito caso a caixa tenha sido usada apenas para ligar o rádio do Dia 1
        if (!energiaCaida) return;

        energiaCaida = false;
        if (fundoCabine != null)
        {
            fundoCabine.color = Color.white;
        }

        if (gerenciadorTriagem != null)
        {
            gerenciadorTriagem.jogoPausadoPorPuzzle = false;
            gerenciadorTriagem.Invoke("ChamarProximoViajante", 1.0f);
        }
    }

    // Fecha os painéis da caixa de força e dispara o ataque da anomalia no escuro
    IEnumerator SequenciaInvasaoNoEscuro()
    {
        if (painelCaixaAberta != null) painelCaixaAberta.SetActive(false);
        if (painelSalaEnergia != null) painelSalaEnergia.SetActive(false);
        if (botaoIrParaCaixa != null) botaoIrParaCaixa.SetActive(false);

        if (audioSource != null && somInvasaoAnomalia != null)
        {
            audioSource.PlayOneShot(somInvasaoAnomalia);
        }

        if (visualAnomaliaInvasao != null)
        {
            visualAnomaliaInvasao.SetActive(true);
            RectTransform rt = visualAnomaliaInvasao.GetComponent<RectTransform>();

            // Avança o visual da criatura rapidamente em direção à tela
            float duracao = 0.35f;
            float tempo = 0f;
            Vector3 escalaInicial = Vector3.one * 0.6f;
            Vector3 escalaFinal = Vector3.one * 1.35f;

            while (tempo < duracao)
            {
                tempo += Time.deltaTime;
                rt.localScale = Vector3.Lerp(escalaInicial, escalaFinal, tempo / duracao);
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.8f);

        if (gerenciadorTriagem != null)
        {
            gerenciadorTriagem.StartCoroutine(gerenciadorTriagem.SequenciaGameOverSemJumpscareJanela());
        }
    }
}