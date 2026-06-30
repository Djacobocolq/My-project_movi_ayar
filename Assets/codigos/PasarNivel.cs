using UnityEngine;
using UnityEngine.SceneManagement;

public class PasarNivel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Algo tocó la puerta: " + collision.gameObject.name);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("¡Jugador tocó la puerta! Cargando siguiente nivel...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}