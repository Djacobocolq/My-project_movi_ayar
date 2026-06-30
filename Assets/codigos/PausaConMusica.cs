using UnityEngine;

public class PausaConMusica : MonoBehaviour
{
    private bool juegoPausado = false;
    private MusicaNivel musicaNivel;

    void Start()
    {
        musicaNivel = FindFirstObjectByType<MusicaNivel>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
                ReanudarJuego();
            else
                PausarJuego();
        }
    }

    public void PausarJuego()
    {
        juegoPausado = true;
        Time.timeScale = 0;

        // La música NO se detiene (no llamamos a PausarMusica)
        Debug.Log("⏸️ Juego pausado (música continúa)");
    }

    public void ReanudarJuego()
    {
        juegoPausado = false;
        Time.timeScale = 1;
        Debug.Log("▶️ Juego reanudado");
    }
}