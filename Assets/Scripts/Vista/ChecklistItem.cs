using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Componente que va en el prefab de cada item del checklist (centro)
public class ChecklistItem : MonoBehaviour
{
    public TMP_Text textoIngrediente;
    public Image checkboxImagen;
    public Button boton;

    private string nombre;
    public IngredienteItem OrigenExpediente { get; private set; }
    private ChecklistController controller;

    public void Configurar(string nom, IngredienteItem origen, ChecklistController ctrl)
    {
        nombre           = nom;
        OrigenExpediente = origen;
        controller       = ctrl;

        if (textoIngrediente != null)
        {
            textoIngrediente.text  = nom;
            textoIngrediente.color = new Color(0.1f, 0.6f, 0.1f); // verde por defecto al marcar
        }

        if (boton != null)
        {
            boton.onClick.RemoveAllListeners();
            boton.onClick.AddListener(OnClick);
        }
    }

void OnClick() { if (OrigenExpediente != null) OrigenExpediente.Desmarcar(); if (controller != null) controller.QuitarItem(nombre); if (AudioManager.Instance != null) AudioManager.Instance.SonarClick(); }

    public void MostrarValidacion(bool esPeligrosoReal)
    {
        if (esPeligrosoReal)
            textoIngrediente.color = new Color(0.1f, 0.6f, 0.1f); // verde
        else
            textoIngrediente.color = new Color(0.8f, 0.1f, 0.1f); // rojo

        if (boton != null)
            boton.interactable = false; // Ya no se puede desmarcar despues de validar
    }
}
