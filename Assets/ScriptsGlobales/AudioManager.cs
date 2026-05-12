using UnityEngine;

// Manager centralizado de audio. Singleton que persiste entre escenas.
// Llamado desde otros scripts via AudioManager.Instance.SonarXxx()
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Configuracion de sonidos")]
    public AudioConfig config;

    private AudioSource sourceMusica;
    private AudioSource sourceSFX;

    private float ultimoTiempoPanel;
    private float ultimoTiempoClick;
    private float ultimoTiempoDecision;
    private const float COOLDOWN_REPETICION = 0.05f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sourceMusica = gameObject.AddComponent<AudioSource>();
        sourceMusica.loop = true;
        sourceMusica.playOnAwake = false;

        sourceSFX = gameObject.AddComponent<AudioSource>();
        sourceSFX.loop = false;
        sourceSFX.playOnAwake = false;
    }

    void Start()
    {
        ReproducirMusica();
    }

    public void ReproducirMusica()
    {
        if (config == null || config.musicaFondo == null) return;
        if (sourceMusica.isPlaying && sourceMusica.clip == config.musicaFondo) return;

        sourceMusica.clip = config.musicaFondo;
        sourceMusica.volume = config.volumenMusica;
        sourceMusica.Play();
    }

    public void DetenerMusica()
    {
        sourceMusica.Stop();
    }

    public void SonarEntradaPanel()
    {
        if (Time.unscaledTime - ultimoTiempoPanel < COOLDOWN_REPETICION) return;
        ultimoTiempoPanel = Time.unscaledTime;
        Reproducir(config?.sfxEntradaPanel);
    }

    public void SonarClick()
    {
        if (Time.unscaledTime - ultimoTiempoClick < COOLDOWN_REPETICION) return;
        ultimoTiempoClick = Time.unscaledTime;
        Reproducir(config?.sfxClickGenerico);
    }

    public void SonarDecision()
    {
        if (Time.unscaledTime - ultimoTiempoDecision < COOLDOWN_REPETICION) return;
        ultimoTiempoDecision = Time.unscaledTime;
        Reproducir(config?.sfxDecision);
    }

    public void SonarAcierto()  { Reproducir(config?.sfxAcierto); }
    public void SonarFallo()    { Reproducir(config?.sfxFallo); }
    public void SonarAbrirEnciclopedia()  { Reproducir(config?.sfxAbrirEnciclopedia); }
    public void SonarCerrarEnciclopedia() { Reproducir(config?.sfxCerrarEnciclopedia); }

    void Reproducir(AudioClip clip)
    {
        if (clip == null || config == null) return;
        sourceSFX.PlayOneShot(clip, config.volumenSFX);
    }
}
