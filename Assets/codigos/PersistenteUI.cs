using UnityEngine;

public class PersistenteUI : MonoBehaviour
{
    private static PersistenteUI instancia;

    void Awake()
    {
        if (instancia != null)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("✅ PersistenteUI: " + gameObject.name + " es persistente");
    }
}