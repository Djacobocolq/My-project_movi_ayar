using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Pausar_juego : MonoBehaviour
{
    public GameObject menuPausa;
    public bool juegoPausado = false;

    private Keyboard keyboard;
    private static Pausar_juego instancia;

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

    public void AlternarPausa()
    {
        if (juegoPausado)
            Reanudar();
        else
            Pausar();
    }
}