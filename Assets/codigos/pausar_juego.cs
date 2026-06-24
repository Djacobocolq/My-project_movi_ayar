using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // ← NUEVA IMPORTACIÓN

public class pausar_juego : MonoBehaviour
{
    public GameObject menuPausa;
    public bool juegoPausado = false;

    private Keyboard keyboard; // ← NUEVO

    private void Start() // ← NUEVO
    {
        keyboard = Keyboard.current;
    }

    private void Update()
    {
        // ← CÓDIGO MIGRADO
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Reanudar()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1;
        juegoPausado = false;
    }

    public void Pausar()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0;
        juegoPausado = true;
    }
}