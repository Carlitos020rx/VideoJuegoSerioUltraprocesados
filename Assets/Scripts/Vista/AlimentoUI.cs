using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlimentoUI : MonoBehaviour
{
    [Header("Panel Expediente (izquierda)")]
    public TMP_Text textoNombre;
    public TMP_Text textoCategoria;
    public Transform contenedorIngredientes;
    public GameObject prefabIngredienteItem;

    [Header("Panel Escaner (derecha)")]
    public Image imagenAlimento;

    public void MostrarAlimento(AlimentoData alimento)
    {
        SetBasico(alimento.nombre, alimento.categoria, alimento.sprite);
        RegenerarIngredientes(alimento.ingredientes);
    }

    public void MostrarAlimentoDificil(AlimentoDificilData alimento)
    {
        SetBasico(alimento.nombre, alimento.categoria, alimento.sprite);
        RegenerarIngredientes(alimento.ingredientes);
    }

    void SetBasico(string nombre, string categoria, Sprite sprite)
    {
        if (textoNombre    != null) textoNombre.text    = "Nombre: " + nombre;
        if (textoCategoria != null) textoCategoria.text = "Categoria: " + categoria;
        if (imagenAlimento != null)
        {
            imagenAlimento.sprite = sprite;
            imagenAlimento.preserveAspect = true;
        }
    }

    void RegenerarIngredientes(System.Collections.Generic.List<string> ingredientes)
    {
        foreach (Transform hijo in contenedorIngredientes) Destroy(hijo.gameObject);

        foreach (var ing in ingredientes)
        {
            GameObject item = Instantiate(prefabIngredienteItem, contenedorIngredientes);
            var ctrl = item.GetComponent<IngredienteItem>();
            if (ctrl != null) ctrl.Configurar(ing);
        }
    }
}
