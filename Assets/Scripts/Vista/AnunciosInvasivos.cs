using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Anuncios invasivos: aparecen pop-ups de comida chatarra que el jugador debe cerrar.
// Se activan en dificultad alta. Se pausan durante simulador o feedback (lo controla el Nivel1Manager).
// Pop-in animado al aparecer, click en X para cerrar.
public class AnunciosInvasivos : MonoBehaviour
{
    [Header("Sprites de anuncios (pon aqui todos los PNG)")]
    public List<Sprite> spritesAnuncios = new List<Sprite>();

    [Header("Contenedor donde se instancian")]
    [Tooltip("Si no se asigna, se usa el RectTransform de este GameObject")]
    public RectTransform contenedor;

    [Header("Configuracion")]
    [Tooltip("Segundos entre cada anuncio nuevo")]
    public float intervaloEntreAnuncios = 8f;

    [Tooltip("Tamaño del anuncio en pixeles")]
    public Vector2 tamanoAnuncio = new Vector2(220f, 220f);

    [Tooltip("Tamaño del boton X (esquina superior derecha)")]
    public float tamanoBotonCerrar = 40f;

    [Tooltip("Sprite opcional del boton X. Si esta vacio se usa un cuadrado rojo")]
    public Sprite spriteBotonCerrar;

    [Header("Animacion de aparicion")]
    public float duracionPopIn = 0.3f;
    public float escalaInicial = 0f;
    public float escalaRebote = 1.15f;

    [Header("Sonido al aparecer (opcional)")]
    public AudioClip sonidoAparecer;
    [Range(0f, 1f)]
    public float volumenAparecer = 0.6f;

    [Header("Mensaje emergente (solo la primera vez)")]
    [Tooltip("Panel del mensaje, posicionado donde quieras que aparezca (tipicamente arriba)")]
    public RectTransform panelMensaje;
    public TMP_Text textoMensaje;
    [TextArea(2, 4)]
    public string mensaje = "¡Anuncios invasivos! La publicidad ahora interrumpe tu atencion.";
    public float duracionMensaje = 3f;
    public float duracionEntradaMensaje = 0.4f;
    public float escalaInicialMensaje = 1.5f;
    public AudioClip sonidoMensaje;
    [Range(0f, 1f)] public float volumenMensaje = 0.9f;

    private bool activo = false;
    private bool pausado = false;
    private Coroutine corrSpawner;
    private AudioSource sourceAudio;
    private bool yaSeMostroMensaje = false;
    private Vector3 escalaOriginalMensaje;

void Awake() { if (contenedor == null) contenedor = transform as RectTransform; sourceAudio = gameObject.AddComponent<AudioSource>(); sourceAudio.playOnAwake = false; sourceAudio.loop = false; if (panelMensaje != null) { escalaOriginalMensaje = panelMensaje.localScale; panelMensaje.gameObject.SetActive(false); } }

    // Llamar desde el Nivel1Manager cuando empieza la dificultad alta
public void Activar() { if (activo) return; activo = true; if (corrSpawner != null) StopCoroutine(corrSpawner); corrSpawner = StartCoroutine(BucleSpawner()); if (!yaSeMostroMensaje) { yaSeMostroMensaje = true; StartCoroutine(MostrarMensaje()); } }

    public void Desactivar()
    {
        activo = false;
        if (corrSpawner != null) StopCoroutine(corrSpawner);
        corrSpawner = null;
        LimpiarAnunciosVisibles();
    }

    // Pausa: el bucle sigue corriendo pero no spawnea nuevos. Llamar antes del simulador/feedback
    public void Pausar() { pausado = true; }
    public void Reanudar() { pausado = false; }

    void LimpiarAnunciosVisibles()
    {
        // Destruye todos los anuncios que esten en pantalla
        for (int i = contenedor.childCount - 1; i >= 0; i--)
        {
            var hijo = contenedor.GetChild(i);
            if (hijo != null && hijo.name.StartsWith("Anuncio_"))
                Destroy(hijo.gameObject);
        }
    }

    IEnumerator BucleSpawner()
    {
        // Primer anuncio sale despues del intervalo, no de inmediato
        while (activo)
        {
            yield return new WaitForSeconds(intervaloEntreAnuncios);
            if (activo && !pausado) SpawnAnuncio();
        }
    }

    void SpawnAnuncio()
    {
        if (spritesAnuncios == null || spritesAnuncios.Count == 0) return;

        // GameObject del anuncio
        var go = new GameObject("Anuncio_" + Random.Range(0, 9999));
        go.transform.SetParent(contenedor, false);
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = tamanoAnuncio;

        // Image con sprite random
        var img = go.AddComponent<Image>();
        img.sprite = spritesAnuncios[Random.Range(0, spritesAnuncios.Count)];
        img.preserveAspect = true;
        img.raycastTarget = true;

        // Posicion random dentro del contenedor
        float anchoCont = contenedor.rect.width;
        float altoCont = contenedor.rect.height;
        float margenX = tamanoAnuncio.x * 0.5f;
        float margenY = tamanoAnuncio.y * 0.5f;
        float x = Random.Range(-anchoCont * 0.5f + margenX, anchoCont * 0.5f - margenX);
        float y = Random.Range(-altoCont * 0.5f + margenY, altoCont * 0.5f - margenY);
        rt.anchoredPosition = new Vector2(x, y);

        // Boton X en esquina superior derecha
        var btnGO = new GameObject("BotonCerrar");
        btnGO.transform.SetParent(go.transform, false);
        var btnRT = btnGO.AddComponent<RectTransform>();
        btnRT.sizeDelta = new Vector2(tamanoBotonCerrar, tamanoBotonCerrar);
        btnRT.anchorMin = new Vector2(1f, 1f);
        btnRT.anchorMax = new Vector2(1f, 1f);
        btnRT.pivot = new Vector2(0.5f, 0.5f);
        btnRT.anchoredPosition = new Vector2(-tamanoBotonCerrar * 0.3f, -tamanoBotonCerrar * 0.3f);

        var btnImg = btnGO.AddComponent<Image>();
        btnImg.sprite = spriteBotonCerrar;
        btnImg.color = spriteBotonCerrar == null ? new Color(0.85f, 0.2f, 0.2f) : Color.white;
        btnImg.raycastTarget = true;

        // Texto X dentro del boton si no hay sprite
        if (spriteBotonCerrar == null)
        {
            var txtGO = new GameObject("X");
            txtGO.transform.SetParent(btnGO.transform, false);
            var txtRT = txtGO.AddComponent<RectTransform>();
            txtRT.anchorMin = Vector2.zero; txtRT.anchorMax = Vector2.one;
            txtRT.offsetMin = Vector2.zero; txtRT.offsetMax = Vector2.zero;
            var txt = txtGO.AddComponent<Text>();
            txt.text = "X";
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.fontSize = (int)(tamanoBotonCerrar * 0.6f);
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.raycastTarget = false;
        }

        var btn = btnGO.AddComponent<Button>();
        var anuncioGO = go;
        btn.onClick.AddListener(() => {
            if (AudioManager.Instance != null) AudioManager.Instance.SonarClick();
            Destroy(anuncioGO);
        });

        // Sonido al aparecer
        if (sourceAudio != null && sonidoAparecer != null)
        {
            sourceAudio.volume = volumenAparecer;
            sourceAudio.PlayOneShot(sonidoAparecer);
        }

        // Animacion pop-in
        StartCoroutine(AnimarPopIn(rt));
    }

    IEnumerator AnimarPopIn(RectTransform rt)
    {
        if (rt == null) yield break;
        float t = 0f;
        while (t < duracionPopIn && rt != null)
        {
            t += Time.deltaTime;
            float p = t / duracionPopIn;
            float escala;
            if (p < 0.6f)
                escala = Mathf.Lerp(escalaInicial, escalaRebote, Mathf.SmoothStep(0f, 1f, p / 0.6f));
            else
                escala = Mathf.Lerp(escalaRebote, 1f, Mathf.SmoothStep(0f, 1f, (p - 0.6f) / 0.4f));
            rt.localScale = Vector3.one * escala;
            yield return null;
        }
        if (rt != null) rt.localScale = Vector3.one;
    }


IEnumerator MostrarMensaje() { if (panelMensaje == null) yield break; if (textoMensaje != null) textoMensaje.text = mensaje; panelMensaje.gameObject.SetActive(true); panelMensaje.localScale = escalaOriginalMensaje * escalaInicialMensaje; if (sourceAudio != null && sonidoMensaje != null) sourceAudio.PlayOneShot(sonidoMensaje, volumenMensaje); float t = 0f; while (t < duracionEntradaMensaje) { t += Time.deltaTime; float p = Mathf.SmoothStep(0f, 1f, t / duracionEntradaMensaje); panelMensaje.localScale = Vector3.Lerp(escalaOriginalMensaje * escalaInicialMensaje, escalaOriginalMensaje, p); yield return null; } panelMensaje.localScale = escalaOriginalMensaje; yield return new WaitForSeconds(duracionMensaje); t = 0f; Vector3 escalaFinal = escalaOriginalMensaje * 0.5f; while (t < duracionEntradaMensaje) { t += Time.deltaTime; float p = Mathf.SmoothStep(0f, 1f, t / duracionEntradaMensaje); panelMensaje.localScale = Vector3.Lerp(escalaOriginalMensaje, escalaFinal, p); yield return null; } panelMensaje.gameObject.SetActive(false); }
}
