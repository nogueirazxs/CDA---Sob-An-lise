using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [Header("Tela")]
    public GameObject painelTutorial;
    public TextMeshProUGUI textoExplicacao;

    [Header("Zoom da Câmera")]
    private Camera cameraPrincipal;
    public float zoomAproximado = 2.5f;
    public float velocidadeZoom = 5f;
    public float deslocamentoLateral = 1.2f;

    private int passoAtual = 0;
    private bool tutorialAtivo = false;
    private bool jaIniciouUmaVez = false;

    private Vector3 posicaoOriginalCamera;
    private float zoomOriginalCamera;

    private Vector3 posicaoDesejada;
    private float zoomDesejado;

    public GerenciadorTriagem gerenciadorTriagem;

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
            painelTutorial.SetActive(false);
    }

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

    public void IniciarTutorial()
    {
        // Impede que o tutorial inicie duas vezes na mesma partida!
        if (jaIniciouUmaVez || passos.Count == 0) return;

        if (cameraPrincipal == null) cameraPrincipal = Camera.main;

        jaIniciouUmaVez = true;
        passoAtual = 0;
        tutorialAtivo = true;

        if (painelTutorial != null)
            painelTutorial.SetActive(true);

        FocarNoPassoAtual();
    }

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

    public void EncerrarTutorial()
    {
        tutorialAtivo = false;

        if (cameraPrincipal != null)
        {
            cameraPrincipal.orthographicSize = zoomOriginalCamera;
            cameraPrincipal.transform.position = posicaoOriginalCamera;
        }

        if (painelTutorial != null)
            painelTutorial.SetActive(false);

        if (gerenciadorTriagem != null)
            gerenciadorTriagem.IniciarDia();
    }
}