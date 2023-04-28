using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Snake snake;
    private int horizontal = 0;
    private int vertical = 0;
    private float lastInputTime;
    private float inputDelay = 0.1f;

    public enum Axis
    {
        Horizontal,
        Vertical
    }

    void Awake()
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

    /// <summary>
    /// Handles Keyboard Input from a player
    /// </summary>
    void GetKeyboardInput()
    {
        //Prevent Snake from turning too quickly
        if (Time.time - lastInputTime < inputDelay) { return; }
        
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

    /// <summary>
    /// Set Direction based on the PlayerInput
    /// </summary>
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

    /// <summary>
    /// Depends on the Input key, it will modify horizontal and vertical axis
    /// </summary>
    /// <param name="axis"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Change current direction of snake, prevents snake from turning opposite direction
    /// </summary>
    /// <param name="inputDir"></param>
    public void SetInputDirection(PlayerDirection inputDir)
    {
        //Prevent change to opposite direction
        if (inputDir == PlayerDirection.UP && snake.CurrentDirection == PlayerDirection.DOWN ||
           inputDir == PlayerDirection.DOWN && snake.CurrentDirection == PlayerDirection.UP ||
           inputDir == PlayerDirection.RIGHT && snake.CurrentDirection == PlayerDirection.LEFT ||
           inputDir == PlayerDirection.LEFT && snake.CurrentDirection == PlayerDirection.RIGHT)
        {
            return;
        }
        else
        {
            snake.CurrentDirection = inputDir;

        }
    }
}
