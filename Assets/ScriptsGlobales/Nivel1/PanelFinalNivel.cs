using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelFinalNivel : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("CanvasGroup del panel completo (overlay + contenido) para hacer el fade")]
    public CanvasGroup canvasGroup;

    [Tooltip("Boton para pasar a la siguiente escena")]
    public Button botonSiguiente;

    [Header("Configuracion")]
    public float duracionFade = 0.8f;

    void Awake()
    {
        // Empezar oculto
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        gameObject.SetActive(false);
    }

    public void Mostrar()
    {
        gameObject.SetActive(true);

        // Parar la musica y sonar victoria
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.DetenerMusica();
            AudioManager.Instance.SonarVictoria();
        }

        // Conectar el boton
        if (botonSiguiente != null)
        {
            botonSiguiente.onClick.RemoveAllListeners();
            botonSiguiente.onClick.AddListener(OnSiguienteEscena);
        }

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < duracionFade)
        {
            t += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duracionFade);
            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    void OnSiguienteEscena()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SonarClick();

        // Sigue el flujo normal del juego
        if (Nivel1Manager.Instance != null && GameManager.Instance != null)
            GameManager.Instance.TerminarNivel1(Nivel1Manager.Instance.Aciertos);
        else
            Debug.Log("[PanelFinalNivel] No hay GameManager para avanzar de escena");
    }
}
