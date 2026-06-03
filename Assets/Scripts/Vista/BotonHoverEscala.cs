using UnityEngine;
using UnityEngine.EventSystems;

// Agregalo a cualquier boton (o elemento UI) para que crezca al pasar el mouse por encima (hover).
// Al quitar el mouse vuelve a su tamaño. Tambien vuelve si se presiona.
public class BotonHoverEscala : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("Escala al hacer hover (1.1 = 10% mas grande)")]
    public float escalaHover = 1.1f;

    [Tooltip("Velocidad de la animacion de escala")]
    public float velocidadEscala = 12f;

    [Tooltip("Si esta vacio, usa el RectTransform de este GameObject")]
    public RectTransform objetivo;

    private Vector3 escalaOriginal;
    private Vector3 escalaObjetivo;

    void Awake()
    {
        if (objetivo == null) objetivo = transform as RectTransform;
        escalaOriginal = objetivo != null ? objetivo.localScale : Vector3.one;
        escalaObjetivo = escalaOriginal;
    }

    void OnEnable()
    {
        // Por si el boton se reactiva, asegurar que vuelva al tamaño base
        escalaObjetivo = escalaOriginal;
        if (objetivo != null) objetivo.localScale = escalaOriginal;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        escalaObjetivo = escalaOriginal * escalaHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        escalaObjetivo = escalaOriginal;
    }

    void Update()
    {
        if (objetivo == null) return;
        objetivo.localScale = Vector3.Lerp(objetivo.localScale, escalaObjetivo, Time.deltaTime * velocidadEscala);
    }
}
