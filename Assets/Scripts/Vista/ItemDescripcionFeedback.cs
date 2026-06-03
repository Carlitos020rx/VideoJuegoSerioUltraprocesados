using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Componente del prefab de cada item del panel de feedback
public class ItemDescripcionFeedback : MonoBehaviour
{
    public TMP_Text textoNombre;
    public TMP_Text textoDescripcion;

    public void Configurar(string nombre, string descripcion, bool esPeligroso)
    {
        if (textoNombre != null)
        {
            textoNombre.text  = nombre;
            textoNombre.color = esPeligroso
                ? new Color(0.1f, 0.6f, 0.1f)   // verde
                : new Color(0.8f, 0.1f, 0.1f);  // rojo
        }

        if (textoDescripcion != null)
            textoDescripcion.text = descripcion;
    }
}
