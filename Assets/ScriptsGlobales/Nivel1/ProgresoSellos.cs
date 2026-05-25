using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Maneja la barra de 6 sellos OK que se llenan con cada acierto.
// Cada sello aparece tras un delay, con animacion de escala (crece con rebote) + fade in.
// Usa escala en vez de posicion para no pelear con el Layout Group del contenedor.
public class ProgresoSellos : MonoBehaviour
{
    [Header("Slots de sellos (uno por cada sello posible)")]
    [Tooltip("Lista de Image en orden. Empiezan vacios y se rellenan al acertar.")]
    public Image[] slotsSellos;

    [Header("Sprite del sello OK")]
    public Sprite spriteSelloOK;

    [Header("Animacion")]
    [Tooltip("Retraso antes de que aparezca el sello (para sincronizar con el flash verde)")]
    public float delayAparicion = 0.4f;

    [Tooltip("Escala inicial del sello (0 = invisible, crece hasta 1)")]
    public float escalaInicial = 0f;

    [Tooltip("Escala maxima del rebote antes de asentarse en 1")]
    public float escalaRebote = 1.25f;

    [Tooltip("Duracion de la animacion de entrada")]
    public float duracionAnimacion = 0.4f;

    private int aciertosVisuales = 0;

    void Start()
    {
        if (slotsSellos != null)
        {
            for (int i = 0; i < slotsSellos.Length; i++)
            {
                var s = slotsSellos[i];
                if (s != null)
                {
                    s.sprite = spriteSelloOK;
                    var c = s.color;
                    c.a = 0f;
                    s.color = c;
                    s.rectTransform.localScale = Vector3.one * escalaInicial;
                }
            }
        }
        aciertosVisuales = 0;
    }

    public void AgregarSello()
    {
        if (slotsSellos == null || aciertosVisuales >= slotsSellos.Length) return;

        int indice = aciertosVisuales;
        aciertosVisuales++;

        var slot = slotsSellos[indice];
        if (slot != null)
            StartCoroutine(AnimarSello(slot));
    }

    IEnumerator AnimarSello(Image slot)
    {
        yield return new WaitForSeconds(delayAparicion);

        var rt = slot.rectTransform;

        float t = 0f;
        while (t < duracionAnimacion)
        {
            t += Time.deltaTime;
            float p = t / duracionAnimacion;

            // Curva con rebote: crece pasando de escalaInicial -> escalaRebote -> 1
            float escala;
            if (p < 0.6f)
            {
                float p2 = p / 0.6f;
                escala = Mathf.Lerp(escalaInicial, escalaRebote, Mathf.SmoothStep(0f, 1f, p2));
            }
            else
            {
                float p2 = (p - 0.6f) / 0.4f;
                escala = Mathf.Lerp(escalaRebote, 1f, Mathf.SmoothStep(0f, 1f, p2));
            }

            rt.localScale = Vector3.one * escala;

            var c = slot.color;
            c.a = Mathf.Clamp01(p * 2f);
            slot.color = c;

            yield return null;
        }

        rt.localScale = Vector3.one;
        var cf = slot.color;
        cf.a = 1f;
        slot.color = cf;
    }

    public void Resetear()
    {
        aciertosVisuales = 0;
        if (slotsSellos == null) return;
        foreach (var s in slotsSellos)
        {
            if (s != null)
            {
                var c = s.color;
                c.a = 0f;
                s.color = c;
                s.rectTransform.localScale = Vector3.one * escalaInicial;
            }
        }
    }
}
