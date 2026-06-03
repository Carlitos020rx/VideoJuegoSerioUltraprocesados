using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Transicion al pasar a dificultad alta: pantalla negra + titulo "Dificultad 2",
// luego un tutorial (mismo sistema) que entra deslizandose mientras el negro se quita.
// Al terminar el tutorial, invoca el callback para que el juego continue en dificultad alta.
public class TransicionDificultad : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Image negra a pantalla completa")]
    public Image panelNegro;

    [Tooltip("Image del titulo 'Dificultad 2'")]
    public Image titulo;

    [Tooltip("Contenedor del tutorial de dificultad 2")]
    public GameObject contenedorTutorial;

    [Tooltip("TutorialManager del tutorial de dificultad 2")]
    public TutorialManager tutorialManager;

    [Header("Audio del titulo")]
    public AudioClip vozTitulo;
    [Range(0f, 1f)]
    public float volumenVozTitulo = 1f;
    [Tooltip("Retraso antes de que suene la voz del titulo")]
    public float delayVozTitulo = 0f;

    [Header("Tiempos (segundos)")]
    [Tooltip("Duracion del fade in del negro (oscurecer)")]
    public float fadeInNegro = 0.8f;

    [Tooltip("Cuanto se queda el titulo en negro antes de continuar")]
    public float duracionTitulo = 2f;

    [Tooltip("Duracion del fade out del negro al entrar el tutorial")]
    public float fadeOutNegro = 1f;

    [Tooltip("Distancia (px) desde abajo de la que entra el tutorial")]
    public float distanciaEntradaTutorial = 600f;

    [Tooltip("Duracion de la entrada deslizante del tutorial")]
    public float duracionEntradaTutorial = 0.5f;

    private AudioSource sourceVoz;
    private System.Action alTerminar;

void Awake() { if (panelNegro != null) { SetAlpha(panelNegro, 0f); panelNegro.gameObject.SetActive(false); } if (titulo != null) { SetAlpha(titulo, 0f); titulo.gameObject.SetActive(false); } if (contenedorTutorial != null) contenedorTutorial.SetActive(false); }

    void AsegurarAudio()
    {
        if (sourceVoz == null)
        {
            sourceVoz = gameObject.AddComponent<AudioSource>();
            sourceVoz.playOnAwake = false;
            sourceVoz.loop = false;
        }
    }

    // Llamado por el Nivel1Manager. callback se ejecuta cuando termina el tutorial de dificultad 2.
    public void Iniciar(System.Action callback)
    {
        AsegurarAudio();
        alTerminar = callback;
        StartCoroutine(Secuencia());
    }

IEnumerator Secuencia() { if (panelNegro != null) { panelNegro.gameObject.SetActive(true); yield return StartCoroutine(FadeImagen(panelNegro, 0f, 1f, fadeInNegro)); } if (titulo != null) { titulo.gameObject.SetActive(true); SetAlpha(titulo, 1f); } if (sourceVoz != null && vozTitulo != null) StartCoroutine(ReproducirVoz()); yield return new WaitForSeconds(duracionTitulo); if (contenedorTutorial != null) { var rt = contenedorTutorial.transform as RectTransform; Vector2 destino = Vector2.zero; if (rt != null) destino = rt.anchoredPosition; var cg = contenedorTutorial.GetComponent<CanvasGroup>(); if (cg == null) cg = contenedorTutorial.AddComponent<CanvasGroup>(); cg.alpha = 0f; if (rt != null) rt.anchoredPosition = destino + Vector2.down * distanciaEntradaTutorial; contenedorTutorial.SetActive(true); yield return null; yield return null; UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt); if (tutorialManager != null) tutorialManager.IniciarTutorial(); cg.alpha = 1f; if (rt != null) { Vector2 inicio = destino + Vector2.down * distanciaEntradaTutorial; if (AudioManager.Instance != null) AudioManager.Instance.SonarEntradaPanel(); float t = 0f; while (t < duracionEntradaTutorial) { t += Time.deltaTime; float p = Mathf.SmoothStep(0f, 1f, t / duracionEntradaTutorial); rt.anchoredPosition = Vector2.Lerp(inicio, destino, p); yield return null; } rt.anchoredPosition = destino; } } if (titulo != null) titulo.gameObject.SetActive(false); yield return StartCoroutine(FadeImagen(panelNegro, 1f, 0f, fadeOutNegro)); if (panelNegro != null) panelNegro.gameObject.SetActive(false); }

    IEnumerator ReproducirVoz()
    {
        if (delayVozTitulo > 0f) yield return new WaitForSeconds(delayVozTitulo);
        if (sourceVoz != null && vozTitulo != null)
        {
            sourceVoz.clip = vozTitulo;
            sourceVoz.volume = volumenVozTitulo;
            sourceVoz.Play();
        }
    }

    // Llamado por el TutorialManager de dificultad 2 al terminar.
    public void Finalizar()
    {
        if (alTerminar != null) alTerminar();
    }

    IEnumerator FadeImagen(Image img, float desde, float hasta, float duracion)
    {
        if (img == null) yield break;
        float t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(desde, hasta, Mathf.SmoothStep(0f, 1f, t / duracion));
            SetAlpha(img, a);
            yield return null;
        }
        SetAlpha(img, hasta);
    }

    void SetAlpha(Image img, float a)
    {
        if (img == null) return;
        var c = img.color;
        c.a = a;
        img.color = c;
    }
}
