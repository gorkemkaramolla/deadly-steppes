using UnityEngine;
using UnityEngine.InputSystem; // New Input System

public class UnitSelectionBox : MonoBehaviour
{
    private Camera myCam;
    private PlayerSelection playerInputActions; // Reference to the generated input actions class

    [SerializeField]
    private RectTransform boxVisual;

    private Rect selectionBox;
    private Vector2 startPosition;
    private Vector2 endPosition;

    private bool isDragging = false;

    private void Awake()
    {
        playerInputActions = new PlayerSelection(); // Initialize the Input Actions
        myCam = Camera.main; // Ensure camera is assigned
    }

    private void Start()
    {
        // Initially hide the box visual
        boxVisual.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        playerInputActions.Enable(); // Enable input actions

        // Bind the actions to their respective callbacks
        playerInputActions.SelectionAction.SelectionControls.performed += OnClickStarted;   // Mouse button pressed
        playerInputActions.SelectionAction.SelectionControls.canceled += OnClickReleased;   // Mouse button released
        playerInputActions.SelectionAction.PointerPosition.performed += OnPointerMove;      // Mouse moved
    }

    private void OnDisable()
    {
        playerInputActions.Disable(); // Disable input actions

        // Unbind the actions when disabled
        playerInputActions.SelectionAction.SelectionControls.performed -= OnClickStarted;
        playerInputActions.SelectionAction.SelectionControls.canceled -= OnClickReleased;
        playerInputActions.SelectionAction.PointerPosition.performed -= OnPointerMove;
    }

    private void OnClickStarted(InputAction.CallbackContext context)
    {
        // Start selection box on mouse click
        startPosition = playerInputActions.SelectionAction.PointerPosition.ReadValue<Vector2>();
        selectionBox = new Rect(); // Initialize new selection box
        isDragging = false; // Reset dragging state
    }

    private void OnClickReleased(InputAction.CallbackContext context)
    {
        // If dragging was happening, select units on mouse release
        if (isDragging)
        {
            SelectUnits();
            isDragging = false; // Reset dragging state
        }

        // Immediately hide the box visual when mouse is released
        boxVisual.gameObject.SetActive(false);

        // Clear the selection
        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        DrawVisual(); // Clear the visual box
    }

    private void OnPointerMove(InputAction.CallbackContext context)
    {
        // Update the end position while dragging (mouse button is still pressed)
        if (playerInputActions.SelectionAction.SelectionControls.ReadValue<float>() > 0) // If the mouse button is pressed
        {
            endPosition = playerInputActions.SelectionAction.PointerPosition.ReadValue<Vector2>();

            // If mouse has moved significantly, start dragging
            if (!isDragging && (endPosition - startPosition).magnitude > 5f) // 5f is a threshold to avoid small accidental drags
            {
                isDragging = true;
                boxVisual.gameObject.SetActive(true); // Show the selection box
            }

            if (isDragging)
            {
                DrawVisual(); // Draw the selection box
                DrawSelection(); // Update selection box bounds
            }
        }
    }

    void DrawVisual()
    {
        // Calculate the start and end positions of the selection box
        Vector2 boxStart = startPosition;
        Vector2 boxEnd = endPosition;

        // Calculate the center of the selection box
        Vector2 boxCenter = (boxStart + boxEnd) / 2;

        // Set the position of the visual selection box based on its center
        boxVisual.position = boxCenter;

        // Calculate the size of the selection box in width and height
        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

        // Set the size of the visual selection box
        boxVisual.sizeDelta = boxSize;
    }

    void DrawSelection()
    {
        // Update the selection box's bounds based on the mouse position
        Vector2 currentMousePosition = playerInputActions.SelectionAction.PointerPosition.ReadValue<Vector2>();

        if (currentMousePosition.x < startPosition.x)
        {
            selectionBox.xMin = currentMousePosition.x;
            selectionBox.xMax = startPosition.x;
        }
        else
        {
            selectionBox.xMin = startPosition.x;
            selectionBox.xMax = currentMousePosition.x;
        }

        if (currentMousePosition.y < startPosition.y)
        {
            selectionBox.yMin = currentMousePosition.y;
            selectionBox.yMax = startPosition.y;
        }
        else
        {
            selectionBox.yMin = startPosition.y;
            selectionBox.yMax = currentMousePosition.y;
        }
    }

    void SelectUnits()
    {
        // Check if the selection manager and the list of units are initialized
        if (UnitSelectionManager.Instance == null || UnitSelectionManager.Instance.allUnits == null)
        {
            return;
        }

        foreach (var unit in UnitSelectionManager.Instance.allUnits)
        {
            if (unit != null && selectionBox.Contains(myCam.WorldToScreenPoint(unit.transform.position)))
            {
                UnitSelectionManager.Instance.DragSelect(unit);
            }
        }
    }
}
