using UnityEngine;
using System.Collections.Generic;

public class DistrictManager : MonoBehaviour
{
    [Header("Material Settings")]
    [Tooltip("Drag your transparent 'Aura' material template here.")]
    public Material auraMaterialTemplate;

    [Tooltip("0 is fully invisible, 1 is fully solid.")]
    [Range(0f, 1f)]
    public float transparency = 0.4f;

    [Header("Color Palette")]
    public Color[] districtColors = new Color[]
    {
        Color.red,                 // Pair 1
        Color.blue,                // Pair 2
        Color.green,               // Pair 3
        Color.yellow,              // Pair 4
        Color.cyan,                // Pair 5
        Color.magenta,             // Pair 6
        new Color(1f, 0.5f, 0f),   // Pair 7: Orange
        new Color(0.5f, 0f, 1f),   // Pair 8: Purple
        new Color(0.5f, 1f, 0f),   // Pair 9: Lime
        new Color(1f, 0.4f, 0.7f), // Pair 10: Pink
        new Color(0f, 0.5f, 0.5f), // Pair 11: Teal
        Color.white,               // Pair 12: White
        new Color(0.4f, 0.8f, 1f), // Pair 13: Light Blue
        new Color(0.6f, 1f, 0.6f), // Pair 14: Mint
        new Color(1f, 0.5f, 0.4f)  // Pair 15: Coral
    };

    void Start()
    {
        AssignColorPairs();
    }

    void AssignColorPairs()
    {
        // Gather all child blocks
        List<Transform> availableBlocks = new List<Transform>();
        foreach (Transform child in transform)
        {
            availableBlocks.Add(child);
        }

        Shuffle(availableBlocks);

        // Track which color we are currently assigning
        int colorIndex = 0;

        // Loop as long as there are at least 2 blocks left to make a pair
        while (availableBlocks.Count >= 2)
        {
            // Pick the color sequentially from the palette
            Color pairColor = districtColors[colorIndex];

            // Apply the transparency value to the color's Alpha channel
            pairColor.a = transparency;

            // Pop two blocks
            Transform blockA = availableBlocks[availableBlocks.Count - 1];
            availableBlocks.RemoveAt(availableBlocks.Count - 1);

            Transform blockB = availableBlocks[availableBlocks.Count - 1];
            availableBlocks.RemoveAt(availableBlocks.Count - 1);

            // Apply the aura material
            ApplyAuraColor(blockA, pairColor);
            ApplyAuraColor(blockB, pairColor);

            // Move to the next color in the list.
            colorIndex = (colorIndex + 1) % districtColors.Length;
        }

        // Catch any single block left over (if you have an odd number of total blocks)
        if (availableBlocks.Count == 1)
        {
            Debug.Log("One block left over without a pair! Assigning it a gray, inactive state.");
            Color grayState = new Color(0.5f, 0.5f, 0.5f, transparency);
            ApplyAuraColor(availableBlocks[0], grayState);
        }
    }

    void ApplyAuraColor(Transform block, Color newColor)
    {
        MeshRenderer renderer = block.GetComponent<MeshRenderer>();

        if (renderer != null && auraMaterialTemplate != null)
        {
            // Create a unique instance of your template material
            Material uniqueMat = new Material(auraMaterialTemplate);

            // Standard assignment fallback for built-in pipelines
            uniqueMat.color = newColor;

            // Force URP BaseColor assignment if the shader uses it
            if (uniqueMat.HasProperty("_BaseColor"))
            {
                uniqueMat.SetColor("_BaseColor", newColor);
            }

            renderer.material = uniqueMat;
        }
        else if (auraMaterialTemplate == null)
        {
            Debug.LogError("Missing Template! Drag your Aura material template into the DistrictManager script in the Inspector.");
        }
    }

    void Shuffle(List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Transform temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}