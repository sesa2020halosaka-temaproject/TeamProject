using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TeamProject
{

public class InputDebug : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.DownArrow))
        {
                Debug.Log("DownArrow"+ InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.DownArrow));
        }
        if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.UpArrow))
        {
                Debug.Log("UpArrow");
        }
        if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.LeftArrow))
        {
                Debug.Log("LeftArrow");
        }
        if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCoad.RightArrow))
        {
                Debug.Log("RightArrow");
        }
    }
}
}
