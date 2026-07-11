using UnityEngine;
using Spine;
using Spine.Unity;

public class BoneFollower : MonoBehaviour
{
    [Header("CONFIGURACIÓN")]
    public SkeletonAnimation skeletonAnimation;
    public string boneName;
    public bool followZPosition = false;
    public bool followBoneRotation = false;

    [Header("OFFSET DERECHA")]
    public Vector3 offsetDerecha = new Vector3(0.4f, 0.6f, 0f);
    public float rotacionIdleDerecha = 35f;

    [Header("OFFSET IZQUIERDA")]
    public Vector3 offsetIzquierda = new Vector3(-0.4f, 0.6f, 0f);
    public float rotacionIdleIzquierda = 55f;

    private Bone bone;
    private Transform jugadorTransform;
    private Vector3 offsetActual;
    private float rotacionIdleActual;

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

        // Obtener referencia al jugador (el padre del SkeletonAnimation)
        if (skeletonAnimation.transform.parent != null)
        {
            jugadorTransform = skeletonAnimation.transform.parent;
        }
        else
        {
            jugadorTransform = skeletonAnimation.transform;
        }

        // Valores iniciales (por defecto derecha)
        offsetActual = offsetDerecha;
        rotacionIdleActual = rotacionIdleDerecha;

        Debug.Log("✅ BoneFollower: Inicializado");
    }

    void Update()
    {
        if (skeletonAnimation == null || skeletonAnimation.Skeleton == null) return;

        if (bone == null)
        {
            bone = skeletonAnimation.Skeleton.FindBone(boneName);
            if (bone == null)
            {
                Debug.LogWarning("⚠️ No se encontró el hueso: " + boneName);
                return;
            }
        }

        // ==========================================
        // DETECTAR DIRECCIÓN DEL JUGADOR
        // ==========================================
        if (jugadorTransform != null)
        {
            float direccion = jugadorTransform.localScale.x;

            // Si mira a la derecha (ScaleX > 0)
            if (direccion > 0)
            {
                offsetActual = offsetDerecha;
                rotacionIdleActual = rotacionIdleDerecha;
            }
            // Si mira a la izquierda (ScaleX < 0)
            else
            {
                offsetActual = offsetIzquierda;
                rotacionIdleActual = rotacionIdleIzquierda;
            }
        }

        // ==========================================
        // POSICIÓN DEL HUESO + OFFSET DINÁMICO
        // ==========================================
        Vector3 worldPos = skeletonAnimation.transform.TransformPoint(
            new Vector3(bone.WorldX, bone.WorldY, 0)
        );
        worldPos += offsetActual;

        // Aplicar posición Z si está activado
        if (followZPosition)
        {
            worldPos.z = transform.position.z;
        }

        transform.position = worldPos;

        // ==========================================
        // ROTACIÓN DINÁMICA
        // ==========================================
        if (followBoneRotation)
        {
            transform.rotation = Quaternion.Euler(0, 0, bone.WorldRotationX);
        }
        else
        {
            // Rotación idle dinámica según dirección
            transform.rotation = Quaternion.Euler(0, 0, rotacionIdleActual);
        }
    }

    // ==========================================
    // MÉTODOS PARA CAMBIAR DINÁMICAMENTE
    // ==========================================
    public void CambiarOffset(Vector3 nuevoOffset)
    {
        offsetActual = nuevoOffset;
        Debug.Log("🔄 Offset cambiado a: " + nuevoOffset);
    }

    public void CambiarRotacion(float nuevaRotacion)
    {
        rotacionIdleActual = nuevaRotacion;
        Debug.Log("🔄 Rotación cambiada a: " + nuevaRotacion);
    }

    public void ForzarActualizacion()
    {
        bone = null;
        Debug.Log("🔄 BoneFollower forzado a actualizar");
    }
}