using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Vector3 diff;
    private Vector3 origin;

    private bool enableDragging = false;
    private bool drag = false;
    void Start()
    {
        origin = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            enableDragging = !enableDragging;
            // reset camera
            if (!enableDragging)
            {
                transform.SetParent(Globals.player.transform);
                transform.localPosition = new Vector3(0, 0, -10);
            }
        }
    }

    private void LateUpdate()
    {
        if (!enableDragging)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            diff = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (!drag)
            {
                drag = true;
                origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        } else
        {
            drag = false;
        }
        if (drag)
        {
            Camera.main.transform.position = origin - diff;
        }
    }
}
