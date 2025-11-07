// ==== SnakePlayer2.cs ====
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake_2 : Snake
{
    

    public override void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W) && direction != Vector2.down)
            direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up)
            direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.right)
            direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left)
            direction = Vector2.right;
    }

   
}
