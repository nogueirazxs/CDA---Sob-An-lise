using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class AnimacaoLuzMenu : MonoBehaviour
{
    public Light2D luz;
    public float intensidadeMinima;
    public float intensidadeMaxima;
    public float velocidade = 3f;
    public TMP_Text texto;

    void Update()
    {
        // Variável que recebe uma variação de valor de forma suavizada.
        float variacao = Mathf.PerlinNoise(Time.time * velocidade, 0f);

        // A luz muda sua intensidade com uma intercalação definida no inspector.
        luz.intensity = Mathf.Lerp(intensidadeMinima, intensidadeMaxima, variacao);
        
        if (texto != null)
        // Texto recebe uma transparência de acordo com a variação.
        texto.alpha = variacao; 
    }
}
