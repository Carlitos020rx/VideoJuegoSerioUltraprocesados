using System.Collections.Generic;
using UnityEngine;

public class SellosCheckboxController : MonoBehaviour
{
    [Tooltip("Contenedor donde se generan los checkboxes de sellos")]
    public Transform contenedor;

    [Tooltip("Prefab de un item con toggle, label y checkbox")]
    public GameObject prefabSelloCheckbox;

    private List<SelloCheckboxItem> itemsActuales = new List<SelloCheckboxItem>();

    public void ConfigurarLista(List<SelloNutricional> sellosDisponibles)
    {
        foreach (Transform hijo in contenedor) Destroy(hijo.gameObject);
        itemsActuales.Clear();

        foreach (var sello in sellosDisponibles)
        {
            GameObject go = Instantiate(prefabSelloCheckbox, contenedor);
            var item = go.GetComponent<SelloCheckboxItem>();
            if (item != null)
            {
                item.Configurar(sello, this);
                itemsActuales.Add(item);
            }
        }
    }

    // Devuelve los sellos que el jugador marco
    public List<SelloNutricional> ObtenerSellosMarcados()
    {
        var lista = new List<SelloNutricional>();
        foreach (var item in itemsActuales)
            if (item.EstaMarcado) lista.Add(item.Sello);
        return lista;
    }

    // Verifica si los marcados coinciden EXACTAMENTE con los correctos
    public bool SonCorrectos(List<SelloNutricional> sellosCorrectos)
    {
        var marcados = ObtenerSellosMarcados();
        if (marcados.Count != sellosCorrectos.Count) return false;
        foreach (var s in marcados)
            if (!sellosCorrectos.Contains(s)) return false;
        return true;
    }

    // Pinta verde/rojo cada checkbox segun si era correcto
    public void MostrarValidacion(List<SelloNutricional> sellosCorrectos)
    {
        foreach (var item in itemsActuales)
        {
            if (!item.EstaMarcado) continue;
            bool esCorrecto = sellosCorrectos.Contains(item.Sello);
            item.MostrarValidacion(esCorrecto);
        }
    }

    public void ResetearParaNuevoAlimento()
    {
        foreach (var item in itemsActuales)
        {
            if (item.toggle != null) item.toggle.isOn = false;
            item.Desbloquear();
            if (item.textoNombre != null) item.textoNombre.color = Color.black;
        }
    }

    public void NotificarCambio()
    {
        // Hook por si despues queremos limitar cantidad maxima de checks, etc.
    }
}
