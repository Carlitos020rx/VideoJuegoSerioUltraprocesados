using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class IngredientePeligroso
{
    public string nombre;
    [TextArea(3, 6)]
    public string descripcion;
}

[CreateAssetMenu(fileName = "Alimento_", menuName = "UltraProcessed/Alimento", order = 2)]
public class AlimentoData : ScriptableObject
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
    [Tooltip("Si es ultraprocesado, la respuesta correcta es RECHAZAR. Si no, es ACEPTAR")]
    public bool esUltraprocesado;

    // Helper: solo nombres de los peligrosos (para validacion)
    public List<string> NombresPeligrosos()
    {
        var lista = new List<string>();
        foreach (var p in ingredientesPeligrosos)
            lista.Add(p.nombre);
        return lista;
    }

    // Helper: busca la descripcion de un ingrediente peligroso
    public string GetDescripcionPeligroso(string nombre)
    {
        foreach (var p in ingredientesPeligrosos)
            if (p.nombre == nombre) return p.descripcion;
        return null;
    }
}
