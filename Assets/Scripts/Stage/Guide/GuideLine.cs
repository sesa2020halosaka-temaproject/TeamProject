using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TeamProject
{
    public class GuideLine : LineQuad
    {
        [SerializeField]
        private Vector3 startPos;
        [SerializeField]
        private Vector3[] pos;

        [SerializeField]
        private float size = 1f;

        private void Start()
        {
            base.Start();
        }
        
        public void Hoge()
        {
            Clear();

            PaintStart(startPos);

            if (pos == null) return;
            foreach (var itr in pos) Paint(itr, size);
        }

        public void Delete() {
            startPos = Vector3.zero;
            pos = null;
        }

        public void SetPoint(Vector3 _pointStart,Vector3[] _points)
        {
            startPos = _pointStart;
            pos = _points;
        }

        private void Update()
        {
            meshRenderer.enabled = pos != null;
        }
    }

    [CustomEditor(typeof(GuideLine))]//拡張するクラスを指定
    public class ExampleScriptEditor : Editor
    {

        /// <summary>
        /// InspectorのGUIを更新
        /// </summary>
        public override void OnInspectorGUI()
        {
            //元のInspector部分を表示
            base.OnInspectorGUI();

            //targetを変換して対象を取得
            GuideLine exampleScript = target as GuideLine;

            //PublicMethodを実行する用のボタン
            if (GUILayout.Button("Hoge"))
            {
                exampleScript.Hoge();
            }

        }

    }
}