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
    private InputAction rightClick;
    public LayerMask groundLayer;
    public LayerMask clickable;
    private Camera cam;
    public GameObject groundMarker;

    private void Start()
    {
        cam = Camera.main;

        leftClick = new InputAction(binding: "<Mouse>/leftButton");
        shiftKey = new InputAction(binding: "<Keyboard>/shift");
        rightClick = new InputAction(binding: "<Mouse>/rightButton");

        leftClick.Enable();
        shiftKey.Enable();
        rightClick.Enable();
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

    private void handleLeftClick()
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
    private void handleRightClick()
    {
        if (!rightClick.WasPerformedThisFrame() || selectedUnits.Count < 1) return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
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
    private void Update()
    {
        handleLeftClick();
        handleRightClick();

    }

    private void DeselectUnits()
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
}
