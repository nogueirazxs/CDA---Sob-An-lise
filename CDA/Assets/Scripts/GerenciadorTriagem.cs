using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GerenciadorTriagem : MonoBehaviour
{
    [Header("Configuração do Dia")]
    public int diaAtual = 1;
    public List<DadosViajante> todosPersonagens = new List<DadosViajante>();
    public ControladorViajante viajanteNaJanela;

    [Header("Barra de Diálogo do Viajante")]
    public GameObject caixaDialogoViajante;
    public TextMeshProUGUI textoFalaViajante;

    [Header("Cartão de Identidade (Mesa e Zoom UI)")]
    public GameObject cartaoNaMesa;
    public GameObject painelCartaoAmpliado;
    public Image imagemCartaoAmpliado;

    [Header("Botões de Decisão e Timer")]
    public GameObject painelDecisao;
    public TextMeshProUGUI textoTimer;
    public float tempoLimiteInvestigacao = 45f;

    [Header("Eventos e Puzzles")]
    public RadioChamada radioChamada;
    public EventoEnergiaDia2 eventoEnergiaDia2;
    public EventoLanterna eventoLanterna;

    [Header("Transição Final do Dia 2 (Tremida + Fade)")]
    public Image painelFadePreto;
    public string nomeCenaFinal = "FimDeTurno";
    public AudioSource audioSource;

    [HideInInspector] public bool jogoPausadoPorPuzzle = false;

    private List<DadosViajante> filaDoDia = new List<DadosViajante>();
    private List<bool> listaAnomaliasDoDia = new List<bool>();
    private HashSet<DadosViajante> personagensJaVistos = new HashSet<DadosViajante>();

    private int indiceViajante = 0;
    private int atendidosNoDia2 = 0;
    private int puzzlesDisparadosDia2 = 0;
    private bool viajanteAtualEhAnomalia = false;
    private int anomaliasQuePassaram = 0;
    private float tempoRestante;

    private bool podeDecidir = false;
    private Coroutine rotinaContagem = null;

    void Start()
    {
        EsconderTodaHUDTriagem();
        viajanteNaJanela.Esconder();

        if (painelFadePreto != null)
        {
            Color c = painelFadePreto.color;
            c.a = 0f;
            painelFadePreto.color = c;
            painelFadePreto.gameObject.SetActive(false);
        }
    }

    void EsconderTodaHUDTriagem()
    {
        caixaDialogoViajante.SetActive(false);
        cartaoNaMesa.SetActive(false);
        painelCartaoAmpliado.SetActive(false);
        painelDecisao.SetActive(false);
    }

    // Reinicia os contadores e monta a fila de viajantes conforme o dia atual
    public void IniciarDia()
    {
        CancelInvoke();
        if (rotinaContagem != null)
        {
            StopCoroutine(rotinaContagem);
        }

        podeDecidir = false;
        jogoPausadoPorPuzzle = false;
        anomaliasQuePassaram = 0;
        indiceViajante = 0;

        EsconderTodaHUDTriagem();

        if (diaAtual == 1)
        {
            personagensJaVistos.Clear();
            SortearFilaDia1();
        }
        else
        {
            atendidosNoDia2 = 0;
            puzzlesDisparadosDia2 = 0;
        }

        ChamarProximoViajante();
    }

    // Seleciona 4 viajantes aleatórios e embaralha quem será anomalia no primeiro dia
    void SortearFilaDia1()
    {
        filaDoDia.Clear();
        listaAnomaliasDoDia.Clear();

        List<DadosViajante> copia = new List<DadosViajante>(todosPersonagens);
        for (int i = 0; i < 4; i++)
        {
            if (copia.Count == 0) break;
            int rand = Random.Range(0, copia.Count);
            filaDoDia.Add(copia[rand]);
            copia.RemoveAt(rand);
        }

        for (int i = 0; i < 4; i++)
        {
            listaAnomaliasDoDia.Add(i < 2 ? true : Random.value < 0.25f);
        }

        for (int i = 0; i < listaAnomaliasDoDia.Count; i++)
        {
            bool temp = listaAnomaliasDoDia[i];
            int rand = Random.Range(i, listaAnomaliasDoDia.Count);
            listaAnomaliasDoDia[i] = listaAnomaliasDoDia[rand];
            listaAnomaliasDoDia[rand] = temp;
        }
    }

    bool TodosPersonagensJaApareceram()
    {
        foreach (var p in todosPersonagens)
        {
            if (p != null && !personagensJaVistos.Contains(p))
                return false;
        }
        return true;
    }

    // Prioriza viajantes inéditos até que todo o elenco tenha aparecido na cabine
    DadosViajante EscolherProximoViajanteDia2()
    {
        List<DadosViajante> aindaNaoVistos = new List<DadosViajante>();
        foreach (var p in todosPersonagens)
        {
            if (p != null && !personagensJaVistos.Contains(p))
            {
                aindaNaoVistos.Add(p);
            }
        }

        if (aindaNaoVistos.Count > 0 && (Random.value < 0.7f || personagensJaVistos.Count == 0))
        {
            return aindaNaoVistos[Random.Range(0, aindaNaoVistos.Count)];
        }

        return todosPersonagens[Random.Range(0, todosPersonagens.Count)];
    }

    // Faz o próximo viajante surgir na janela ou encerra o turno quando os objetivos forem cumpridos
    public void ChamarProximoViajante()
    {
        if (jogoPausadoPorPuzzle) return;

        CancelInvoke(nameof(ChamarProximoViajante));
        if (rotinaContagem != null)
        {
            StopCoroutine(rotinaContagem);
        }

        EsconderTodaHUDTriagem();
        podeDecidir = false;

        DadosViajante atual = null;

        if (diaAtual == 1)
        {
            if (indiceViajante >= filaDoDia.Count)
            {
                FimDoDia();
                return;
            }
            atual = filaDoDia[indiceViajante];
            viajanteAtualEhAnomalia = listaAnomaliasDoDia[indiceViajante];
        }
        else
        {
            if (TodosPersonagensJaApareceram() && puzzlesDisparadosDia2 >= 2)
            {
                FimDoDia();
                return;
            }

            atual = EscolherProximoViajanteDia2();
            viajanteAtualEhAnomalia = Random.value < 0.65f;
        }

        personagensJaVistos.Add(atual);
        viajanteNaJanela.ConfigurarViajante(atual, viajanteAtualEhAnomalia);

        // Anomalias podem mentir sobre a própria origem durante a apresentação
        string origemFalada = atual.origemCorreta;
        if (viajanteAtualEhAnomalia && Random.value < 0.5f)
        {
            origemFalada = atual.origemMentirosa;
        }

        string falaFinal = atual.falaApresentacao.Replace("{origem}", origemFalada);

        caixaDialogoViajante.SetActive(true);
        textoFalaViajante.text = falaFinal;
        imagemCartaoAmpliado.sprite = atual.spriteCartaoPronto;
    }

    // Libera o cartão de identidade na mesa e inicia o cronômetro de decisão
    public void TerminarFalaDoViajante()
    {
        if (!caixaDialogoViajante.activeSelf || jogoPausadoPorPuzzle) return;

        caixaDialogoViajante.SetActive(false);

        cartaoNaMesa.SetActive(true);
        painelDecisao.SetActive(true);
        tempoRestante = tempoLimiteInvestigacao;
        podeDecidir = true;

        if (rotinaContagem != null)
        {
            StopCoroutine(rotinaContagem);
        }
        rotinaContagem = StartCoroutine(ContagemRegressiva());
    }

    // Se o tempo esgotar sem resposta, o viajante entra automaticamente
    IEnumerator ContagemRegressiva()
    {
        while (tempoRestante > 0)
        {
            if (!jogoPausadoPorPuzzle)
            {
                tempoRestante -= Time.deltaTime;
                textoTimer.text = Mathf.CeilToInt(tempoRestante).ToString() + "s";
            }
            yield return null;
        }

        DecidirEntrada(true);
    }

    public void AbrirCartao()
    {
        if (!podeDecidir) return;
        painelCartaoAmpliado.SetActive(true);
    }

    public void FecharCartao()
    {
        painelCartaoAmpliado.SetActive(false);
    }

    // Avalia a escolha do jogador, checa o limite de erros e dispara os eventos do dia
    public void DecidirEntrada(bool aprovado)
    {
        if (!podeDecidir) return;
        podeDecidir = false;

        if (rotinaContagem != null)
        {
            StopCoroutine(rotinaContagem);
        }
        CancelInvoke(nameof(ChamarProximoViajante));

        EsconderTodaHUDTriagem();

        if (aprovado && viajanteAtualEhAnomalia)
        {
            anomaliasQuePassaram++;
        }

        int limiteErros = (diaAtual == 1) ? 2 : 1;

        if (anomaliasQuePassaram >= limiteErros)
        {
            StartCoroutine(SequenciaGameOver());
            return;
        }

        viajanteNaJanela.Esconder();

        if (diaAtual == 1)
        {
            indiceViajante++;
            Invoke(nameof(ChamarProximoViajante), 1.5f);
        }
        else
        {
            atendidosNoDia2++;

            // Dispara o evento dos olhos na sala lateral após o segundo atendimento
            if (atendidosNoDia2 == 2 && puzzlesDisparadosDia2 == 0 && eventoLanterna != null)
            {
                puzzlesDisparadosDia2 = 1;
                jogoPausadoPorPuzzle = true;
                eventoLanterna.DispararEventoOlhos();
                return;
            }
            // Dispara a queda de energia na cabine após o quarto atendimento
            else if (atendidosNoDia2 == 4 && puzzlesDisparadosDia2 == 1 && eventoEnergiaDia2 != null)
            {
                puzzlesDisparadosDia2 = 2;
                jogoPausadoPorPuzzle = true;
                eventoEnergiaDia2.DispararQuedaDeEnergia();
                return;
            }

            Invoke(nameof(ChamarProximoViajante), 1.5f);
        }
    }

    IEnumerator SequenciaGameOver()
    {
        EsconderTodaHUDTriagem();
        yield return StartCoroutine(viajanteNaJanela.ExecutarJumpscare());
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void FimDoDia()
    {
        EsconderTodaHUDTriagem();

        if (diaAtual == 1 && radioChamada != null)
        {
            radioChamada.AtivarChamadaFimDia1();
        }
        else if (diaAtual == 2)
        {
            StartCoroutine(SequenciaFimDoDia2());
        }
    }

    // Faz a câmera tremer, escurece a tela e carrega a cena de encerramento do turno
    IEnumerator SequenciaFimDoDia2()
    {
        audioSource.Stop();
        Transform cam = Camera.main.transform;
        Vector3 posOriginal = cam.position;
        float tempoTremida = 0f;

        while (tempoTremida < 1.8f)
        {
            tempoTremida += Time.deltaTime;
            float forca = Mathf.Lerp(0.05f, 0.25f, tempoTremida / 1.8f);
            cam.position = posOriginal + (Vector3)Random.insideUnitCircle * forca;
            yield return null;
        }
        cam.position = posOriginal;

        if (painelFadePreto != null)
        {
            painelFadePreto.gameObject.SetActive(true);
            float tempoFade = 0f;
            float duracaoFade = 1.5f;
            Color c = painelFadePreto.color;

            while (tempoFade < duracaoFade)
            {
                tempoFade += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, tempoFade / duracaoFade);
                painelFadePreto.color = c;
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nomeCenaFinal);
    }

    public IEnumerator SequenciaGameOverSemJumpscareJanela()
    {
        EsconderTodaHUDTriagem();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}