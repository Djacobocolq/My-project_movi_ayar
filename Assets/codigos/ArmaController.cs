using UnityEngine;
using Spine.Unity;

public class ArmaController : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public SkeletonAnimation skeletonAnimation;
    public string nombreHueso = "[base]elbowL";

    [Header("OFFSET DERECHA")]
    public Vector3 offsetDerecha = new Vector3(0.4f, 0.6f, 0f);
    public float rotacionIdleDerecha = 35f;

    [Header("OFFSET IZQUIERDA")]
    public Vector3 offsetIzquierda = new Vector3(-0.4f, 0.6f, 0f);
    public float rotacionIdleIzquierda = 55f;

    [Header("ATAQUE")]
    public float rotacionAtaqueDerecha = -90f;
    public float rotacionAtaqueIzquierda = 90f;
    public float duracionAtaque = 0.3f;
    public float profundidadZ = -1f;
    public float escalaX = 0.6f;
    public float escalaY = 0.6f;

    private bool atacando = false;
    private float tiempoAtaque = 0f;
    private bool activo = false;
    private Transform huesoTransform;

    // ==========================================
    // VALORES ACTUALES SEGÚN DIRECCIÓN
    // ==========================================
    private Vector3 offsetActual;
    private float rotacionIdleActual;
    private float rotacionAtaqueActual;
    private float direccionActual = 1f;

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

        transform.localScale = new Vector3(escalaX, escalaY, 1f);
        gameObject.SetActive(false);

        // Valores iniciales (por defecto derecha)
        offsetActual = offsetDerecha;
        rotacionIdleActual = rotacionIdleDerecha;
        rotacionAtaqueActual = rotacionAtaqueDerecha;
        direccionActual = 1f;
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
        // ACTUALIZAR POSICIÓN CON OFFSET DINÁMICO
        // ==========================================
        Vector3 pos = huesoTransform.position + offsetActual;
        pos.z = profundidadZ;
        transform.position = pos;

        // ==========================================
        // VOLTEAR SPRITE
        // ==========================================
        if (transform.parent != null)
        {
            transform.localScale = new Vector3(direccionActual * escalaX, escalaY, 1f);
        }

        // ==========================================
        // ROTACIÓN DINÁMICA
        // ==========================================
        if (atacando)
        {
            float progreso = (Time.time - tiempoAtaque) / duracionAtaque;
            float angulo = Mathf.Lerp(rotacionIdleActual, rotacionAtaqueActual, progreso * 2f);
            transform.rotation = Quaternion.Euler(0, 0, angulo);

            if (progreso >= 1f)
            {
                atacando = false;
                transform.rotation = Quaternion.Euler(0, 0, rotacionIdleActual);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, rotacionIdleActual);
        }
    }

    // ==========================================
    // MÉTODO PARA CAMBIAR DIRECCIÓN
    // ==========================================
    public void CambiarDireccion(float nuevaDireccion)
    {
        if (direccionActual == nuevaDireccion) return;

        direccionActual = nuevaDireccion;

        if (direccionActual > 0)
        {
            offsetActual = offsetDerecha;
            rotacionIdleActual = rotacionIdleDerecha;
            rotacionAtaqueActual = rotacionAtaqueDerecha;
            Debug.Log("➡️ Bastón: DERECHA");
        }
        else
        {
            offsetActual = offsetIzquierda;
            rotacionIdleActual = rotacionIdleIzquierda;
            rotacionAtaqueActual = rotacionAtaqueIzquierda;
            Debug.Log("⬅️ Bastón: IZQUIERDA");
        }

        transform.localScale = new Vector3(direccionActual * escalaX, escalaY, 1f);

        if (!atacando)
        {
            transform.rotation = Quaternion.Euler(0, 0, rotacionIdleActual);
        }
    }

    public void Activar()
    {
        activo = true;
        gameObject.SetActive(true);
        transform.localScale = new Vector3(direccionActual * escalaX, escalaY, 1f);
        BuscarHueso();
        Debug.Log("✅ Bastón activado!");
    }

    public void ActivarAtaque()
    {
        if (!activo) return;

        // ==========================================
        // ROTACIÓN DE ATAQUE SEGÚN DIRECCIÓN
        // ==========================================
        if (direccionActual > 0)
        {
            rotacionAtaqueActual = rotacionAtaqueDerecha;
        }
        else
        {
            rotacionAtaqueActual = rotacionAtaqueIzquierda;
        }

        atacando = true;
        tiempoAtaque = Time.time;
        Debug.Log("⚔️ Ataque! Dirección: " + (direccionActual > 0 ? "Derecha" : "Izquierda"));
    }

    void OnDestroy()
    {
        if (huesoTransform != null)
        {
            Destroy(huesoTransform.gameObject);
        }
    }
}