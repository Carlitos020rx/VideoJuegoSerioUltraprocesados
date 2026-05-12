using UnityEngine;
using UnityEngine.EventSystems;

// Va sobre el "mango" del cuaderno (un GameObject hijo en la parte superior).
// Al arrastrarlo, mueve el RectTransform "objetivoArrastrar" (el cuaderno completo).
// Limita el movimiento para que siempre quede al menos un porcentaje visible en pantalla.
public class ArrastreCuaderno : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Tooltip("RectTransform del cuaderno completo (lo que se mueve)")]
    public RectTransform objetivoArrastrar;

    [Tooltip("Referencia al EnciclopediaController para leer el porcentaje minimo visible")]
    public EnciclopediaController controller;

    private RectTransform canvasRT;
    private Camera camaraUI;
    private Vector2 offsetArrastre;
    private bool arrastrando;

    void Awake()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRT = canvas.transform as RectTransform;
            camaraUI = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (objetivoArrastrar == null) return;
        arrastrando = true;

        Vector2 cursorLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT, eventData.position, camaraUI, out cursorLocal);

        offsetArrastre = objetivoArrastrar.anchoredPosition - cursorLocal;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!arrastrando || objetivoArrastrar == null) return;

        Vector2 cursorLocal;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT, eventData.position, camaraUI, out cursorLocal)) return;

        Vector2 nuevaPos = cursorLocal + offsetArrastre;

        // Limitar para que siempre quede al menos X% visible
        float pctMinVisible = controller != null && controller.config != null
            ? controller.config.porcentajeMinimoVisible
            : 0.5f;

        Rect canvasRect = canvasRT.rect;
        Rect cuadernoRect = objetivoArrastrar.rect;

        // Cuanto se puede ocultar = (1 - pctMinVisible) del tamaño del cuaderno
        float maxFueraX = cuadernoRect.width  * (1f - pctMinVisible);
        float maxFueraY = cuadernoRect.height * (1f - pctMinVisible);

        // Limites del canvas (asumiendo pivot 0.5 y anclas centradas)
        float minX = canvasRect.xMin + cuadernoRect.width  * 0.5f - maxFueraX;
        float maxX = canvasRect.xMax - cuadernoRect.width  * 0.5f + maxFueraX;
        float minY = canvasRect.yMin + cuadernoRect.height * 0.5f - maxFueraY;
        float maxY = canvasRect.yMax - cuadernoRect.height * 0.5f + maxFueraY;

        nuevaPos.x = Mathf.Clamp(nuevaPos.x, minX, maxX);
        nuevaPos.y = Mathf.Clamp(nuevaPos.y, minY, maxY);

        objetivoArrastrar.anchoredPosition = nuevaPos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        arrastrando = false;
    }
}
