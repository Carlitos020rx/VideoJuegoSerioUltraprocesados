using System.Collections;
using UnityEngine;

// Anima los paneles de expediente y checklist a sus posiciones de dificultad alta
// y trae los nuevos paneles (info nutricional y sellos) desde fuera de pantalla.
public class AnimadorDificultad : MonoBehaviour
{
    [Header("Paneles dificultad facil (se mueven)")]
    public RectTransform panelExpediente;
    public RectTransform panelChecklist;

    [Header("Posiciones dificultad alta para los paneles existentes")]
    public Vector2 posExpedienteDificil;
    public Vector2 posChecklistDificil;

    [Header("Paneles nuevos (entran desde abajo)")]
    public RectTransform panelInfoNutricional;
    public RectTransform panelSellosCheckbox;

    [Header("Configuracion")]
    public float duracion = 0.6f;
    public float distanciaSalidaNuevos = 600f;

    private Vector2 posExpedienteFacil;
    private Vector2 posChecklistFacil;

    private Vector2 posInfoNutricionalOriginal;
    private Vector2 posSellosOriginal;

    void Awake()
    {
        if (panelExpediente != null) posExpedienteFacil = panelExpediente.anchoredPosition;
        if (panelChecklist  != null) posChecklistFacil  = panelChecklist.anchoredPosition;

        if (panelInfoNutricional != null) posInfoNutricionalOriginal = panelInfoNutricional.anchoredPosition;
        if (panelSellosCheckbox  != null) posSellosOriginal          = panelSellosCheckbox.anchoredPosition;

        // Esconder paneles nuevos al inicio
        if (panelInfoNutricional != null)
        {
            panelInfoNutricional.anchoredPosition = posInfoNutricionalOriginal + Vector2.down * distanciaSalidaNuevos;
            panelInfoNutricional.gameObject.SetActive(false);
        }
        if (panelSellosCheckbox != null)
        {
            panelSellosCheckbox.anchoredPosition = posSellosOriginal + Vector2.down * distanciaSalidaNuevos;
            panelSellosCheckbox.gameObject.SetActive(false);
        }
    }

    public IEnumerator EntrarDificultadAlta()
    {
        // Activar paneles nuevos pero fuera de pantalla
        if (panelInfoNutricional != null) panelInfoNutricional.gameObject.SetActive(true);
        if (panelSellosCheckbox  != null) panelSellosCheckbox.gameObject.SetActive(true);

        // Posiciones de partida y destino
        Vector2 expedienteDesde = panelExpediente != null ? panelExpediente.anchoredPosition : Vector2.zero;
        Vector2 checklistDesde  = panelChecklist  != null ? panelChecklist.anchoredPosition  : Vector2.zero;
        Vector2 infoDesde       = panelInfoNutricional != null ? panelInfoNutricional.anchoredPosition : Vector2.zero;
        Vector2 sellosDesde     = panelSellosCheckbox  != null ? panelSellosCheckbox.anchoredPosition  : Vector2.zero;

        float t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duracion);

            if (panelExpediente != null)
                panelExpediente.anchoredPosition = Vector2.Lerp(expedienteDesde, posExpedienteDificil, p);
            if (panelChecklist != null)
                panelChecklist.anchoredPosition = Vector2.Lerp(checklistDesde, posChecklistDificil, p);
            if (panelInfoNutricional != null)
                panelInfoNutricional.anchoredPosition = Vector2.Lerp(infoDesde, posInfoNutricionalOriginal, p);
            if (panelSellosCheckbox != null)
                panelSellosCheckbox.anchoredPosition = Vector2.Lerp(sellosDesde, posSellosOriginal, p);

            yield return null;
        }

        if (panelExpediente      != null) panelExpediente.anchoredPosition      = posExpedienteDificil;
        if (panelChecklist       != null) panelChecklist.anchoredPosition       = posChecklistDificil;
        if (panelInfoNutricional != null) panelInfoNutricional.anchoredPosition = posInfoNutricionalOriginal;
        if (panelSellosCheckbox  != null) panelSellosCheckbox.anchoredPosition  = posSellosOriginal;
    }
}
