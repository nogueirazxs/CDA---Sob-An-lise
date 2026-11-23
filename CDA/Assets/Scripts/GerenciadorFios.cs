using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GerenciadorFios : MonoBehaviour
{
    [Header("Todos os 8 Pontos da Caixa")]
    public List<PontoFio> todosOsPontos = new List<PontoFio>();

    [Header("Configuração de Sensibilidade")]
    public float raioEncaixe = 35f; // Distância para conectar no par certo
    public float raioColisaoErro = 28f; // Distância para falhar se passar por cima de outra cor

    [Header("Quando terminar todos os 4 fios")]
    public UnityEvent aoConcluirFios;

    private PontoFio pontoArrastando = null;
    private bool puzzleConcluido = false;

    public void IniciarArraste(PontoFio ponto)
    {
        if (puzzleConcluido || ponto.jaConectado) return;
        pontoArrastando = ponto;
    }

    public void AtualizarArraste(Vector2 posicaoTela)
    {
        if (pontoArrastando == null || puzzleConcluido) return;

        // Estica o fio visual até o dedo/mouse
        pontoArrastando.AtualizarVisualFio(posicaoTela);

        // Verifica se passou por cima de algum ponto de OUTRA cor
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

        // Verifica se chegou no ponto da MESMA cor (par certo)
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

    void CancelarArrasteAtual()
    {
        if (pontoArrastando != null)
        {
            pontoArrastando.ResetarFio();
            pontoArrastando = null;
        }
    }

    void ConectarPar(PontoFio a, PontoFio b)
    {
        if (a.jaConectado || b.jaConectado) return;

        a.jaConectado = true;
        b.jaConectado = true;
        a.AtualizarVisualFio(b.rectTransform.position);
        pontoArrastando = null;

        // Só conclui se TODOS os 8 pontos da lista estiverem conectados!
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

    public void ResetarPuzzleCompleto()
    {
        puzzleConcluido = false;
        pontoArrastando = null;
        foreach (PontoFio p in todosOsPontos)
        {
            if (p != null) p.ResetarFio();
        }
    }
}