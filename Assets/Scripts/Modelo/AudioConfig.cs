using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "UltraProcessed/Audio Config", order = 10)]
public class AudioConfig : ScriptableObject
{
    [Header("Musica de fondo")]
    public AudioClip musicaFondo;
    [Range(0f, 1f)] public float volumenMusica = 0.4f;

    [Header("Efectos UI")]
    [Tooltip("Cuando entra un panel (cualquiera de los del juego)")]
    public AudioClip sfxEntradaPanel;

    [Tooltip("Click generico de boton/interactuable")]
    public AudioClip sfxClickGenerico;

    [Tooltip("Al presionar Aceptar o Rechazar")]
    public AudioClip sfxDecision;

    [Tooltip("Cuando acerta")]
    public AudioClip sfxAcierto;

    [Tooltip("Cuando falla")]
    public AudioClip sfxFallo;

    [Tooltip("Al abrir la enciclopedia")]
    public AudioClip sfxAbrirEnciclopedia;

    [Tooltip("Al cerrar la enciclopedia")]
    public AudioClip sfxCerrarEnciclopedia;

    [Tooltip("Al completar el nivel - panel final")]
    public AudioClip sfxVictoria;

    [Tooltip("Hover de boton (cuando el mouse pasa por encima)")]
    public AudioClip sfxHoverBoton;

    [Header("Volumen global de SFX")]
    [Range(0f, 1f)] public float volumenSFX = 0.8f;
}
