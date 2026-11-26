using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GerenciadorMenu : MonoBehaviour
{
    [Header("Mudança de Cena")]
    public string cenaJogar, cenaTutorial, cenaCreditos;

    [Header("Animação de Texto")]
    public RectTransform texto1, texto2;
    public float velocidade = 250f;
    public float distancia = 600f;

    [Header("Textos do Tutorial")]
    public TextMeshProUGUI textoTitulo, textoTutorial;

    private InputAction back;

    void Start()
    {
        back = InputSystem.actions.FindAction("Voltar");

        // Alinha os dois blocos de texto lado a lado para iniciar o letreiro contínuo
        if (texto1 != null && texto2 != null)
        {
            texto1.anchoredPosition = new Vector2(0, texto1.anchoredPosition.y);
            texto2.anchoredPosition = new Vector2(distancia, texto2.anchoredPosition.y);
        }
    }

    void Update()
    {
        // Só volta pro menu se o toque/clique NÃO estiver em cima de nenhum botão da UI
        if (back != null && back.WasPressedThisFrame() && !ClicouEmBotaoUI())
        {
            SceneManager.LoadScene("Menu");
        }

        // Movimenta os dois textos para a esquerda e reposiciona no final para criar um loop infinito
        if (texto1 != null && texto2 != null)
        {
            texto1.anchoredPosition += Vector2.left * velocidade * Time.deltaTime;
            texto2.anchoredPosition += Vector2.left * velocidade * Time.deltaTime;

            if (texto1.anchoredPosition.x <= -distancia)
            {
                texto1.anchoredPosition = new Vector2(texto2.anchoredPosition.x + distancia, texto1.anchoredPosition.y);
            }

            if (texto2.anchoredPosition.x <= -distancia)
            {
                texto2.anchoredPosition = new Vector2(texto1.anchoredPosition.x + distancia, texto2.anchoredPosition.y);
            }
        }
    }

    // Verifica se o clique ou toque na tela acertou algum elemento clicável da interface
    bool ClicouEmBotaoUI()
    {
        if (EventSystem.current == null) return false;

        // Checa o mouse / ponteiro padrão
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        // Checa o dedo específico na tela sensível ao toque (celular)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            int touchId = Touchscreen.current.primaryTouch.touchId.ReadValue();
            if (EventSystem.current.IsPointerOverGameObject(touchId))
                return true;
        }

        return false;
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

    // Atualiza o título e a descrição exibidos na aba de instruções
    public void TextoTutorial(string tutorialTitulo) => textoTitulo.text = tutorialTitulo;
    public void TextoTutorial2(string tutorial) => textoTutorial.text = tutorial;
}