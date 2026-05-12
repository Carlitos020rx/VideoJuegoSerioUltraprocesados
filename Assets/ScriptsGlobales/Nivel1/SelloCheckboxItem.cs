using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelloCheckboxItem : MonoBehaviour
{
    [Header("Referencias")]
    public Toggle toggle;
    public TMP_Text textoNombre;
    public Image checkboxImagen;

    public SelloNutricional Sello { get; private set; }
    public bool EstaMarcado => toggle != null && toggle.isOn;

    private SellosCheckboxController controller;

    public void Configurar(SelloNutricional sello, SellosCheckboxController ctrl)
    {
        Sello = sello;
        controller = ctrl;

        if (textoNombre != null)
        {
            textoNombre.text  = "Sello de " + sello.nombreSello.ToUpper();
            textoNombre.color = Color.black;
        }

        if (toggle != null)
        {
            toggle.isOn = false;
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    void OnToggleChanged(bool valor)
    {
        if (controller != null) controller.NotificarCambio();
    }

    public void MostrarValidacion(bool esCorrecto)
    {
        if (textoNombre != null)
            textoNombre.color = esCorrecto
                ? new Color(0.1f, 0.6f, 0.1f)
                : new Color(0.8f, 0.1f, 0.1f);

        if (toggle != null) toggle.interactable = false;
    }

    public void Bloquear()
    {
        if (toggle != null) toggle.interactable = false;
    }

    public void Desbloquear()
    {
        if (toggle != null) toggle.interactable = true;
    }
}
