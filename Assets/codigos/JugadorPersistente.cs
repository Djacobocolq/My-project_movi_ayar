using UnityEngine;

public class JugadorPersistente : MonoBehaviour
{
    private static JugadorPersistente instancia;

    void Awake()
    {
        // Si ya existe una instancia, destruir esta
        if (instancia != null)
        {
            Destroy(gameObject);
            return;
        }

        // Si no existe, guardar esta instancia
        instancia = this;
        DontDestroyOnLoad(gameObject);
    }
}