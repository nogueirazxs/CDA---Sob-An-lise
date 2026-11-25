using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GerenciadorFios : MonoBehaviour
{
    public static bool estaArrastandoFio = false;

    [Header("Todos os 8 Pontos da Caixa")]
    public List<PontoFio> todosOsPontos = new List<PontoFio>();

    [Header("Configuração de Sensibilidade")]
    public float raioEncaixe = 35f;
    public float raioColisaoErro = 28f;

    [Header("Áudio do Fio")]
    public AudioSource audioSourceFio;
    public AudioClip somChoqueArrastando;
    [Range(0f, 1f)] public float volumeChoque = 0.25f;

    [Header("Evento de Conclusão")]
    public UnityEvent aoConcluirFios;

    private float volumeOriginalAudioSource = 1f;
    private PontoFio pontoArrastando = null;
    private bool puzzleConcluido = false;

    // Inicia o arraste do fio selecionado e ajusta temporariamente o volume do choque
    public void IniciarArraste(PontoFio ponto)
    {
        if (puzzleConcluido || ponto.jaConectado) return;

        pontoArrastando = ponto;
        estaArrastandoFio = true;

        if (audioSourceFio != null && somChoqueArrastando != null)
        {
            volumeOriginalAudioSource = audioSourceFio.volume;
            audioSourceFio.volume = volumeChoque;
            audioSourceFio.clip = somChoqueArrastando;
            audioSourceFio.Play();
        }
    }

    // Estica o fio até o cursor, cancela se encostar em outra cor ou encaixa no par certo
    public void AtualizarArraste(Vector2 posicaoTela)
    {
        if (pontoArrastando == null || puzzleConcluido) return;

        pontoArrastando.AtualizarVisualFio(posicaoTela);

        foreach (PontoFio outro in todosOsPontos)
        {
            if (outro == null || outro == pontoArrastando || outro == pontoArrastando.parCorrespondente)
                continue;

            float distOutro = Vector2.Distance(posicaoTela, outro.rectTransform.position);
            if (distOutro <= raioColisaoErro)
            {
                CancelarArrasteAtual();
                return;
            }
        }

        if (pontoArrastando.parCorrespondente != null)
        {
            float distPar = Vector2.Distance(posicaoTela, pontoArrastando.parCorrespondente.rectTransform.position);
            if (distPar <= raioEncaixe)
            {
                ConectarPar(pontoArrastando, pontoArrastando.parCorrespondente);
            }
        }
    }

    public void SoltarArraste()
    {
        if (pontoArrastando != null && !pontoArrastando.jaConectado)
        {
            CancelarArrasteAtual();
        }
    }

    // Recolhe o fio de volta ao pino inicial quando o jogador erra ou solta antes da hora
    void CancelarArrasteAtual()
    {
        PararSomFio();

        if (pontoArrastando != null)
        {
            pontoArrastando.ResetarFio();
            pontoArrastando = null;
        }

        estaArrastandoFio = false;
    }

    // Trava a ligação entre dois pinos da mesma cor e verifica se a caixa inteira foi consertada
    void ConectarPar(PontoFio a, PontoFio b)
    {
        if (a.jaConectado || b.jaConectado) return;

        estaArrastandoFio = false;
        PararSomFio();

        a.jaConectado = true;
        b.jaConectado = true;
        a.AtualizarVisualFio(b.rectTransform.position);
        pontoArrastando = null;

        if (TodosOsPontosConectados())
        {
            puzzleConcluido = true;
            aoConcluirFios?.Invoke();
        }
    }

    bool TodosOsPontosConectados()
    {
        if (todosOsPontos.Count == 0) return false;

        foreach (PontoFio p in todosOsPontos)
        {
            if (p != null && !p.jaConectado)
                return false;
        }
        return true;
    }

    // Desfaz todas as ligações para reutilizar a caixa de energia em eventos futuros
    public void ResetarPuzzleCompleto()
    {
        PararSomFio();
        estaArrastandoFio = false;
        puzzleConcluido = false;
        pontoArrastando = null;

        foreach (PontoFio p in todosOsPontos)
        {
            if (p != null) p.ResetarFio();
        }
    }

    // Interrompe o som de choque e devolve o volume original ao AudioSource
    void PararSomFio()
    {
        if (audioSourceFio != null)
        {
            audioSourceFio.Stop();
            audioSourceFio.volume = volumeOriginalAudioSource;
        }
    }
}