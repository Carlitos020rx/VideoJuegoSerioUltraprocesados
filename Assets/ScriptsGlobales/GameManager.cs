using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum Segmento
    {
        Intro,
        ExplicacionNivel1,
        PuenteNivel1aNivel2,
        Cierre
    }

    public Segmento SegmentoActual { get; private set; }
    public int ScoreNivel1 { get; private set; }
    public int ScoreNivel2 { get; private set; }

    const string ESCENA_MENU        = "MenuPrincipal";
    const string ESCENA_PRESENTADOR = "Presentador";
    const string ESCENA_NIVEL1      = "Nivel1";
    const string ESCENA_NIVEL2      = "Nivel2";

    [Header("Fade")]
    public float duracionFade = 1.5f;

    private Image panelFade;
    private bool cargando = false;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CrearPanelFade();
    }

    // Crea el Canvas de fade persistente en codigo
void CrearPanelFade() { var canvasGO = new GameObject("FadeCanvas"); canvasGO.transform.SetParent(this.transform); var canvas = canvasGO.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 999; canvasGO.AddComponent<CanvasScaler>(); canvasGO.AddComponent<GraphicRaycaster>(); var panelGO = new GameObject("PanelFade"); panelGO.transform.SetParent(canvasGO.transform, false); panelFade = panelGO.AddComponent<Image>(); panelFade.color = new Color(0f, 0f, 0f, 0f); panelFade.raycastTarget = true; panelFade.gameObject.SetActive(false); var rt = panelGO.GetComponent<RectTransform>(); rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero; }

    // ── API publica ────────────────────────────────────────────

    public void IniciarJuego()
    {
        SegmentoActual = Segmento.Intro;
        CargarEscenaConFade(ESCENA_PRESENTADOR);
    }

    public void SegmentoTerminado()
    {
        switch (SegmentoActual)
        {
            case Segmento.Intro:
                SegmentoActual = Segmento.ExplicacionNivel1;
                var ctrl = FindObjectOfType<PresentadorController>();
                if (ctrl != null) ctrl.MostrarSegmentoActual();
                break;
            case Segmento.ExplicacionNivel1:
                CargarEscenaConFade(ESCENA_NIVEL1);
                break;
            case Segmento.PuenteNivel1aNivel2:
                CargarEscenaConFade(ESCENA_NIVEL2);
                break;
            case Segmento.Cierre:
                CargarEscenaConFade(ESCENA_MENU);
                break;
        }
    }

    public void TerminarNivel1(int score)
    {
        ScoreNivel1    = score;
        SegmentoActual = Segmento.PuenteNivel1aNivel2;
        CargarEscenaConFade(ESCENA_PRESENTADOR);
    }

    public void TerminarNivel2(int score)
    {
        ScoreNivel2    = score;
        SegmentoActual = Segmento.Cierre;
        CargarEscenaConFade(ESCENA_PRESENTADOR);
    }

    public void IrAlMenu()
    {
        CargarEscenaConFade(ESCENA_MENU);
    }

    // ── Fade centralizado ──────────────────────────────────────

    public void CargarEscenaConFade(string nombreEscena)
    {
        if (cargando) return;
        StartCoroutine(FadeYCargar(nombreEscena));
    }

IEnumerator FadeYCargar(string nombreEscena) { cargando = true; yield return StartCoroutine(Fade(0f, 1f)); AsyncOperation op = SceneManager.LoadSceneAsync(nombreEscena); op.allowSceneActivation = false; while (op.progress < 0.9f) { yield return null; } op.allowSceneActivation = true; yield return null; yield return null; yield return StartCoroutine(Fade(1f, 0f)); cargando = false; }

IEnumerator Fade(float desde, float hasta) { panelFade.gameObject.SetActive(true); float t = 0f; while (t < duracionFade) { t += Time.deltaTime; float a = Mathf.Lerp(desde, hasta, t / duracionFade); panelFade.color = new Color(0f, 0f, 0f, a); yield return null; } panelFade.color = new Color(0f, 0f, 0f, hasta); if (hasta == 0f) panelFade.gameObject.SetActive(false); }
}
