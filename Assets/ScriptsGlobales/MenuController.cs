using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelInicio;
    public GameObject panelMenu;

    [Header("Duracion transicion entre paneles")]
    public float duracionTransicion = 0.5f;

    private CanvasGroup cgInicio;
    private CanvasGroup cgMenu;

    void Start()
    {
        cgInicio = ObtenerOAgregarCanvasGroup(panelInicio);
        cgMenu   = ObtenerOAgregarCanvasGroup(panelMenu);

        cgInicio.alpha           = 1f;
        cgInicio.interactable    = true;
        cgInicio.blocksRaycasts  = true;
        panelInicio.SetActive(true);

        cgMenu.alpha          = 0f;
        cgMenu.interactable   = false;
        cgMenu.blocksRaycasts = false;
        panelMenu.SetActive(true);

        ConectarBoton(panelInicio, "BtnPressStart", IrAlMenu);
        ConectarBoton(panelMenu,   "BtnJugar",      Jugar);
        ConectarBoton(panelMenu,   "BtnSalir",      Salir);
    }

    void ConectarBoton(GameObject panel, string nombreBoton, UnityEngine.Events.UnityAction accion)
    {
        var btn = panel.transform.Find(nombreBoton)?.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(accion);
    }

    public void IrAlMenu()
    {
        StartCoroutine(TransicionFade(cgInicio, cgMenu));
    }

    public void Jugar()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.IniciarJuego();
    }

    public void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator TransicionFade(CanvasGroup desde, CanvasGroup hacia)
    {
        desde.interactable   = false;
        desde.blocksRaycasts = false;

        float t = 0f;
        while (t < duracionTransicion)
        {
            t += Time.deltaTime;
            float p = t / duracionTransicion;
            desde.alpha = 1f - p;
            hacia.alpha = p;
            yield return null;
        }

        desde.alpha          = 0f;
        hacia.alpha          = 1f;
        hacia.interactable   = true;
        hacia.blocksRaycasts = true;
    }

    CanvasGroup ObtenerOAgregarCanvasGroup(GameObject go)
    {
        var cg = go.GetComponent<CanvasGroup>();
        if (cg == null) cg = go.AddComponent<CanvasGroup>();
        return cg;
    }
}
