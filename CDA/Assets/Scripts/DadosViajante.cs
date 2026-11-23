using UnityEngine;

[CreateAssetMenu(menuName = "CDA/Novo Viajante")]
public class DadosViajante : ScriptableObject
{
    public string nomePersonagem;
    public Sprite spriteNormal;
    public Sprite spriteAnomalo;
    
    public Sprite spriteCartaoPronto;

    [Header("Dados do Documento (Cartão)")]
    public string origemCorreta;
    public string idade;

    [Header("Mentira (Caso seja Anomalia)")]
    public string origemMentirosa; 
    [TextArea(2, 3)]
    public string falaApresentacao;
}