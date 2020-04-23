using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject {

    public class BetaPickArrow : MonoBehaviour
    {
        [SerializeField]
        [Header("メッシュ表示に使用")]
        private MeshRenderer meshRenderer;

        private PlayerVer2 playerComponent;

        public PlayerVer2 PlayerComponent { set { playerComponent = value; } }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // Choiceしていれば描画する
            meshRenderer.enabled = !playerComponent.NotChoice;
        }
    }
}
