using System.Collections;
using UnityEngine;

// Controlador del presentador: segun el segmento actual del GameManager,
// elige el DialogoPresentadorData correspondiente y se lo pasa a la Vista (novela visual).
// Si el dialogo tiene tituloEntrada, primero ejecuta la transicion de titulo.
// Al terminar el dialogo, avisa al GameManager que el segmento termino.
public class PresentadorController : MonoBehaviour
{
    [Header("Vista de novela visual")]
    public PresentadorVisualNovel vista;

    [Header("Transicion de titulo entre segmentos")]
    public TransicionTituloPresentador transicionTitulo;

    [Tooltip("Transicion separada para el titulo final (Fin). Si no se asigna, usa la misma de arriba")]
    public TransicionTituloPresentador transicionTituloFinal;

    [Tooltip("Sprite del titulo final 'Fin' que se muestra al terminar el Cierre")]
    public Sprite tituloFin;

    [Header("Dialogos por segmento")]
    public DialogoPresentadorData dialogoIntro;
    public DialogoPresentadorData dialogoIntroSegundaParte;
    public DialogoPresentadorData dialogoCierre;

    void Start()
    {
        MostrarSegmentoActual();
    }

    public void MostrarSegmentoActual()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("[PresentadorController] No hay GameManager en escena");
            return;
        }

        DialogoPresentadorData dialogo = ElegirDialogo(GameManager.Instance.SegmentoActual);
        if (dialogo == null)
        {
            Debug.LogWarning("[PresentadorController] No hay dialogo asignado para el segmento " + GameManager.Instance.SegmentoActual);
            return;
        }

        // Si el dialogo tiene titulo de entrada, mostrar primero la transicion
        if (dialogo.tituloEntrada != null && transicionTitulo != null)
        {
            transicionTitulo.Mostrar(dialogo.tituloEntrada, () => ReproducirDialogo(dialogo));
        }
        else
        {
            ReproducirDialogo(dialogo);
        }
    }

    void ReproducirDialogo(DialogoPresentadorData dialogo)
    {
        if (vista != null)
            vista.Reproducir(dialogo, OnDialogoTerminado);
        else
            Debug.LogError("[PresentadorController] Falta asignar la Vista (PresentadorVisualNovel)");
    }

    // Llamado por el GameManager al terminar el segmento Cierre.
    // Muestra el titulo "Fin" + pantalla blanca y luego vuelve al menu.
public void MostrarTransicionFinal() { var t = transicionTituloFinal != null ? transicionTituloFinal : transicionTitulo; if (t != null && tituloFin != null) t.Mostrar(tituloFin, () => GameManager.Instance.IrAlMenu()); else GameManager.Instance.IrAlMenu(); }

    DialogoPresentadorData ElegirDialogo(GameManager.Segmento segmento)
    {
        switch (segmento)
        {
            case GameManager.Segmento.Intro:              return dialogoIntro;
            case GameManager.Segmento.IntroSegundaParte:  return dialogoIntroSegundaParte;
            case GameManager.Segmento.Cierre:             return dialogoCierre;
        }
        return null;
    }

    void OnDialogoTerminado()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SegmentoTerminado();
    }
}
