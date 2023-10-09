using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI distanceText;
    private LineRenderer lineRenderer;
    private DebugMode currentDebugMode = DebugMode.Normal;

    public enum DebugMode
    {
        Normal,
        Distance,
        Vision
    }

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentDebugMode = (DebugMode)(((int)currentDebugMode + 1) % 3);
        }

        GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickUp");

        switch (currentDebugMode)
        {
            case DebugMode.Distance:
                HandleDistanceMode(pickups);
                break;

            case DebugMode.Vision:
                HandleVisionMode(pickups);
                break;

            default:
                HandleNormalMode(pickups);
                break;
        }
    }

    void HandleDistanceMode(GameObject[] pickups)
    {
        GameObject closestPickup = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject pickup in pickups)
        {
            if (pickup.activeSelf)
            {
                float distance = Vector3.Distance(transform.position, pickup.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPickup = pickup;
                }
            }
        }

        if (closestPickup != null)
        {
            distanceText.text = "Distance to closest pickup: " + closestDistance.ToString("F2");

            // Draw line
            lineRenderer.positionCount = 2;  // Ensure position count is set correctly
            lineRenderer.SetPosition(0, transform.position);

            // Calculate direction towards closest pickup
            Vector3 direction = closestPickup.transform.position - transform.position;
            // Normalize the direction to get a unit vector
            direction.Normalize();
            // Multiply by a distance (you can adjust this value)
            float targetDistance = closestDistance; // Use closest distance as target distance
            Vector3 targetPosition = transform.position + direction * targetDistance;

            lineRenderer.SetPosition(1, targetPosition);

            // Highlight closest pickup
            foreach (GameObject pickup in pickups)
            {
                pickup.GetComponent<Renderer>().material.color = Color.white;
            }
            closestPickup.GetComponent<Renderer>().material.color = Color.blue;
        }
        else
        {
            distanceText.text = "No active pickups";

            // Clear line renderer
            lineRenderer.positionCount = 0;
        }
    }

    void HandleVisionMode(GameObject[] pickups)
    {
        GameObject closestPickup = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject pickup in pickups)
        {
            if (pickup.activeSelf)
            {
                float distance = Vector3.Distance(transform.position, pickup.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPickup = pickup;
                }
            }
        }

        if (closestPickup != null)
        {
            distanceText.text = "Distance to closest pickup: " + closestDistance.ToString("F2");

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + GetComponent<Rigidbody>().velocity);

            GameObject likelyToCollect = FindLikelyToCollectPickup(pickups);

            foreach (GameObject pickup in pickups)
            {
                pickup.GetComponent<Renderer>().material.color = Color.white;
            }
            if (likelyToCollect != null)
            {
                likelyToCollect.GetComponent<Renderer>().material.color = Color.green;
                likelyToCollect.transform.LookAt(transform);
            }
        }
        else
        {
            distanceText.text = "No active pickups";
        }
    }

    void HandleNormalMode(GameObject[] pickups)
    {
        foreach (GameObject pickup in pickups)
        {
            pickup.GetComponent<Renderer>().material.color = Color.white;
        }

        distanceText.text = "";
        lineRenderer.positionCount = 0;
    }

    GameObject FindLikelyToCollectPickup(GameObject[] pickups)
    {
        float closestApproach = float.MaxValue;
        GameObject likelyPickup = null;

        foreach (GameObject pickup in pickups)
        {
            if (pickup.activeSelf)
            {
                Vector3 relativePosition = pickup.transform.position - transform.position;
                float approachDotProduct = Vector3.Dot(relativePosition.normalized, GetComponent<Rigidbody>().velocity.normalized);
                if (approachDotProduct > 0 && relativePosition.magnitude / approachDotProduct < closestApproach)
                {
                    closestApproach = relativePosition.magnitude / approachDotProduct;
                    likelyPickup = pickup;
                }
            }
        }

        return likelyPickup;
    }
}
