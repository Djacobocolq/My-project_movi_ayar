using UnityEngine;
using Spine.Unity;

public class ArmaController : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public SkeletonAnimation skeletonAnimation;
    public string nombreHueso = "[base]elbowL";
    public Vector3 offsetLocal = new Vector3(0f, 0f, 0f);
    public float rotacionIdle = 45f;
    public float rotacionAtaque = -90f;
    public float duracionAtaque = 0.3f;
    public float profundidadZ = -1f;
    public float escalaX = 0.6f; // ← NUEVO
    public float escalaY = 0.6f; // ← NUEVO

    private bool atacando = false;
    private float tiempoAtaque = 0f;
    private bool activo = false;
    private Transform huesoTransform;

    void Start()
    {
        if (skeletonAnimation == null)
        {
            skeletonAnimation = GetComponentInParent<SkeletonAnimation>();
            if (skeletonAnimation == null)
            {
                Debug.LogError("❌ No se encontró SkeletonAnimation!");
                return;
            }
        }

        // ==========================================
        // ESCALA INICIAL
        // ==========================================
        transform.localScale = new Vector3(escalaX, escalaY, 1f);

        gameObject.SetActive(false);
    }

    void BuscarHueso()
    {
        if (skeletonAnimation == null || skeletonAnimation.Skeleton == null) return;

        if (huesoTransform == null)
        {
            GameObject boneFollower = new GameObject("BoneFollower");
            boneFollower.transform.SetParent(transform.parent);

            BoneFollower follower = boneFollower.AddComponent<BoneFollower>();
            follower.skeletonAnimation = skeletonAnimation;
            follower.boneName = nombreHueso;
            follower.followZPosition = false;
            follower.followBoneRotation = false;

            huesoTransform = boneFollower.transform;
            Debug.Log("✅ Hueso encontrado: " + nombreHueso);
        }
    }

    void Update()
    {
        if (!activo || skeletonAnimation == null) return;

        if (huesoTransform == null)
        {
            BuscarHueso();
            if (huesoTransform == null) return;
        }

        // ==========================================
        // POSICIÓN DEL BASTÓN (CON PROFUNDIDAD Z)
        // ==========================================
        Vector3 pos = huesoTransform.position + offsetLocal;
        pos.z = profundidadZ;
        transform.position = pos;

        // ==========================================
        // VOLTEAR CON EL JUGADOR (PERO MANTENIENDO ESCALA)
        // ==========================================
        if (transform.parent != null)
        {
            float direccion = transform.parent.localScale.x > 0 ? 1 : -1;
            transform.localScale = new Vector3(direccion * escalaX, escalaY, 1f);
        }

        // Rotación durante el ataque
        if (atacando)
        {
            float progreso = (Time.time - tiempoAtaque) / duracionAtaque;
            float angulo = Mathf.Lerp(rotacionIdle, rotacionAtaque, progreso * 2f);
            transform.rotation = Quaternion.Euler(0, 0, angulo);

            if (progreso >= 1f)
            {
                atacando = false;
                transform.rotation = Quaternion.Euler(0, 0, rotacionIdle);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, rotacionIdle);
        }
    }

    public void Activar()
    {
        activo = true;
        gameObject.SetActive(true);

        // ==========================================
        // ESCALA DEL BASTÓN AL ACTIVAR
        // ==========================================
        transform.localScale = new Vector3(escalaX, escalaY, 1f);

        BuscarHueso();
        Debug.Log("✅ Bastón activado! Escala: " + escalaX + ", " + escalaY);
    }

    public void ActivarAtaque()
    {
        if (!activo) return;
        atacando = true;
        tiempoAtaque = Time.time;
    }

    void OnDestroy()
    {
        if (huesoTransform != null)
        {
            Destroy(huesoTransform.gameObject);
        }
    }
}