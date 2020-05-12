using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TeamProject.InputManager;

namespace TeamProject
{
    public class Camera : System.TransitionObject
    {
        public enum TRANS
        {
            None,
            Upd,
            Max
        }

        [SerializeField]
        private float speed;

        [SerializeField]
        private float min, max;

        private Volume volume;

        // Start is called before the first frame update
        void Start()
        {
            SetMaxFunctionSize((uint)TRANS.Max);

            CreateFunction((uint)TRANS.None,None);
            CreateFunction((uint)TRANS.Upd, Upd);

            SetFunction((uint)TRANS.Upd);

            var a = UnityEngine.Camera.main;

            volume = GetComponentInChildren<Volume>();
        }

        private void None()
        {
            // None
            volume.enabled = true;
        }

        private void Upd()
        {
            // volumeをオフに
            volume.enabled = false;

            var stick = InputManager.InputManager.Instance.GetRStick();

            Vector3 rot = transform.rotation.eulerAngles;

            float x, y;

            x = stick.x * speed * Time.deltaTime;
            y = stick.y * speed * Time.deltaTime;

            rot.x += y;
            rot.y -= x;

            if (rot.x + x < min)
            {
                rot.x = min;
            }
            if (max < rot.x + x)
            {
                rot.x = max;
            }

            var qua = transform.rotation;
            rot.z = 0.0f;
            qua.eulerAngles = rot;
            transform.rotation = qua;
        }
    }
}