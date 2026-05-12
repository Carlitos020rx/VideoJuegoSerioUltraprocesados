using System.Collections;
using UnityEngine;

public class AnimadorBotones : MonoBehaviour
{
    [Header("RectTransforms a animar")]
    public RectTransform btnAceptar;
    public RectTransform btnRechazar;
    public RectTransform btnSiguiente;
    public RectTransform panelResultado;
    public RectTransform panelResultadoDificil;

    [Header("Configuracion")]
    public float distanciaSalida = 400f;
    public float duracion = 0.4f;

    private Vector2 posAceptar, posRechazar, posSiguiente;
    private Vector2 posPanelResultado, posPanelResultadoDificil;

    void Awake()
    {
        if (btnAceptar             != null) posAceptar             = btnAceptar.anchoredPosition;
        if (btnRechazar            != null) posRechazar            = btnRechazar.anchoredPosition;
        if (btnSiguiente           != null) posSiguiente           = btnSiguiente.anchoredPosition;
        if (panelResultado         != null) posPanelResultado      = panelResultado.anchoredPosition;
        if (panelResultadoDificil  != null) posPanelResultadoDificil = panelResultadoDificil.anchoredPosition;
    }

    public void PrepararEstadoInicial()
    {
        if (btnAceptar  != null) btnAceptar.anchoredPosition  = posAceptar;
        if (btnRechazar != null) btnRechazar.anchoredPosition = posRechazar;

        if (btnSiguiente != null)
            btnSiguiente.anchoredPosition = posSiguiente + Vector2.down * distanciaSalida;
        if (panelResultado != null)
            panelResultado.anchoredPosition = posPanelResultado + Vector2.down * distanciaSalida;
        if (panelResultadoDificil != null)
            panelResultadoDificil.anchoredPosition = posPanelResultadoDificil + Vector2.down * distanciaSalida;
    }

    public IEnumerator OcultarAceptarRechazar()
    {
        yield return Mover2(
            btnAceptar,  posAceptar,  posAceptar  + Vector2.down * distanciaSalida,
            btnRechazar, posRechazar, posRechazar + Vector2.down * distanciaSalida);
    }

    public IEnumerator MostrarAceptarRechazar()
    {
        yield return Mover2(
            btnAceptar,  posAceptar  + Vector2.down * distanciaSalida, posAceptar,
            btnRechazar, posRechazar + Vector2.down * distanciaSalida, posRechazar);
    }

    public IEnumerator MostrarPanelResultadoYSiguiente()
    {
        yield return Mover2(
            panelResultado, posPanelResultado + Vector2.down * distanciaSalida, posPanelResultado,
            btnSiguiente,   posSiguiente      + Vector2.down * distanciaSalida, posSiguiente);
    }

    public IEnumerator OcultarPanelResultadoYSiguiente()
    {
        yield return Mover2(
            panelResultado, posPanelResultado, posPanelResultado + Vector2.down * distanciaSalida,
            btnSiguiente,   posSiguiente,      posSiguiente      + Vector2.down * distanciaSalida);
    }

    public IEnumerator MostrarPanelResultadoYSiguienteDificil()
    {
        yield return Mover2(
            panelResultadoDificil, posPanelResultadoDificil + Vector2.down * distanciaSalida, posPanelResultadoDificil,
            btnSiguiente,          posSiguiente             + Vector2.down * distanciaSalida, posSiguiente);
    }

    public IEnumerator OcultarPanelResultadoYSiguienteDificil()
    {
        yield return Mover2(
            panelResultadoDificil, posPanelResultadoDificil, posPanelResultadoDificil + Vector2.down * distanciaSalida,
            btnSiguiente,          posSiguiente,             posSiguiente             + Vector2.down * distanciaSalida);
    }

    IEnumerator Mover2(
        RectTransform a, Vector2 aDesde, Vector2 aHasta,
        RectTransform b, Vector2 bDesde, Vector2 bHasta)
    {
        float t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / duracion);
            if (a != null) a.anchoredPosition = Vector2.Lerp(aDesde, aHasta, p);
            if (b != null) b.anchoredPosition = Vector2.Lerp(bDesde, bHasta, p);
            yield return null;
        }
        if (a != null) a.anchoredPosition = aHasta;
        if (b != null) b.anchoredPosition = bHasta;
    }
}
