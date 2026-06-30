using UnityEngine;

public class PosicionarJugador : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public int vidaMaxima = 3;

    void Start()
    {
        Posicionar();
    }

    void Posicionar()
    {
        // Buscar al jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Buscar el SpawnPoint
            GameObject spawnPoint = GameObject.Find("SpawnPoint");

            if (spawnPoint != null)
            {
                // Mover al jugador al SpawnPoint
                player.transform.position = spawnPoint.transform.position;
                Debug.Log("✅ Jugador posicionado en SpawnPoint");
            }
            else
            {
                Debug.LogWarning("⚠️ No se encontró SpawnPoint en el nivel");
                player.transform.position = new Vector3(0, 0, 0);
            }

            // ==========================================
            // REVIVIR AL JUGADOR (LLAMA AL MÉTODO REVIVIR)
            // ==========================================
            JugadorController jugadorController = player.GetComponent<JugadorController>();
            if (jugadorController != null)
            {
                jugadorController.Revivir(vidaMaxima);
                Debug.Log("✅ Jugador revivido con vida: " + vidaMaxima);
            }
            else
            {
                Debug.LogError("❌ No se encontró JugadorController en el jugador!");
            }
        }
        else
        {
            Debug.LogError("❌ No se encontró al jugador con Tag 'Player'");
        }
    }

    void OnEnable()
    {
        Posicionar();
    }
}