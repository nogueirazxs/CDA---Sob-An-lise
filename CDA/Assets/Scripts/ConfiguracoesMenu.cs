using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ConfiguracoesMenu : MonoBehaviour
{
    public Image[] cursores;
    public Slider sliderVolume;

    public GameObject menuDeOpcoes;

    InputAction acaoEsc, acaoToque, acaoPosicaoToque;

    Vector2 inicioToque;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        menuDeOpcoes.SetActive(false);
        int salvo = PlayerPrefs.GetInt("CursorEscolhido", 0);
        AplicarCursorPorIndice(salvo);
        sliderVolume.value = AudioListener.volume;
        acaoEsc = InputSystem.actions.FindAction("AbrirEFechar");
        acaoToque = InputSystem.actions.FindAction("Toque");
        acaoPosicaoToque = InputSystem.actions.FindAction("PosicaoToque");
    }

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

    void Update()
    {
        if (acaoEsc.WasPressedThisFrame())
        {
            menuDeOpcoes.SetActive(!menuDeOpcoes.activeSelf);
        }
        if (acaoToque.WasPressedThisFrame())
        {
            inicioToque = acaoPosicaoToque.ReadValue<Vector2>();
        }
        if (acaoToque.WasReleasedThisFrame())
        {
            Vector2 fimToque = acaoPosicaoToque.ReadValue<Vector2>();
            float distanciaY = fimToque.y - inicioToque.y;
            if (distanciaY < -120f)
            menuDeOpcoes.SetActive(true);
            else if (distanciaY > 120f)
            menuDeOpcoes.SetActive(false);
        }
    }
}
