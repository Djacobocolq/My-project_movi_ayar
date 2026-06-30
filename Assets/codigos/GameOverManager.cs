using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI DEL GAME OVER")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI tituloGameOver;
    public Button btnReiniciar;
    public Button btnSalir;

    [Header("CONFIGURACIÓN")]
    public string nombreEscenaMenu = "Menu"; // ← Escena a la que irá

    private static GameOverManager instancia;

    void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Start()
    {
        ConectarBotonesGameOver();
    }

    void ConectarBotonesGameOver()
    {
        if (gameOverPanel == null) return;

        if (btnReiniciar == null)
            btnReiniciar = GameObject.Find("BtnReiniciar")?.GetComponent<Button>();

        if (btnSalir == null)
            btnSalir = GameObject.Find("BtnSalir")?.GetComponent<Button>();

        if (btnReiniciar != null)
        {
            btnReiniciar.onClick.RemoveAllListeners();
            btnReiniciar.onClick.AddListener(IrAlMenu); // ← CAMBIADO
            Debug.Log("✅ BtnReiniciar: IrAlMenu");
        }

        if (btnSalir != null)
        {
            btnSalir.onClick.RemoveAllListeners();
            btnSalir.onClick.AddListener(SalirJuego);
            Debug.Log("✅ BtnSalir: SalirJuego");
        }
    }

    public void MostrarGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0;
        Debug.Log("💀 GAME OVER - Mostrado");
    }

    public void OcultarGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        Time.timeScale = 1;
        Debug.Log("💀 GAME OVER - Ocultado");
    }

    // ==========================================
    // BOTÓN REINICIAR → MENÚ
    // ==========================================
    public void IrAlMenu()
    {
        Debug.Log("🏠 Volviendo al menú principal...");
        Time.timeScale = 1;
        OcultarGameOver();
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    // ==========================================
    // BOTÓN SALIR
    // ==========================================
    public void SalirJuego()
    {
        Debug.Log("🚪 Saliendo del juego...");
        Time.timeScale = 1;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public static void ActivarGameOver()
    {
        if (instancia != null)
        {
            instancia.MostrarGameOver();
        }
        else
        {
            Debug.LogError("❌ GameOverManager no encontrado!");
        }
    }

    void OnDestroy()
    {
        if (btnReiniciar != null)
            btnReiniciar.onClick.RemoveAllListeners();
        if (btnSalir != null)
            btnSalir.onClick.RemoveAllListeners();
    }
}