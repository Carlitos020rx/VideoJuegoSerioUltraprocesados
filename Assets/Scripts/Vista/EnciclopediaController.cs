using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnciclopediaController : MonoBehaviour
{
    [Header("Configuracion")]
    public EnciclopediaConfig config;

    [Header("Referencias UI")]
    [Tooltip("RectTransform del cuaderno completo (lo que se mueve)")]
    public RectTransform cuaderno;

    [Tooltip("Image donde se muestra el sprite de la pagina actual")]
    public Image imagenPagina;

    [Tooltip("Boton flecha izquierda")]
    public Button btnAnterior;

    [Tooltip("Boton flecha derecha")]
    public Button btnSiguiente;

    [Tooltip("Boton de cerrar la enciclopedia")]
    public Button btnCerrar;

    [Tooltip("La pestaña sobresaliente (siempre visible) que abre la enciclopedia")]
    public PestanaEnciclopedia pestaña;

    [Header("Texto de paginacion (opcional)")]
    public TMPro.TMP_Text textoPaginacion;

    private int paginaActual = 0;
    private bool abierta = false;
    private Vector2 posicionAbierta;
    private Vector2 posicionCerrada;
    private bool animando = false;

    void Start()
    {
        if (config == null || config.paginas.Count == 0)
        {
            Debug.LogError("[Enciclopedia] Falta config o no hay paginas asignadas");
            return;
        }

        // Calcular posiciones — la abierta es la que pusiste en el editor
        posicionAbierta = cuaderno.anchoredPosition;
        // La cerrada es completamente debajo de la pantalla
        float altoCanvas = (cuaderno.parent as RectTransform).rect.height;
        posicionCerrada = new Vector2(posicionAbierta.x, -altoCanvas - cuaderno.rect.height - config.offsetCerrado);

        // Empezar cerrada
        cuaderno.anchoredPosition = posicionCerrada;
        cuaderno.gameObject.SetActive(false);

        // Conectar botones
        if (btnAnterior  != null) btnAnterior.onClick.AddListener(PaginaAnterior);
        if (btnSiguiente != null) btnSiguiente.onClick.AddListener(PaginaSiguiente);
        if (btnCerrar    != null) btnCerrar.onClick.AddListener(Cerrar);

        // Conectar la pestaña
        if (pestaña != null) pestaña.SetController(this);

        ActualizarPagina();
    }

public void Abrir() { if (animando || abierta) return; if (AudioManager.Instance != null) AudioManager.Instance.SonarAbrirEnciclopedia(); StartCoroutine(AnimarApertura()); }

public void Cerrar() { if (animando || !abierta) return; if (AudioManager.Instance != null) AudioManager.Instance.SonarCerrarEnciclopedia(); StartCoroutine(AnimarCierre()); }

    public void Toggle()
    {
        if (abierta) Cerrar();
        else Abrir();
    }

    IEnumerator AnimarApertura()
    {
        animando = true;
        cuaderno.gameObject.SetActive(true);
        cuaderno.anchoredPosition = posicionCerrada;

        float t = 0f;
        while (t < config.duracionAnimacion)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / config.duracionAnimacion);
            cuaderno.anchoredPosition = Vector2.Lerp(posicionCerrada, posicionAbierta, p);
            yield return null;
        }
        cuaderno.anchoredPosition = posicionAbierta;
        abierta = true;
        animando = false;
    }

    IEnumerator AnimarCierre()
    {
        animando = true;
        Vector2 desde = cuaderno.anchoredPosition;

        float t = 0f;
        while (t < config.duracionAnimacion)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / config.duracionAnimacion);
            cuaderno.anchoredPosition = Vector2.Lerp(desde, posicionCerrada, p);
            yield return null;
        }
        cuaderno.anchoredPosition = posicionCerrada;
        cuaderno.gameObject.SetActive(false);
        abierta = false;
        animando = false;
    }

    public void PaginaSiguiente()
    {
        if (paginaActual < config.paginas.Count - 1)
        {
            paginaActual++;
            ActualizarPagina();
        }
    }

    public void PaginaAnterior()
    {
        if (paginaActual > 0)
        {
            paginaActual--;
            ActualizarPagina();
        }
    }

    void ActualizarPagina()
    {
        if (imagenPagina != null && config.paginas.Count > paginaActual)
        {
            imagenPagina.sprite = config.paginas[paginaActual];
            imagenPagina.preserveAspect = true;
        }

        if (btnAnterior  != null) btnAnterior.interactable  = paginaActual > 0;
        if (btnSiguiente != null) btnSiguiente.interactable = paginaActual < config.paginas.Count - 1;

        if (textoPaginacion != null)
            textoPaginacion.text = (paginaActual + 1) + " / " + config.paginas.Count;
    }
}
