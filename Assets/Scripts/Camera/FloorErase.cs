using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    public class FloorErase : MonoBehaviour
    {
        // 各種階層のゲームオブジェクト
        [Header("一番下の階層です" )]
        [SerializeField]
        private GameObject[] f1; // 一番上の階層
        [SerializeField]
        private GameObject[] f1KakiwakeObject; // 一番上の階層
        [SerializeField]
        private GameObject[] f1ColliderObject; // 地面のコライダー 

        [SerializeField]
        private GameObject[] f2;
        [SerializeField]
        private GameObject[] f2KakiwakeObject; // 一番上の階層
        [SerializeField]
        private GameObject[] f2ColliderObject; // 地面のコライダー 

        [SerializeField]
        private GameObject[] f3;
        [SerializeField]
        private GameObject[] f3KakiwakeObject; // 一番上の階層
        [SerializeField]
        private GameObject[] f3ColliderObject; // 地面のコライダー 

        [SerializeField]
        private GameObject[] f4;
        [SerializeField]
        private GameObject[] f4KakiwakeObject; // 一番上の階層
        [SerializeField]
        private GameObject[] f4ColliderObject; // 地面のコライダー 

        [SerializeField]
        private GameObject[] f5;
        [SerializeField]
        private GameObject[] f5KakiwakeObject; // 一番上の階層
        [SerializeField]
        private GameObject[] f5ColliderObject; // 地面のコライダー 


        // Rendererデータ
        private MeshRenderer[][] meshRenderer;
        private MeshRenderer[][] meshKakiwakeRenderer;

        private GameObject[][] floorHitObject;
        public GameObject[][] FloorHitObject { get { return floorHitObject; } }

        private Camera hightData;

        private int maxFloor = 5;

        [SerializeField]
        [Header("Alphaの値を変更したい場合はここ")]
        private float changeColorAlpha = 0.3f;

        // Start is called before the first frame update
        void Start()
        {
            meshRenderer = new MeshRenderer[maxFloor][];
            meshKakiwakeRenderer = new MeshRenderer[maxFloor][];
            floorHitObject = new GameObject[maxFloor][];

            meshRenderer[0] = GetAllComponent<MeshRenderer>(f1);
            meshRenderer[1] = GetAllComponent<MeshRenderer>(f2);
            meshRenderer[2] = GetAllComponent<MeshRenderer>(f3);
            meshRenderer[3] = GetAllComponent<MeshRenderer>(f4);
            meshRenderer[4] = GetAllComponent<MeshRenderer>(f5);

            meshKakiwakeRenderer[0] = GetAllComponent<MeshRenderer>(f1KakiwakeObject);
            meshKakiwakeRenderer[1] = GetAllComponent<MeshRenderer>(f2KakiwakeObject);
            meshKakiwakeRenderer[2] = GetAllComponent<MeshRenderer>(f3KakiwakeObject);
            meshKakiwakeRenderer[3] = GetAllComponent<MeshRenderer>(f4KakiwakeObject);
            meshKakiwakeRenderer[4] = GetAllComponent<MeshRenderer>(f5KakiwakeObject);


            floorHitObject[0] = f1ColliderObject;
            floorHitObject[1] = f2ColliderObject;
            floorHitObject[2] = f3ColliderObject;
            floorHitObject[3] = f4ColliderObject;
            floorHitObject[4] = f5ColliderObject;

            hightData = GetComponent<Camera>();

        }

        // Update is called once per frame
        void Update()
        {
            // 高さ2の時は2～4は消えそうになる
            //for(int i=0; i<meshRenderer[0].Length; i++)
            //{
            //    meshRenderer[0][i].material.SetColor("_BaseColor", new Color(0f, 0f, 0f, 0f));
            //}
            for (int i = 0; i < maxFloor; i++)
            {
                float alpha = 1f;
                KakiwakeEnable(i, true);
                if (hightData.NowHight <= i)
                {
                    KakiwakeEnable(i, false);
                    alpha = changeColorAlpha;
                }

                SetAlpha(i, alpha);
            }
        }

        private T[] GetAllComponent<T>(GameObject[] gameObject)
        {
            List<T> returnObject = new List<T>();

            foreach(var itr in gameObject)
            {
                var childCom = itr.GetComponentsInChildren<T>();

                returnObject.AddRange(childCom);
            }

            return returnObject.ToArray();
        }

        private void SetAlpha(int _i, float _a)
        {
            foreach(var itr in meshRenderer[_i])
            {
                var col = itr.material.color;// GetColor("_BaseColor");

                col.a = _a;

                itr.material.SetColor("_BaseColor", col);
            }
        }
        private void KakiwakeEnable(int _i,bool flag)
        {
            foreach (var itr in meshKakiwakeRenderer[_i])
            {
                itr.enabled = flag;
            }
        }
    }
}