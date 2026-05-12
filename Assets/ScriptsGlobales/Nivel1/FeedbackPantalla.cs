using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Flash rojo (fallo) o verde (acierto) en pantalla completa
public class FeedbackPantalla : MonoBehaviour
{
    [Header("Image fullscreen para el flash")]
    public Image panelFlash;

    [Header("Configuracion")]
    public Color colorAcierto = new Color(0.1f, 0.7f, 0.1f, 0.5f);
    public Color colorFallo   = new Color(0.8f, 0.1f, 0.1f, 0.5f);
    public float duracionFlash = 0.6f;

    void Start()
    {
        if (panelFlash != null)
        {
            var c = panelFlash.color;
            c.a = 0f;
            panelFlash.color = c;
            panelFlash.raycastTarget = false; // que no bloquee clicks
        }
    }

    public void MostrarFlash(bool verde)
    {
        if (panelFlash == null) return;
        StopAllCoroutines();
        StartCoroutine(Flash(verde ? colorAcierto : colorFallo));
    }

    IEnumerator Flash(Color colorObjetivo)
    {
        // Fade in
        float t = 0f;
        float mitad = duracionFlash * 0.3f;
        while (t < mitad)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(0f, colorObjetivo.a, t / mitad);
            panelFlash.color = new Color(colorObjetivo.r, colorObjetivo.g, colorObjetivo.b, a);
            yield return null;
        }

        // Fade out
        t = 0f;
        float salida = duracionFlash * 0.7f;
        while (t < salida)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(colorObjetivo.a, 0f, t / salida);
            panelFlash.color = new Color(colorObjetivo.r, colorObjetivo.g, colorObjetivo.b, a);
            yield return null;
        }

        panelFlash.color = new Color(colorObjetivo.r, colorObjetivo.g, colorObjetivo.b, 0f);
    }
}
