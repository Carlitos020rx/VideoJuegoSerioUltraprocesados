using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Mecanica de Perdida de Memoria: aleatoriamente, los paneles Expediente y Alimento
// parpadean y se vuelven transparentes. Aparecen 4 simbolos en cada esquina (8 total).
// El jugador debe darles click a todos para recuperar la memoria.
// Mientras dura, bloquea las decisiones (Nivel1Manager.EsperandoDecision = false).
public class PerdidaMemoria : MonoBehaviour
{
    [Header("Paneles que se ven afectados")]
    [Tooltip("Panel del expediente (con la lista de ingredientes)")]
    public CanvasGroup panelExpediente;
    [Tooltip("Panel del alimento (con la imagen del producto)")]
    public CanvasGroup panelAlimento;

    [Header("Simbolos de recuperacion")]
    [Tooltip("Sprite del simbolo que aparece en las esquinas")]
    public Sprite spriteSimbolo;
    [Tooltip("Tamaño del simbolo en px")]
    public float tamanoSimbolo = 60f;
    [Tooltip("Cuanto se separa el simbolo del borde del panel (px)")]
    public float offsetEsquina = 10f;

    [Header("Animacion de parpadeo + desvanecimiento")]
    [Tooltip("Cuanto dura el parpadeo antes de desaparecer (segundos)")]
    public float duracionParpadeo = 2f;
    [Tooltip("Cuantos parpadeos durante esa duracion")]
    public int cantidadParpadeos = 2;

    [Header("Animacion al hacer click en simbolo")]
    public float escalaPicoClick = 1.5f;
    public float duracionAnimacionClick = 0.4f;

    [Header("Aparicion de simbolos (estilo osu)")]
    [Tooltip("Escala con la que aparecen los simbolos antes de asentarse en 1")]
    public float escalaInicialSimbolo = 2.5f;
    [Tooltip("Duracion de la animacion de aparicion de cada simbolo")]
    public float duracionAparicionSimbolo = 0.35f;
    [Tooltip("Retraso entre la aparicion de cada simbolo (efecto secuencial)")]
    public float delayEntreSimbolos = 0.15f;

    [Header("Probabilidad de activacion")]
    [Tooltip("Cada cuantos segundos se intenta disparar")]
    public float intervaloEvaluacion = 5f;
    [Tooltip("Probabilidad (0-1) de que se dispare en cada evaluacion")]
    [Range(0f, 1f)]
    public float probabilidadDisparo = 0.35f;

    [Tooltip("Segundos antes del disparo forzado en el primer alimento")]
    public float delayDisparoForzado = 7f;

    [Header("Mensaje emergente")]
    [Tooltip("Panel con el mensaje (debe tener un TMP_Text hijo)")]
    public RectTransform panelMensaje;
    public TMP_Text textoMensaje;
    [TextArea(2, 4)]
    public string mensaje = "¡Pérdida de memoria! Los ultraprocesados afectan tu capacidad de recordar.";
    public float duracionMensaje = 3f;
    public float duracionEntradaMensaje = 0.4f;
    public float escalaInicialMensaje = 1.5f;

    [Header("Sonidos")]
    public AudioClip sonidoMensaje;
    public AudioClip sonidoSimboloRecuperado;
    [Range(0f, 1f)]
    public float volumen = 0.8f;

    [Header("Referencias")]
    public Nivel1Manager nivel1Manager;

    private bool activo = false;
    private bool yaSeMostroMensaje = false;
    private bool enModoPerdida = false;
    private int simbolosRecuperados = 0;
    private int simbolosTotal = 0;
    private List<GameObject> simbolosEnPantalla = new List<GameObject>();
    private AudioSource sourceAudio;
    private Vector2 escalaOriginalMensaje;
    private Vector3 posOriginalMensaje;

    void Awake()
    {
        sourceAudio = gameObject.AddComponent<AudioSource>();
        sourceAudio.playOnAwake = false;
        sourceAudio.loop = false;

        if (panelMensaje != null)
        {
            escalaOriginalMensaje = panelMensaje.localScale;
            panelMensaje.gameObject.SetActive(false);
        }
    }

    // ── API publica que el Nivel1Manager llama ──

public void Activar() { activo = true; if (corrEvaluador == null) corrEvaluador = StartCoroutine(BucleEvaluador()); }
public void Desactivar() { activo = false; if (corrEvaluador != null) { StopCoroutine(corrEvaluador); corrEvaluador = null; } if (corrEjecutar != null) { StopCoroutine(corrEjecutar); corrEjecutar = null; } CancelarPerdidaSiActiva(); }
    public bool EstaEnModoPerdida() { return enModoPerdida; }

    private Coroutine corrEvaluador;
    private Coroutine corrEjecutar;

IEnumerator BucleEvaluador() { while (activo) { yield return new WaitForSeconds(intervaloEvaluacion); if (!activo) yield break; if (enModoPerdida) continue; if (Random.value < probabilidadDisparo) corrEjecutar = StartCoroutine(EjecutarPerdida()); } }

void CancelarPerdidaSiActiva() { enModoPerdida = false; foreach (var s in simbolosEnPantalla) if (s != null) Destroy(s); simbolosEnPantalla.Clear(); simbolosTotal = 0; simbolosRecuperados = 0; if (panelExpediente != null) { panelExpediente.alpha = 1f; panelExpediente.interactable = true; panelExpediente.blocksRaycasts = true; } if (panelAlimento != null) { panelAlimento.alpha = 1f; panelAlimento.interactable = true; panelAlimento.blocksRaycasts = true; } }

IEnumerator EjecutarPerdida() { enModoPerdida = true; if (nivel1Manager != null) nivel1Manager.SetEsperandoDecision(false); if (!yaSeMostroMensaje) { yaSeMostroMensaje = true; StartCoroutine(MostrarMensaje()); } CrearSimbolos(); StartCoroutine(AnimarAparicionSimbolos()); yield return StartCoroutine(ParpadearYDesvanecer()); yield return new WaitUntil(() => simbolosRecuperados >= simbolosTotal); yield return StartCoroutine(RecuperarPaneles()); if (nivel1Manager != null) nivel1Manager.SetEsperandoDecision(true); enModoPerdida = false; }

    IEnumerator ParpadearYDesvanecer()
    {
        float duracionParpadeoIndividual = duracionParpadeo / (cantidadParpadeos * 2f);
        for (int i = 0; i < cantidadParpadeos; i++)
        {
            yield return StartCoroutine(LerpAlphaAmbos(1f, 0.2f, duracionParpadeoIndividual));
            yield return StartCoroutine(LerpAlphaAmbos(0.2f, 1f, duracionParpadeoIndividual));
        }
        // Ahora desvanece a casi invisible
        yield return StartCoroutine(LerpAlphaAmbos(1f, 0.05f, 0.5f));
        if (panelExpediente != null) { panelExpediente.interactable = false; panelExpediente.blocksRaycasts = false; }
        if (panelAlimento != null) { panelAlimento.interactable = false; panelAlimento.blocksRaycasts = false; }
    }

    IEnumerator LerpAlphaAmbos(float desde, float hasta, float duracion)
    {
        float t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(desde, hasta, t / duracion);
            if (panelExpediente != null) panelExpediente.alpha = a;
            if (panelAlimento != null) panelAlimento.alpha = a;
            yield return null;
        }
        if (panelExpediente != null) panelExpediente.alpha = hasta;
        if (panelAlimento != null) panelAlimento.alpha = hasta;
    }

    void CrearSimbolos()
    {
        simbolosEnPantalla.Clear();
        simbolosRecuperados = 0;
        simbolosTotal = 0;
        if (panelExpediente != null) CrearSimbolosEnPanel(panelExpediente.transform as RectTransform);
        if (panelAlimento != null) CrearSimbolosEnPanel(panelAlimento.transform as RectTransform);
    }

void CrearSimbolosEnPanel(RectTransform panel) { if (panel == null) return; Vector3[] corners = new Vector3[4]; panel.GetWorldCorners(corners); var canvas = panel.GetComponentInParent<Canvas>(); if (canvas == null) return; var canvasRT = canvas.transform as RectTransform; var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera; for (int i = 0; i < 4; i++) { Vector2 localPoint; RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, RectTransformUtility.WorldToScreenPoint(cam, corners[i]), cam, out localPoint); float dx = (i == 0 || i == 1) ? offsetEsquina : -offsetEsquina; float dy = (i == 0 || i == 3) ? offsetEsquina : -offsetEsquina; CrearSimbolo(canvasRT, localPoint + new Vector2(dx, dy)); } }

void CrearSimbolo(RectTransform parent, Vector2 posLocal) { var go = new GameObject("Simbolo_" + simbolosTotal); go.transform.SetParent(parent, false); var rt = go.AddComponent<RectTransform>(); rt.anchorMin = new Vector2(0.5f, 0.5f); rt.anchorMax = new Vector2(0.5f, 0.5f); rt.pivot = new Vector2(0.5f, 0.5f); rt.sizeDelta = new Vector2(tamanoSimbolo, tamanoSimbolo); rt.anchoredPosition = posLocal; rt.localScale = Vector3.zero; var img = go.AddComponent<Image>(); img.sprite = spriteSimbolo; img.raycastTarget = true; img.preserveAspect = true; var c = (spriteSimbolo == null) ? new Color(1f, 0.85f, 0.2f, 0f) : new Color(1f, 1f, 1f, 0f); img.color = c; var btn = go.AddComponent<Button>(); btn.interactable = false; var capturado = go; btn.onClick.AddListener(() => OnClickSimbolo(capturado)); simbolosEnPantalla.Add(go); simbolosTotal++; }

    void OnClickSimbolo(GameObject simbolo)
    {
        if (simbolo == null) return;
        var btn = simbolo.GetComponent<Button>();
        if (btn != null) btn.interactable = false;
        if (sourceAudio != null && sonidoSimboloRecuperado != null) sourceAudio.PlayOneShot(sonidoSimboloRecuperado, volumen);
        StartCoroutine(AnimarYDestruirSimbolo(simbolo));
    }

    IEnumerator AnimarYDestruirSimbolo(GameObject go)
    {
        if (go == null) yield break;
        var rt = go.transform as RectTransform;
        var img = go.GetComponent<Image>();
        Vector3 escalaBase = rt.localScale;
        Color colorBase = img != null ? img.color : Color.white;

        float t = 0f;
        while (t < duracionAnimacionClick && go != null)
        {
            t += Time.deltaTime;
            float p = t / duracionAnimacionClick;
            float escala;
            if (p < 0.4f) escala = Mathf.Lerp(1f, escalaPicoClick, p / 0.4f);
            else escala = Mathf.Lerp(escalaPicoClick, 0f, (p - 0.4f) / 0.6f);
            if (rt != null) rt.localScale = escalaBase * escala;
            if (img != null) { var c = colorBase; c.a = Mathf.Lerp(1f, 0f, p); img.color = c; }
            yield return null;
        }
        simbolosRecuperados++;
        if (go != null) Destroy(go);
    }

    IEnumerator RecuperarPaneles()
    {
        yield return StartCoroutine(LerpAlphaAmbos(0.05f, 1f, 0.5f));
        if (panelExpediente != null) { panelExpediente.interactable = true; panelExpediente.blocksRaycasts = true; }
        if (panelAlimento != null) { panelAlimento.interactable = true; panelAlimento.blocksRaycasts = true; }
    }

    IEnumerator MostrarMensaje()
    {
        if (panelMensaje == null) yield break;
        if (textoMensaje != null) textoMensaje.text = mensaje;
        panelMensaje.gameObject.SetActive(true);
        panelMensaje.localScale = escalaOriginalMensaje * escalaInicialMensaje;
        if (sourceAudio != null && sonidoMensaje != null) sourceAudio.PlayOneShot(sonidoMensaje, volumen);

        // Entrada: del tamaño grande al normal
        float t = 0f;
        while (t < duracionEntradaMensaje)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duracionEntradaMensaje);
            panelMensaje.localScale = Vector3.Lerp(escalaOriginalMensaje * escalaInicialMensaje, escalaOriginalMensaje, p);
            yield return null;
        }
        panelMensaje.localScale = escalaOriginalMensaje;

        // Quedarse visible
        yield return new WaitForSeconds(duracionMensaje);

        // Salida: encogerse y desaparecer
        t = 0f;
        Vector3 escalaFinal = escalaOriginalMensaje * 0.5f;
        while (t < duracionEntradaMensaje)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duracionEntradaMensaje);
            panelMensaje.localScale = Vector3.Lerp(escalaOriginalMensaje, escalaFinal, p);
            yield return null;
        }
        panelMensaje.gameObject.SetActive(false);
    }


IEnumerator AnimarAparicionSimbolos() { var copia = new List<GameObject>(simbolosEnPantalla); foreach (var go in copia) { StartCoroutine(AnimarUnSimboloAparicion(go)); yield return new WaitForSeconds(delayEntreSimbolos); } }

IEnumerator AnimarUnSimboloAparicion(GameObject go) { if (go == null) yield break; var rt = go.transform as RectTransform; var img = go.GetComponent<Image>(); var btn = go.GetComponent<Button>(); Color colorFinal = (spriteSimbolo == null) ? new Color(1f, 0.85f, 0.2f, 1f) : Color.white; float t = 0f; while (t < duracionAparicionSimbolo && go != null) { t += Time.deltaTime; float p = Mathf.SmoothStep(0f, 1f, t / duracionAparicionSimbolo); float escala = Mathf.Lerp(escalaInicialSimbolo, 1f, p); if (rt != null) rt.localScale = Vector3.one * escala; if (img != null) { var c = colorFinal; c.a = p; img.color = c; } yield return null; } if (rt != null) rt.localScale = Vector3.one; if (img != null) img.color = colorFinal; if (btn != null) btn.interactable = true; if (go != null && go.GetComponent<BotonHoverEscala>() == null) go.AddComponent<BotonHoverEscala>(); }


public void ActivarConDisparoInicial() { activo = true; if (corrEvaluador == null) corrEvaluador = StartCoroutine(BucleConDisparoInicial()); }

IEnumerator BucleConDisparoInicial() { yield return new WaitForSeconds(delayDisparoForzado); if (activo && !enModoPerdida) corrEjecutar = StartCoroutine(EjecutarPerdida()); while (activo) { yield return new WaitForSeconds(intervaloEvaluacion); if (!activo) yield break; if (enModoPerdida) continue; if (Random.value < probabilidadDisparo) corrEjecutar = StartCoroutine(EjecutarPerdida()); } }
}
