using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimadorPaneles : MonoBehaviour
{
    public enum Direccion { Izquierda, Derecha, Arriba, Abajo }

    [System.Serializable]
    public class PanelAnimado
    {
        public RectTransform panel;
        public Direccion direccionSalida;
        [Tooltip("Si es true, este panel solo se anima cuando esta en dificultad alta")]
        public bool soloDificultadAlta;
        [HideInInspector] public Vector2 posicionOriginal;
    }

    [Header("Paneles que se animan")]
    public List<PanelAnimado> paneles = new List<PanelAnimado>();

    [Header("Configuracion")]
    [Tooltip("Distancia extra fuera de pantalla (px) para asegurar que salen completos")]
    public float margenFuera = 200f;
    public float duracion = 0.5f;

    private float anchoCanvas = 1920f;
    private float altoCanvas = 1080f;

void Awake() { var canvas = GetComponentInParent<Canvas>(); if (canvas != null) { var rt = canvas.transform as RectTransform; if (rt != null) { anchoCanvas = rt.rect.width; altoCanvas = rt.rect.height; } } foreach (var p in paneles) if (p.panel != null) p.posicionOriginal = p.panel.anchoredPosition; ColocarFuera(false); }

    Vector2 PosicionFuera(PanelAnimado p)
    {
        Vector2 origen = p.posicionOriginal;
        switch (p.direccionSalida)
        {
            case Direccion.Izquierda: return new Vector2(-anchoCanvas - margenFuera, origen.y);
            case Direccion.Derecha:   return new Vector2( anchoCanvas + margenFuera, origen.y);
            case Direccion.Arriba:    return new Vector2(origen.x,  altoCanvas + margenFuera);
            case Direccion.Abajo:     return new Vector2(origen.x, -altoCanvas - margenFuera);
        }
        return origen;
    }

    // Decide si un panel participa segun la dificultad actual
    bool Participa(PanelAnimado p, bool dificultadAlta)
    {
        if (p.panel == null) return false;
        if (p.soloDificultadAlta && !dificultadAlta) return false;
        return true;
    }

    // Coloca fuera de pantalla los paneles que participan
    public void ColocarFuera(bool dificultadAlta)
    {
        foreach (var p in paneles)
            if (Participa(p, dificultadAlta))
                p.panel.anchoredPosition = PosicionFuera(p);
    }

    public IEnumerator Salir(bool dificultadAlta)
    {
        yield return StartCoroutine(Animar(true, dificultadAlta));
    }

    public IEnumerator Entrar(bool dificultadAlta)
    {
        yield return StartCoroutine(Animar(false, dificultadAlta));
    }

    IEnumerator Animar(bool haciaFuera, bool dificultadAlta)
    {
        var activos   = new List<PanelAnimado>();
        var inicios   = new List<Vector2>();
        var destinos  = new List<Vector2>();

        foreach (var p in paneles)
        {
            if (!Participa(p, dificultadAlta)) continue;
            activos.Add(p);
            inicios.Add(p.panel.anchoredPosition);
            destinos.Add(haciaFuera ? PosicionFuera(p) : p.posicionOriginal);
        }

        float t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float prog = Mathf.SmoothStep(0f, 1f, t / duracion);
            for (int i = 0; i < activos.Count; i++)
                activos[i].panel.anchoredPosition = Vector2.Lerp(inicios[i], destinos[i], prog);
            yield return null;
        }

        for (int i = 0; i < activos.Count; i++)
            activos[i].panel.anchoredPosition = destinos[i];
    }


public void ActualizarPosicionOriginal(RectTransform panel, Vector2 nuevaPos) { foreach (var p in paneles) if (p.panel == panel) { p.posicionOriginal = nuevaPos; return; } }
}
