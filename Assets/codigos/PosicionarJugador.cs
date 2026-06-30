using UnityEngine;

public class PosicionarJugador : MonoBehaviour
{
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            GameObject spawnPoint = GameObject.Find("SpawnPoint");

            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                Debug.Log("Jugador posicionado en SpawnPoint");
            }
            else
            {
                Debug.LogWarning("No se encontró SpawnPoint");
                player.transform.position = new Vector3(0, 0, 0);
            }
        }
    }
}