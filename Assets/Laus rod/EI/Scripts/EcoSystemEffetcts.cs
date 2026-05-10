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

    public Color[] leafColors;
    public Color[] trunkColors;
    public Color groundColor;
}

public class EcoSystemEffetcts : MonoBehaviour
{
    [SerializeField] private EcoSystemControls ecosystemControls;
    [SerializeField] private List<SeasonColors> seasonColors;

    [Header("Season Influences")]
    [Range(0, 1)] public float springInfluence;
    [Range(0, 1)] public float summerInfluence;
    [Range(0, 1)] public float fallInfluence;
    [Range(0, 1)] public float winterInfluence;

    [Header("Light")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Color coldLightColor;
    [SerializeField] private Color warmLightColor;

    [Header("Trees")]
    [Range(0.6f, 1)] private float treeMotionTime;

    [SerializeField] private Material leafMaterial;
    [SerializeField] private Material trunkMaterial;


    [Header("Terrain")]
    [SerializeField] private Material groundMaterial;
    [SerializeField] private Terrain terrain;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //----------------------------------------------------
        //------------ Seasons -------------------------------
        //----------------------------------------------------
        Vector2 ecoSystemPoint =
            new Vector2(
                ecosystemControls.calmness, 
                ecosystemControls.fertility
                );

        Vector2 springPoint = new Vector2(0.8f, 0.5f);
        Vector2 summerPoint = new Vector2(1f, 1f);
        Vector2 fallPoint = new Vector2(0.3f, 0.5f);
        Vector2 winterPoint = new Vector2(0f, 0f);

        float springDistance = Vector2.Distance(ecoSystemPoint, springPoint);
        float summerDistance = Vector2.Distance(ecoSystemPoint, summerPoint);
        float fallDistance = Vector2.Distance(ecoSystemPoint, fallPoint);
        float winterDistance = Vector2.Distance(ecoSystemPoint, winterPoint);

        springInfluence = 1f - springDistance;
        springInfluence = Mathf.Pow(springInfluence, 3f);
        summerInfluence = 1f - summerDistance;
        summerInfluence = Mathf.Pow(summerInfluence, 3f);
        fallInfluence = 1f - fallDistance;
        fallInfluence = Mathf.Pow(fallInfluence, 3f);
        winterInfluence = 1f - winterDistance;
        winterInfluence = Mathf.Pow(winterInfluence, 3f);

        float total = springInfluence + summerInfluence + fallInfluence + winterInfluence;

        springInfluence /= total;
        summerInfluence /= total;
        fallInfluence /= total;
        winterInfluence /= total;

        //----------------------------------------------------
        //------------ Lights --------------------------------
        //----------------------------------------------------

        directionalLight.intensity =
        Mathf.Lerp(
            0.3f,
            1.5f,
            ecosystemControls.shortTermFertility
        );

        directionalLight.color =
        Color.Lerp(
            coldLightColor,
            warmLightColor,
            ecosystemControls.shortTermWarmth 
                * 0.5f
        );

        //----------------------------------------------------
        //------------ Terrain -------------------------------
        //----------------------------------------------------

        terrain.terrainData.wavingGrassStrength = ecosystemControls.shortTermCalmness;
        
        terrain.terrainData.wavingGrassAmount = ecosystemControls.shortTermFertility;
        

        //----------------------------------------------------
        //------------ Season Colours ------------------------
        //----------------------------------------------------

        SeasonColors spring = GetSeasonColours(Season.Spring);

        SeasonColors summer = GetSeasonColours(Season.Summer);

        SeasonColors fall = GetSeasonColours(Season.Fall);

        SeasonColors winter = GetSeasonColours(Season.Winter);

        Color lightLeafColour = GetColor(spring.leafColors, summer.leafColors, 
                                        fall.leafColors, winter.leafColors, 0);

        Color midLeafColour = GetColor(spring.leafColors, summer.leafColors,
                                        fall.leafColors, winter.leafColors, 1);

        Color darkLeafColour = GetColor(spring.leafColors, summer.leafColors,
                                        fall.leafColors, winter.leafColors, 2);

        

        Color lightTrunkColor = GetColor(spring.trunkColors, summer.trunkColors,
                                        fall.trunkColors, winter.trunkColors, 0);

        Color midTrunkColor = GetColor(spring.trunkColors, summer.trunkColors,
                                        fall.trunkColors, winter.trunkColors, 1);

        Color darkTrunkColor = GetColor(spring.trunkColors, summer.trunkColors,
                                        fall.trunkColors, winter.trunkColors, 2);

        
        Color groundColor = spring.groundColor * springInfluence +
                            summer.groundColor * summerInfluence +
                            fall.groundColor * fallInfluence +
                            winter.groundColor * winterInfluence;

        leafMaterial.SetColor("_LightGreen", lightLeafColour);
        leafMaterial.SetColor("_MidGreen", midLeafColour);
        leafMaterial.SetColor("_DarkGreen", darkLeafColour);

        trunkMaterial.SetColor("_LightGreen", lightTrunkColor);
        trunkMaterial.SetColor("_MidGreen", midTrunkColor);
        trunkMaterial.SetColor("_DarkGreen", darkTrunkColor);

        groundMaterial.color = groundColor;
    }

    private SeasonColors GetSeasonColours(Season season)
    {
        return seasonColors.Find(s => s.season == season);
    }

    private Color GetColor(Color[] springColours, Color[] summerColours,
                           Color[] fallColours, Color[] winterColours ,int index)
    {
        Color color = springColours[index] * springInfluence +
              summerColours[index] * summerInfluence +
              fallColours[index] * fallInfluence +
              winterColours[index] * winterInfluence;
        return color;   
    }
}
