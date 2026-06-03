using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelFeedbackResultado : MonoBehaviour
{
    [Header("Textos del panel")]
    public TMP_Text textoTitulo;
    public TMP_Text textoSubtitulo;
    public TMP_Text textoContador;

    [Header("Contenedor donde se generan los items de descripcion")]
    public Transform contenedorItems;
    public GameObject prefabItemDescripcion;

    [Header("Mensajes")]
    public string mensajeAcierto = "¡Acertaste!";
    public string mensajeFallo   = "¡Fallaste!";
    public string subtitulo      = "Este es un resumen de los ingredientes";
    public string mensajeNoPeligroso = "Este no es un ingrediente peligroso";

    public void MostrarResultado(
        bool acerto,
        int peligrososAcertados,
        int totalPeligrosos,
        List<string> ingredientesMarcados,
        AlimentoData alimento)
    {
        textoTitulo.text    = acerto ? mensajeAcierto : mensajeFallo;
        textoTitulo.color   = acerto ? new Color(0.1f, 0.6f, 0.1f) : new Color(0.8f, 0.1f, 0.1f);
        textoSubtitulo.text = subtitulo;
        textoContador.text  = peligrososAcertados + "/" + totalPeligrosos;

        // Limpiar items anteriores
        foreach (Transform hijo in contenedorItems)
            Destroy(hijo.gameObject);

        var nombresPeligrosos = alimento.NombresPeligrosos();

        // Generar un item por cada ingrediente marcado
        foreach (var marcado in ingredientesMarcados)
        {
            GameObject go = Instantiate(prefabItemDescripcion, contenedorItems);
            var item = go.GetComponent<ItemDescripcionFeedback>();
            if (item == null) continue;

            bool esPeligroso = nombresPeligrosos.Contains(marcado);
            string descripcion = esPeligroso
                ? alimento.GetDescripcionPeligroso(marcado)
                : mensajeNoPeligroso;

            item.Configurar(marcado, descripcion, esPeligroso);
        }
    }
}
