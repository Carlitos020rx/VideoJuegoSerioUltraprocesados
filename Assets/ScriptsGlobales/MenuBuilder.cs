
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

public class MenuBuilder : MonoBehaviour
{
    [MenuItem("Tools/Construir Menu Principal")]
    public static void Build()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var red  = new Color(0.78f, 0.12f, 0.12f);
        var blue = new Color(0.08f, 0.31f, 0.51f);
        var grey = new Color(0.95f, 0.95f, 0.95f);
        var dark = new Color(0.2f, 0.2f, 0.2f);

        // Canvas
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // EventSystem
        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // PanelInicio
        var pInicio = MakePanel("PanelInicio", canvasGO, true);
        MakeText("TextUltra",     pInicio, "Ultra",       120, red,                    new Vector2(0.1f,0.52f), new Vector2(0.9f,0.72f), font, FontStyle.BoldAndItalic);
        MakeText("TextProcessed", pInicio, "processed",   120, blue,                   new Vector2(0.1f,0.36f), new Vector2(0.9f,0.56f), font, FontStyle.BoldAndItalic);
        MakeButton("BtnPressStart", pInicio, "PRESS START", new Vector2(0.35f,0.18f),  new Vector2(0.65f,0.28f), red, Color.white, font);
        MakeText("LabelBy",       pInicio, "BY MR.C",    20,  new Color(0.7f,0.7f,0.7f), new Vector2(0.35f,0.11f), new Vector2(0.65f,0.17f), font, FontStyle.Normal);

        // PanelMenu (empieza inactivo)
        var pMenu = MakePanel("PanelMenu", canvasGO, false);
        MakeText("TextUltraMenu",     pMenu, "Ultra",     90, red,  new Vector2(0.1f,0.62f), new Vector2(0.9f,0.78f), font, FontStyle.BoldAndItalic);
        MakeText("TextProcessedMenu", pMenu, "processed", 90, blue, new Vector2(0.1f,0.48f), new Vector2(0.9f,0.65f), font, FontStyle.BoldAndItalic);
        MakeButton("BtnJugar",    pMenu, "JUGAR",    new Vector2(0.35f,0.36f), new Vector2(0.65f,0.45f), red,  Color.white, font);
        MakeButton("BtnCreditos", pMenu, "CREDITOS", new Vector2(0.35f,0.26f), new Vector2(0.65f,0.34f), grey, dark,       font);
        MakeButton("BtnAjustes",  pMenu, "AJUSTES",  new Vector2(0.35f,0.16f), new Vector2(0.65f,0.24f), grey, dark,       font);
        MakeButton("BtnSalir",    pMenu, "SALIR",    new Vector2(0.35f,0.06f), new Vector2(0.65f,0.14f), Color.white, red, font);

        EditorUtility.SetDirty(canvasGO);
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Menu construido correctamente");
    }

    static GameObject MakePanel(string name, GameObject parent, bool active)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var img = go.AddComponent<Image>();
        img.color = Color.white;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        go.SetActive(active);
        return go;
    }

    static void MakeText(string name, GameObject parent, string txt, int size, Color col, Vector2 aMin, Vector2 aMax, Font font, FontStyle style)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var t = go.AddComponent<Text>();
        t.text = txt; t.fontSize = size; t.color = col;
        t.alignment = TextAnchor.MiddleCenter;
        t.font = font; t.fontStyle = style;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    static void MakeButton(string name, GameObject parent, string txt, Vector2 aMin, Vector2 aMax, Color bg, Color fg, Font font)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var img = go.AddComponent<Image>();
        img.color = bg;
        go.AddComponent<Button>();
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

        var tgo = new GameObject("Text");
        tgo.transform.SetParent(go.transform, false);
        var t = tgo.AddComponent<Text>();
        t.text = txt; t.fontSize = 36; t.color = fg;
        t.alignment = TextAnchor.MiddleCenter;
        t.font = font; t.fontStyle = FontStyle.Bold;
        var trt = tgo.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero; trt.offsetMax = Vector2.zero;
    }
}
#endif
