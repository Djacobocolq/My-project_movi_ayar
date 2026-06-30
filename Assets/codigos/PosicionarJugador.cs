using UnityEngine;

public class PosicionarJugador : MonoBehaviour
{
    void Start()
    {
        // Buscar al jugador que viene del nivel anterior
        GameObject player = GameObject.FindGameObjectWithTag("Jugador");

        if (player != null)
        {
            // Buscar el punto de spawn
            GameObject spawnPoint = GameObject.Find("SpawnPoint");

            if (spawnPoint != null)
            {
                // Mover al jugador al spawn point
                player.transform.position = spawnPoint.transform.position;
                Debug.Log("Jugador posicionado en SpawnPoint");
            }
            else
            {
                Debug.LogWarning("No se encontró SpawnPoint en el nivel");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró al jugador");
        }
    }
}