using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChecklistController : MonoBehaviour
{
    [Header("Panel del checklist (centro arriba)")]
    public Transform contenedorChecklist;
    public GameObject prefabChecklistItem;

    private int maxItems;
    private Dictionary<string, ChecklistItem> itemsActuales = new Dictionary<string, ChecklistItem>();

    public void LimpiarYConfigurar(AlimentoData alimento, int max)
    {
        LimpiarYConfigurarBase(max);
    }

    public void LimpiarYConfigurarDificil(AlimentoDificilData alimento, int max)
    {
        LimpiarYConfigurarBase(max);
    }

    void LimpiarYConfigurarBase(int max)
    {
        maxItems = max;
        foreach (Transform hijo in contenedorChecklist) Destroy(hijo.gameObject);
        itemsActuales.Clear();
    }

    public void AgregarItemAlChecklist(string nombre, IngredienteItem origen)
    {
        if (itemsActuales.ContainsKey(nombre)) return;
        if (itemsActuales.Count >= maxItems) return;

        GameObject go = Instantiate(prefabChecklistItem, contenedorChecklist);
        var item = go.GetComponent<ChecklistItem>();
        if (item != null)
        {
            item.Configurar(nombre, origen, this);
            itemsActuales.Add(nombre, item);
        }
    }

    public void QuitarItem(string nombre)
    {
        if (!itemsActuales.ContainsKey(nombre)) return;

        var item = itemsActuales[nombre];
        Destroy(item.gameObject);
        itemsActuales.Remove(nombre);

        if (Nivel1Manager.Instance != null)
            Nivel1Manager.Instance.DesmarcarIngrediente(nombre);
    }

    public void MostrarValidacion(List<string> ingredientesPeligrososReales)
    {
        foreach (var par in itemsActuales)
        {
            bool esPeligrosoReal = ingredientesPeligrososReales.Contains(par.Key);
            par.Value.MostrarValidacion(esPeligrosoReal);
            if (par.Value.OrigenExpediente != null)
                par.Value.OrigenExpediente.MostrarValidacion(esPeligrosoReal);
        }
    }
}
