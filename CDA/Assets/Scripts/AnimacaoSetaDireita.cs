using UnityEngine;

public class AnimacaoSetaDireita : MonoBehaviour
{
    [Header("Configuração do Movimento")]
    public float distanciaMovimento = 20f;
    public float velocidade = 6f;

    private RectTransform rectTransform;
    private Vector2 posicaoBase;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        posicaoBase = rectTransform.anchoredPosition;
    }

    // Garante que a seta sempre reapareça alinhada no ponto de origem
    void OnEnable()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = posicaoBase;
        }
    }

    // Cria um movimento contínuo de vai-e-vem horizontal para chamar atenção
    void Update()
    {
        float deslocamento = Mathf.Sin(Time.time * velocidade) * distanciaMovimento;
        rectTransform.anchoredPosition = new Vector2(posicaoBase.x + deslocamento, posicaoBase.y);
    }
}