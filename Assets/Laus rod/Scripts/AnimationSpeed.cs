using UnityEngine;
using UnityEngine.UIElements;

public class AnimationSpeed : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float speedIncrement = 1f;
    private float animationSpeed = 0;

    [SerializeField] private ForestControl forestControlScript;

    // Breath rate animation control
    private float flow;
    
    private bool triggerHit = false;
    

    public bool attachStop = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GrowTree()
    {
        animator.SetTrigger("GrowTree");
    }

    public void StopFlow()
    {
        attachStop = true;
    }

    public void IncreaseSpeed()
    {
        if (!triggerHit)
        {
            animator.SetTrigger("GrowTree");
            triggerHit = true;
        }
        animationSpeed += speedIncrement;
        animator.SetFloat("Speed", animationSpeed);
        Debug.Log("Increasing speed: " + animationSpeed);
    }
    public void DecreaseSpeed()
    {
        animationSpeed -= speedIncrement;
        animator.SetFloat("Speed", animationSpeed);
        Debug.Log("Decreasing speed: " + animationSpeed);
    }

    public void GrowForest()
    {
        forestControlScript.StartGrowingForest();
    }
}
