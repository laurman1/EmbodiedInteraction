using Meta.Voice.UnityOpus;
using System.Collections.Generic;
using UnityEngine;


public enum Season
{
    Spring,
    Summer,
    Fall,
    Winter
}

[System.Serializable]
public class SeasonColors
{
    public Season season;

    public Color[] leafcolors;
    public Color[] trunkColors;
    public Color groundColor;
    public Color lightColor;
    public float lightIntensity;
}


public class SeasonControl : MonoBehaviour
{
    [SerializeField]
    private List<SeasonColors> seasonColors;
    
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
