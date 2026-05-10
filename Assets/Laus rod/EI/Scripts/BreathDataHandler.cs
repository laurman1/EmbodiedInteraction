using LSL4Unity.Samples.SimpleInlet;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BreathDataHandler : MonoBehaviour
{
    [SerializeField] private SimpleInletScaleObject inletScript;

    private float currentValue;
    private float previousValue;

    [SerializeField] private int shortTermAverageSampleCount = 5;
    [SerializeField] private int longTermAverageSampleCount = 20;

    [SerializeField] private Queue<float> recentBPMs = new Queue<float>();
    [SerializeField] private Queue<float> recentDepths = new Queue<float>();

    public float breathRateBPM;
    public float breathDepth;

    [Header("Peak Detection")]

    [SerializeField] private float minimumPeakInterval = 1f;
    [SerializeField] private float riseThreshold;
    [SerializeField] private float fallThreshold;
    [SerializeField] private float minimumPeakValue = 1.5f;
    private int peaks = 0;

    private float lastPeakTime = -999f;
    private float previousPeakTime;

    private bool wasRising = false;
    private bool wasFalling = false;

    private float lastPeakValue;
    private float lastTroughValue;

    [Header("Peak Detection")]
    [SerializeField] private float minimumTroughValue = 2f;
    [SerializeField] private float minimumTroughInterval = 1f;
    private float lastTroughTime = -999f;

    [Header("Breath Metrics")]
    
    public float shortTermAverageBreathRateBPM;
    public float longTermAverageBreathRateBPM;

    public float shortTermAverageBreathDepth;
    public float longTermAverageBreathDepth;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentValue = inletScript.Value - 1.5f;
        //Debug.Log(currentValue);
        
        DetectBreathFeatures();

        previousValue = currentValue;
    }

    private void DetectBreathFeatures()
    {
        float delta = currentValue - previousValue;

        bool isRising = delta > riseThreshold;
        bool isFalling = delta < -fallThreshold;

        //------------------------------------
        //---------- Peak Detection ----------
        //------------------------------------

        if (wasRising && isFalling)
            
            if(currentValue > minimumPeakValue)
            {

                if (Time.time - lastPeakTime > minimumPeakInterval)
                {
                previousPeakTime = lastPeakTime;

                lastPeakTime = Time.time;
                
                lastPeakValue = currentValue;

                float breathDuration = lastPeakTime - previousPeakTime;

                if (breathDuration > 0f)
                {
                    breathRateBPM = 60f / breathDuration;
                    AddBPMReading(breathRateBPM);
                }

                // Breath Depth
                breathDepth = lastPeakValue - lastTroughValue;
                AddDepthReading(breathDepth);

                peaks++;
                //Debug.Log("Peak detected at " + currentValue + " | BPM: " + breathRateBPM + " | Peaks:" + peaks + " | Depth: " + breathDepth);       
                }
            
            }

        //------------------------------------
        //---------- Trough Detection --------
        //------------------------------------

        if (wasFalling && isRising)
        {
            if (currentValue < minimumTroughValue)
            {
                if (Time.time - lastTroughTime > minimumTroughInterval)
                {
                    lastTroughTime = Time.time;

                    lastTroughValue = currentValue;

                    //Debug.Log("Trough detected at " + lastTroughValue);
                }
            }
        }

        //------------------------------------
        //---------- State Tracking ----------
        //------------------------------------

        if (isRising)
        {
            wasRising = true;
            wasFalling = false;
        }

        if (isFalling)
        {
            wasFalling = true;
            wasRising = false;
        }
    }

    private void AddBPMReading(float bpm)
    {
        recentBPMs.Enqueue(bpm);

        if (recentBPMs.Count > longTermAverageSampleCount)
        {
            recentBPMs.Dequeue();            
        }

        longTermAverageBreathRateBPM = recentBPMs.Average();
        shortTermAverageBreathRateBPM = recentBPMs.TakeLast(shortTermAverageSampleCount).Average();
    }

    private void AddDepthReading(float depth)
    {
        recentDepths.Enqueue(depth);

        if (recentDepths.Count > longTermAverageSampleCount)
        {
            recentDepths.Dequeue();
        }

        longTermAverageBreathDepth = recentDepths.Average();
        shortTermAverageBreathDepth = recentDepths.TakeLast(shortTermAverageSampleCount).Average();
    }
}
