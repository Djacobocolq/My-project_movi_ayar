using UnityEngine;
using UnityEngine.SceneManagement;

public class PasarNivel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("¡Jugador tocó la puerta! Cargando siguiente nivel...");

            // NO destruir al jugador (se mantiene con DontDestroyOnLoad)
            // Solo cambiar de nivel
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}