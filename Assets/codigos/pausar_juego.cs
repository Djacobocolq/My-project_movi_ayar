using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class pausar_juego : MonoBehaviour
{
    public GameObject menuPausa;
    public bool juegoPausado = false;

    private Keyboard keyboard;
    private static pausar_juego instancia;

    // ==========================================
    // PERSISTENCIA (Singleton)
    // ==========================================
    void Awake()
    {
        // Si ya existe una instancia, destruir esta
        if (instancia != null)
        {
            Destroy(gameObject);
            return;
        }

        // Guardar esta instancia y hacerla persistente
        instancia = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("Sistema de pausa creado (persistente)");
    }

    void Start()
    {
        keyboard = Keyboard.current;
        BuscarMenuPausa();
    }

    void Update()
    {
        // Pausa con tecla ESC en PC
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            if (juegoPausado)
                Reanudar();
            else
                Pausar();
        }
    }

    // ==========================================
    // BUSCAR MENÚ DE PAUSA EN LA ESCENA ACTUAL
    // ==========================================
    public void BuscarMenuPausa()
    {
        // Buscar el menú de pausa en la escena actual
        GameObject menu = GameObject.Find("MenuPause");

        if (menu != null)
        {
            menuPausa = menu;
            menuPausa.SetActive(false);
            Debug.Log("Menú de pausa encontrado: " + menu.name);
        }
        else
        {
            Debug.LogWarning("No se encontró 'MenuPause' en la escena actual");
        }
    }

    // ==========================================
    // MÉTODOS PÚBLICOS
    // ==========================================
    public void Reanudar()
    {
        if (menuPausa != null)
            menuPausa.SetActive(false);

        Time.timeScale = 1;
        juegoPausado = false;
        Debug.Log("Juego reanudado");
    }

    public void Pausar()
    {
        if (menuPausa != null)
            menuPausa.SetActive(true);

        Time.timeScale = 0;
        juegoPausado = true;
        Debug.Log("Juego pausado");
    }

    // ==========================================
    // MÉTODO PARA ALTERNAR PAUSA (para botones)
    // ==========================================
    public void AlternarPausa()
    {
        if (juegoPausado)
            Reanudar();
        else
            Pausar();
    }
}