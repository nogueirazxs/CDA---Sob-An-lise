using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadioChamada : MonoBehaviour
{
    [Header("Sprites do Rádio")]
    public Sprite spriteNormal;
    public Sprite spriteChamando;

    [Header("Áudio e Diálogo")]
    public AudioSource audioSource;
    public AudioClip somChamadaRadio;
    public AudioClip somTransicaoDia2;
    public Dialogo dialogoFimDia1;

    [Header("Transição Visual 'DIA 2'")]
    public Image painelFadePreto;
    public TextMeshProUGUI textoTransicaoDia;

    [Header("Eventos após o Diálogo")]
    public GameObject botaoIrParaCaixa;
    public SpriteRenderer fundoCabine;
    public Color corEscura = new Color(0.25f, 0.25f, 0.3f, 1f);

    [Header("Referências para Restaurar Luz e Dia 2")]
    public GameObject painelSalaEnergia;
    public GameObject painelCaixaAberta;
    public GerenciadorTriagem gerenciadorTriagem;

    private SpriteRenderer spriteRenderer;
    private bool temMensagemPendente = false;
    private bool aguardandoFimDoDialogo = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (botaoIrParaCaixa != null)
        {
            botaoIrParaCaixa.SetActive(false);
        }
    }

    // Inicia o alerta visual e sonoro no rádio quando o turno do primeiro dia termina
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
            {
                audioSource.PlayOneShot(somChamadaRadio);
            }

            yield return new WaitForSeconds(0.5f);

            spriteRenderer.sprite = spriteNormal;
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Atende o chamado ao clicar no rádio e abre o diálogo de encerramento do dia
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
        // Dispara a tela de transição assim que o diálogo do rádio terminar
        if (aguardandoFimDoDialogo && TelaPretaDialogo.instancia != null && TelaPretaDialogo.instancia.acabou)
        {
            aguardandoFimDoDialogo = false;
            StartCoroutine(SequenciaTransicaoDia2());
        }
    }

    // Escurece a tela, exibe o aviso de Dia 2 e prepara a cabine sem luz para o próximo puzzle
    IEnumerator SequenciaTransicaoDia2()
    {
        if (painelFadePreto != null)
        {
            painelFadePreto.gameObject.SetActive(true);
            Color c = painelFadePreto.color;
            float t = 0f;
            while (t < 1.0f)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, t / 1.0f);
                painelFadePreto.color = c;
                yield return null;
            }
        }

        if (textoTransicaoDia != null)
        {
            textoTransicaoDia.text = "DIA 2";
            textoTransicaoDia.gameObject.SetActive(true);
        }

        if (audioSource != null && somTransicaoDia2 != null)
        {
            audioSource.PlayOneShot(somTransicaoDia2);
        }

        ApagarLuzELiberarSeta();

        yield return new WaitForSeconds(2.2f);

        if (textoTransicaoDia != null)
        {
            textoTransicaoDia.gameObject.SetActive(false);
        }

        if (painelFadePreto != null)
        {
            Color c = painelFadePreto.color;
            float t = 0f;
            while (t < 1.0f)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(1f, 0f, t / 1.0f);
                painelFadePreto.color = c;
                yield return null;
            }
            painelFadePreto.gameObject.SetActive(false);
        }
    }

    void ApagarLuzELiberarSeta()
    {
        if (fundoCabine != null)
        {
            fundoCabine.color = corEscura;
        }

        if (botaoIrParaCaixa != null)
        {
            botaoIrParaCaixa.SetActive(true);
        }
    }

    // Fecha a caixa de energia, religa as luzes e dá início oficial ao segundo dia
    public void AoRestaurarEnergia()
    {
        if (painelCaixaAberta != null) painelCaixaAberta.SetActive(false);
        if (painelSalaEnergia != null) painelSalaEnergia.SetActive(false);

        if (fundoCabine != null)
        {
            fundoCabine.color = Color.white;
        }

        if (gerenciadorTriagem != null && gerenciadorTriagem.diaAtual == 1)
        {
            gerenciadorTriagem.diaAtual = 2;
            gerenciadorTriagem.IniciarDia();
        }
    }
}