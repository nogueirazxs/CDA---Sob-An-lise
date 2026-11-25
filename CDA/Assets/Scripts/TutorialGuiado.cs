using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Estrutura que vincula cada objeto da cabine à sua respectiva explicação na tela
[System.Serializable]
public class PassoTutorial
{
    public Transform objetoAlvo;
    [TextArea(2, 4)]
    public string explicacao;
}

public class TutorialGuiado : MonoBehaviour
{
    [Header("Lista de Itens")]
    public List<PassoTutorial> passos = new List<PassoTutorial>();

    [Header("Elementos de Interface")]
    public GameObject painelTutorial;
    public TextMeshProUGUI textoExplicacao;

    [Header("Zoom da Câmera")]
    public float zoomAproximado = 2.5f;
    public float velocidadeZoom = 5f;
    public float deslocamentoLateral = 1.2f;

    [Header("Conexão com o Jogo")]
    public GerenciadorTriagem gerenciadorTriagem;

    private Camera cameraPrincipal;
    private int passoAtual = 0;
    private bool tutorialAtivo = false;
    private bool jaIniciouUmaVez = false;

    private Vector3 posicaoOriginalCamera;
    private float zoomOriginalCamera;
    private Vector3 posicaoDesejada;
    private float zoomDesejado;

    void Awake()
    {
        tutorialAtivo = false;
        jaIniciouUmaVez = false;
    }

    void Start()
    {
        cameraPrincipal = Camera.main;

        if (cameraPrincipal != null)
        {
            posicaoOriginalCamera = cameraPrincipal.transform.position;
            zoomOriginalCamera = cameraPrincipal.orthographicSize;
        }

        if (painelTutorial != null)
        {
            painelTutorial.SetActive(false);
        }
    }

    // Move e aproxima a câmera suavemente em direção ao item destacado no passo atual
    void Update()
    {
        if (!tutorialAtivo || cameraPrincipal == null) return;

        cameraPrincipal.orthographicSize = Mathf.MoveTowards(
            cameraPrincipal.orthographicSize,
            zoomDesejado,
            Time.unscaledDeltaTime * velocidadeZoom * 4f
        );

        cameraPrincipal.transform.position = Vector3.MoveTowards(
            cameraPrincipal.transform.position,
            posicaoDesejada,
            Time.unscaledDeltaTime * velocidadeZoom * 4f
        );
    }

    // Inicia o tour guiado pela cabine apenas uma vez por partida
    public void IniciarTutorial()
    {
        if (jaIniciouUmaVez || passos.Count == 0) return;

        if (cameraPrincipal == null)
        {
            cameraPrincipal = Camera.main;
        }

        jaIniciouUmaVez = true;
        passoAtual = 0;
        tutorialAtivo = true;

        if (painelTutorial != null)
        {
            painelTutorial.SetActive(true);
        }

        FocarNoPassoAtual();
    }

    // Atualiza o texto na tela e define as coordenadas de enquadramento do alvo atual
    void FocarNoPassoAtual()
    {
        if (passos[passoAtual].objetoAlvo == null) return;

        Transform alvo = passos[passoAtual].objetoAlvo;
        textoExplicacao.text = passos[passoAtual].explicacao;

        zoomDesejado = zoomAproximado;
        posicaoDesejada = new Vector3(
            alvo.position.x + deslocamentoLateral,
            alvo.position.y,
            posicaoOriginalCamera.z
        );
    }

    public void ProximoPasso()
    {
        passoAtual++;

        if (passoAtual < passos.Count)
        {
            FocarNoPassoAtual();
        }
        else
        {
            EncerrarTutorial();
        }
    }

    // Devolve a câmera para a posição padrão da cabine e libera a entrada do primeiro viajante
    public void EncerrarTutorial()
    {
        tutorialAtivo = false;

        if (cameraPrincipal != null)
        {
            cameraPrincipal.orthographicSize = zoomOriginalCamera;
            cameraPrincipal.transform.position = posicaoOriginalCamera;
        }

        if (painelTutorial != null)
        {
            painelTutorial.SetActive(false);
        }

        if (gerenciadorTriagem != null)
        {
            gerenciadorTriagem.IniciarDia();
        }
    }
}