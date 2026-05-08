using LSL4Unity.Samples.SimpleInlet;
using System.Collections;
using UnityEngine;

public class GrowForestTrees : MonoBehaviour
{
    private Animator animator;
    private float animationSpeed;
    private bool hasFinishedGrowing = false;

    private ForestControl forestControlScript;

    private SimpleInletScaleObject inletScript;
    private bool hasTriggered = false;

    public float distanceToCenter;
    public float delay;

    [SerializeField] private GameObject treeTrunk;
    [SerializeField] private GameObject parent;
    [SerializeField] private Material[] leafMaterials;
    [SerializeField] private Material[] trunkMaterials;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        forestControlScript = GetComponentInParent<ForestControl>();

        animator = GetComponent<Animator>();

        inletScript = SimpleInletScaleObject.Instance;
        if (inletScript == null)
        {
            Debug.LogError("SimpleInletScaleObject Instance not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        animationSpeed = inletScript.GetDelayedValue(delay) - 1.5f;
        animator.SetFloat("Speed", animationSpeed);
    }

    public void StartGrowing()
    {
        if (!hasTriggered)
        {
            animator.SetTrigger("GrowTree");
            hasTriggered = true;
        }
        
    }

    public void StartGrowingAfterDelay(float delay)
    {
        StartCoroutine(GrowAfterDelay(delay));
    }

    private IEnumerator GrowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartGrowing();
    }

    private void GrowthFinished()
    {
        if (hasFinishedGrowing) return;

        hasFinishedGrowing = true;

        forestControlScript.NotifyTreeFinished(this);
    }


    public void SetLeafMaterial()
    {
        int leafmaterialIndex = Random.Range(0, leafMaterials.Length);
        int trunkMaterialIndex = Random.Range(0, trunkMaterials.Length);

        treeTrunk.GetComponent<Renderer>().material = trunkMaterials[trunkMaterialIndex];

        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();

        foreach ( Renderer r in renderers)
        {
            r.material = leafMaterials[leafmaterialIndex];
        }
    }
}
