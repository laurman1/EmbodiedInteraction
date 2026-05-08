using UnityEngine;

public class SeasonControl : MonoBehaviour
{
    public Color topColor;
    

    public Material material;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        topColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
        //topColor = material.GetColor("LightGreen");

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeColor()
    {
        //material.SetColor("LightGreen", topColor);
        material.SetVector("LightGreen", topColor);

    }
}
