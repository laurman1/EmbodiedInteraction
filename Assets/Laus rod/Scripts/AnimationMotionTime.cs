using LSL4Unity.Samples.SimpleInlet;
using StarterAssets;
using System.Collections;
using UnityEngine;

public class AnimationMotionTime : MonoBehaviour
{
    private Animator treeAnimator;
    private float treeMotionTime;
    EcoSystemControls ecosystemControls;

    private void Start()
    {
        treeAnimator = GetComponent<Animator>();
        ecosystemControls = FindFirstObjectByType<EcoSystemControls>();
    }
    private void Update()
    {
        treeMotionTime =
        Mathf.Lerp(
            0.6f,
            1f,
            ecosystemControls.shortTermFertility
        );
        treeAnimator.SetFloat("MotionTime", treeMotionTime);
    }
}
