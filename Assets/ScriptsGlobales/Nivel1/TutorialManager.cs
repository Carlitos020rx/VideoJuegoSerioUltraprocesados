using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// Tutorial estilo novela visual: typewriter + sonido de tecleo en loop + imagen.
// El tecleo suena mientras se escribe y se pausa al terminar el texto.
// Click para completar/avanzar. Boton para saltar.
public class TutorialManager : MonoBehaviour, IPointerClickHandler
{
    [Header("Datos del tutorial")]
    public TutorialData tutorial;

    [Header("Referencias UI")]
    [Tooltip("Contenedor raiz del tutorial (se desactiva al terminar)")]
    public GameObject contenedorTutorial;

    [Tooltip("Texto donde se escribe el dialogo")]
    public TMP_Text textoDialogo;

    [Tooltip("Imagen que muestra la captura del paso actual")]
    public Image imagenPaso;

    [Tooltip("Boton para saltar todo el tutorial")]
    public Button botonSaltar;

    [Header("Configuracion typewriter")]
    [Tooltip("Segundos entre cada caracter")]
    public float velocidadTypewriter = 0.03f;

    [Header("Sonido de tecleo")]
    [Tooltip("Clip de tecleo que suena en loop mientras se escribe")]
    public AudioClip sonidoTecleo;

    [Tooltip("Velocidad de reproduccion del tecleo (2 = x2)")]
    public float velocidadTecleo = 2f;

    [Range(0f, 1f)]
    public float volumenTecleo = 1f;

    [Header("Referencia al manager del nivel")]
    public Nivel1Manager nivel1Manager;

    private int pasoActual = 0;
    private bool escribiendo = false;
    private string textoCompleto = "";
    private Coroutine corrTypewriter;
    private AudioSource sourceTecleo;

void Start() { sourceTecleo = gameObject.AddComponent<AudioSource>(); sourceTecleo.playOnAwake = false; sourceTecleo.loop = true; sourceTecleo.clip = sonidoTecleo; sourceTecleo.pitch = velocidadTecleo; sourceTecleo.volume = volumenTecleo; if (botonSaltar != null) botonSaltar.onClick.AddListener(SaltarTutorial); }

    void MostrarPaso(int indice)
    {
        var paso = tutorial.pasos[indice];

        // Imagen
        if (imagenPaso != null)
        {
            if (paso.imagen != null)
            {
                imagenPaso.sprite = paso.imagen;
                imagenPaso.preserveAspect = true;
                imagenPaso.enabled = true;
            }
            else
            {
                imagenPaso.enabled = false;
            }
        }

        // Typewriter
        textoCompleto = paso.texto;
        if (corrTypewriter != null) StopCoroutine(corrTypewriter);
        corrTypewriter = StartCoroutine(Typewriter());
    }

    IEnumerator Typewriter()
    {
        escribiendo = true;
        textoDialogo.text = "";

        // Empezar el tecleo
        IniciarTecleo();

        foreach (char c in textoCompleto)
        {
            textoDialogo.text += c;
            yield return new WaitForSeconds(velocidadTypewriter);
        }

        escribiendo = false;
        DetenerTecleo();
    }

    void IniciarTecleo()
    {
        if (sourceTecleo != null && sonidoTecleo != null && !sourceTecleo.isPlaying)
            sourceTecleo.Play();
    }

    void DetenerTecleo()
    {
        if (sourceTecleo != null && sourceTecleo.isPlaying)
            sourceTecleo.Stop();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Avanzar();
    }

    void Avanzar()
    {
        if (escribiendo)
        {
            // Completar el texto de golpe
            if (corrTypewriter != null) StopCoroutine(corrTypewriter);
            textoDialogo.text = textoCompleto;
            escribiendo = false;
            DetenerTecleo();
            return;
        }

        // Pasar al siguiente paso
        pasoActual++;
        if (pasoActual >= tutorial.pasos.Count)
            TerminarTutorial();
        else
            MostrarPaso(pasoActual);
    }

    void SaltarTutorial()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SonarClick();
        DetenerTecleo();
        TerminarTutorial();
    }

    void TerminarTutorial()
    {
        DetenerTecleo();
        if (contenedorTutorial != null) contenedorTutorial.SetActive(false);

        if (nivel1Manager != null)
            nivel1Manager.IniciarJuegoDespuesDeTutorial();
    }


public void IniciarTutorial() { if (tutorial == null || tutorial.pasos.Count == 0) { Debug.LogWarning("[TutorialManager] No hay pasos, arrancando juego directo"); TerminarTutorial(); return; } if (contenedorTutorial != null) contenedorTutorial.SetActive(true); pasoActual = 0; MostrarPaso(pasoActual); }
}
