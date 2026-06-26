using UnityEngine;

public class SistemaPersistente : MonoBehaviour
{
    private static SistemaPersistente instancia;

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