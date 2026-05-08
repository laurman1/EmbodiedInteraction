// BreathSimulator.cs
// Hold Space = inhale (amplitude rises)
// Release Space = exhale (pulse fires, amplitude decays)
// Attach to any GameObject in the scene

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using LSL;


public class BreathSimulator : MonoBehaviour
{
    [Header("LSL Input")]
    [Tooltip("Enable LSL inlet. If disabled or no stream found, keyboard input is used.")]
    public bool useLSLInput = true;

    [Tooltip("LSL stream name to resolve (must match stream's name property).")]
    public string streamName = "Breath";

    [Tooltip("Channel index to read from each sample chunk.")]
    public int lslChannelIndex = 1;

    [Tooltip("Applied to raw LSL value before normalization.")]
    public float lslOffset = 3f;

    [Tooltip("Raw+offset value mapped to normalized inhale=0.")]
    public float lslMin = 0f;

    [Tooltip("Raw+offset value mapped to normalized inhale=1.")]
    public float lslMax = 4f;

    [Tooltip("Normalized threshold used to detect inhale/exhale state from LSL.")]
    [Range(0f, 1f)]
    public float lslHoldThreshold = 0.15f;

    [Tooltip("Chunk duration for pull buffer allocation (seconds).")]
    public float maxChunkDuration = 0.2f;

    [Header("Manual Trigger")]
    [Tooltip("Hold to inhale, release to exhale")]
    public KeyCode breathKey = KeyCode.Space;

    [Header("Breath Timing")]
    [Tooltip("How fast amplitude rises while holding (seconds to full)")]
    [Range(0.5f, 4f)]
    public float inhaleTime = 1.5f;

    [Tooltip("How fast amplitude decays after release (seconds to zero)")]
    [Range(0.5f, 5f)]
    public float exhaleDecayTime = 2f;

    [Tooltip("Maximum breath amplitude")]
    [Range(0.1f, 1f)]
    public float maxAmplitude = 0.8f;

    [Header("Root Renderers")]
    [Tooltip("Drag root mesh renderers here")]
    public Renderer[] rootRenderers;

    // Internal
    private float customTime;
    private float breathAmplitude;
    private float breathPhase;
    private float lastExhaleTime = -10f;
    private float exhaleAmplitude; // amplitude at moment of release
    private float timeSinceExhale;
    private bool isHolding;
    private bool wasHolding;
    private int frameCount;
    private float lslNormalizedValue;

    private ContinuousResolver resolver;
    private StreamInlet inlet;
    private float[,] dataBuffer;
    private double[] timestampBuffer;

    void Start()
    {
        Debug.Log("[UMWELT] BreathSimulator started. Hold Space to inhale, release to exhale.");
        if (useLSLInput)
        {
            if (!string.IsNullOrWhiteSpace(streamName))
            {
                resolver = new ContinuousResolver("name", streamName);
                StartCoroutine(ResolveExpectedStream());
                Debug.Log($"[UMWELT] LSL resolver started for stream '{streamName}'.");
            }
            else
            {
                Debug.LogWarning("[UMWELT] LSL input enabled but streamName is empty. Falling back to keyboard.");
            }
        }

        if (rootRenderers == null || rootRenderers.Length == 0)
        {
            Debug.LogWarning("[UMWELT] No Root Renderers assigned.");
        }
        else
        {
            Debug.Log($"[UMWELT] {rootRenderers.Length} renderer(s) assigned.");
            foreach (var r in rootRenderers)
            {
                if (r != null)
                    Debug.Log($"[UMWELT]   -> {r.gameObject.name} (Mat: {r.sharedMaterial?.name ?? "None"})");
            }
        }
    }

    void Update()
    {
        customTime += Time.deltaTime;

        // Read control state from LSL if available, otherwise keyboard.
        isHolding = false;
        bool readFromLsl = false;
        if (useLSLInput && inlet != null && dataBuffer != null && timestampBuffer != null)
        {
            int samplesReturned = inlet.pull_chunk(dataBuffer, timestampBuffer);
            if (samplesReturned > 0)
            {
                int channelCount = inlet.info().channel_count();
                int safeChannel = Mathf.Clamp(lslChannelIndex, 0, Mathf.Max(0, channelCount - 1));
                float rawValue = dataBuffer[samplesReturned - 1, safeChannel];
                float mappedValue = rawValue + lslOffset;
                lslNormalizedValue = Mathf.InverseLerp(lslMin, lslMax, mappedValue);
                isHolding = lslNormalizedValue >= lslHoldThreshold;
                readFromLsl = true;

                if (frameCount % 60 == 0)
                {
                    Debug.Log($"[UMWELT] LSL raw={rawValue:F3} mapped={mappedValue:F3} norm={lslNormalizedValue:F3} hold={isHolding}");
                }
            }
        }

        if (!readFromLsl)
        {
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                Key keyEnum = GetKeyFromKeyCode(breathKey);
                if (keyEnum != Key.None)
                {
                    isHolding = keyboard[keyEnum].isPressed;
                }
            }
        }

        if (isHolding)
        {
            // INHALE: amplitude rises while holding
            breathAmplitude = Mathf.MoveTowards(breathAmplitude, maxAmplitude, (maxAmplitude / inhaleTime) * Time.deltaTime);

            // Phase: 0.0 to 0.5 during inhale, proportional to amplitude
            breathPhase = (breathAmplitude / maxAmplitude) * 0.5f;
        }
        else if (wasHolding && !isHolding)
        {
            // RELEASE: exhale triggers
            TriggerExhale();
        }
        else if (!isHolding && timeSinceExhale < exhaleDecayTime)
        {
            // EXHALE DECAY: amplitude falls after release
            timeSinceExhale = customTime - lastExhaleTime;
            float t = timeSinceExhale / exhaleDecayTime;
            breathAmplitude = exhaleAmplitude * (1f - t * t); // quadratic falloff

            // Phase: 0.5 to 1.0 during exhale decay
            breathPhase = 0.5f + t * 0.5f;
        }
        else if (!isHolding)
        {
            // IDLE: nothing happening
            breathAmplitude = Mathf.MoveTowards(breathAmplitude, 0f, 0.5f * Time.deltaTime);
            breathPhase = 0f;
        }

        wasHolding = isHolding;

        // Push global shader params
        Shader.SetGlobalFloat("_BreathAmplitude", breathAmplitude);
        Shader.SetGlobalFloat("_BreathPhase", breathPhase);
        Shader.SetGlobalFloat("_TimeCustom", customTime);

        // Push to root renderers
        if (rootRenderers != null)
        {
            foreach (var rend in rootRenderers)
            {
                if (rend == null) continue;

                MaterialPropertyBlock block = new MaterialPropertyBlock();
                rend.GetPropertyBlock(block);
                block.SetFloat("_BreathPhase", breathPhase);
                block.SetFloat("_BreathAmplitude", breathAmplitude);
                block.SetFloat("_TimeCustom", customTime);
                rend.SetPropertyBlock(block);
            }
        }

        // Debug log while active
        if (breathAmplitude > 0.01f && frameCount % 60 == 0)
        {
            string state = isHolding ? "INHALE" : "EXHALE";
            Debug.Log($"[UMWELT] {state} | Amp: {breathAmplitude:F3}  Phase: {breathPhase:F3}");
        }

        frameCount++;
    }

    IEnumerator ResolveExpectedStream()
    {
        if (resolver == null)
        {
            yield break;
        }

        var results = resolver.results();
        while (results.Length == 0)
        {
            yield return new WaitForSeconds(0.1f);
            results = resolver.results();
        }

        inlet = new StreamInlet(results[0]);
        int bufSamples = Mathf.CeilToInt((float)(inlet.info().nominal_srate() * maxChunkDuration));
        bufSamples = Mathf.Max(bufSamples, 1);
        int nChannels = inlet.info().channel_count();
        dataBuffer = new float[bufSamples, nChannels];
        timestampBuffer = new double[bufSamples];

        Debug.Log($"[UMWELT] LSL inlet connected. Channels={nChannels}, BufferSamples={bufSamples}");
    }

    void TriggerExhale()
    {
        lastExhaleTime = customTime;
        timeSinceExhale = 0f;
        exhaleAmplitude = breathAmplitude; // capture how deep the inhale was

        Debug.Log($"[UMWELT] >>> EXHALE <<< Strength: {exhaleAmplitude:F2}");

        if (rootRenderers != null)
        {
            foreach (var rend in rootRenderers)
            {
                if (rend == null) continue;

                MaterialPropertyBlock block = new MaterialPropertyBlock();
                rend.GetPropertyBlock(block);
                block.SetFloat("_PulseTriggerTime", customTime);
                block.SetFloat("_BreathAmplitude", exhaleAmplitude);
                rend.SetPropertyBlock(block);
            }
        }
    }

    private Key GetKeyFromKeyCode(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Space: return Key.Space;
            case KeyCode.W: return Key.W;
            case KeyCode.A: return Key.A;
            case KeyCode.S: return Key.S;
            case KeyCode.D: return Key.D;
            case KeyCode.Return: return Key.Enter;
            case KeyCode.Escape: return Key.Escape;
            default: return Key.None;
        }
    }

    void OnDestroy()
    {
        if (inlet != null)
        {
            inlet.Close();
            inlet = null;
        }

        resolver = null;
    }

    void OnDrawGizmosSelected()
    {
        // Yellow sphere scales with amplitude
        Gizmos.color = isHolding ? Color.green : Color.yellow;
        float scale = Mathf.Max(breathAmplitude, 0.1f) * 2f;
        Gizmos.DrawWireSphere(transform.position, scale);

        // Cyan line shows phase
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * breathPhase);
    }
}