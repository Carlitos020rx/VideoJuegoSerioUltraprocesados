using UnityEngine;
using UnityEngine.UI;

// Maneja la barra de 6 sellos OK que se llenan con cada acierto
public class ProgresoSellos : MonoBehaviour
{
    [Header("Slots de sellos (asignar en Inspector — uno por cada sello posible)")]
    [Tooltip("Lista de Image en orden. Empiezan vacios (alpha 0) y se rellenan al acertar.")]
    public Image[] slotsSellos;

    [Header("Sprite del sello OK")]
    public Sprite spriteSelloOK;

    private int aciertosVisuales = 0;

    void Start()
    {
        // Inicializar todos vacios
        foreach (var s in slotsSellos)
        {
            if (s != null)
            {
                s.sprite = spriteSelloOK;
                var c = s.color;
                c.a = 0f;
                s.color = c;
            }
        }
        aciertosVisuales = 0;
    }

    public void AgregarSello()
    {
        if (aciertosVisuales >= slotsSellos.Length) return;

        var slot = slotsSellos[aciertosVisuales];
        if (slot != null)
        {
            var c = slot.color;
            c.a = 1f;
            slot.color = c;
        }
        aciertosVisuales++;
    }

    public void Resetear()
    {
        aciertosVisuales = 0;
        Start();
    }
}
