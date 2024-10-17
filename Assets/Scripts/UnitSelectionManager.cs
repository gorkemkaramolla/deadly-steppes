using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionManager : MonoBehaviour
{
    public List<GameObject> allUnits = new();
    public List<GameObject> selectedUnits = new();
    public static UnitSelectionManager Instance { get; set; }
    private InputAction leftClick;
    private InputAction shiftKey;
    public LayerMask groundLayer;
    public LayerMask clickable;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        leftClick = new InputAction(binding: "<Mouse>/leftButton");
        shiftKey = new InputAction(binding: "<Keyboard>/shift");
        leftClick.Enable();
        shiftKey.Enable();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (!leftClick.WasPerformedThisFrame()) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickable))
        {
            if (shiftKey.IsPressed())
            {
                MultiSelectUnits(hit.collider.gameObject);
            }
            else
            {
                SelectUnits(hit.collider.gameObject);
            }
        }
        else if (!shiftKey.IsPressed())
        {
            DeselectUnits();
        }
    }

    private void DeselectUnits()
    {
        selectedUnits.ForEach(unit =>
        {
            EnableUnitMovement(unit, false);
            Transform selectionHare = unit.transform.Find("SelectionHare");
            if (selectionHare != null)
            {
                if (selectionHare.TryGetComponent<MeshRenderer>(out var meshRenderer))
                {
                    meshRenderer.enabled = false;
                }
            }
        });

        selectedUnits.Clear();

    }

    private void SelectUnits(GameObject unit)
    {
        DeselectUnits();
        PerformSelection(unit);

    }

    private void MultiSelectUnits(GameObject unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            PerformSelection(unit);
        }
    }

    private void EnableUnitMovement(GameObject unit, bool shouldMove)
    {
        unit.GetComponent<UnitMovement>().enabled = shouldMove;
    }


    private void PerformSelection(GameObject unit)
    {
        selectedUnits.Add(unit);
        EnableUnitMovement(unit, true);

        // Find the child object named "Cylinder"
        Transform selectionHare = unit.transform.Find("SelectionHare");
        if (selectionHare != null)
        {
            if (selectionHare.TryGetComponent<MeshRenderer>(out var meshRenderer))
            {
                meshRenderer.enabled = true;
            }
        }
    }
}
