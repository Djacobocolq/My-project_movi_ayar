using UnityEngine;
using Spine; // ← AGREGADO
using Spine.Unity;

public class BoneFollower : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public string boneName;
    public bool followZPosition = false;
    public bool followBoneRotation = false;

    private Bone bone;

    void Start()
    {
        if (skeletonAnimation == null)
        {
            skeletonAnimation = GetComponentInParent<SkeletonAnimation>();
        }
    }

    void Update()
    {
        if (skeletonAnimation == null || skeletonAnimation.Skeleton == null) return;

        if (bone == null)
        {
            bone = skeletonAnimation.Skeleton.FindBone(boneName);
            if (bone == null) return;
        }

        // Obtener la posición del hueso en el mundo
        Vector3 worldPos = skeletonAnimation.transform.TransformPoint(
            new Vector3(bone.WorldX, bone.WorldY, 0)
        );

        transform.position = worldPos;

        if (followBoneRotation)
        {
            transform.rotation = Quaternion.Euler(0, 0, bone.WorldRotationX);
        }
    }
}