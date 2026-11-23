using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GerenciadorTriagem : MonoBehaviour
{
    [Header("Configuração do Dia")]
    public int diaAtual = 1; // Mude para 2 no segundo dia
    public List<DadosViajante> todosPersonagens = new List<DadosViajante>();
    public ControladorViajante viajanteNaJanela;

    [Header("Barra de Diálogo do Viajante")]
    public GameObject caixaDialogoViajante;
    public TextMeshProUGUI textoFalaViajante;

    [Header("Cartão de Identidade (Mesa e Zoom UI)")]
    public GameObject cartaoNaMesa; // Sprite/Botão pequeno na mesa
    public GameObject painelCartaoAmpliado; // Painel grande na tela
    public Image imagemCartaoAmpliado; // Image central que mostra o sprite do cartão pronto

    [Header("Botões de Decisão e Timer")]
    public GameObject painelDecisao; // Guarda botões Aprovar/Negar e o Timer
    public TextMeshProUGUI textoTimer;
    public float tempoLimiteInvestigacao = 45f;

    // Controle interno
    private List<DadosViajante> filaDoDia = new List<DadosViajante>();
    private List<bool> listaAnomaliasDoDia = new List<bool>();
    private int indiceViajante = 0;
    private bool viajanteAtualEhAnomalia = false;
    private int anomaliasQuePassaram = 0;
    private float tempoRestante;

    // Trava flexível para seus PUZZLES no meio do jogo!
    [HideInInspector] public bool jogoPausadoPorPuzzle = false;

    public RadioChamada radioChamada;

    public EventoEnergiaDia2 eventoEnergiaDia2;

    void Start()
    {
        caixaDialogoViajante.SetActive(false);
        cartaoNaMesa.SetActive(false);
        painelCartaoAmpliado.SetActive(false);
        painelDecisao.SetActive(false);
        viajanteNaJanela.Esconder();

        // Chame IniciarDia() aqui ou depois do seu tutorial acabar!
    }

    public void IniciarDia()
    {
        anomaliasQuePassaram = 0;
        indiceViajante = 0;
        SortearFilaDe4Viajantes();

        // Garante no mínimo 2 anomalias no Dia 1 (com 25% de chance extra) e 3 no Dia 2 (com 40% de chance extra)
        int minAnomalias = (diaAtual == 1) ? 2 : 3;
        float chanceExtra = (diaAtual == 1) ? 0.25f : 0.40f;

        listaAnomaliasDoDia.Clear();
        for (int i = 0; i < 4; i++)
        {
            if (i < minAnomalias)
                listaAnomaliasDoDia.Add(true);
            else
                listaAnomaliasDoDia.Add(Random.value < chanceExtra);
        }

        // Embaralha a ordem na fila para ser imprevisível
        for (int i = 0; i < listaAnomaliasDoDia.Count; i++)
        {
            bool temp = listaAnomaliasDoDia[i];
            int rand = Random.Range(i, listaAnomaliasDoDia.Count);
            listaAnomaliasDoDia[i] = listaAnomaliasDoDia[rand];
            listaAnomaliasDoDia[rand] = temp;
        }

        ChamarProximoViajante();
    }

    void SortearFilaDe4Viajantes()
    {
        filaDoDia.Clear();
        List<DadosViajante> copia = new List<DadosViajante>(todosPersonagens);

        for (int i = 0; i < 4; i++)
        {
            if (copia.Count == 0) break;
            int rand = Random.Range(0, copia.Count);
            filaDoDia.Add(copia[rand]);
            copia.RemoveAt(rand);
        }
    }

    void ChamarProximoViajante()
    {
        if (indiceViajante >= filaDoDia.Count)
        {
            FimDoDia();
            return;
        }

        DadosViajante atual = filaDoDia[indiceViajante];

        // Pega o resultado já sorteado e embaralhado para este viajante
        viajanteAtualEhAnomalia = listaAnomaliasDoDia[indiceViajante];

        // Mostra o personagem na janela
        viajanteNaJanela.ConfigurarViajante(atual, viajanteAtualEhAnomalia);

        // Prepara a fala (se for anomalia, 50% de chance de mentir a origem na fala!)
        string origemFalada = atual.origemCorreta;
        if (viajanteAtualEhAnomalia && Random.value < 0.5f)
        {
            origemFalada = atual.origemMentirosa;
        }

        string falaFinal = atual.falaApresentacao.Replace("{origem}", origemFalada);

        // Exibe a barra de diálogo do viajante
        caixaDialogoViajante.SetActive(true);
        textoFalaViajante.text = falaFinal;

        // Coloca o sprite do cartão pronto daquele personagem na UI ampliada
        imagemCartaoAmpliado.sprite = atual.spriteCartaoPronto;
    }

    // Ligue este método num botão invisível ou botão "Continuar" na barra de diálogo do viajante
    public void TerminarFalaDoViajante()
    {
        caixaDialogoViajante.SetActive(false);

        // Entrega o cartão na mesa e libera a investigação + timer
        cartaoNaMesa.SetActive(true);
        painelDecisao.SetActive(true);
        tempoRestante = tempoLimiteInvestigacao;

        StopAllCoroutines();
        StartCoroutine(ContagemRegressiva());
    }

    IEnumerator ContagemRegressiva()
    {
        while (tempoRestante > 0)
        {
            // Se um puzzle acontecer no meio, o timer congela e espera o puzzle acabar!
            if (!jogoPausadoPorPuzzle)
            {
                tempoRestante -= Time.deltaTime;
                textoTimer.text = Mathf.CeilToInt(tempoRestante).ToString() + "s";
            }
            yield return null;
        }

        // Se o tempo acabar sem o jogador decidir, conta como erro/invasão!
        DecidirEntrada(true);
    }

    // --- FUNÇÕES DOS BOTÕES DA TELA ---

    public void AbrirCartao()
    {
        painelCartaoAmpliado.SetActive(true);
    }

    public void FecharCartao()
    {
        painelCartaoAmpliado.SetActive(false);
    }

    // Ligue no botão APROVAR (true) e no botão NEGAR (false)
    public void DecidirEntrada(bool aprovado)
    {
        StopAllCoroutines();
        painelDecisao.SetActive(false);
        cartaoNaMesa.SetActive(false);
        painelCartaoAmpliado.SetActive(false);

        // Verifica se deixou uma anomalia passar!
        if (aprovado && viajanteAtualEhAnomalia)
        {
            anomaliasQuePassaram++;
        }

        // Checa derrota imediata: Dia 1 perde com 2 anomalias | Dia 2 perde com 1 anomalia
        int limiteErros = (diaAtual == 1) ? 2 : 1;

        if (anomaliasQuePassaram >= limiteErros)
        {
            StartCoroutine(SequenciaGameOver());
            return;
        }

        // Se ainda não perdeu, manda o viajante embora e chama o próximo
        viajanteNaJanela.Esconder();
        indiceViajante++;
        if (diaAtual == 2 && indiceViajante == 2 && eventoEnergiaDia2 != null)
        {
            jogoPausadoPorPuzzle = true; // Pausa o timer da triagem enquanto conserta a luz
            eventoEnergiaDia2.DispararQuedaDeEnergia();
        }
        Invoke(nameof(ChamarProximoViajante), 1.5f); // Pequena pausa de 1.5s entre viajantes
    }

    IEnumerator SequenciaGameOver()
    {
        // Roda o jumpscare da anomalia na tela antes de reiniciar a cena!
        yield return StartCoroutine(viajanteNaJanela.ExecutarJumpscare());
        yield return new WaitForSeconds(0.5f);

        // Reinicia a cena inteira do zero
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void FimDoDia()
    {
        if (diaAtual == 1 && radioChamada != null)
        {
            radioChamada.AtivarChamadaFimDia1();
        }
    }

    public IEnumerator SequenciaGameOverSemJumpscareJanela()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}