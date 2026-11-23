using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PontoFio : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public PontoFio parCorrespondente;
    public RectTransform visualFio; // Imagem retangular (Square) filha deste ponto
    public GerenciadorFios gerenciador;

    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public bool jaConectado = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ResetarFio();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (jaConectado) return;
        if (visualFio != null) visualFio.gameObject.SetActive(true);
        gerenciador.IniciarArraste(this);
        gerenciador.AtualizarArraste(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (jaConectado) return;
        gerenciador.AtualizarArraste(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        gerenciador.SoltarArraste();
    }

    public void AtualizarVisualFio(Vector2 posicaoDestino)
    {
        if (visualFio == null) return;

        Vector2 origem = rectTransform.position;
        Vector2 direcao = posicaoDestino - origem;
        float distancia = direcao.magnitude;

        visualFio.position = origem;
        visualFio.sizeDelta = new Vector2(distancia / visualFio.lossyScale.x, visualFio.sizeDelta.y);
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        visualFio.rotation = Quaternion.Euler(0, 0, angulo);
    }

    public void ResetarFio()
    {
        jaConectado = false;
        if (visualFio != null)
        {
            visualFio.sizeDelta = new Vector2(0, visualFio.sizeDelta.y);
            visualFio.gameObject.SetActive(false);
        }
    }
}