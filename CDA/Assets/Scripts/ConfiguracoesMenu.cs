using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ConfiguracoesMenu : MonoBehaviour
{
    [Header("Elementos de Interface")]
    public Image[] cursores;
    public Slider sliderVolume;
    public GameObject menuDeOpcoes;

    private InputAction acaoEsc;
    private InputAction acaoToque;
    private InputAction acaoPosicaoToque;

    private Vector2 inicioToque;
    private bool toqueValido = false;

    void Awake()
    {
        // Mantém o painel de configurações persistente ao trocar de cena
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        menuDeOpcoes.SetActive(false);

        // Recupera a preferência de cursor salva anteriormente e sincroniza o áudio
        int salvo = PlayerPrefs.GetInt("CursorEscolhido", 0);
        AplicarCursorPorIndice(salvo);
        sliderVolume.value = AudioListener.volume;

        acaoEsc = InputSystem.actions.FindAction("AbrirEFechar");
        acaoToque = InputSystem.actions.FindAction("Toque");
        acaoPosicaoToque = InputSystem.actions.FindAction("PosicaoToque");
    }

    void Update()
    {
        // Bloqueia o gesto de deslizar enquanto o jogador estiver conectando os fios
        if (GerenciadorFios.estaArrastandoFio)
        {
            toqueValido = false;
            return;
        }

        if (acaoEsc.WasPressedThisFrame())
        {
            menuDeOpcoes.SetActive(!menuDeOpcoes.activeSelf);
        }

        if (acaoToque.WasPressedThisFrame())
        {
            inicioToque = acaoPosicaoToque.ReadValue<Vector2>();
            toqueValido = true;
        }

        // Detecta deslize vertical na tela para abrir ou recolher o menu no celular
        if (acaoToque.WasReleasedThisFrame() && toqueValido)
        {
            toqueValido = false;
            Vector2 fimToque = acaoPosicaoToque.ReadValue<Vector2>();
            float distanciaY = fimToque.y - inicioToque.y;

            if (distanciaY < -120f)
            {
                menuDeOpcoes.SetActive(true);
            }
            else if (distanciaY > 120f)
            {
                menuDeOpcoes.SetActive(false);
            }
        }
    }

    // Aplica o cursor selecionado e salva a escolha no armazenamento local
    public void AplicarCursorPorIndice(int indice)
    {
        Cursor.SetCursor(cursores[indice].sprite.texture, Vector2.zero, CursorMode.Auto);
        PlayerPrefs.SetInt("CursorEscolhido", indice);
        PlayerPrefs.Save();
    }

    public void AlterarVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}