using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawManager : MonoBehaviour
{

    [SerializeField] private Line linePrefab;

    private Line currentLine;
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, transform);
        }

        if (Input.GetMouseButton(0))
        {
            if (currentLine.CanAppend(mousePos))
            {
                currentLine.AddPosition(mousePos);
            }
            else if(currentLine.IsAcuteAngle(mousePos))
            {
                currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, transform);
            }
        }
    }
}
