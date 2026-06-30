using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class pausar_juego : MonoBehaviour
{
    public GameObject menuPausa;
    public bool juegoPausado = false;

    private Keyboard keyboard;
    private static pausar_juego instancia;

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
        keyboard = Keyboard.current;
        BuscarMenuPausa();
    }

    void Update()
    {
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            if (juegoPausado)
                Reanudar();
            else
                Pausar();
        }
    }

    public void BuscarMenuPausa()
    {
        GameObject menu = GameObject.Find("MenuPause");
        if (menu != null)
        {
            menuPausa = menu;
            menuPausa.SetActive(false);
            Debug.Log("Menú de pausa encontrado");
        }
        else
        {
            Debug.LogWarning("No se encontró 'MenuPause' en la escena actual");
        }
    }

    public void Reanudar()
    {
        if (menuPausa != null)
            menuPausa.SetActive(false);

        Time.timeScale = 1;
        juegoPausado = false;

        // ==========================================
        // REANUDAR MÚSICA DEL NIVEL ACTUAL
        // ==========================================
        MusicaNivel musica = FindFirstObjectByType<MusicaNivel>();
        if (musica != null)
        {
            musica.ReanudarMusica();
            Debug.Log("🎵 Música reanudada");
        }

        Debug.Log("🎮 Juego reanudado");
    }

    public void Pausar()
    {
        if (menuPausa != null)
            menuPausa.SetActive(true);

        Time.timeScale = 0;
        juegoPausado = true;

        // ==========================================
        // PAUSAR MÚSICA DEL NIVEL ACTUAL
        // ==========================================
        MusicaNivel musica = FindFirstObjectByType<MusicaNivel>();
        if (musica != null)
        {
            musica.PausarMusica();
            Debug.Log("🎵 Música pausada");
        }

        Debug.Log("⏸️ Juego pausado");
    }

    public void irAlMenu()
    {
        Time.timeScale = 1;
        juegoPausado = false;
        // ==========================================
        // REANUDAR MÚSICA DEL NIVEL ACTUAL
        // ==========================================
        MusicaNivel musica = FindFirstObjectByType<MusicaNivel>();
        if (musica != null)
        {
            musica.ReanudarMusica();
            Debug.Log("🎵 Música reanudada");
        }
        // Cargar la escena del menú principal
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void AlternarPausa()
    {
        if (juegoPausado)
            Reanudar();
        else
            Pausar();
    }
}