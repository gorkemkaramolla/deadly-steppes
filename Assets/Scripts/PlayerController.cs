using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 10.0f;
    private Vector2 move;

    public void onMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }
    public void movePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y);
        transform.Translate(movement * playerSpeed * Time.deltaTime, Space.World);

    }
    void Start()
    {

    }

    void Update()
    {
        movePlayer();
    }

}
