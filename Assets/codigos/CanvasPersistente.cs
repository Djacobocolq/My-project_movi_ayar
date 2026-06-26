using UnityEngine;

public class CanvasPersistente : MonoBehaviour
{
    private static CanvasPersistente instancia;

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