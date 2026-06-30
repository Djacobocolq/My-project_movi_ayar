using UnityEngine;

public class JugadorPersistente : MonoBehaviour
{
    private static JugadorPersistente instancia;

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
}