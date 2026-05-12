using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Nivel1Config", menuName = "UltraProcessed/Nivel1 Config", order = 3)]
public class Nivel1Config : ScriptableObject
{
    [Header("Reglas del nivel")]
    [Tooltip("Cuantos aciertos se necesitan para pasar el nivel")]
    public int aciertosParaGanar = 6;

    [Tooltip("En que numero de acierto cambia a dificultad alta")]
    public int aciertoCambioDificultad = 3;

    [Tooltip("Maximo de ingredientes que el jugador puede marcar en el checklist")]
    public int maxIngredientesMarcados = 6;

    [Header("Dificultad facil — pool de alimentos")]
    public List<AlimentoData> alimentosDisponibles = new List<AlimentoData>();

    [Header("Dificultad alta — pool de alimentos")]
    public List<AlimentoDificilData> alimentosDificiles = new List<AlimentoDificilData>();

    [Header("Dificultad alta — sellos disponibles (lista fija)")]
    [Tooltip("Sellos que el jugador puede marcar en dificultad alta. Son los mismos siempre.")]
    public List<SelloNutricional> sellosDisponibles = new List<SelloNutricional>();
}
