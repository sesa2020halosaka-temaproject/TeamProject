using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    public class GuideLine : LineQuad
    {
        private Vector3 startPos;
        private Vector3[] pos;

        private float size;

        private float uvSpeed;

        public float UvSpeed { set { uvSpeed = value; } }

        private Pause pause;

        private PlayerVer2 player;

        private void Start()
        {
            base.Start();
            GameObject uiChild = GameObject.FindGameObjectWithTag("StageObjectCunvas");

            //var uiParentPbject = uiChild.transform.root.gameObject;

            pause = uiChild.GetComponentInChildren<Pause>();

            var playerObj = GameObject.FindGameObjectWithTag("Player");

            player = playerObj.transform.root.GetComponent<PlayerVer2>();
            uvSpeed = 0.5f;
            size = 0.65f;
        }

        public void Hoge()
        {
            Clear();

            PaintStart(startPos);

            if (pos == null) return;
            foreach (var itr in pos) Paint(itr, size);
        }

        public void Delete()
        {
            startPos = Vector3.zero;
            pos = null;
            MeshDalate();
        }

        public void SetPoint(Vector3 _pointStart, Vector3[] _points)
        {
            startPos = _pointStart;
            pos = _points;
        }

        private void Update()
        {
            //mat.SetTextureOffset("_BaseColorMap", new Vector2(-Time.time * uvSpeed, 0f));
            mat.SetTextureOffset("_UnlitColorMap", new Vector2(-Time.time * uvSpeed, 0f));

            if (meshRenderer)
            {
                var flag = pos != null;

                var flag2 = !(player.NowFunctionNum == (uint)PlayerVer2.TRANSITION.Goal || pause.NowFunctionNum != (uint)Pause.TRANS.PauseWait);

                meshRenderer.enabled = flag && flag2;

                return;
            }
        }

        private void CopyLine(GuideLine _base)
        {
            startPos = _base.startPos;
            pos = _base.pos;
            size = _base.size;
            uvSpeed = _base.uvSpeed;
        }
    }
}