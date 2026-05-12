using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Item del prefab de cada fila de info nutricional (nombre - valor)
public class ItemInfoNutricional : MonoBehaviour
{
    public TMP_Text textoNombre;
    public TMP_Text textoValor;

    public void Configurar(string nombre, string valor)
    {
        if (textoNombre != null) textoNombre.text = nombre;
        if (textoValor  != null) textoValor.text  = valor;
    }
}
