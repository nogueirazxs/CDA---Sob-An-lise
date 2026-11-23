using UnityEngine;

public class Gerenciador : MonoBehaviour
{
    public Dialogo dialogoInicio;
    public TutorialGuiado tutorial;
    public bool iniciou = false;

    private bool dialogoComecou = false;

    void Start()
    {
        if (TelaPretaDialogo.instancia != null)
        {
            TelaPretaDialogo.instancia.acabou = false;
            TelaPretaDialogo.instancia.ComecarDialogo(dialogoInicio);
            dialogoComecou = true;
        }
    }

    void Update()
    {
        if (dialogoComecou && TelaPretaDialogo.instancia != null && TelaPretaDialogo.instancia.acabou && !iniciou)
        {
            iniciou = true;
            tutorial.IniciarTutorial();
        }
    }
}