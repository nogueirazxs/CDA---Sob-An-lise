using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Fala
{
    public string texto; // Guarda o texto de cada cutscene.
    public AudioClip audio; // Guarda o áudio respectivo de cada fala.
}

[CreateAssetMenu(menuName = "CDA/Novo Dialogo")] // Cria um novo asset no menu em Projects.
public class Dialogo : ScriptableObject
{
    public List<Fala> falas = new List<Fala>(); // Adiciona os itens de texto e audio na lista.
}