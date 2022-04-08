using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Experimental.Rendering.Universal;

public class GuardAI : MonoBehaviour
{
    public Transform guardLocParent;
    public float speed = 3;

    private List<Transform> guardLocations;
    private int goingPos = 1;
    private Light2D flashLight;
    private Light2D surroundLight;
    private float flashLightRange;
    private float surroundLightRange;
    private float flashLightAngle;
    private LineRenderer lineRenderer;
    private bool playerInSightFlash = false;
    private bool playerInSightSurround = false;

    void Start()
    {
        guardLocations = new List<Transform>();
        for (int i = 0; i < guardLocParent.childCount; i++)
        {
            guardLocations.Add(guardLocParent.GetChild(i));
        }

        // Set starting pos and rotation
        transform.position = guardLocations[goingPos - 1].position;
        transform.up = (guardLocations[goingPos].position - guardLocations[goingPos - 1].position).normalized;

        // Setting light things
        flashLight = GetComponent<Light2D>();
        surroundLight = GetComponentsInChildren<Light2D>().First(x => x != flashLight);
        flashLightRange = flashLight.pointLightOuterRadius;
        surroundLightRange = surroundLight.pointLightOuterRadius;
        flashLightAngle = flashLight.pointLightOuterAngle;
        Debug.Log($"flashLightRange: {flashLightRange}, surroundLightRange: {surroundLightRange}, flashLightAngle: {flashLightAngle}");

        // Set up line render
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        FollowPath();
        FlashLight();
        Surroundlight();
        RenderLine();
        InformPlayer();
    }

    private void InformPlayer()
    {
        if (playerInSightSurround || playerInSightFlash)
        {
            Globals.playerScript.IncreaseDetection(flashLightRange - Vector2.Distance(transform.position, Globals.player.transform.position));
        }
        if (Globals.playerScript.footSteps)
        {

        }
    }

    private void RenderLine()
    {
        // make sure the line doesn't cross the player
        lineRenderer.SetPosition(0, transform.position);
        if ((Globals.player.transform.position - transform.position).magnitude < 
            ((Globals.player.transform.position - transform.position).normalized * flashLightRange).magnitude)
        {
            lineRenderer.SetPosition(1, transform.position +
                Globals.player.transform.position - transform.position);
        } else
        {
            lineRenderer.SetPosition(1, transform.position +
                (Globals.player.transform.position - transform.position).normalized * flashLightRange);
        }

        // colors depending if player in sight
        if (playerInSightSurround || playerInSightFlash)
        {
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        } else if (Globals.playerScript.footSteps && 
            Globals.playerScript.velocity.magnitude / 2 > 
            Vector2.Distance(Globals.player.transform.position, transform.position))
        {
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;
        }
        else
        {
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;
        }
    }

    private void Surroundlight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Globals.player.transform.position - transform.position, surroundLightRange);
        if (hit.collider != null && hit.collider.gameObject == Globals.player)
        {
            playerInSightSurround = true;
            return;
        }
        playerInSightSurround = false;
    }

    private void FlashLight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Globals.player.transform.position - transform.position, flashLightRange);
        if (hit.collider != null && hit.collider.gameObject == Globals.player)
        {
            if (Vector2.Angle(Globals.player.transform.position - transform.position, transform.up) < flashLightAngle / 2)
            {
                playerInSightFlash = true;
                return;
            }
        }
        playerInSightFlash = false;
    }

    private void FollowPath()
    {
        // Updating rotation and going to next position
        if (transform.position == guardLocations[goingPos % guardLocations.Count].position)
        {
            goingPos = (goingPos + 1) % guardLocations.Count;
            if (goingPos == 0)
            {
                transform.up = (guardLocations[0].position - guardLocations[guardLocations.Count - 1].position).normalized;
            }
            else
            {
                transform.up = (guardLocations[goingPos].position - guardLocations[goingPos - 1].position).normalized;
            }
        }

        // Moving in each frame
        transform.position = Vector2.MoveTowards(transform.position,
            guardLocations[goingPos].position,
            speed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        guardLocations = new List<Transform>();
        for (int i = 0; i < guardLocParent.childCount; i++)
        {
            guardLocations.Add(guardLocParent.GetChild(i));
        }
        Gizmos.color = Color.red;
        if(guardLocations!= null && guardLocations.Count > 2)
        {
            for (int i = 0; i < guardLocations.Count - 1; i++)
            {
                Gizmos.DrawLine(guardLocations[i].position, guardLocations[i + 1].position);
            }
            Gizmos.DrawLine(guardLocations[guardLocations.Count - 1].position, guardLocations[0].position);
        }
    }
}
