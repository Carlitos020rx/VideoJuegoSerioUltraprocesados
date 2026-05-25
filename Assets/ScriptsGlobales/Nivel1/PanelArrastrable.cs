using UnityEngine;
using UnityEngine.EventSystems;

// Hace que un panel se pueda arrastrar por la pantalla agarrandolo desde una zona (mango).
// Limita el movimiento para que siempre quede un porcentaje visible.
public class PanelArrastrable : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Tooltip("RectTransform del panel completo que se mueve. Si se deja vacio, usa el de este GameObject.")]
    public RectTransform objetivo;

    [Tooltip("Porcentaje minimo del panel que debe quedar visible (0.5 = 50%)")]
    [Range(0.1f, 1f)]
    public float porcentajeMinimoVisible = 0.5f;

    private RectTransform canvasRT;
    private Camera camaraUI;
    private Vector2 offsetArrastre;
    private bool arrastrando;

    void Awake()
    {
        if (objetivo == null) objetivo = transform as RectTransform;

        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRT = canvas.transform as RectTransform;
            camaraUI = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        }
    }

public void OnPointerDown(PointerEventData eventData) { if (objetivo == null) return; arrastrando = true; if (AudioManager.Instance != null) AudioManager.Instance.SonarClick(); Vector2 cursorLocal; RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, eventData.position, camaraUI, out cursorLocal); offsetArrastre = objetivo.anchoredPosition - cursorLocal; }

public void OnDrag(PointerEventData eventData) { if (!arrastrando || objetivo == null) return; Vector2 cursorLocal; if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, eventData.position, camaraUI, out cursorLocal)) return; Vector2 deseada = cursorLocal + offsetArrastre; objetivo.anchoredPosition = deseada; Vector3[] esquinasPanel = new Vector3[4]; objetivo.GetWorldCorners(esquinasPanel); Vector3[] esquinasCanvas = new Vector3[4]; canvasRT.GetWorldCorners(esquinasCanvas); float panelMinX = esquinasPanel[0].x, panelMaxX = esquinasPanel[2].x; float panelMinY = esquinasPanel[0].y, panelMaxY = esquinasPanel[2].y; float canvasMinX = esquinasCanvas[0].x, canvasMaxX = esquinasCanvas[2].x; float canvasMinY = esquinasCanvas[0].y, canvasMaxY = esquinasCanvas[2].y; float anchoPanel = panelMaxX - panelMinX; float altoPanel = panelMaxY - panelMinY; float visibleX = anchoPanel * porcentajeMinimoVisible; float visibleY = altoPanel * porcentajeMinimoVisible; float corrX = 0f, corrY = 0f; if (panelMaxX < canvasMinX + visibleX) corrX = (canvasMinX + visibleX) - panelMaxX; else if (panelMinX > canvasMaxX - visibleX) corrX = (canvasMaxX - visibleX) - panelMinX; if (panelMaxY < canvasMinY + visibleY) corrY = (canvasMinY + visibleY) - panelMaxY; else if (panelMinY > canvasMaxY - visibleY) corrY = (canvasMaxY - visibleY) - panelMinY; float escala = canvasRT.lossyScale.x; if (escala == 0f) escala = 1f; objetivo.anchoredPosition = deseada + new Vector2(corrX, corrY) / escala; }

public void OnPointerUp(PointerEventData eventData) { if (arrastrando && AudioManager.Instance != null) AudioManager.Instance.SonarClick(); arrastrando = false; }
}
