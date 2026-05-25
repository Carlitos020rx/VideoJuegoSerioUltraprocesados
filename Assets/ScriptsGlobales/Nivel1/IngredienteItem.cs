using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Componente que va en el prefab de cada item de ingrediente del expediente.
// Es un Text con un Button — al clickear se manda al checklist.
public class IngredienteItem : MonoBehaviour
{
    public TMP_Text textoIngrediente;
    public Button boton;

    private string nombreIngrediente;
    private bool yaEnChecklist = false;

    public void Configurar(string nombre)
    {
        nombreIngrediente = nombre;
        if (textoIngrediente != null)
            textoIngrediente.text = nombre;

        textoIngrediente.color = Color.black;

        if (boton != null)
        {
            boton.onClick.RemoveAllListeners();
            boton.onClick.AddListener(OnClick);
        }
    }

void OnClick() { if (yaEnChecklist) return; if (Nivel1Manager.Instance == null) return; bool agregado = Nivel1Manager.Instance.IntentarMarcarIngrediente(nombreIngrediente); if (agregado) { yaEnChecklist = true; textoIngrediente.color = new Color(0.1f, 0.6f, 0.1f); Nivel1Manager.Instance.checklist.AgregarItemAlChecklist(nombreIngrediente, this); if (AudioManager.Instance != null) AudioManager.Instance.SonarClick(); } }

    // Llamado por el ChecklistController cuando el jugador desmarca el item desde el checklist
    public void Desmarcar()
    {
        yaEnChecklist = false;
        textoIngrediente.color = Color.black;
    }

    // Llamado al validar la respuesta — colorea segun si era peligroso o no
    public void MostrarValidacion(bool esPeligrosoReal)
    {
        if (esPeligrosoReal)
            textoIngrediente.color = new Color(0.1f, 0.6f, 0.1f); // verde
        else
            textoIngrediente.color = new Color(0.8f, 0.1f, 0.1f); // rojo
    }
}
