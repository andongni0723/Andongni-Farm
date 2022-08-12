using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public const float itemfadeDuration = 0.35f;
    public const float targetAlpha = 0.45f;

    // Time
    public const float secondThreshold = 0.01f; // Value smaller, Game Time faster
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 10;
    public const int seasonHold = 3;

    //Transition
    public const float fadeDuration = 0.5f;

    // Max reap grasses amount in a time
    public const int reapAmount = 3;

    // NPC grid movement
    public const float gridCellSize = 1;
    public const float gridCellDiagonaSize = 1.41f;
    public const float pixelSize = 0.05f; // 20*20 => 1 unit
    public const float animationBreakTime = 5f;
    public const int maxGridSize = 9999;
}