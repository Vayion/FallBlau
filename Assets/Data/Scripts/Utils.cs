using System;
using UnityEngine;

public class Utils : MonoBehaviour
{
    
    public static bool checkRaycast(GameObject obj)
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
            
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == obj)
            {
                return true;
            }
        }

        return false;
    }
}
