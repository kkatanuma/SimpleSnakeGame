using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerDirection direction;
    private int horizontal = 0;
    private int vertical = 0;
    public Snake snake;
    private float lastInputTime;
    public float inputDelay = 0.1f;

    public enum Axis
    {
        Horizontal,
        Vertical
    }

    private void Awake()
    {
        snake = GetComponent<Snake>();
    }

    void Update()
    {
        horizontal = 0;
        vertical = 0;
        GetKeyboardInput();
        SetMovement();
    }

    void GetKeyboardInput()
    {
        if (Time.time - lastInputTime < inputDelay)
        {
            return;
        }

        int newHorizontal = GetAxisRaw(Axis.Horizontal);
        int newVertical = GetAxisRaw(Axis.Vertical);

        if (newHorizontal != 0)
        {
            horizontal = newHorizontal;
            vertical = 0;
            lastInputTime = Time.time;
        }
        else if (newVertical != 0)
        {
            vertical = newVertical;
            lastInputTime = Time.time;
        }
    }

    void SetMovement()
    {
        if (vertical != 0)
        {
            SetInputDirection((vertical == 1) ? PlayerDirection.UP : PlayerDirection.DOWN);
        }
        else if (horizontal != 0)
        {
            SetInputDirection((horizontal == 1) ? PlayerDirection.RIGHT : PlayerDirection.LEFT);
        }
    }

    int GetAxisRaw(Axis axis)
    {
        if (axis == Axis.Horizontal)
        {
            bool left = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
            bool right = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);

            if (left)
            {
                return -1;
            }
            if (right)
            {
                return 1;
            }
            return 0;
        }
        else if (axis == Axis.Vertical)
        {
            bool up = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
            bool down = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);

            if (up)
            {
                return 1;
            }
            if (down)
            {
                return -1;
            }
            return 0;
        }
        return 0;
    }

    public void SetInputDirection(PlayerDirection inputDir)
    {
        //Prevent change direction to opposite sides
        if (inputDir == PlayerDirection.UP && snake.m_currentDirection == PlayerDirection.DOWN ||
           inputDir == PlayerDirection.DOWN && snake.m_currentDirection == PlayerDirection.UP ||
           inputDir == PlayerDirection.RIGHT && snake.m_currentDirection == PlayerDirection.LEFT ||
           inputDir == PlayerDirection.LEFT && snake.m_currentDirection == PlayerDirection.RIGHT)
        {
            return;
        }
        else
        {
            snake.m_currentDirection = inputDir;
            //TO handle playerInput immediately
            //snake.ForceMove();
        }
    }
}
