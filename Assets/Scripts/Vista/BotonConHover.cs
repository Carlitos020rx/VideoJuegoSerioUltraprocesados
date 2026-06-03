using UnityEngine;
using UnityEngine.EventSystems;

// Agregalo a cualquier elemento UI (boton, panel) para que suene un click de hover
// cuando el mouse pase por encima. Usa el sfxHoverBoton del AudioConfig.
public class BotonConHover : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SonarHover();
    }
}
