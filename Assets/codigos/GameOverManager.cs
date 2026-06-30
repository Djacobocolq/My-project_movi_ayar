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
    public string nombreEscenaReinicio = "Nivel1";

    private static GameOverManager instancia;

    void Awake()
    {
        if (instancia != null)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (btnReiniciar != null)
            btnReiniciar.onClick.AddListener(ReiniciarJuego);

        if (btnSalir != null)
            btnSalir.onClick.AddListener(SalirJuego);
    }

    // ==========================================
    // MÉTODOS PÚBLICOS
    // ==========================================

    public void MostrarGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0;
        Debug.Log("💀 GAME OVER");
    }

    public void OcultarGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ReiniciarJuego()
    {
        Debug.Log("🔄 Reiniciando juego...");
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        OcultarGameOver();
    }

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

    // ==========================================
    // MÉTODO PARA ACCEDER DESDE OTROS SCRIPTS
    // ==========================================

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