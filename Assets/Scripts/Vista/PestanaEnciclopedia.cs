using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PestanaEnciclopedia : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private EnciclopediaController controller;
    private RectTransform rt;
    private Vector3 escalaOriginal;
    private Coroutine corrEscala;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        escalaOriginal = rt.localScale;
    }

    public void SetController(EnciclopediaController ctrl)
    {
        controller = ctrl;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (controller == null) return;
        if (corrEscala != null) StopCoroutine(corrEscala);
        corrEscala = StartCoroutine(EscalarA(escalaOriginal * controller.config.escalaHover, controller.config.duracionHover));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (controller == null) return;
        if (corrEscala != null) StopCoroutine(corrEscala);
        corrEscala = StartCoroutine(EscalarA(escalaOriginal, controller.config.duracionHover));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controller != null) controller.Abrir();
    }

    IEnumerator EscalarA(Vector3 destino, float duracion)
    {
        Vector3 inicio = rt.localScale;
        float t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duracion);
            rt.localScale = Vector3.Lerp(inicio, destino, p);
            yield return null;
        }
        rt.localScale = destino;
    }
}
