
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

public class PresentadorBuilder : MonoBehaviour
{
    [MenuItem("Tools/Construir Escena Presentador")]
    public static void Build()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var font  = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Canvas
        var canvasGO = new GameObject("Canvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode        = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // EventSystem
        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // Colores identificadores de cada panel
        var colores = new Color[]
        {
            new Color(0.85f, 0.93f, 1.0f),   // Intro        — azul claro
            new Color(0.85f, 1.0f,  0.88f),  // Explicacion  — verde claro
            new Color(1.0f,  0.95f, 0.80f),  // Puente       — amarillo claro
            new Color(1.0f,  0.85f, 0.85f),  // Cierre       — rosado claro
        };

        var nombres = new string[]
        {
            "PanelIntro",
            "PanelExplicacionNivel1",
            "PanelPuente",
            "PanelCierre"
        };

        var titulos = new string[]
        {
            "SEGMENTO 1 — INTRO\nEl presentador introduce los ultraprocesados",
            "SEGMENTO 2 — EXPLICACION NIVEL 1\nEl presentador habla sobre los alimentos y presenta el Minijuego 1",
            "SEGMENTO 3 — PUENTE\nEl presentador hace la transicion entre Nivel 1 y Nivel 2",
            "SEGMENTO 4 — CIERRE\nEl presentador da el mensaje educativo final"
        };

        var paneles = new GameObject[4];

        for (int i = 0; i < 4; i++)
        {
            // Panel fondo
            var panel = new GameObject(nombres[i]);
            panel.transform.SetParent(canvasGO.transform, false);
            var img = panel.AddComponent<Image>();
            img.color = colores[i];
            var rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            panel.SetActive(i == 0); // solo el primero activo

            // Titulo del segmento (centro)
            var tituloGO = new GameObject("TextTitulo");
            tituloGO.transform.SetParent(panel.transform, false);
            var tituloT = tituloGO.AddComponent<Text>();
            tituloT.text      = titulos[i];
            tituloT.fontSize  = 42;
            tituloT.color     = new Color(0.15f, 0.15f, 0.15f);
            tituloT.alignment = TextAnchor.MiddleCenter;
            tituloT.font      = font;
            tituloT.fontStyle = FontStyle.Bold;
            var tituloRt = tituloGO.GetComponent<RectTransform>();
            tituloRt.anchorMin = new Vector2(0.1f, 0.4f);
            tituloRt.anchorMax = new Vector2(0.9f, 0.75f);
            tituloRt.offsetMin = Vector2.zero;
            tituloRt.offsetMax = Vector2.zero;

            // Texto placeholder video
            var videoGO = new GameObject("TextPlaceholderVideo");
            videoGO.transform.SetParent(panel.transform, false);
            var videoT = videoGO.AddComponent<Text>();
            videoT.text      = "[ AQUI VA EL VIDEO DEL PRESENTADOR ]";
            videoT.fontSize  = 26;
            videoT.color     = new Color(0.5f, 0.5f, 0.5f);
            videoT.alignment = TextAnchor.MiddleCenter;
            videoT.font      = font;
            videoT.fontStyle = FontStyle.Italic;
            var videoRt = videoGO.GetComponent<RectTransform>();
            videoRt.anchorMin = new Vector2(0.1f, 0.25f);
            videoRt.anchorMax = new Vector2(0.9f, 0.38f);
            videoRt.offsetMin = Vector2.zero;
            videoRt.offsetMax = Vector2.zero;

            // Temporizador (esquina inferior derecha)
            var timerGO = new GameObject("TextTimer");
            timerGO.transform.SetParent(panel.transform, false);
            var timerT = timerGO.AddComponent<Text>();
            timerT.text      = "VIDEO EN: 10s";
            timerT.fontSize  = 32;
            timerT.color     = new Color(0.2f, 0.2f, 0.2f);
            timerT.alignment = TextAnchor.MiddleCenter;
            timerT.font      = font;
            timerT.fontStyle = FontStyle.Bold;
            var timerRt = timerGO.GetComponent<RectTransform>();
            timerRt.anchorMin = new Vector2(0.35f, 0.08f);
            timerRt.anchorMax = new Vector2(0.65f, 0.18f);
            timerRt.offsetMin = Vector2.zero;
            timerRt.offsetMax = Vector2.zero;

            paneles[i] = panel;
        }

        // Agregar PresentadorController al Canvas y conectar referencias
        var ctrl = canvasGO.AddComponent<PresentadorController>();
        ctrl.panelIntro             = paneles[0];
        ctrl.panelExplicacionNivel1 = paneles[1];
        ctrl.panelPuente            = paneles[2];
        ctrl.panelCierre            = paneles[3];

        // Conectar timers
        ctrl.timerTextIntro      = paneles[0].transform.Find("TextTimer").GetComponent<Text>();
        ctrl.timerTextExplicacion = paneles[1].transform.Find("TextTimer").GetComponent<Text>();
        ctrl.timerTextPuente     = paneles[2].transform.Find("TextTimer").GetComponent<Text>();
        ctrl.timerTextCierre     = paneles[3].transform.Find("TextTimer").GetComponent<Text>();

        EditorUtility.SetDirty(canvasGO);
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Escena Presentador construida correctamente con 4 paneles y temporizadores");
    }
}
#endif
