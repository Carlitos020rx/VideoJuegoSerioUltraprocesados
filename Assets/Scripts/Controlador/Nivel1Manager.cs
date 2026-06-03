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
    public AnimadorPaneles animadorPaneles;
    public PanelFinalNivel panelFinal;

    [Tooltip("Transicion animada al pasar a dificultad alta (negro + titulo + tutorial 2)")]
    public TransicionDificultad transicionDificultad;

    [Header("Tutorial")]
    [Tooltip("Si es true, el juego espera a que el tutorial termine antes de arrancar")]
    public bool usarTutorial = true;

    [Header("Feedback dificultad facil")]
    public PanelFeedbackResultado panelResultadoFacil;

    [Header("Componentes dificultad alta")]
    public InfoNutricionalUI infoNutricionalUI;
    public SellosCheckboxController sellosCheckbox;
    public PanelFeedbackResultadoDificil panelResultadoDificil;

    [Tooltip("Modal del Simulador de Porciones (aparece una vez por tipo de alimento dificil)")]
    public SimuladorPorciones simuladorPorciones;

    [Tooltip("Sistema de anuncios invasivos que aparecen en dificultad alta")]
    public AnunciosInvasivos anunciosInvasivos;

    [Tooltip("Mecanica de Perdida de Memoria. Se activa desde el segundo alimento")]
    public PerdidaMemoria perdidaMemoria;

    [Tooltip("Paneles nuevos de dificultad alta que empiezan ocultos")]
    public GameObject panelInfoNutricionalGO;
    public GameObject panelSellosCheckboxGO;

    [Header("Audio")]
    [Tooltip("Retraso entre el sonido de decision (sello) y el de acierto/fallo")]
    public float delaySonidoResultado = 0.4f;
    [Tooltip("Retraso antes de que suba el panel de feedback y el boton Siguiente")]
    public float delayPanelResultado = 0.8f;
    [Tooltip("Retraso antes de que bajen los botones Aceptar/Rechazar tras presionarlos")]
    public float delayBajarBotones = 0.5f;

    public AlimentoData        AlimentoActual        { get; private set; }
    public AlimentoDificilData AlimentoDificilActual { get; private set; }
    public int  Aciertos          { get; private set; }
    public bool DificultadAlta    { get; private set; }
    public bool EsperandoDecision { get; private set; }

    private List<string> ingredientesMarcados = new List<string>();
    private bool ultimoAcierto;
    private bool transicionTerminada;
    private bool panelResultadoDificilAbierto;
    private bool simuladorTerminado;
    private int alimentosMostrados = 0;
    private HashSet<string> tiposDificilesYaVistos = new HashSet<string>();

    void Awake() { Instance = this; }

void Start() { if (config == null) { Debug.LogError("[Nivel1Manager] Falta asignar Nivel1Config"); return; } Aciertos = 0; DificultadAlta = false; if (sellosCheckbox != null && config.sellosDisponibles != null) sellosCheckbox.ConfigurarLista(config.sellosDisponibles); if (panelInfoNutricionalGO != null) panelInfoNutricionalGO.SetActive(false); if (panelSellosCheckboxGO != null) panelSellosCheckboxGO.SetActive(false); if (animadorPaneles != null) animadorPaneles.ColocarFuera(false); if (!usarTutorial) StartCoroutine(SecuenciaPrimerAlimento()); }

IEnumerator SecuenciaPrimerAlimento() { if (animadorPaneles != null) animadorPaneles.ColocarFuera(false); SiguienteAlimento(esPrimero: true); yield return null; if (AudioManager.Instance != null) AudioManager.Instance.SonarEntradaPanel(); if (animadorPaneles != null) yield return StartCoroutine(animadorPaneles.Entrar(false)); }

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
        StartCoroutine(SonarResultadoConDelay());

        if (ultimoAcierto) { progresoSellos.AgregarSello(); Aciertos++; }

        int total = nombresPeligrosos.Count;
        int acertados = 0;
        foreach (var m in ingredientesMarcados)
            if (nombresPeligrosos.Contains(m)) acertados++;

        StartCoroutine(SecuenciaMostrarResultadoFacil(acertados, total));
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
        StartCoroutine(SonarResultadoConDelay());

        if (ultimoAcierto) { progresoSellos.AgregarSello(); Aciertos++; }

        int totalIng = nombresPeligrosos.Count;
        int acertadosIng = 0;
        foreach (var m in ingredientesMarcados)
            if (nombresPeligrosos.Contains(m)) acertadosIng++;

        var sellosMarcados = sellosCheckbox.ObtenerSellosMarcados();
        int totalSellos = AlimentoDificilActual.sellosCorrectos.Count;
        int acertadosSellos = 0;
        foreach (var s in sellosMarcados)
            if (AlimentoDificilActual.sellosCorrectos.Contains(s)) acertadosSellos++;

        StartCoroutine(SecuenciaMostrarResultadoDificil(acertadosIng, totalIng,
            acertadosSellos, totalSellos, sellosMarcados));
    }

    IEnumerator SonarResultadoConDelay()
    {
        yield return new WaitForSeconds(delaySonidoResultado);
        if (AudioManager.Instance != null)
        {
            if (ultimoAcierto) AudioManager.Instance.SonarAcierto();
            else               AudioManager.Instance.SonarFallo();
        }
    }

    bool ValidarIngredientes(List<string> peligrosos)
    {
        if (ingredientesMarcados.Count != peligrosos.Count) return false;
        foreach (var m in ingredientesMarcados)
            if (!peligrosos.Contains(m)) return false;
        return true;
    }

IEnumerator SecuenciaMostrarResultadoFacil(int aciertos, int total) { yield return new WaitForSeconds(delayBajarBotones); yield return StartCoroutine(animadorBotones.OcultarAceptarRechazar()); yield return new WaitForSeconds(delayPanelResultado); panelResultadoDificilAbierto = false; if (anunciosInvasivos != null) anunciosInvasivos.Pausar(); if (perdidaMemoria != null) perdidaMemoria.Desactivar(); panelResultadoFacil.MostrarResultado(ultimoAcierto, aciertos, total, ingredientesMarcados, AlimentoActual); if (AudioManager.Instance != null) AudioManager.Instance.SonarEntradaPanel(); yield return StartCoroutine(animadorBotones.MostrarPanelResultadoYSiguiente()); }

IEnumerator SecuenciaMostrarResultadoDificil(int ingAcertados, int totalIng, int sellosAcertados, int totalSellos, List<SelloNutricional> sellosMarcados) { yield return new WaitForSeconds(delayBajarBotones); yield return StartCoroutine(animadorBotones.OcultarAceptarRechazar()); yield return new WaitForSeconds(delayPanelResultado); panelResultadoDificilAbierto = true; if (anunciosInvasivos != null) anunciosInvasivos.Pausar(); if (perdidaMemoria != null) perdidaMemoria.Desactivar(); panelResultadoDificil.MostrarResultado(ultimoAcierto, ingAcertados, totalIng, sellosAcertados, totalSellos, ingredientesMarcados, sellosMarcados, AlimentoDificilActual); if (AudioManager.Instance != null) AudioManager.Instance.SonarEntradaPanel(); yield return StartCoroutine(animadorBotones.MostrarPanelResultadoYSiguienteDificil()); }

IEnumerator SecuenciaSiguiente() { yield return new WaitForSeconds(delayBajarBotones); if (panelResultadoDificilAbierto) yield return StartCoroutine(animadorBotones.OcultarPanelResultadoYSiguienteDificil()); else yield return StartCoroutine(animadorBotones.OcultarPanelResultadoYSiguiente()); if (Aciertos >= config.aciertosParaGanar) { TerminarNivel(); yield break; } bool acabaDeCambiar = false; if (!DificultadAlta && Aciertos >= config.aciertoCambioDificultad) { DificultadAlta = true; acabaDeCambiar = true; } if (animadorPaneles != null) { if (AudioManager.Instance != null) AudioManager.Instance.SonarEntradaPanel(); yield return StartCoroutine(animadorPaneles.Salir(DificultadAlta && !acabaDeCambiar)); } if (acabaDeCambiar) { if (transicionDificultad != null) { transicionTerminada = false; transicionDificultad.Iniciar(() => transicionTerminada = true); yield return new WaitUntil(() => transicionTerminada); } if (panelInfoNutricionalGO != null) panelInfoNutricionalGO.SetActive(true); if (panelSellosCheckboxGO != null) panelSellosCheckboxGO.SetActive(true); if (animadorPaneles != null) animadorPaneles.ColocarFuera(true); if (anunciosInvasivos != null) anunciosInvasivos.Activar(); } if (anunciosInvasivos != null) anunciosInvasivos.Reanudar(); SiguienteAlimento(esPrimero: false); if (animadorPaneles != null) { if (AudioManager.Instance != null) AudioManager.Instance.SonarEntradaPanel(); yield return StartCoroutine(animadorPaneles.Entrar(DificultadAlta)); } yield return StartCoroutine(animadorBotones.MostrarAceptarRechazar()); }

void SiguienteAlimento(bool esPrimero) { ingredientesMarcados.Clear(); alimentosMostrados++; if (!DificultadAlta) { int idx = Random.Range(0, config.alimentosDisponibles.Count); AlimentoActual = config.alimentosDisponibles[idx]; alimentoUI.MostrarAlimento(AlimentoActual); checklist.LimpiarYConfigurar(AlimentoActual, config.maxIngredientesMarcados); EsperandoDecision = true; } else { int idx = Random.Range(0, config.alimentosDificiles.Count); AlimentoDificilActual = config.alimentosDificiles[idx]; alimentoUI.MostrarAlimentoDificil(AlimentoDificilActual); checklist.LimpiarYConfigurarDificil(AlimentoDificilActual, config.maxIngredientesMarcados); sellosCheckbox.ResetearParaNuevoAlimento(); infoNutricionalUI.MostrarInfo(AlimentoDificilActual); EsperandoDecision = true; if (simuladorPorciones != null && !tiposDificilesYaVistos.Contains(AlimentoDificilActual.nombre)) { EsperandoDecision = false; StartCoroutine(FlujoSimulacionConBoton()); } } if (perdidaMemoria != null) { if (DificultadAlta) perdidaMemoria.Desactivar(); else if (alimentosMostrados == 1) perdidaMemoria.ActivarConDisparoInicial(); else perdidaMemoria.Activar(); } if (esPrimero) animadorBotones.PrepararEstadoInicial(); }

    IEnumerator FlujoSimulacionConBoton() { yield return new WaitForSeconds(0.3f); yield return StartCoroutine(animadorBotones.OcultarAceptarRechazar()); yield return StartCoroutine(animadorBotones.MostrarSimular()); }

    public void OnBotonSimularPresionado() { StartCoroutine(SecuenciaBotonSimular()); }

IEnumerator SecuenciaBotonSimular() { if (AudioManager.Instance != null) AudioManager.Instance.SonarClick(); yield return StartCoroutine(animadorBotones.OcultarSimular()); tiposDificilesYaVistos.Add(AlimentoDificilActual.nombre); if (anunciosInvasivos != null) anunciosInvasivos.Pausar(); if (perdidaMemoria != null) perdidaMemoria.Desactivar(); simuladorTerminado = false; simuladorPorciones.Mostrar(AlimentoDificilActual, () => simuladorTerminado = true); yield return new WaitUntil(() => simuladorTerminado); if (anunciosInvasivos != null) anunciosInvasivos.Reanudar(); if (perdidaMemoria != null) perdidaMemoria.Activar(); yield return StartCoroutine(animadorBotones.MostrarAceptarRechazar()); EsperandoDecision = true; }

IEnumerator MostrarSimuladorYContinuar() { yield break; }

void TerminarNivel() { if (panelFinal != null) panelFinal.Mostrar(); else if (GameManager.Instance != null) GameManager.Instance.TerminarNivel1(Aciertos); else Debug.Log("[Nivel1Manager] Nivel terminado con " + Aciertos + " aciertos"); }

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


public void IniciarJuegoDespuesDeTutorial() { StartCoroutine(SecuenciaPrimerAlimento()); }


public void SetEsperandoDecision(bool valor) { EsperandoDecision = valor; }
}
