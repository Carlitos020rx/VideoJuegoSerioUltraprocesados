using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DatoNutricional
{
    [Tooltip("Ej: Calorias (kcal), Grasa total, Sodio")]
    public string nombre;

    [Tooltip("Ej: 400, 28g, 350mg")]
    public string valor;
}

[CreateAssetMenu(fileName = "AlimentoDificil_", menuName = "UltraProcessed/Alimento Dificil", order = 6)]
public class AlimentoDificilData : ScriptableObject
{
    [Header("Info basica")]
    public string nombre;
    public string categoria;
    public Sprite sprite;

    [Header("Ingredientes")]
    [Tooltip("Lista completa de ingredientes que se muestran en el expediente")]
    public List<string> ingredientes = new List<string>();

    [Tooltip("Ingredientes peligrosos con su descripcion educativa")]
    public List<IngredientePeligroso> ingredientesPeligrosos = new List<IngredientePeligroso>();

    [Header("Decision correcta")]
    public bool esUltraprocesado;

    [Header("Informacion nutricional")]
    [Tooltip("Pares clave-valor que se muestran en el panel nutricional")]
    public List<DatoNutricional> infoNutricional = new List<DatoNutricional>();

    [Header("Sellos correctos")]
    [Tooltip("Sellos que este alimento DEBE llevar (los que el jugador debe marcar)")]
    public List<SelloNutricional> sellosCorrectos = new List<SelloNutricional>();

    [Header("Simulador de Porciones")]
    [Tooltip("Calorias por una porcion (en kcal)")]
    public float kcalPorPorcion;

    [Tooltip("Sodio por una porcion (en mg)")]
    public float sodioPorPorcion;

    [Tooltip("Azucares por una porcion (en g)")]
    public float azucaresPorPorcion;

    [Tooltip("Grasa saturada por una porcion (en g)")]
    public float grasaSaturadaPorPorcion;

    public enum NutrienteClave { Calorias, Sodio, Azucares, GrasasSaturadas }

    [Tooltip("Que nutriente se destaca")]
    public NutrienteClave nutrienteClave = NutrienteClave.Calorias;

    [TextArea(2, 4)]
    [Tooltip("Mensaje cuando sale 1 en el dado")]
    public string mensajeUnaPorcion;

    [TextArea(2, 4)]
    [Tooltip("Mensaje cuando salen 2 o 3 porciones")]
    public string mensajeVariasPorciones;

    [TextArea(2, 4)]
    [Tooltip("Mensaje cuando salen 4 porciones")]
    public string mensajeMuchasPorciones;

    public List<string> NombresPeligrosos()
    {
        var lista = new List<string>();
        foreach (var p in ingredientesPeligrosos) lista.Add(p.nombre);
        return lista;
    }

    public string GetDescripcionPeligroso(string nombre)
    {
        foreach (var p in ingredientesPeligrosos)
            if (p.nombre == nombre) return p.descripcion;
        return null;
    }
}
