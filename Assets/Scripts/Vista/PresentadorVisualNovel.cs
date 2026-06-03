using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// Vista de novela visual del presentador: cuadro de texto con typewriter + imagen del presentador
// + imagenes emergentes por linea (con pop y fade out).
// No decide flujo: reproduce un DialogoPresentadorData y avisa por callback cuando termina.
// Click completa el texto si esta escribiendo, o avanza a la siguiente linea.
public class PresentadorVisualNovel : MonoBehaviour, IPointerClickHandler
{
    [Header("Referencias UI")]
    public TMP_Text textoDialogo;
    public Image imagenPresentador;

    [Tooltip("Contenedor donde se instancian las imagenes emergentes de cada linea")]
    public RectTransform contenedorImagenes;

    [Header("Configuracion typewriter")]
    public float velocidadTypewriter = 0.03f;

    [Header("Sonido de tecleo")]
    public AudioClip sonidoTecleo;
    public float velocidadTecleo = 2f;
    [Range(0f, 1f)] public float volumenTecleo = 1f;

    [Header("Imagenes emergentes - aparicion (pop)")]
    [Tooltip("Delay antes de que aparezca la PRIMERA imagen al empezar la linea")]
    public float delayPrimeraImagen = 0.3f;
    [Tooltip("Delay entre imagenes consecutivas")]
    public float delayEntreImagenes = 0.4f;
    [Tooltip("Duracion del pop (escala 0 -> 1.2 -> 1)")]
    public float duracionPop = 0.3f;
    [Tooltip("Escala maxima del rebote del pop")]
    public float escalaPico = 1.2f;
    [Tooltip("Sonido cuando aparece cada imagen")]
    public AudioClip sonidoPop;
    [Range(0f, 1f)] public float volumenPop = 0.8f;

    [Header("Imagenes emergentes - salida (fade out al avanzar)")]
    [Tooltip("Duracion del fade out + shrink cuando se cambia de linea")]
    public float duracionSalida = 0.2f;

    private DialogoPresentadorData dialogoActual;
    private int lineaActual = 0;
    private bool escribiendo = false;
    private string textoCompleto = "";
    private Coroutine corrTypewriter;
    private Coroutine corrImagenes;
    private AudioSource sourceTecleo;
    private AudioSource sourceVoz;
    private AudioSource sourceEfectos;
    private Action alTerminar;
    private List<GameObject> imagenesActivas = new List<GameObject>();
    private List<ImagenLinea> imagenesPendientes = new List<ImagenLinea>();

    void AsegurarAudioSources()
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
        if (sourceVoz == null)
        {
            sourceVoz = gameObject.AddComponent<AudioSource>();
            sourceVoz.playOnAwake = false;
            sourceVoz.loop = false;
        }
        if (sourceEfectos == null)
        {
            sourceEfectos = gameObject.AddComponent<AudioSource>();
            sourceEfectos.playOnAwake = false;
            sourceEfectos.loop = false;
        }
    }

    public void Reproducir(DialogoPresentadorData dialogo, Action callback)
    {
        AsegurarAudioSources();
        dialogoActual = dialogo;
        alTerminar = callback;
        lineaActual = 0;

        if (dialogoActual == null || dialogoActual.lineas.Count == 0)
        {
            if (alTerminar != null) alTerminar();
            return;
        }

        MostrarLinea(lineaActual);
    }

    void MostrarLinea(int indice)
    {
        var linea = dialogoActual.lineas[indice];

        // Imagen del presentador
        if (imagenPresentador != null && linea.spritePresentador != null)
        {
            imagenPresentador.sprite = linea.spritePresentador;
            imagenPresentador.preserveAspect = true;
            imagenPresentador.enabled = true;
        }

        // Voz en off
        if (sourceVoz != null)
        {
            sourceVoz.Stop();
            if (linea.vozEnOff != null)
            {
                sourceVoz.clip = linea.vozEnOff;
                sourceVoz.Play();
            }
        }

        // Imagenes emergentes: empezar a spawnearlas en paralelo
        if (corrImagenes != null) StopCoroutine(corrImagenes);
        imagenesPendientes = new List<ImagenLinea>(linea.imagenes);
        if (imagenesPendientes.Count > 0)
            corrImagenes = StartCoroutine(SpawnImagenes());

        // Typewriter
        textoCompleto = linea.texto;
        if (corrTypewriter != null) StopCoroutine(corrTypewriter);
        corrTypewriter = StartCoroutine(Typewriter());
    }

    IEnumerator SpawnImagenes()
    {
        yield return new WaitForSeconds(delayPrimeraImagen);
        while (imagenesPendientes.Count > 0)
        {
            var img = imagenesPendientes[0];
            imagenesPendientes.RemoveAt(0);
            CrearImagenConPop(img);
            if (imagenesPendientes.Count > 0)
                yield return new WaitForSeconds(delayEntreImagenes);
        }
        corrImagenes = null;
    }

    void CrearImagenConPop(ImagenLinea data)
    {
        if (contenedorImagenes == null || data == null || data.sprite == null) return;

        var go = new GameObject("ImagenLinea");
        go.transform.SetParent(contenedorImagenes, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = data.tamano;
        rt.anchoredPosition = data.posicion;
        rt.localScale = Vector3.zero;

        var img = go.AddComponent<Image>();
        img.sprite = data.sprite;
        img.preserveAspect = true;
        img.raycastTarget = false;

        imagenesActivas.Add(go);

        if (sourceEfectos != null && sonidoPop != null)
            sourceEfectos.PlayOneShot(sonidoPop, volumenPop);

        StartCoroutine(AnimarPop(rt));
    }

    IEnumerator AnimarPop(RectTransform rt)
    {
        float t = 0f;
        while (t < duracionPop && rt != null)
        {
            t += Time.deltaTime;
            float p = t / duracionPop;
            float escala;
            if (p < 0.5f) escala = Mathf.Lerp(0f, escalaPico, Mathf.SmoothStep(0f, 1f, p / 0.5f));
            else escala = Mathf.Lerp(escalaPico, 1f, Mathf.SmoothStep(0f, 1f, (p - 0.5f) / 0.5f));
            rt.localScale = Vector3.one * escala;
            yield return null;
        }
        if (rt != null) rt.localScale = Vector3.one;
    }

    void SpawnearTodasLasPendientes()
    {
        // Llamado cuando se skipea: aparecen instantaneamente todas las que faltaban
        if (corrImagenes != null) { StopCoroutine(corrImagenes); corrImagenes = null; }
        foreach (var img in imagenesPendientes)
            CrearImagenConPop(img);
        imagenesPendientes.Clear();
    }

    IEnumerator LimpiarImagenesActivas()
    {
        // Fade out + shrink en paralelo
        var copia = new List<GameObject>(imagenesActivas);
        imagenesActivas.Clear();
        foreach (var go in copia) if (go != null) StartCoroutine(AnimarSalida(go));
        yield return new WaitForSeconds(duracionSalida);
    }

    IEnumerator AnimarSalida(GameObject go)
    {
        if (go == null) yield break;
        var rt = go.transform as RectTransform;
        var img = go.GetComponent<Image>();
        Vector3 escalaInicial = rt != null ? rt.localScale : Vector3.one;
        float t = 0f;
        while (t < duracionSalida && go != null)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duracionSalida);
            if (rt != null) rt.localScale = Vector3.Lerp(escalaInicial, Vector3.zero, p);
            if (img != null) { var c = img.color; c.a = Mathf.Lerp(1f, 0f, p); img.color = c; }
            yield return null;
        }
        if (go != null) Destroy(go);
    }

    IEnumerator Typewriter()
    {
        escribiendo = true;
        textoDialogo.text = "";
        IniciarTecleo();
        foreach (char c in textoCompleto)
        {
            textoDialogo.text += c;
            yield return new WaitForSeconds(velocidadTypewriter);
        }
        escribiendo = false;
        DetenerTecleo();
    }

    void IniciarTecleo() { if (sourceTecleo != null && sonidoTecleo != null && !sourceTecleo.isPlaying) sourceTecleo.Play(); }
    void DetenerTecleo() { if (sourceTecleo != null && sourceTecleo.isPlaying) sourceTecleo.Stop(); }

    public void OnPointerClick(PointerEventData eventData) { Avanzar(); }

    void Avanzar()
    {
        if (dialogoActual == null) return;

        if (escribiendo)
        {
            // Skip del texto: completar de golpe y mostrar todas las imagenes pendientes
            if (corrTypewriter != null) StopCoroutine(corrTypewriter);
            textoDialogo.text = textoCompleto;
            escribiendo = false;
            DetenerTecleo();
            SpawnearTodasLasPendientes();
            return;
        }

        // Avanzar a siguiente linea: limpiar imagenes de la linea anterior
        StartCoroutine(AvanzarLinea());
    }

    IEnumerator AvanzarLinea()
    {
        yield return StartCoroutine(LimpiarImagenesActivas());
        lineaActual++;
        if (lineaActual >= dialogoActual.lineas.Count)
        {
            DetenerTecleo();
            if (sourceVoz != null) sourceVoz.Stop();
            if (alTerminar != null) alTerminar();
        }
        else
        {
            MostrarLinea(lineaActual);
        }
    }
}
