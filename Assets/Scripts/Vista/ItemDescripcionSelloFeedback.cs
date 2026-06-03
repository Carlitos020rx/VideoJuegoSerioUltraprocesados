using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Item del feedback de sello (nombre, descripcion, color segun acierto)
public class ItemDescripcionSelloFeedback : MonoBehaviour
{
    public TMP_Text textoNombre;
    public TMP_Text textoDescripcion;
    public Image iconoSello;

    public void Configurar(SelloNutricional sello, string descripcion, bool esCorrecto)
    {
        if (textoNombre != null)
        {
            textoNombre.text  = sello.nombreSello;
            textoNombre.color = esCorrecto
                ? new Color(0.1f, 0.6f, 0.1f)
                : new Color(0.8f, 0.1f, 0.1f);
        }

        if (textoDescripcion != null)
            textoDescripcion.text = descripcion;

        if (iconoSello != null && sello.sprite != null)
            iconoSello.sprite = sello.sprite;
    }
}
