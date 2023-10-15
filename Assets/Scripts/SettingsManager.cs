using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    public Settings Settings;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}

[Serializable]
public class Settings
{
    public Vector2 GridSpaceXInWorldSpace = new Vector2(0.31f, 0.17f);
    public Vector2 GridSpaceYInWorldSpace = new Vector2(-0.31f, 0.17f);
}
