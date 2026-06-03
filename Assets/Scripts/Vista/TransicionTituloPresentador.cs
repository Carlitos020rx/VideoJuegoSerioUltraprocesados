using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Vista de la transicion de titulo entre segmentos del presentador.
// El titulo cae desde arriba, se queda visible, y se va deslizando hacia la izquierda.
// El controlador llama Mostrar(sprite, callback) y se ejecuta callback al terminar.
public class TransicionTituloPresentador : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Panel de fondo (suele ser blanco) que aparece detras del titulo")]
    public Image panelFondo;

    [Tooltip("Image del titulo (PNG)")]
    public Image titulo;

    [Header("Tiempos")]
    [Tooltip("Duracion de la caida del titulo desde arriba")]
    public float duracionCaida = 0.5f;

    [Tooltip("Cuanto se queda el titulo visible antes de salir (segundos)")]
    public float duracionEstancia = 2f;

    [Tooltip("Duracion del deslizamiento hacia la izquierda")]
    public float duracionSalida = 0.5f;

    [Tooltip("Distancia (px) desde la que cae el titulo")]
    public float distanciaCaida = 600f;

    [Tooltip("Distancia (px) hacia la izquierda al salir")]
    public float distanciaSalida = 2200f;

    [Header("Sonido")]
    public AudioClip sonidoEntrada;
    [Range(0f, 1f)]
    public float volumen = 0.9f;

    private AudioSource sourceAudio;

    void Awake()
    {
        sourceAudio = gameObject.AddComponent<AudioSource>();
        sourceAudio.playOnAwake = false;

        if (panelFondo != null) panelFondo.gameObject.SetActive(false);
        if (titulo != null) titulo.gameObject.SetActive(false);
    }

    // Punto de entrada: Mostrar el titulo + callback al terminar.
    public void Mostrar(Sprite spriteTitulo, Action alTerminar)
    {
        StartCoroutine(SecuenciaTitulo(spriteTitulo, alTerminar));
    }

IEnumerator SecuenciaTitulo(Sprite spriteTitulo, Action alTerminar) { if (panelFondo != null) panelFondo.gameObject.SetActive(true); if (titulo != null) { titulo.sprite = spriteTitulo; titulo.preserveAspect = true; titulo.gameObject.SetActive(true); } var rt = titulo != null ? titulo.transform as RectTransform : null; var rtFondo = panelFondo != null ? panelFondo.transform as RectTransform : null; Vector2 destinoTitulo = rt != null ? rt.anchoredPosition : Vector2.zero; Vector2 destinoFondo = rtFondo != null ? rtFondo.anchoredPosition : Vector2.zero; Vector2 inicioTitulo = destinoTitulo + Vector2.up * distanciaCaida; Vector2 inicioFondo = destinoFondo + Vector2.up * distanciaCaida; if (rt != null) rt.anchoredPosition = inicioTitulo; if (rtFondo != null) rtFondo.anchoredPosition = inicioFondo; if (sourceAudio != null && sonidoEntrada != null) sourceAudio.PlayOneShot(sonidoEntrada, volumen); float t = 0f; while (t < duracionCaida) { t += Time.deltaTime; float p = Mathf.SmoothStep(0f, 1f, t / duracionCaida); if (rt != null) rt.anchoredPosition = Vector2.Lerp(inicioTitulo, destinoTitulo, p); if (rtFondo != null) rtFondo.anchoredPosition = Vector2.Lerp(inicioFondo, destinoFondo, p); yield return null; } if (rt != null) rt.anchoredPosition = destinoTitulo; if (rtFondo != null) rtFondo.anchoredPosition = destinoFondo; yield return new WaitForSeconds(duracionEstancia); Vector2 destinoSalidaTitulo = destinoTitulo + Vector2.left * distanciaSalida; Vector2 destinoSalidaFondo = destinoFondo + Vector2.left * distanciaSalida; t = 0f; while (t < duracionSalida) { t += Time.deltaTime; float p = Mathf.SmoothStep(0f, 1f, t / duracionSalida); if (rt != null) rt.anchoredPosition = Vector2.Lerp(destinoTitulo, destinoSalidaTitulo, p); if (rtFondo != null) rtFondo.anchoredPosition = Vector2.Lerp(destinoFondo, destinoSalidaFondo, p); yield return null; } if (titulo != null) { titulo.gameObject.SetActive(false); if (rt != null) rt.anchoredPosition = destinoTitulo; } if (panelFondo != null) { panelFondo.gameObject.SetActive(false); if (rtFondo != null) rtFondo.anchoredPosition = destinoFondo; } if (alTerminar != null) alTerminar(); }
}
