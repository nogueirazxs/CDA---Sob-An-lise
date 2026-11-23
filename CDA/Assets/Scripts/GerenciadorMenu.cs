using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GerenciadorMenu : MonoBehaviour
{
    [Header("Mudança de Cena")]
    public string cenaJogar, cenaTutorial, cenaCreditos;

    [Header("Animação de Texto")]
    public RectTransform texto1, texto2;
    public float velocidade = 250f; public float distancia = 600f;

    InputAction back;

    void Start()
    {
        back = InputSystem.actions.FindAction("Voltar");
        if (texto1 != null && texto2 != null)
        {
            // Posição inicial dos textos.
            texto1.anchoredPosition = new Vector2(0, texto1.anchoredPosition.y); 
            texto2.anchoredPosition = new Vector2(distancia, texto2.anchoredPosition.y);
        }        
    }

    void Update()
    {
        if (back != null && back.WasPressedThisFrame())
        {
            SceneManager.LoadScene("Menu");
        }

        if (texto1 != null && texto2 != null)
        {
            // Ambos os textos se movimentam para a esquerda.
            texto1.anchoredPosition += Vector2.left * velocidade * Time.deltaTime;
            texto2.anchoredPosition += Vector2.left * velocidade * Time.deltaTime;

            if (texto1.anchoredPosition.x <= -distancia) // Se ele alcançou a uma distância específica:
            {
                // O texto se posicionará na posição do outro texto + a distância programada, em formato de loop.
                texto1.anchoredPosition = new Vector2(texto2.anchoredPosition.x + distancia, texto1.anchoredPosition.y); 
            }

            if (texto2.anchoredPosition.x <= -distancia)
            {
                texto2.anchoredPosition = new Vector2(texto1.anchoredPosition.x + distancia, texto2.anchoredPosition.y);
            }
        }
    }
    public void Jogar()
    {
        SceneManager.LoadScene(cenaJogar);
    }
    public void Tutorial()
    {
        SceneManager.LoadScene(cenaTutorial);
    }
    public void Creditos()
    {
        SceneManager.LoadScene(cenaCreditos);
    }
    public void Sair()
    {
        Application.Quit();
    }
}
