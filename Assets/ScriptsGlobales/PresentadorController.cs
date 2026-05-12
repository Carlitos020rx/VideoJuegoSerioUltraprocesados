
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PresentadorController : MonoBehaviour
{
    [Header("Paneles de cada segmento")]
    public GameObject panelIntro;
    public GameObject panelExplicacionNivel1;
    public GameObject panelPuente;
    public GameObject panelCierre;

    [Header("Textos del temporizador (uno por panel)")]
    public Text timerTextIntro;
    public Text timerTextExplicacion;
    public Text timerTextPuente;
    public Text timerTextCierre;

    [Header("Duraciones simuladas (segundos)")]
    public float duracionIntro       = 10f;
    public float duracionExplicacion = 10f;
    public float duracionPuente      = 10f;
    public float duracionCierre      = 10f;

    [Header("Animacion slide")]
    public float duracionSlide = 1.2f;

    private GameObject panelActivo;
    private Text timerActivo;

    void Start()
    {
        panelIntro.SetActive(false);
        panelExplicacionNivel1.SetActive(false);
        panelPuente.SetActive(false);
        panelCierre.SetActive(false);

        switch (GameManager.Instance.SegmentoActual)
        {
            case GameManager.Segmento.Intro:
                IniciarPanel(panelIntro, timerTextIntro, duracionIntro);
                break;
            case GameManager.Segmento.ExplicacionNivel1:
                IniciarPanel(panelExplicacionNivel1, timerTextExplicacion, duracionExplicacion);
                break;
            case GameManager.Segmento.PuenteNivel1aNivel2:
                IniciarPanel(panelPuente, timerTextPuente, duracionPuente);
                break;
            case GameManager.Segmento.Cierre:
                IniciarPanel(panelCierre, timerTextCierre, duracionCierre);
                break;
        }
    }

    public void MostrarSegmentoActual()
    {
        StopAllCoroutines();

        switch (GameManager.Instance.SegmentoActual)
        {
            case GameManager.Segmento.Intro:
                StartCoroutine(SlideYIniciar(panelIntro, timerTextIntro, duracionIntro));
                break;
            case GameManager.Segmento.ExplicacionNivel1:
                StartCoroutine(SlideYIniciar(panelExplicacionNivel1, timerTextExplicacion, duracionExplicacion));
                break;
            case GameManager.Segmento.PuenteNivel1aNivel2:
                StartCoroutine(SlideYIniciar(panelPuente, timerTextPuente, duracionPuente));
                break;
            case GameManager.Segmento.Cierre:
                StartCoroutine(SlideYIniciar(panelCierre, timerTextCierre, duracionCierre));
                break;
        }
    }

    void IniciarPanel(GameObject panel, Text timerText, float duracion)
    {
        panel.SetActive(true);
        panel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        panelActivo = panel;
        timerActivo = timerText;
        StartCoroutine(ContarTiempo(duracion));
    }

IEnumerator SlideYIniciar(GameObject panelNuevo, Text timerText, float duracion) { float ancho = 1920f; panelNuevo.SetActive(true); var rtNuevo = panelNuevo.GetComponent<RectTransform>(); var rtViejo = panelActivo != null ? panelActivo.GetComponent<RectTransform>() : null; rtNuevo.anchoredPosition = new Vector2(ancho, 0f); float t = 0f; while (t < duracionSlide) { t += Time.deltaTime; float p = Mathf.SmoothStep(0f, 1f, t / duracionSlide); rtNuevo.anchoredPosition = new Vector2(Mathf.Lerp(ancho, 0f, p), 0f); if (rtViejo != null) rtViejo.anchoredPosition = new Vector2(Mathf.Lerp(0f, -ancho, p), 0f); yield return null; } rtNuevo.anchoredPosition = Vector2.zero; if (panelActivo != null) panelActivo.SetActive(false); panelActivo = panelNuevo; timerActivo = timerText; StartCoroutine(ContarTiempo(duracion)); }

    IEnumerator ContarTiempo(float duracion)
    {
        float tiempoRestante = duracion;
        while (tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime;
            if (timerActivo != null)
                timerActivo.text = "VIDEO EN: " + Mathf.CeilToInt(tiempoRestante) + "s";
            yield return null;
        }

        if (timerActivo != null)
            timerActivo.text = "LISTO";

        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.SegmentoTerminado();
    }
}
