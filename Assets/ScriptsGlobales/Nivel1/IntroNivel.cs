using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Intro de la escena: pantalla negra que se desvanece revelando el fondo + titulo,
// suena una voz que dice el titulo, el cuadro de tutorial entra deslizandose desde abajo
// y solo entonces arranca el typewriter.
// Cadena: IntroNivel -> TutorialManager -> Nivel1Manager
public class IntroNivel : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Image negra a pantalla completa (el velo que se quita al inicio)")]
    public Image panelNegro;

    [Tooltip("Image del titulo/logo del juego")]
    public Image titulo;

    [Tooltip("Imagen de fondo del titulo, se desactiva al empezar el tutorial")]
    public Image tituloFondo;

    [Header("Audio del titulo")]
    [Tooltip("Audio que dice el titulo del juego, suena al aparecer el titulo")]
    public AudioClip vozTitulo;
    [Range(0f, 1f)]
    public float volumenVozTitulo = 1f;

    [Header("Tutorial")]
    [Tooltip("Contenedor del tutorial que se activa al terminar la intro")]
    public GameObject contenedorTutorial;

    [Tooltip("TutorialManager que arranca despues de la intro")]
    public TutorialManager tutorialManager;

    [Header("Tiempos (segundos)")]
    [Tooltip("Negro total al inicio antes de empezar a desvanecerse")]
    public float esperaInicial = 0.5f;

    [Tooltip("Duracion del fade out del negro (revela fondo + titulo)")]
    public float fadeOutNegro = 1.5f;

    [Tooltip("Cuanto se queda el titulo visible")]
    public float duracionTitulo = 2f;

    [Tooltip("Distancia (px) desde abajo de la que entra el tutorial")]
    public float distanciaEntradaTutorial = 600f;

    [Tooltip("Duracion de la entrada deslizante del tutorial")]
    public float duracionEntradaTutorial = 0.7f;

    private AudioSource sourceVoz;

    void Start()
    {
        sourceVoz = gameObject.AddComponent<AudioSource>();
        sourceVoz.playOnAwake = false;
        sourceVoz.loop = false;

        if (contenedorTutorial != null) contenedorTutorial.SetActive(false);
        if (panelNegro != null) SetAlpha(panelNegro, 1f);
        if (titulo != null) SetAlpha(titulo, 1f);

        StartCoroutine(SecuenciaIntro());
    }

    IEnumerator SecuenciaIntro()
    {
        yield return new WaitForSeconds(esperaInicial);

        // Fade out del negro -> revela fondo + titulo
        yield return StartCoroutine(FadeImagen(panelNegro, 1f, 0f, fadeOutNegro));
        if (panelNegro != null) panelNegro.gameObject.SetActive(false);

        // Suena la voz del titulo
        if (sourceVoz != null && vozTitulo != null)
        {
            sourceVoz.clip = vozTitulo;
            sourceVoz.volume = volumenVozTitulo;
            sourceVoz.Play();
        }

        // El titulo se queda visible un rato
        yield return new WaitForSeconds(duracionTitulo);

        // El tutorial entra deslizandose desde abajo
        if (contenedorTutorial != null)
        {
            contenedorTutorial.SetActive(true);
            var rt = contenedorTutorial.transform as RectTransform;
            if (rt != null)
            {
                Vector2 destino = rt.anchoredPosition;
                Vector2 inicio = destino + Vector2.down * distanciaEntradaTutorial;
                rt.anchoredPosition = inicio;

                if (AudioManager.Instance != null) AudioManager.Instance.SonarEntradaPanel();

                float t = 0f;
                while (t < duracionEntradaTutorial)
                {
                    t += Time.deltaTime;
                    float p = Mathf.SmoothStep(0f, 1f, t / duracionEntradaTutorial);
                    rt.anchoredPosition = Vector2.Lerp(inicio, destino, p);
                    yield return null;
                }
                rt.anchoredPosition = destino;
            }
        }

        // Ocultar titulos (el tutorial ya esta encima)
        if (titulo != null) titulo.gameObject.SetActive(false);
        if (tituloFondo != null) tituloFondo.gameObject.SetActive(false);

        // Ahora si arranca el typewriter, ya en posicion
        if (tutorialManager != null) tutorialManager.IniciarTutorial();
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
