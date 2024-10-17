using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : MonoBehaviour
{
    Camera cam;
    NavMeshAgent agent;
    public LayerMask groundLayer;
    private InputAction clickAction;

    void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();

        clickAction = new InputAction(binding: "<Mouse>/RightButton");
        clickAction.Enable();
    }

    void Update()
    {
        // Check if the left mouse button was clicked
        if (clickAction.WasPerformedThisFrame())
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    // Optionally, disable the InputAction when the object is destroyed
    void OnDestroy()
    {
        clickAction.Disable();

    }
}
