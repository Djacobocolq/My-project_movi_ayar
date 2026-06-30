using UnityEngine;

public class MusicaNivel : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Configurar audio
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = 0.5f;
        audioSource.spatialBlend = 0;

        // Buscar el sistema de pausa (sin necesidad de referencia directa)
        Debug.Log("🎵 MusicaNivel: Inicializado correctamente");
    }

    public void PausarMusica()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
            Debug.Log("⏸️ Música del nivel pausada");
        }
    }

    public void ReanudarMusica()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.UnPause();
            Debug.Log("▶️ Música del nivel reanudada");
        }
    }

    void OnDestroy()
    {
        Debug.Log("🎵 MusicaNivel: Música detenida al cambiar de nivel");
    }
}