using System.Collections;
using UnityEngine;

public class RadioChamada : MonoBehaviour
{
    [Header("Sprites do Rádio")]
    public Sprite spriteNormal; // Arraste Radio 1
    public Sprite spriteChamando; // Arraste Radio 2

    [Header("Áudio e Diálogo")]
    public AudioSource audioSource;
    public AudioClip somChamadaRadio;
    public Dialogo dialogoFimDia1;

    [Header("Eventos após o diálogo")]
    public GameObject botaoIrParaCaixa;
    public SpriteRenderer fundoCabine;
    public Color corEscura = new Color(0.25f, 0.25f, 0.3f, 1f); 

    private SpriteRenderer spriteRenderer;
    private bool temMensagemPendente = false;
    private bool aguardandoFimDoDialogo = false;

    [Header("Referências para Restaurar Luz e Dia 2")]
    public GameObject painelSalaEnergia;
    public GameObject painelCaixaAberta;
    public GerenciadorTriagem gerenciadorTriagem;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (botaoIrParaCaixa != null) botaoIrParaCaixa.SetActive(false);
    }

    // O GerenciadorTriagem vai chamar isso no FimDoDia()!
    public void AtivarChamadaFimDia1()
    {
        temMensagemPendente = true;
        StartCoroutine(AnimarRadioChamando());
    }

    IEnumerator AnimarRadioChamando()
    {
        while (temMensagemPendente)
        {
            spriteRenderer.sprite = spriteChamando;
            if (audioSource != null && somChamadaRadio != null)
                audioSource.PlayOneShot(somChamadaRadio);

            yield return new WaitForSeconds(0.5f);

            spriteRenderer.sprite = spriteNormal;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Ligue este método no OnClick do Botão do Rádio!
    public void ClicarNoRadio()
    {
        if (!temMensagemPendente) return;

        temMensagemPendente = false;
        spriteRenderer.sprite = spriteNormal;

        if (TelaPretaDialogo.instancia != null)
        {
            TelaPretaDialogo.instancia.acabou = false;
            TelaPretaDialogo.instancia.ComecarDialogo(dialogoFimDia1);
            aguardandoFimDoDialogo = true;
        }
    }

    void Update()
    {
        // Quando o jogador terminar de ler as falas do Faven:
        if (aguardandoFimDoDialogo && TelaPretaDialogo.instancia != null && TelaPretaDialogo.instancia.acabou)
        {
            aguardandoFimDoDialogo = false;
            ApagarLuzELiberarSeta();
        }
    }

    void ApagarLuzELiberarSeta()
    {
        // Escurece o fundo da cabine indicando queda de energia!
        if (fundoCabine != null)
            fundoCabine.color = corEscura;

        // Mostra o sinal/seta na lateral para o jogador ir até a caixa de fiação
        if (botaoIrParaCaixa != null)
            botaoIrParaCaixa.SetActive(true);
    }

    public void AoRestaurarEnergia()
    {
        // 1. Fecha a caixa aberta e a sala da esquerda
        if (painelCaixaAberta != null) painelCaixaAberta.SetActive(false);
        if (painelSalaEnergia != null) painelSalaEnergia.SetActive(false);

        // 2. Acende a luz da cabine de volta para o branco normal!
        if (fundoCabine != null)
            fundoCabine.color = Color.white;

        // 3. Inicia o Dia 2 no GerenciadorTriagem!
        if (gerenciadorTriagem != null)
        {
            gerenciadorTriagem.diaAtual = 2;
            gerenciadorTriagem.IniciarDia();
        }
    }
}