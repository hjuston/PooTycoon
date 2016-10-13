using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    public static bool GetInput()
    {
        if ((Input.touchCount > 0))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                return true;
            }
        }

        return false;
    }

    public static Vector3 GetInputPosition()
    {
        return Input.GetTouch(0).position;
    }
}
