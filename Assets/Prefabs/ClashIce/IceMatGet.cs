using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TeamProject
{
    public class IceMatGet : MonoBehaviour
    {
        private Renderer renderer;

        [SerializeField]
        private float speed = 1f;

        private float nowAlpha = 1f;

        private bool flag = false;

        private Material mat;

        // Start is called before the first frame update
        void Start()
        {
            renderer = gameObject.GetComponentInChildren<Renderer>();
            mat = renderer.material;

            Debug.Log("aaaa");
            // renderer.material.SetColor("_BaseColor", new Color(0.2f, 0.2f, 0.2f, 0f));
            // .material.SetColor("_BaseColor", new Color(0.2f, 0.2f, 0.2f, 0.3f));
        }

        // Update is called once per frame
        void Update()
        {
            if (!flag) return;
            nowAlpha -= speed * Time.deltaTime;
            if (nowAlpha < 0f)
            {
                nowAlpha = 0f;
                renderer.enabled = false;
            }

            var matCol = mat.color;
            matCol.a = nowAlpha;
            renderer.material.SetColor("_BaseColor", matCol);
        }

        public void On()
        {
            flag = true;
        }
    }
}