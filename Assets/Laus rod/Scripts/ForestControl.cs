using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ForestControl : MonoBehaviour
{
    private List<GrowForestTrees> forestTrees;
    
    private float maxDistance;
    [SerializeField] private float totalForestGrowthSeconds;
    [SerializeField] private float maxGrowthDelaySeconds;

    private int completedTrees = 0;

    [SerializeField] private LifeCycleControl lifeCycleControlScript;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        forestTrees = new List<GrowForestTrees>(GetComponentsInChildren<GrowForestTrees>());
        CalculateDistances();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CalculateDistances()
    {
        foreach (var tree in forestTrees)
        {
            tree.distanceToCenter = Vector3.Distance(
                tree.transform.position,
                transform.position);

            tree.SetLeafMaterial();
        }
        
        maxDistance = forestTrees.Max(t => t.distanceToCenter);

        foreach (var tree in forestTrees)
        {
            tree.delay =
                (tree.distanceToCenter / maxDistance) * maxGrowthDelaySeconds;
        }

        forestTrees.Sort((a, b) => a.distanceToCenter.CompareTo(b.distanceToCenter));
         
    }

    public void StartGrowingForest()
    {
        Debug.Log("Starting to grow forest with " + forestTrees.Count + " trees.");
        foreach (var tree in forestTrees)
        {
            float delay = (tree.distanceToCenter / maxDistance) * totalForestGrowthSeconds;

            tree.StartGrowingAfterDelay(delay);
        }
    }

    public void NotifyTreeFinished(GrowForestTrees tree)
    {
        completedTrees++;

        if (completedTrees >= forestTrees.Count)
        {
            WaitForSeconds wait = new WaitForSeconds(2f);
            
            lifeCycleControlScript.EndGame();
        }
    }
}
