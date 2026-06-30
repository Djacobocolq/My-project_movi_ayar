using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class pausar_juego : MonoBehaviour
{
    public GameObject menuPausa;
    public bool juegoPausado = false;

    private static pausar_juego instancia;

    void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("✅ SystemPause: Awake");
    }

    void Start()
    {
        // Si no está asignado en el Inspector, buscarlo automáticamente
        if (menuPausa == null)
        {
            Debug.Log("🔍 menuPausa no asignado en Inspector, buscando...");
            menuPausa = GameObject.Find("MenuPause");
        }

        if (menuPausa != null)
        {
            menuPausa.SetActive(false);
            Debug.Log("✅ MenuPause encontrado y oculto: " + menuPausa.name);
        }
        else
        {
            Debug.LogError("❌ No se encontró MenuPause en la escena!");
        }
    }

    public void Pausar()
    {
        Debug.Log("⏸️ Pausar() llamado");

        // Si el menú se perdió, buscarlo nuevamente
        if (menuPausa == null)
        {
            menuPausa = GameObject.Find("MenuPause");
        }

        if (menuPausa != null)
        {
            menuPausa.SetActive(true);
            Debug.Log("✅ MenuPause ACTIVADO");
        }
        else
        {
            Debug.LogError("❌ No se encontró MenuPause!");
            return;
        }

        Time.timeScale = 0;
        juegoPausado = true;
        Debug.Log("⏸️ Juego pausado");
    }

    public void Reanudar()
    {
        if (menuPausa != null)
            menuPausa.SetActive(false);

        Time.timeScale = 1;
        juegoPausado = false;
        Debug.Log("🎮 Juego reanudado");
    }

    public void IrAlMenu()
    {
        Debug.Log("🏠 Volviendo al menú principal...");
        Time.timeScale = 1;
        juegoPausado = false;

        if (menuPausa != null)
            menuPausa.SetActive(false);

        SceneManager.LoadScene("Menu");
    }
}