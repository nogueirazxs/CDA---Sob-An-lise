using UnityEngine;

// Cria a opção no menu da Unity para cadastrar cada viajante e suas informações de triagem
[CreateAssetMenu(menuName = "CDA/Novo Viajante")]
public class DadosViajante : ScriptableObject
{
    [Header("Identidade e Aparência")]
    public string nomePersonagem;
    public Sprite spriteNormal;
    public Sprite spriteAnomalo;
    public Sprite spriteCartaoPronto;

    [Header("Dados do Documento (Cartão)")]
    public string origemCorreta;
    public string idade;

    [Header("Mentira (Caso seja Anomalia)")]
    public string origemMentirosa;

    [Header("Diálogo na Janela")]
    [TextArea(2, 3)]
    public string falaApresentacao;
}