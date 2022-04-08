using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 3;
    public bool detectionMeter;
    public bool footSteps;

    private Rigidbody2D myRigidbody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Slider detectionSlider;

    private LineRenderer lineRenderer;
    private float maxRadius;
    private float lerpTime;
    private float pulseFrequency = .3f;

    [HideInInspector]
    public Vector2 velocity;
    void Start()
    {
        Globals.player = gameObject;
        Globals.playerScript = this;
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (detectionMeter)
        {
            GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
            detectionSlider = GetComponentInChildren<Slider>();
            detectionSlider.value = 1;
        }

        if (footSteps)
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = true;
            StartCoroutine(StepStarter());
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        DecreaseDetection();
        UpdateFootsteps();
    }

    private void UpdateFootsteps()
    {
        if (!footSteps)
        {
            return;
        }
        // Draw Footsteps
        float radius = Mathf.Lerp(0, maxRadius, (Time.time - lerpTime) / pulseFrequency);
        lineRenderer.positionCount = 30;
        for (int i = 0; i < 30; i++)
        {
            float current = (float)i / 30;
            float currentRad = current * 2 * Mathf.PI;

            float xScale = Mathf.Cos(currentRad);
            float yScale = Mathf.Sin(currentRad);

            lineRenderer.SetPosition(i, transform.position + new Vector3(xScale * radius, yScale * radius, 0));
        }
    }

    IEnumerator StepStarter()
    {
        while (true)
        {
            maxRadius = velocity.magnitude / 2;
            lerpTime = Time.time;
            yield return new WaitForSeconds(pulseFrequency);
            //Debug.Log(lerpTime);
        }
    }

    private void Movement()
    {
        velocity = Vector2.right * Input.GetAxisRaw("Horizontal") * speed + Vector2.up * Input.GetAxisRaw("Vertical") * speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocity = velocity / 2;
        }
        myRigidbody.velocity = velocity;
        if (velocity.magnitude > 0)
        {
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
        if (velocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (velocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void DecreaseDetection()
    {
        if (!detectionMeter)
        {
            return; 
        }
        detectionSlider.value = Mathf.Clamp(detectionSlider.value + Time.deltaTime / 2, 0, 1);
    }

    internal void IncreaseDetection(float v)
    {
        if (!detectionMeter)
        {
            return;
        }
        detectionSlider.value = Mathf.Clamp(detectionSlider.value - Time.deltaTime / 2, 0, 1);
        detectionSlider.value = Mathf.Clamp(detectionSlider.value - Time.deltaTime * v, 0, 1);
    }
}
