// Bootstrap.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    void Awake()
    {
        GameObject gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();
        SceneManager.LoadScene("MenuPrincipal");
    }
}