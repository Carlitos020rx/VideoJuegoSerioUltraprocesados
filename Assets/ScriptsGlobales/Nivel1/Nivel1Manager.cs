using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nivel1Manager : MonoBehaviour
{
    public static Nivel1Manager Instance { get; private set; }

    [Header("Configuracion del nivel")]
    public Nivel1Config config;

    [Header("Componentes UI compartidos")]
    public AlimentoUI alimentoUI;
    public ChecklistController checklist;
    public ProgresoSellos progresoSellos;
    public FeedbackPantalla feedback;
    public AnimadorBotones animadorBotones;

    [Header("Feedback dificultad facil")]
    public PanelFeedbackResultado panelResultadoFacil;

    [Header("Componentes dificultad alta")]
    public InfoNutricionalUI infoNutricionalUI;
    public SellosCheckboxController sellosCheckbox;
    public PanelFeedbackResultadoDificil panelResultadoDificil;
    public AnimadorDificultad animadorDificultad;

    public AlimentoData        AlimentoActual        { get; private set; }
    public AlimentoDificilData AlimentoDificilActual { get; private set; }
    public int  Aciertos          { get; private set; }
    public bool DificultadAlta    { get; private set; }
    public bool EsperandoDecision { get; private set; }

    private List<string> ingredientesMarcados = new List<string>();
    private bool ultimoAcierto;

    void Awake() { Instance = this; }

    void Start()
    {
        if (config == null) { Debug.LogError("[Nivel1Manager] Falta asignar Nivel1Config"); return; }

        Aciertos = 0;
        DificultadAlta = false;

        if (sellosCheckbox != null && config.sellosDisponibles != null)
            sellosCheckbox.ConfigurarLista(config.sellosDisponibles);

        SiguienteAlimento(esPrimero: true);
    }

    public void OnAceptar()
    {
        if (!EsperandoDecision) return;
        if (AudioManager.Instance != null) AudioManager.Instance.SonarDecision();
        ProcesarDecision(decisionAceptar: true);
    }

    public void OnRechazar()
    {
        if (!EsperandoDecision) return;
        if (AudioManager.Instance != null) AudioManager.Instance.SonarDecision();
        ProcesarDecision(decisionAceptar: false);
    }

    public void OnSiguiente() { StartCoroutine(SecuenciaSiguiente()); }

    void ProcesarDecision(bool decisionAceptar)
    {
        EsperandoDecision = false;
        if (!DificultadAlta) ProcesarDecisionFacil(decisionAceptar);
        else                 ProcesarDecisionDificil(decisionAceptar);
    }

    void ProcesarDecisionFacil(bool decisionAceptar)
    {
        bool decisionCorrecta = (AlimentoActual.esUltraprocesado && !decisionAceptar)
                             || (!AlimentoActual.esUltraprocesado && decisionAceptar);
        var nombresPeligrosos = AlimentoActual.NombresPeligrosos();
        bool ingredientesOk = ValidarIngredientes(nombresPeligrosos);
        ultimoAcierto = decisionCorrecta && ingredientesOk;

        checklist.MostrarValidacion(nombresPeligrosos);
        feedback.MostrarFlash(verde: ultimoAcierto);

        if (AudioManager.Instance != null)
        {
            if (ultimoAcierto) AudioManager.Instance.SonarAcierto();
            else               AudioManager.Instance.SonarFallo();
        }

        if (ultimoAcierto) { progresoSellos.AgregarSello(); Aciertos++; }

        int totalPeligrosos = nombresPeligrosos.Count;
        int peligrososAcertados = 0;
        foreach (var marcado in ingredientesMarcados)
            if (nombresPeligrosos.Contains(marcado)) peligrososAcertados++;

        StartCoroutine(SecuenciaMostrarResultadoFacil(peligrososAcertados, totalPeligrosos));
    }

    void ProcesarDecisionDificil(bool decisionAceptar)
    {
        bool decisionCorrecta = (AlimentoDificilActual.esUltraprocesado && !decisionAceptar)
                             || (!AlimentoDificilActual.esUltraprocesado && decisionAceptar);
        var nombresPeligrosos = AlimentoDificilActual.NombresPeligrosos();
        bool ingredientesOk = ValidarIngredientes(nombresPeligrosos);
        bool sellosOk = sellosCheckbox.SonCorrectos(AlimentoDificilActual.sellosCorrectos);

        ultimoAcierto = decisionCorrecta && ingredientesOk && sellosOk;

        checklist.MostrarValidacion(nombresPeligrosos);
        sellosCheckbox.MostrarValidacion(AlimentoDificilActual.sellosCorrectos);
        feedback.MostrarFlash(verde: ultimoAcierto);

        if (AudioManager.Instance != null)
        {
            if (ultimoAcierto) AudioManager.Instance.SonarAcierto();
            else               AudioManager.Instance.SonarFallo();
        }

        if (ultimoAcierto) { progresoSellos.AgregarSello(); Aciertos++; }

        int totalPeligrosos = nombresPeligrosos.Count;
        int peligrososAcertados = 0;
        foreach (var marcado in ingredientesMarcados)
            if (nombresPeligrosos.Contains(marcado)) peligrososAcertados++;

        var sellosMarcados = sellosCheckbox.ObtenerSellosMarcados();
        int totalSellos = AlimentoDificilActual.sellosCorrectos.Count;
        int sellosAcertados = 0;
        foreach (var s in sellosMarcados)
            if (AlimentoDificilActual.sellosCorrectos.Contains(s)) sellosAcertados++;

        StartCoroutine(SecuenciaMostrarResultadoDificil(peligrososAcertados, totalPeligrosos,
            sellosAcertados, totalSellos, sellosMarcados));
    }

    bool ValidarIngredientes(List<string> peligrosos)
    {
        if (ingredientesMarcados.Count != peligrosos.Count) return false;
        foreach (var m in ingredientesMarcados)
            if (!peligrosos.Contains(m)) return false;
        return true;
    }

    IEnumerator SecuenciaMostrarResultadoFacil(int aciertos, int total)
    {
        yield return StartCoroutine(animadorBotones.OcultarAceptarRechazar());

        panelResultadoFacil.MostrarResultado(ultimoAcierto, aciertos, total, ingredientesMarcados, AlimentoActual);

        if (AudioManager.Instance != null) AudioManager.Instance.SonarEntradaPanel();

        yield return StartCoroutine(animadorBotones.MostrarPanelResultadoYSiguiente());
    }

    IEnumerator SecuenciaMostrarResultadoDificil(int ingAcertados, int totalIng, int sellosAcertados, int totalSellos, List<SelloNutricional> sellosMarcados)
    {
        yield return StartCoroutine(animadorBotones.OcultarAceptarRechazar());

        panelResultadoDificil.MostrarResultado(ultimoAcierto, ingAcertados, totalIng,
            sellosAcertados, totalSellos, ingredientesMarcados, sellosMarcados, AlimentoDificilActual);

        if (AudioManager.Instance != null) AudioManager.Instance.SonarEntradaPanel();

        yield return StartCoroutine(animadorBotones.MostrarPanelResultadoYSiguienteDificil());
    }

    IEnumerator SecuenciaSiguiente()
    {
        if (!DificultadAlta || Aciertos < config.aciertoCambioDificultad + 1)
            yield return StartCoroutine(animadorBotones.OcultarPanelResultadoYSiguiente());
        else
            yield return StartCoroutine(animadorBotones.OcultarPanelResultadoYSiguienteDificil());

        if (Aciertos >= config.aciertosParaGanar)
        {
            TerminarNivel();
            yield break;
        }

        if (!DificultadAlta && Aciertos >= config.aciertoCambioDificultad)
        {
            DificultadAlta = true;
            if (animadorDificultad != null)
            {
                if (AudioManager.Instance != null) AudioManager.Instance.SonarEntradaPanel();
                yield return StartCoroutine(animadorDificultad.EntrarDificultadAlta());
            }
        }

        SiguienteAlimento(esPrimero: false);
        yield return StartCoroutine(animadorBotones.MostrarAceptarRechazar());
    }

    void SiguienteAlimento(bool esPrimero)
    {
        ingredientesMarcados.Clear();

        if (!DificultadAlta)
        {
            int idx = Random.Range(0, config.alimentosDisponibles.Count);
            AlimentoActual = config.alimentosDisponibles[idx];
            alimentoUI.MostrarAlimento(AlimentoActual);
            checklist.LimpiarYConfigurar(AlimentoActual, config.maxIngredientesMarcados);
        }
        else
        {
            int idx = Random.Range(0, config.alimentosDificiles.Count);
            AlimentoDificilActual = config.alimentosDificiles[idx];
            alimentoUI.MostrarAlimentoDificil(AlimentoDificilActual);
            checklist.LimpiarYConfigurarDificil(AlimentoDificilActual, config.maxIngredientesMarcados);
            sellosCheckbox.ResetearParaNuevoAlimento();
            infoNutricionalUI.MostrarInfo(AlimentoDificilActual);
        }

        EsperandoDecision = true;

        if (esPrimero)
            animadorBotones.PrepararEstadoInicial();
    }

    void TerminarNivel()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.TerminarNivel1(Aciertos);
        else
            Debug.Log("[Nivel1Manager] Nivel terminado con " + Aciertos + " aciertos");
    }

    public bool IntentarMarcarIngrediente(string ingrediente)
    {
        if (ingredientesMarcados.Count >= config.maxIngredientesMarcados) return false;
        if (ingredientesMarcados.Contains(ingrediente)) return false;
        ingredientesMarcados.Add(ingrediente);
        return true;
    }

    public void DesmarcarIngrediente(string ingrediente)
    {
        ingredientesMarcados.Remove(ingrediente);
    }
}
