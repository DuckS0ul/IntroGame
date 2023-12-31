using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour{

    public Vector2 moveValue;
    public float speed;
    private int count = 0;
    private int numPickups = 5;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI positionText;

    private Vector3 oldPosition;

    void Start()
    {
        count = 0;
        winText.text = "";
        SetCountText();
    }

    void OnMove(InputValue value){
        moveValue = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);

        GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime);

        Vector3 currentPosition = transform.position;
        Vector3 velocity = (currentPosition - oldPosition) / Time.fixedDeltaTime;

        positionText.text = "Position: " + currentPosition.ToString("F2");
        velocityText.text = "Velocity: " + velocity.ToString("F2");

        oldPosition = currentPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PickUp")
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    private void SetCountText()
    {
        scoreText.text = "Score: " + count.ToString();
        if(count >= numPickups)
        {
            winText.text = "You win!";
        }
    }
}