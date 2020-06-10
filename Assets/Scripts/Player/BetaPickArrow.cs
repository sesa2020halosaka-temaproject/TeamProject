using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject {

    public class BetaPickArrow : MonoBehaviour
    {
        public enum MoveType
        {
            Normal,
            NotFound,
            NotMove,
        }

        [SerializeField]
        [Header("メッシュ表示に使用")]
        private MeshRenderer meshRenderer;

        private PlayerVer2 playerComponent;

        public PlayerVer2 PlayerComponent { set { playerComponent = value; } }

        private Vector3 choicePosition;

        public Vector3 ChoicePosition { set { choicePosition = value; } }

        [SerializeField]
        private float distanceMagnification = 0.6f;

        private float hight = 0f;

        [SerializeField]
        private float hightWight = 0.5f;

        private float hightSpeed = 1.4f;

        private float rotSpeed = 1f;

        // 見つからない時の色
        Material notFoundMat;
        // いけない時の色
        Material notMove;
        // 通常時
        Material mat;

        private MoveType moveType;
        public MoveType MoveTypeMode { set { moveType = value; } }

        // Start is called before the first frame update
        void Start()
        {
            notMove = notFoundMat = mat = meshRenderer.material;

            notMove.color = Color.blue;

            notFoundMat.color = Color.grey;

            moveType = MoveType.Normal;
        }

        // Update is called once per frame
        void Update()
        {
            // Choiceしていれば描画する
            meshRenderer.enabled = !playerComponent.NotChoice;

            Move();

            ColorChange();
        }

        private void Move()
        {
            //hight = Mathf.Sin(Time.time * hightSpeed);

            //hight *= hightWight;

            transform.rotation *= Quaternion.AngleAxis(rotSpeed * Time.deltaTime * 360f, Vector3.up);

            var nowPos = transform.position;

            nowPos = choicePosition - nowPos;

            nowPos *= distanceMagnification;

            // nowPos.y += hight;

            transform.position += nowPos;
        }

        private void ColorChange()
        {
            switch (moveType) {
                case MoveType.Normal:
                    meshRenderer.material = mat;
                    break;

                case MoveType.NotMove:
                    meshRenderer.material = notMove;
                    break;

                case MoveType.NotFound:
                    meshRenderer.material = notFoundMat;
                    break;
            }
        }
    }
}
