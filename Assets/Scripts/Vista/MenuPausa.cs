using UnityEngine;

// Menu de pausa simple: ESC para abrir/cerrar. Pausa el tiempo del juego.
// Boton de salir vuelve al menu principal.
public class MenuPausa : MonoBehaviour
{
    [Header("Panel del menu de pausa (con fondo semitransparente y boton Salir)")]
    public GameObject panelPausa;

    private bool pausado = false;

    void Awake()
    {
        if (panelPausa != null) panelPausa.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePausa();
    }

    public void TogglePausa()
    {
        pausado = !pausado;
        if (panelPausa != null) panelPausa.SetActive(pausado);
        Time.timeScale = pausado ? 0f : 1f;
        if (AudioManager.Instance != null) AudioManager.Instance.SonarClick();
    }

    // Llamado por el boton "Salir" del panel
    public void SalirAlMenu()
    {
        Time.timeScale = 1f; // restaurar antes de cambiar de escena
        if (GameManager.Instance != null) GameManager.Instance.IrAlMenu();
    }

    void OnDisable()
    {
        // Por si se destruye el GameObject mientras el juego esta pausado
        Time.timeScale = 1f;
    }


public void Volver() { if (!pausado) return; pausado = false; if (panelPausa != null) panelPausa.SetActive(false); Time.timeScale = 1f; if (AudioManager.Instance != null) AudioManager.Instance.SonarClick(); }
}
