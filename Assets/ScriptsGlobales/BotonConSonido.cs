using UnityEngine;
using UnityEngine.UI;

// Componente helper: agregalo a cualquier Button para que suene el click automaticamente.
[RequireComponent(typeof(Button))]
public class BotonConSonido : MonoBehaviour
{
    void Start()
    {
        var btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SonarClick();
    }
}
