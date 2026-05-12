using UnityEngine;

// Controlador del panel que muestra la info nutricional del alimento dificil
public class InfoNutricionalUI : MonoBehaviour
{
    [Tooltip("Contenedor donde se generan las filas (debe tener Vertical Layout Group)")]
    public Transform contenedor;

    [Tooltip("Prefab de una fila (nombre - valor)")]
    public GameObject prefabItem;

    public void MostrarInfo(AlimentoDificilData alimento)
    {
        // Limpiar items anteriores
        foreach (Transform hijo in contenedor)
            Destroy(hijo.gameObject);

        if (alimento.infoNutricional == null) return;

        foreach (var dato in alimento.infoNutricional)
        {
            GameObject go = Instantiate(prefabItem, contenedor);
            var item = go.GetComponent<ItemInfoNutricional>();
            if (item != null)
                item.Configurar(dato.nombre, dato.valor);
        }
    }

    public void Limpiar()
    {
        foreach (Transform hijo in contenedor)
            Destroy(hijo.gameObject);
    }
}
