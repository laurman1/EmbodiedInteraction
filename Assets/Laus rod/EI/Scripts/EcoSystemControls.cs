
using System.Collections.Generic;
using UnityEngine;





public class EcoSystemControls : MonoBehaviour
{
    [SerializeField] private BreathDataHandler breathData;

    [Header("Breathing Ranges")]

    [SerializeField] private float minBPM = 6f;
    [SerializeField] private float maxBPM = 40f;

    [SerializeField] private float minDepth = 0.1f;
    [SerializeField] private float maxDepth = 2.2f;

    [Header("Normalized Metrics")]

    [Range(0, 1)] public float normalizedLongTermBreathRate;
    [Range(0, 1)] public float normalizedLongTermBreathDepth;
    [Range(0, 1)] public float normalizedShortTermBreathRate;
    [Range(0, 1)] public float normalizedShortTermBreathDepth;

    [Header("Ecosystem States")]
    [Range(0, 1)] public float fertility;
    [Range(0, 1)] public float calmness;
    [Range(0, 1)] public float warmth;

    private float targetFertility;
    private float targetCalmness;
    private float targetWarmth;

    [Range(0, 1)] public float shortTermFertility;
    [Range(0, 1)] public float shortTermCalmness;
    [Range(0, 1)] public float shortTermWarmth;

    private float shortTermTargetFertility;
    private float shortTermTargetCalmness;
    private float shortTermTargetWarmth;

    [SerializeField] private float ecosystemTransitionSpeed = 1f;

    

    
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    

    // Update is called once per frame
    void Update()
    {   //----------------------------------------------------
        //------------Ecosystem Controls ---------------------
        //----------------------------------------------------
        normalizedLongTermBreathRate =
        Mathf.InverseLerp(
            minBPM,
            maxBPM,
            breathData.longTermAverageBreathRateBPM
            );

        normalizedLongTermBreathDepth =
            Mathf.InverseLerp(
                minDepth,
                maxDepth,
                breathData.longTermAverageBreathDepth
                );

        normalizedShortTermBreathRate =
        Mathf.InverseLerp(
            minBPM,
            maxBPM,
            breathData.shortTermAverageBreathRateBPM
            );

        normalizedShortTermBreathDepth =
            Mathf.InverseLerp(
                minDepth,
                maxDepth,
                breathData.shortTermAverageBreathDepth
                );

        targetFertility = normalizedLongTermBreathDepth;
        shortTermTargetFertility = normalizedShortTermBreathDepth;

        targetCalmness = 1f - normalizedLongTermBreathRate;
        shortTermTargetCalmness = 1f - normalizedShortTermBreathRate;

        targetWarmth = (targetFertility + targetCalmness) * 0.5f;
        shortTermTargetWarmth = (shortTermTargetFertility + shortTermTargetCalmness) * 0.5f;

        fertility = Mathf.Lerp(
            fertility,
            targetFertility,
            Time.deltaTime * ecosystemTransitionSpeed
        );
        shortTermFertility = Mathf.Lerp(
            shortTermFertility,
            shortTermTargetFertility,
            Time.deltaTime * ecosystemTransitionSpeed
        );

        calmness = Mathf.Lerp(
            calmness,
            targetCalmness,
            Time.deltaTime * ecosystemTransitionSpeed
        );
        shortTermCalmness = Mathf.Lerp(
            shortTermCalmness,
            shortTermTargetCalmness,
            Time.deltaTime * ecosystemTransitionSpeed
        );

        warmth = Mathf.Lerp(
            warmth,
            targetWarmth,
            Time.deltaTime * ecosystemTransitionSpeed
        );
        shortTermWarmth = Mathf.Lerp(
            shortTermWarmth,
            shortTermTargetWarmth,
            Time.deltaTime * ecosystemTransitionSpeed
        );

        Debug.Log($"Fertility: {fertility}, Calmness: {calmness}, Warmth: {warmth}");
    }


}
