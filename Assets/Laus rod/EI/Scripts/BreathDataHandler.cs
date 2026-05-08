using LSL4Unity.Samples.SimpleInlet;
using UnityEngine;

public class BreathDataHandler : MonoBehaviour
{
    [SerializeField] private SimpleInletScaleObject inletScript;

    private float currentValue;
    private float previousValue;

    private bool wasRising = false;

    [Header("Peak Detection")]

    [SerializeField] private float minimumPeakInterval = 1f;

    private float lastPeakTime = -999f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentValue = inletScript.Value;

        DetectPeak();

        previousValue = currentValue;
    }

    private void DetectPeak()
    { 
        bool isRising = currentValue > previousValue

        //peak detected
        if (wasRising && !isRising)
        {
            if (Time.time - lastPeakTime > minimumPeakInterval)
            {
                lastPeakTime = Time.time;
                Debug.Log("Breath peak detected at value: " + currentValue)
            }
            wasRising = isRising;
        }
    }

}
