using System.Collections.Generic;
using UnityEngine;

public class Tags
{
    public static string WALL = "Wall";
    public static string POWERUP = "Powerup";
    public static string TAIL = "Tail";
    public static string SNAKE = "Snake";
}


public class Metrics
{
    public static float NODE = 0.95f;
}

public enum PlayerDirection
{
    LEFT = 0,
    UP = 1,
    RIGHT = 2,
    DOWN = 3,
    COUNT = 4
}
