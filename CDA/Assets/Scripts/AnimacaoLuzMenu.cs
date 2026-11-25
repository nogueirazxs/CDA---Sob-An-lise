using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AnimacaoLuzMenu : MonoBehaviour
{
    [Header("Componentes")]
    public Light2D luz;
    public TMP_Text texto;

    [Header("Configuração da Oscilação")]
    public float intensidadeMinima;
    public float intensidadeMaxima;
    public float velocidade = 3f;

    void Update()
    {
        // Gera uma oscilação suave para simular falha elétrica
        float variacao = Mathf.PerlinNoise(Time.time * velocidade, 0f);

        luz.intensity = Mathf.Lerp(intensidadeMinima, intensidadeMaxima, variacao);

        if (texto != null)
        {
            texto.alpha = variacao;
        }
    }
}