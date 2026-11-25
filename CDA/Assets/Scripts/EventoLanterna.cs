using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class EventoLanterna : MonoBehaviour
{
    [Header("Referências da Cena")]
    public GameObject salaLanterna;
    public GameObject botaoSetaDireita;
    public Transform lanternaJogador;
    public SpriteRenderer olhoAtual;
    public Sprite[] spritesOlhos;
    public Transform[] pontosDeAparicao;
    public Light2D luzGlobalCena;
    public GerenciadorTriagem gerenciadorTriagem;

    [Header("Jumpscare e Áudio")]
    public GameObject visualOlhoJumpscare;
    public AudioSource audioSource;
    public AudioClip somBarulhoDireita;
    public AudioClip somEspantarOlho;
    public AudioClip somJumpscareOlho;

    [Header("Configurações de Tempo e Tamanho")]
    public int totalOlhosParaEspantar = 4;
    public float tempoParaClicarNaSeta = 10f;
    public float multiplicadorTamanhoMorte = 2.5f;
    public float velocidadeCrescimento = 0.25f;
    public float raioAcertoLanterna = 1.8f;
    public float tempoLuzParaEspantar = 0.6f;

    [Header("Ajuste de Rotação da Lanterna")]
    public float compensacaoAnguloArte = -135f;

    private bool eventoAtivo = false;
    private bool dentroDaSala = false;
    private float cronometroSeta = 0f;
    private int olhosEspantados = 0;
    private float tempoIluminado = 0f;
    private Vector3 escalaOriginalOlho;

    void Awake()
    {
        if (olhoAtual != null)
        {
            escalaOriginalOlho = olhoAtual.transform.localScale;
        }
    }

    void Update()
    {
        if (!eventoAtivo) return;

        // Conta o tempo limite para o jogador notar o barulho e ir até a sala lateral
        if (!dentroDaSala)
        {
            cronometroSeta -= Time.deltaTime;
            if (cronometroSeta <= 0f)
            {
                eventoAtivo = false;
                StartCoroutine(SequenciaJumpscareOlho());
            }
            return;
        }

        GirarLanternaParaCursor();

        if (olhoAtual != null && olhoAtual.gameObject.activeSelf)
        {
            // Faz o olho crescer gradualmente enquanto estiver no escuro
            float fatorCrescimento = 1f + (velocidadeCrescimento * Time.deltaTime);
            olhoAtual.transform.localScale *= fatorCrescimento;

            if (olhoAtual.transform.localScale.x >= escalaOriginalOlho.x * multiplicadorTamanhoMorte)
            {
                eventoAtivo = false;
                olhoAtual.gameObject.SetActive(false);
                StartCoroutine(SequenciaJumpscareOlho());
                return;
            }

            // Verifica se o feixe da lanterna está mirando em cima do olho
            Vector2 posCursorMundo = ObterPosicaoCursorMundo();
            Vector2 posOlho = new Vector2(olhoAtual.transform.position.x, olhoAtual.transform.position.y);
            float distancia = Vector2.Distance(posCursorMundo, posOlho);

            if (distancia <= raioAcertoLanterna)
            {
                tempoIluminado += Time.deltaTime;
                olhoAtual.transform.localPosition += (Vector3)Random.insideUnitCircle * 0.04f;

                if (tempoIluminado >= tempoLuzParaEspantar)
                {
                    EspantarOlhoAtual();
                }
            }
            else
            {
                tempoIluminado = Mathf.Max(0f, tempoIluminado - Time.deltaTime);
            }
        }
    }

    // Inicia o alerta sonoro e libera o botão de seta para o corredor
    public void DispararEventoOlhos()
    {
        if (eventoAtivo) return;

        eventoAtivo = true;
        dentroDaSala = false;
        olhosEspantados = 0;
        cronometroSeta = tempoParaClicarNaSeta;

        if (audioSource != null && somBarulhoDireita != null)
        {
            audioSource.PlayOneShot(somBarulhoDireita);
        }

        if (botaoSetaDireita != null)
        {
            botaoSetaDireita.SetActive(true);
        }
    }

    // Escurece o ambiente, ativa a visão da sala escura e faz o primeiro olho surgir
    public void EntrarNaSalaLanterna()
    {
        if (!eventoAtivo || dentroDaSala) return;

        dentroDaSala = true;
        if (botaoSetaDireita != null) botaoSetaDireita.SetActive(false);
        if (luzGlobalCena != null) luzGlobalCena.intensity = 0.05f;
        if (salaLanterna != null) salaLanterna.SetActive(true);

        SpawnarProximoOlho();
    }

    // Sorteia a aparência e um dos pontos marcados na cena para o próximo olho aparecer
    void SpawnarProximoOlho()
    {
        tempoIluminado = 0f;

        if (spritesOlhos != null && spritesOlhos.Length > 0)
        {
            olhoAtual.sprite = spritesOlhos[Random.Range(0, spritesOlhos.Length)];
        }

        if (pontosDeAparicao != null && pontosDeAparicao.Length > 0)
        {
            Transform pontoEscolhido = pontosDeAparicao[Random.Range(0, pontosDeAparicao.Length)];
            olhoAtual.transform.position = pontoEscolhido.position;
        }

        olhoAtual.transform.localScale = escalaOriginalOlho;
        olhoAtual.gameObject.SetActive(true);
    }

    // Contabiliza o olho espantado e encerra o evento ao atingir a meta
    void EspantarOlhoAtual()
    {
        if (audioSource != null && somEspantarOlho != null)
        {
            audioSource.PlayOneShot(somEspantarOlho);
        }

        olhoAtual.gameObject.SetActive(false);
        olhosEspantados++;

        if (olhosEspantados >= totalOlhosParaEspantar)
        {
            ConcluirEventoLanterna();
        }
        else
        {
            Invoke(nameof(SpawnarProximoOlho), 0.5f);
        }
    }

    // Restaura a iluminação normal e devolve o controle para a triagem
    void ConcluirEventoLanterna()
    {
        eventoAtivo = false;
        dentroDaSala = false;

        if (salaLanterna != null) salaLanterna.SetActive(false);
        if (luzGlobalCena != null) luzGlobalCena.intensity = 1f;

        if (gerenciadorTriagem != null)
        {
            gerenciadorTriagem.jogoPausadoPorPuzzle = false;
            gerenciadorTriagem.Invoke("ChamarProximoViajante", 1.0f);
        }
    }

    void GirarLanternaParaCursor()
    {
        Vector2 posCursor = ObterPosicaoCursorMundo();
        Vector2 direcao = posCursor - (Vector2)lanternaJogador.position;
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        lanternaJogador.rotation = Quaternion.Euler(0f, 0f, angulo + compensacaoAnguloArte);
    }

    // Converte a posição do mouse ou toque na tela para as coordenadas 2D do mundo
    Vector2 ObterPosicaoCursorMundo()
    {
        Vector2 posTela = Vector2.zero;
        if (Mouse.current != null)
        {
            posTela = Mouse.current.position.ReadValue();
        }
        else if (Touchscreen.current != null)
        {
            posTela = Touchscreen.current.primaryTouch.position.ReadValue();
        }

        Vector3 posMundo = Camera.main.ScreenToWorldPoint(new Vector3(posTela.x, posTela.y, 10f));
        return new Vector2(posMundo.x, posMundo.y);
    }

    // Fecha a sala da lanterna, exibe o ataque na tela e aciona o fim de jogo
    IEnumerator SequenciaJumpscareOlho()
    {
        if (salaLanterna != null) salaLanterna.SetActive(false);
        if (botaoSetaDireita != null) botaoSetaDireita.SetActive(false);
        if (luzGlobalCena != null) luzGlobalCena.intensity = 1f;

        if (audioSource != null && somJumpscareOlho != null)
        {
            audioSource.PlayOneShot(somJumpscareOlho);
        }

        if (visualOlhoJumpscare != null)
        {
            visualOlhoJumpscare.SetActive(true);
            RectTransform rt = visualOlhoJumpscare.GetComponent<RectTransform>();
            float t = 0f;
            while (t < 0.35f)
            {
                t += Time.deltaTime;
                rt.localScale = Vector3.Lerp(Vector3.one * 0.6f, Vector3.one * 1.4f, t / 0.35f);
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