﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    const float DEAD_VALUE = 0.4f;
    public static readonly float SPEED = 5f;
    Rigidbody2D thisRigidbody;

    bool activateSpeedModifier;
    float speedModifier;

    [SerializeField] Animator playerAnimator;

    string horizontalFloatName = "HorizontalValue";
    string verticalFloatName = "VerticalValue";

    [SerializeField] GameObject bigMap;

    GameplayManager gameplayManager;

    enum Direction {
        Idle,
        Up,
        Down,
        Left,
        Right
    };

    Direction playerDirection;

    // Mobile-specific variables
    Vector2 touchOrigin = -Vector2.one;

    // Start is called before the first frame update
    void Start()
    {
        thisRigidbody = GetComponent<Rigidbody2D>();
        // transform.position = GameObject.Find("GameplayManager").GetComponent<GameplayManager>().CurrentSpawnLocation;

        activateSpeedModifier = false;
        speedModifier = 1;

        playerAnimator = gameObject.GetComponent<Animator>();

        playerDirection = Direction.Idle;

        IsWalkingEnabled = true;

        if (bigMap != null)
            bigMap.SetActive(false);

        gameplayManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float xMovement = 0;
        float yMovement = 0;

        if (IsWalkingEnabled)
        {
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
            // xMovement = Input.GetAxis("Horizontal");
            // yMovement = Input.GetAxis("Vertical");
            xMovement = Input.GetAxisRaw("Horizontal");
            yMovement = Input.GetAxisRaw("Vertical");
#else
            if (Input.touchCount > 0)
            {
                Touch myTouch = Input.touches[0];
                if (myTouch.phase == TouchPhase.Began)
                {
                    touchOrigin = myTouch.position;
                }
                else if (myTouch.phase == TouchPhase.Ended)
                {
                    touchOrigin = -Vector2.one;
                }
                else if (myTouch.phase == TouchPhase.Moved || myTouch.phase == TouchPhase.Stationary)
                {
                    // Inside the bounds of the screen
                    Vector2 touchEnd = myTouch.position;
                    float x = touchEnd.x - touchOrigin.x;
                    float y = touchEnd.y - touchOrigin.y;
                
                    if (Mathf.Abs(x) > Mathf.Abs(y))
                    {
                        if (x > 0)
                        {
                            xMovement = 1;
                        }
                        else
                        {
                            xMovement = -1;
                        }
                    }
                    else
                    {
                        if (y > 0)
                        {
                            yMovement = 1;
                        }
                        else
                        {
                            yMovement = -1;
                        }
                    }
                }
            }
#endif

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (gameplayManager.HasUpgrade("Company Running Shoes"))
                {
                    speedModifier = 3;
                }
            }
            else
            {
                speedModifier = 1;
            }

            // Move
            thisRigidbody.velocity = new Vector2(xMovement * SPEED * speedModifier, yMovement * SPEED * speedModifier);

            Animate(thisRigidbody.velocity);

            if (Input.GetKeyDown(KeyCode.M))
            {
                bigMap.SetActive(!bigMap.activeInHierarchy);
            }
        }
        else
        {
            Animate(Vector2.zero);
        }
    }

    public void Animate(Vector2 direction)
    {
        playerAnimator.SetFloat(horizontalFloatName, direction.x);
        playerAnimator.SetFloat(verticalFloatName, direction.y);
    }

#if UNITY_EDITOR
    public void DebugActivateSpeedModifier(float mod)
    {
        speedModifier = mod;
    }
#endif

    public bool IsWalkingEnabled { get; set; }
}
