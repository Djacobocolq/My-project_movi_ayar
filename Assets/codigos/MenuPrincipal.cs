using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("REFERENCIAS")]
    public Button btnJugar;
    public Button btnSalir;
    public string nombreNivel1 = "Nivel1";

    void Start()
    {
        // Verificar que el EventSystem exista
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogError("❌ No hay EventSystem en la escena! Los botones no funcionarán.");
            return;
        }

        // Buscar botones si no están asignados
        if (btnJugar == null)
            btnJugar = GameObject.Find("Jugar")?.GetComponent<Button>();

        if (btnSalir == null)
            btnSalir = GameObject.Find("Salir")?.GetComponent<Button>();

        // Conectar botones
        if (btnJugar != null)
        {
            btnJugar.onClick.AddListener(Jugar);
            Debug.Log("✅ Botón Jugar conectado");
        }
        else
        {
            Debug.LogError("❌ No se encontró el botón Jugar!");
        }

        if (btnSalir != null)
        {
            btnSalir.onClick.AddListener(Salir);
            Debug.Log("✅ Botón Salir conectado");
        }
        else
        {
            Debug.LogError("❌ No se encontró el botón Salir!");
        }

        // Asegurar que el tiempo esté en 1
        Time.timeScale = 1;
        Debug.Log("🏠 Menú Principal: Iniciado");
    }

    public void Jugar()
    {
        Debug.Log("🎮 Cargando nivel 1...");
        Time.timeScale = 1;
        SceneManager.LoadScene(nombreNivel1);
    }

    public void Salir()
    {
        Debug.Log("🚪 Saliendo del juego...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    void OnDestroy()
    {
        if (btnJugar != null)
            btnJugar.onClick.RemoveAllListeners();
        if (btnSalir != null)
            btnSalir.onClick.RemoveAllListeners();
    }
}