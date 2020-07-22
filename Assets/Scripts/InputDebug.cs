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
        if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCode.DownArrow))
        {
                Debug.Log("DownArrow"+ InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCode.DownArrow));
        }
        if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCode.UpArrow))
        {
                Debug.Log("UpArrow");
        }
        if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCode.LeftArrow))
        {
                Debug.Log("LeftArrow");
        }
        if (InputManager.InputManager.Instance.GetArrow(InputManager.ArrowCode.RightArrow))
        {
                Debug.Log("RightArrow");
        }
    }
}
}
