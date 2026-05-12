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
