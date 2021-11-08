﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private CapsuleCollider col;
    private Animator anim;
    private Vector3 dir;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private int coins;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Text coinsText;

    private bool isSliding;


    private int lineToMove = 1;
    public float lineDistance = 4;
    private float maxSpeed = 120;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        col = GetComponent<CapsuleCollider>();
        Time.timeScale = 1;
        anim = GetComponentInChildren<Animator>();
        coins = PlayerPrefs.GetInt("coins");
        coinsText.text = coins.ToString();
        StartCoroutine(SpeedIncrease());
    }

    void Update()
    {
        if (SwipeController.swipeRight)
        {
            if (lineToMove < 2)
                lineToMove++;
        }
        if (SwipeController.swipeLeft)
        {
            if (lineToMove > 0)
                lineToMove--;
        }

        if (SwipeController.swipeUp)
        {
            if (controller.isGrounded)
                Jump();
        }

        if (SwipeController.swipeDown)
        {
            StartCoroutine(Slide());
        }

        if (controller.isGrounded && !isSliding)
            anim.SetBool("isRunning", true);
        else
            anim.SetBool("isRunning", false);

        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (lineToMove == 0)
            targetPosition += Vector3.left * lineDistance;
        else if (lineToMove == 2)
            targetPosition += Vector3.right * lineDistance;

        if (transform.position == targetPosition)
            return;
        Vector3 diff = targetPosition - transform.position;
        Vector3 MoveDir = diff.normalized * 25 * Time.deltaTime;
        if (MoveDir.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(MoveDir);
        else
            controller.Move(diff);

        //speed += 0.1f * Time.deltaTime;
    }

    private void Jump()
    {
        dir.y = jumpForce;
        anim.SetTrigger("isJumping");
    }

    private void FixedUpdate()
    {
        dir.z = speed;
        dir.y += gravity * Time.fixedDeltaTime;
        controller.Move(dir * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "obstacle")
        {
            losePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.tag == "Coin")
        {
            coins++;
            PlayerPrefs.SetInt("coins", coins);
            coinsText.text = coins.ToString();
            Destroy(other.gameObject);
        } 
    }

    private IEnumerator SpeedIncrease()
    {
        yield return new WaitForSeconds(4);
        if (speed < maxSpeed)
        {
            speed += 3;
            StartCoroutine(SpeedIncrease());
        }
    }
    private IEnumerator Slide()
    {
        col.center = new Vector3(0, -0.4f, 0);
        col.height = 2;
        isSliding = true;
        yield return new WaitForSeconds(1);
        col.center = new Vector3(0, 0.8184886f, 0);
        col.height = 4.420422f;
        isSliding = false;
    }
}
