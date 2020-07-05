using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    public class LineQuad : MonoBehaviour
    {
        // 頂点データ 頂点
        private List<Vector3> verPos = new List<Vector3>();
        // 頂点データ UV
        private List<Vector2> verUv = new List<Vector2>();

        List<Vector3> point = new List<Vector3>();

        private List<int> index = new List<int>();

        // インデックスデータの記録用
        private int offset = 0;

        private float xoffset = 0f;

        [SerializeField]
        // マテリアル
        protected Material mat;

        public Material Mat { set { mat = value; } }
        
        private float _uvSize = 2.0f;

        // メッシュ
        private Mesh mesh;

        // メッシュを描画する部分
        protected MeshRenderer meshRenderer;
        private MeshFilter meshFilter;

        protected void Start()
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshFilter = gameObject.AddComponent<MeshFilter>();
            mesh = new Mesh();
        }

        public void PaintStart(Vector3 _tp)
        {
            // 開始点を保存
            point.Add(_tp);

            // 頂点生成
            verPos.Add(_tp);
            verUv.Add(new Vector2(0 + Time.time, 0f));

            // 頂点生成
            verPos.Add(_tp);
            verUv.Add(new Vector2(0 + Time.time, 1f));

            offset = 0;
            xoffset = 0;

            // メッシュ生成
            mesh = new Mesh();
        }

        public void Paint(Vector3 _tp, float _size)
        {
            point.Add(_tp);

            CreateMesh(_size);
        }

        public void Clear()
        {
            // 頂点データ 頂点
            verPos = new List<Vector3>();
            // 頂点データ UV
            verUv = new List<Vector2>();

            point = new List<Vector3>();

            index = new List<int>();
        }

        private void CreateMesh(float _size)
        {
            Vector3 prev = point[point.Count - 2];
            Vector3 top = point[point.Count - 1];
            Vector3 dir = (top - prev).normalized;

            Vector3 plus90 = top + new Vector3(-dir.z, dir.y, dir.x) * _size;
            Vector3 minus90 = top + new Vector3(dir.z, dir.y, -dir.x) * _size;
            
            if (offset == 0)
            {
                verPos[0] = prev+ new Vector3(dir.z, dir.y, -dir.x) * _size;
                verPos[1] = prev+ new Vector3(-dir.z, dir.y, dir.x) * _size;
            }

            // 頂点生成
            verPos.Add(minus90);
            verUv.Add(new Vector2(xoffset + Time.time, 0f));

            // 頂点生成
            verPos.Add(plus90);
            verUv.Add(new Vector2(xoffset + Time.time, 1f));

            xoffset += (top - prev).magnitude / _uvSize;////uScrollSpeed; 

            // インデックスを追加
            index.Add(offset);
            index.Add(offset + 1);
            index.Add(offset + 2);
            index.Add(offset + 1);
            index.Add(offset + 3);
            index.Add(offset + 2);

            offset += 2;

            mesh.vertices = verPos.ToArray();
            mesh.uv = verUv.ToArray();
            mesh.triangles = index.ToArray();

            meshFilter.mesh = mesh;

            meshRenderer.material = mat;
        }
        protected void MeshDalate()
        {
            meshFilter.mesh = new Mesh();
        }
    }
}