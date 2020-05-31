using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{

    public class GoalInDestroy : MonoBehaviour
    {
        private Camera cameraCom;

        // Start is called before the first frame update
        void Start()
        {
            cameraCom = UnityEngine.Camera.main.transform.root.gameObject.GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            if (cameraCom.NowFunctionNum == (uint)Camera.TRANS.Goal)
            {
                Destroy(gameObject);
            }
        }
    }
}