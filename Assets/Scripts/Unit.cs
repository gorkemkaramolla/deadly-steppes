using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnitSelectionManager.Instance.allUnits.Add(gameObject);
    }

    private void onDestroy()
    {
        UnitSelectionManager.Instance.allUnits.Remove(gameObject);
    }
    void Update()
    {

    }
}
