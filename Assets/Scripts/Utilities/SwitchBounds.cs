using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchBounds : MonoBehaviour
{
    // TODO: Use this Func again on Change Scene
    private void Start()
    {
        SwitchConfinerShape();
    }

    private void SwitchConfinerShape()
    {
        PolygonCollider2D configerShape = GameObject.FindGameObjectWithTag("BoundsConfiger").GetComponent<PolygonCollider2D>();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

        confiner.m_BoundingShape2D = configerShape;

        // Call this if the bounding shape's points change at runtime
        confiner.InvalidatePathCache();
    }
}
