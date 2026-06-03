using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// Modal del Simulador de Porciones. Aparece antes de Aceptar/Rechazar la primera vez
// que se ve cada tipo de alimento dificil. Muestra un dado, una barra de calorias diarias
// y un dialogo con typewriter del presentador (estilo tutorial).
public class SimuladorPorciones : MonoBehaviour, IPointerClickHandler
{
    public const float KCAL_DIARIAS = 2000f;

    [Header("Referencias UI principales")]
    [Tooltip("CanvasGroup del modal completo para hacer fade in/out")]
    public CanvasGroup canvasGroup;

    [Header("Dado")]
    public TMP_Text textoDado;
    public Button botonTirar;

    [Tooltip("Duracion total de la animacion del dado (parpadeo entre numeros)")]
    public float duracionTiradaDado = 1.2f;

    [Tooltip("Intervalo entre cada numero del parpadeo (al inicio rapido, al final lento)")]
    public float intervaloMin = 0.05f;
    public float intervaloMax = 0.2f;

    [Header("Barra de calorias")]
    [Tooltip("Image con type=Filled para la barra")]
    public Image barraFill;
    [Tooltip("Texto que muestra kcal consumidas vs limite")]
    public TMP_Text textoBarra;
    [Tooltip("Color de la barra al inicio (bajo)")]
    public Color colorBajo = new Color(0.4f, 0.9f, 0.4f);
    [Tooltip("Color medio")]
    public Color colorMedio = new Color(1f, 0.8f, 0.3f);
    [Tooltip("Color rojo (cerca o sobre el limite)")]
    public Color colorAlto = new Color(0.9f, 0.3f, 0.3f);
    [Tooltip("Duracion del llenado de la barra")]
    public float duracionLlenadoBarra = 0.8f;

    [Header("Cuadro de dialogo (estilo tutorial)")]
    public GameObject contenedorDialogo;
    public TMP_Text textoDialogo;
    [Tooltip("Segundos entre cada caracter del typewriter")]
    public float velocidadTypewriter = 0.03f;

    [Header("Sonidos")]
    public AudioClip sonidoTirada;
    public AudioClip sonidoTecleo;
    public float velocidadTecleo = 2f;
    [Range(0f, 1f)]
    public float volumenTecleo = 1f;

    [Header("Tiempos")]
    public float fadeInModal = 0.4f;
    public float fadeOutModal = 0.3f;
    [Tooltip("Espera entre que sale el numero y empieza a llenarse la barra")]
    public float esperaAntesBarra = 0.3f;
    [Tooltip("Espera entre que termina la barra y aparece el dialogo")]
    public float esperaAntesDialogo = 0.5f;

    private AlimentoDificilData alimentoActual;
    private int resultadoDado;
    private bool escribiendo = false;
    private string textoCompleto = "";
    private Coroutine corrTypewriter;
    private AudioSource sourceTecleo;
    private AudioSource sourceEfectos;
    private System.Action alTerminar;

    void Awake()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        gameObject.SetActive(false);
    }

    void AsegurarAudio()
    {
        if (sourceTecleo == null)
        {
            sourceTecleo = gameObject.AddComponent<AudioSource>();
            sourceTecleo.playOnAwake = false;
            sourceTecleo.loop = true;
            sourceTecleo.clip = sonidoTecleo;
            sourceTecleo.pitch = velocidadTecleo;
            sourceTecleo.volume = volumenTecleo;
        }
        if (sourceEfectos == null)
        {
            sourceEfectos = gameObject.AddComponent<AudioSource>();
            sourceEfectos.playOnAwake = false;
            sourceEfectos.loop = false;
        }
    }

    // Punto de entrada: el Nivel1Manager llama esto. callback se ejecuta cuando el modal se cierra.
    public void Mostrar(AlimentoDificilData alimento, System.Action callback)
    {
        AsegurarAudio();
        alimentoActual = alimento;
        alTerminar = callback;

        gameObject.SetActive(true);

        // Reset visual
        if (textoDado != null) textoDado.text = "?";
        if (barraFill != null) { barraFill.fillAmount = 0f; barraFill.color = colorBajo; }
        if (textoBarra != null) textoBarra.text = "";
        if (contenedorDialogo != null) contenedorDialogo.SetActive(false);
        if (textoDialogo != null) textoDialogo.text = "";
        if (botonTirar != null)
        {
            botonTirar.gameObject.SetActive(true);
            botonTirar.interactable = true;
            botonTirar.onClick.RemoveAllListeners();
            botonTirar.onClick.AddListener(TirarDado);
        }

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;
        float t = 0f;
        while (t < fadeInModal)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInModal);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void TirarDado()
    {
        if (botonTirar != null) botonTirar.interactable = false;
        StartCoroutine(AnimarTirada());
    }

    IEnumerator AnimarTirada()
    {
        if (sourceEfectos != null && sonidoTirada != null) sourceEfectos.PlayOneShot(sonidoTirada);

        // Parpadeo entre numeros, empezando rapido y desacelerando
        float t = 0f;
        while (t < duracionTiradaDado)
        {
            int n = Random.Range(1, 5);
            if (textoDado != null) textoDado.text = n.ToString();
            float intervalo = Mathf.Lerp(intervaloMin, intervaloMax, t / duracionTiradaDado);
            yield return new WaitForSeconds(intervalo);
            t += intervalo;
        }

        // Resultado final
        resultadoDado = Random.Range(1, 5); // 1, 2, 3 o 4
        if (textoDado != null) textoDado.text = resultadoDado.ToString();

        // Oculta el boton tirar
        if (botonTirar != null) botonTirar.gameObject.SetActive(false);

        yield return new WaitForSeconds(esperaAntesBarra);

        // Llenar la barra
        yield return StartCoroutine(LlenarBarra());

        yield return new WaitForSeconds(esperaAntesDialogo);

        // Mostrar el dialogo
        MostrarDialogo();
    }

    IEnumerator LlenarBarra()
    {
        float kcalTotal = alimentoActual.kcalPorPorcion * resultadoDado;
        float fraccion = Mathf.Clamp01(kcalTotal / KCAL_DIARIAS);

        float t = 0f;
        while (t < duracionLlenadoBarra)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duracionLlenadoBarra);
            float valor = Mathf.Lerp(0f, fraccion, p);
            if (barraFill != null)
            {
                barraFill.fillAmount = valor;
                barraFill.color = ColorPorValor(valor);
            }
            int kcalMostrada = Mathf.RoundToInt(Mathf.Lerp(0f, kcalTotal, p));
            if (textoBarra != null)
                textoBarra.text = kcalMostrada + " / " + (int)KCAL_DIARIAS + " kcal";
            yield return null;
        }

        if (barraFill != null)
        {
            barraFill.fillAmount = fraccion;
            barraFill.color = ColorPorValor(fraccion);
        }
        if (textoBarra != null)
            textoBarra.text = (int)kcalTotal + " / " + (int)KCAL_DIARIAS + " kcal";
    }

    Color ColorPorValor(float v)
    {
        // 0-40% verde, 40-75% amarillo, 75-100% rojo
        if (v < 0.4f) return colorBajo;
        if (v < 0.75f) return colorMedio;
        return colorAlto;
    }

    void MostrarDialogo()
    {
        if (contenedorDialogo != null) contenedorDialogo.SetActive(true);

        // Elige el mensaje segun el resultado
        string mensaje = "";
        if (resultadoDado == 1) mensaje = alimentoActual.mensajeUnaPorcion;
        else if (resultadoDado <= 3) mensaje = alimentoActual.mensajeVariasPorciones;
        else mensaje = alimentoActual.mensajeMuchasPorciones;

        if (string.IsNullOrEmpty(mensaje))
            mensaje = "Con " + resultadoDado + " porciones consumes " + (int)(alimentoActual.kcalPorPorcion * resultadoDado) + " kcal.";

        textoCompleto = mensaje;
        if (corrTypewriter != null) StopCoroutine(corrTypewriter);
        corrTypewriter = StartCoroutine(Typewriter());
    }

    IEnumerator Typewriter()
    {
        escribiendo = true;
        if (textoDialogo != null) textoDialogo.text = "";
        IniciarTecleo();

        foreach (char c in textoCompleto)
        {
            if (textoDialogo != null) textoDialogo.text += c;
            yield return new WaitForSeconds(velocidadTypewriter);
        }

        escribiendo = false;
        DetenerTecleo();
    }

    void IniciarTecleo()
    {
        if (sourceTecleo != null && sonidoTecleo != null && !sourceTecleo.isPlaying) sourceTecleo.Play();
    }

    void DetenerTecleo()
    {
        if (sourceTecleo != null && sourceTecleo.isPlaying) sourceTecleo.Stop();
    }

    // Click en cualquier parte del modal: completa texto, o cierra modal si ya termino
    public void OnPointerClick(PointerEventData eventData)
    {
        // Solo procesar clicks despues de que aparecio el dialogo
        if (contenedorDialogo == null || !contenedorDialogo.activeSelf) return;

        if (escribiendo)
        {
            if (corrTypewriter != null) StopCoroutine(corrTypewriter);
            if (textoDialogo != null) textoDialogo.text = textoCompleto;
            escribiendo = false;
            DetenerTecleo();
            return;
        }

        // Cerrar modal
        StartCoroutine(CerrarModal());
    }

    IEnumerator CerrarModal()
    {
        DetenerTecleo();
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            float t = 0f;
            while (t < fadeOutModal)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutModal);
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }
        gameObject.SetActive(false);
        if (alTerminar != null) alTerminar();
    }
}
