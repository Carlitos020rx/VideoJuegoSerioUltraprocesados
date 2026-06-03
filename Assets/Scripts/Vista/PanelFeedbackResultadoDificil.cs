using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelFeedbackResultadoDificil : MonoBehaviour
{
    [Header("Textos generales")]
    public TMP_Text textoTitulo;
    public TMP_Text textoSubtitulo;
    public TMP_Text textoContadorIngredientes;
    public TMP_Text textoContadorSellos;

    [Header("Contenedor de ingredientes")]
    public Transform contenedorIngredientes;
    public GameObject prefabItemIngrediente;

    [Header("Contenedor de sellos")]
    public Transform contenedorSellos;
    public GameObject prefabItemSello;

    [Header("Mensajes")]
    public string mensajeAcierto = "¡Acertaste!";
    public string mensajeFallo   = "¡Fallaste!";
    public string subtitulo      = "Resumen del alimento";
    public string mensajeNoPeligroso = "Este no es un ingrediente peligroso";
    public string mensajeSelloIncorrecto = "Este sello no aplica a este alimento";

    public void MostrarResultado(
        bool acerto,
        int ingredientesAcertados, int totalIngredientes,
        int sellosAcertados, int totalSellos,
        List<string> ingredientesMarcados,
        List<SelloNutricional> sellosMarcados,
        AlimentoDificilData alimento)
    {
        textoTitulo.text  = acerto ? mensajeAcierto : mensajeFallo;
        textoTitulo.color = acerto ? new Color(0.1f, 0.6f, 0.1f) : new Color(0.8f, 0.1f, 0.1f);
        textoSubtitulo.text = subtitulo;
        textoContadorIngredientes.text = "Ingredientes: " + ingredientesAcertados + "/" + totalIngredientes;
        textoContadorSellos.text       = "Sellos: " + sellosAcertados + "/" + totalSellos;

        // Limpiar
        foreach (Transform h in contenedorIngredientes) Destroy(h.gameObject);
        foreach (Transform h in contenedorSellos)       Destroy(h.gameObject);

        var nombresPeligrosos = alimento.NombresPeligrosos();

        // Items de ingredientes marcados
        foreach (var marcado in ingredientesMarcados)
        {
            GameObject go = Instantiate(prefabItemIngrediente, contenedorIngredientes);
            var item = go.GetComponent<ItemDescripcionFeedback>();
            if (item == null) continue;

            bool esPeligroso = nombresPeligrosos.Contains(marcado);
            string desc = esPeligroso
                ? alimento.GetDescripcionPeligroso(marcado)
                : mensajeNoPeligroso;

            item.Configurar(marcado, desc, esPeligroso);
        }

        // Items de sellos marcados
        foreach (var sello in sellosMarcados)
        {
            GameObject go = Instantiate(prefabItemSello, contenedorSellos);
            var item = go.GetComponent<ItemDescripcionSelloFeedback>();
            if (item == null) continue;

            bool esCorrecto = alimento.sellosCorrectos.Contains(sello);
            string desc = esCorrecto ? sello.descripcion : mensajeSelloIncorrecto;
            item.Configurar(sello, desc, esCorrecto);
        }
    }
}
