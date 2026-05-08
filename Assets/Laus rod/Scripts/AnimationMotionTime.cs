using LSL4Unity.Samples.SimpleInlet;
using StarterAssets;
using System.Collections;
using UnityEngine;

public class AnimationMotionTime : MonoBehaviour
{
    private float motionTime;
    [SerializeField] private float duration;
    [SerializeField] private float motionTimeIncrement = 0.1f;
    public bool canGrow = false;

    [SerializeField] private Animator animator;
    private bool triggerHit = false;

    // Breathing rate control
    private bool startFlow = false;
    private float flow;
    [SerializeField] private float breathSpeed = 1f;

    public SimpleInletScaleObject inletScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startFlow && animator.GetFloat("MotionTime") <= 1)
        {
            flow = (Mathf.Sin(Time.time * breathSpeed * Mathf.PI * 2f) + 1f) / 2f;
            Debug.Log("Motion Time Tree flow: " + flow);


            if (animator.GetFloat("MotionTime") >= 1)
            {
                Debug.Log("TREE FINISHED GROWING");
                startFlow = false;
                canGrow = false;
                animator.Play("GrowingTreeFinished");
            }
        }
        
        

        if (canGrow && flow > 0.9f)
        {
            MotionTime();
        }

        if (inletScript.Value > 4)
        {
            MotionTime();
        }
    }

    public void GrowTree()
    {
        startFlow = true;
        animator.SetTrigger("GrowTree");
        
    }

    public void MotionTime()
    {
        if (triggerHit == false)
        {
            animator.SetTrigger("GrowTree");
            triggerHit = true;
        }
        
        if (canGrow)
        {
            StartCoroutine(IncreaseMotionTime());
            Debug.Log("Increasing motion time: " + motionTime);
        }
        else
        {
            Debug.Log("Cannot grow yet");
        }
    }

    IEnumerator IncreaseMotionTime()
    {
        float startValue = motionTime;
        float targetValue = motionTime + motionTimeIncrement;

        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            t = Mathf.SmoothStep(0, 1f, t);

            motionTime = Mathf.Lerp(startValue, targetValue, t);

            animator.SetFloat("MotionTime", motionTime);
            canGrow = false;
            yield return null;
        }

        motionTime = targetValue;
        animator.SetFloat("MotionTime", motionTime);
        canGrow = true;
    }
}
