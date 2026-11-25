using System.Collections.Generic;
using UnityEngine;

// Estrutura que representa cada linha de diálogo com seu texto e áudio correspondente
[System.Serializable]
public class Fala
{
    public string texto;
    public AudioClip audio;
}

// Cria a opção no menu da Unity para montar novos arquivos de diálogo no projeto
[CreateAssetMenu(menuName = "CDA/Novo Dialogo")]
public class Dialogo : ScriptableObject
{
    [Header("Lista de Falas da Cutscene")]
    public List<Fala> falas = new List<Fala>();
}