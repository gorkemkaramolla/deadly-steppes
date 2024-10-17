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

    private PlayerSelection playerInputActions; // Reference to the generated input actions class

    public LayerMask groundLayer;
    public LayerMask clickable;
    private Camera cam;
    public GameObject groundMarker;

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

        // Initialize input actions
        playerInputActions = new PlayerSelection();
    }

    private void OnEnable()
    {
        playerInputActions.Enable();

        // Bind the actions from the Input Actions asset
        playerInputActions.SelectionAction.SelectionControls.performed += ctx => HandleLeftClick();
        playerInputActions.SelectionAction.PointerPosition.performed += ctx => HandleRightClick();
        playerInputActions.SelectionAction.ControlA.performed += ctx => ToggleSelectAllUnits(); // Ctrl+A for Windows/Linux
    }

    private void OnDisable()
    {
        playerInputActions.Disable();

        // Unbind the actions when disabled
        playerInputActions.SelectionAction.SelectionControls.performed -= ctx => HandleLeftClick();
        playerInputActions.SelectionAction.PointerPosition.performed -= ctx => HandleRightClick();
        playerInputActions.SelectionAction.ControlA.performed -= ctx => ToggleSelectAllUnits();
    }

    private void Start()
    {
        cam = Camera.main; // Get the main camera
    }

    private void HandleLeftClick()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickable))
        {
            if (playerInputActions.SelectionAction.SelectShift.ReadValue<float>() > 0)
            {
                MultiSelectUnits(hit.collider.gameObject);
            }
            else
            {
                SelectUnits(hit.collider.gameObject);
            }
        }
        else if (playerInputActions.SelectionAction.SelectShift.ReadValue<float>() == 0)
        {
            DeselectUnits();
        }
    }

    private void HandleRightClick()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame && selectedUnits.Count > 0)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                groundMarker.SetActive(false);
                groundMarker.SetActive(true);
                groundMarker.transform.position = hit.point;
            }
        }
    }

    internal void DeselectUnits()
    {
        selectedUnits.ForEach(unit =>
        {
            unit.transform.GetChild(0).gameObject.SetActive(false);
            EnableUnitMovement(unit, false);
        });
        groundMarker.SetActive(false);
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
        unit.transform.GetChild(0).gameObject.SetActive(true);
        selectedUnits.Add(unit);
        EnableUnitMovement(unit, true);
    }

    public void DragSelect(GameObject unit)
    {
        if (!selectedUnits.Contains(unit))
        {
            PerformSelection(unit);
        }
    }

    private void SelectAllUnits()
    {
        DeselectUnits(); // Clear current selection before selecting all

        foreach (var unit in allUnits)
        {
            if (unit != null)
            {
                PerformSelection(unit); // Add all units to selection
            }
        }
    }

    private void ToggleSelectAllUnits()
    {
        if (selectedUnits.Count == allUnits.Count)
        {
            // If all units are already selected, deselect all
            DeselectUnits();
        }
        else
        {
            // Otherwise, select all units
            if (selectedUnits.Count > 0)
            {
                SelectAllUnits();

            }
        }
    }
}
