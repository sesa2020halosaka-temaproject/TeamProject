using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

namespace TeamProject
{

    public class WalkSoundManage : MonoBehaviour
    {
        private string[] walkNormalSEPath;
        private string[] walkGrassSEPath;
        private string[] walkGroundSEPath;
        private string[] walkRainSEPath;
        private string[] walkSnowSEPaht;
        private string[] walkWaterSEPaht;
        private string[] walkIceSEPaht;
        private string[] walkWoodSEPaht;

        private string[] separateDrySEPaht;
        private string[] separateRainSEPaht;
        private string[] separateSnowSEPaht;

        // SEをbitごとに管理
        // ステージの状況(雨が降っているのか、とか)
        private byte bitFlagStageType;

        // 実際に再生用に使う
        private byte bitFlag;

        private enum WalkMoad
        {
            Normal = 0b0000001,
            Grass = 0b0000010,
            Ground = 0b0000100,
            Rain = 0b0001000,
            Snow = 0b0010000,
            Water = 0b0100000,
            Ice = 0b1000000,
            Wood = 0b01000000,
        }

        // Start is called before the first frame update
        void Start()
        {
            // walkSEPath = new string[4] { SEPath.SE_PLAYER_SEPARATE_DRY1, SEPath.SE_PLAYER_SEPARATE_DRY2, SEPath.SE_PLAYER_SEPARATE_DRY3, SEPath.SE_PLAYER_SEPARATE_DRY4 };
            // 足音
            walkNormalSEPath = new string[4] { SEPath.SE_PLAYER_WALK_NORMAL1, SEPath.SE_PLAYER_WALK_NORMAL2, SEPath.SE_PLAYER_WALK_NORMAL3, SEPath.SE_PLAYER_WALK_NORMAL4 };
            walkGrassSEPath = new string[4] { SEPath.SE_PLAYER_WALK_GRASS1, SEPath.SE_PLAYER_WALK_GRASS2, SEPath.SE_PLAYER_WALK_GRASS3, SEPath.SE_PLAYER_WALK_GRASS4 };
            walkGroundSEPath = new string[4] { SEPath.SE_PLAYER_WALK_GROUND1, SEPath.SE_PLAYER_WALK_GROUND2, SEPath.SE_PLAYER_WALK_GROUND3, SEPath.SE_PLAYER_WALK_GROUND4 };
            walkRainSEPath = new string[4] { SEPath.SE_PLAYER_WALK_RAIN1, SEPath.SE_PLAYER_WALK_RAIN2, SEPath.SE_PLAYER_WALK_RAIN3, SEPath.SE_PLAYER_WALK_RAIN4 };
            walkSnowSEPaht = new string[4] { SEPath.SE_PLAYER_WALK_SNOW1, SEPath.SE_PLAYER_WALK_SNOW2, SEPath.SE_PLAYER_WALK_SNOW3, SEPath.SE_PLAYER_WALK_SNOW4 };
            walkWaterSEPaht = new string[4] { SEPath.SE_PLAYER_WALK_WATER1, SEPath.SE_PLAYER_WALK_WATER2, SEPath.SE_PLAYER_WALK_WATER3, SEPath.SE_PLAYER_WALK_WATER4 };
            walkIceSEPaht = new string[4] { SEPath.SE_PLAYER_WALK_ICE1, SEPath.SE_PLAYER_WALK_ICE2, SEPath.SE_PLAYER_WALK_ICE3, SEPath.SE_PLAYER_WALK_ICE4 };
            walkIceSEPaht = new string[4] { SEPath.SE_PLAYER_WALK_WOOD1, SEPath.SE_PLAYER_WALK_WOOD2, SEPath.SE_PLAYER_WALK_WOOD3, SEPath.SE_PLAYER_WALK_WOOD4 };

            // 草のかき分け等
            separateDrySEPaht = new string[4] { SEPath.SE_PLAYER_SEPARATE_DRY1, SEPath.SE_PLAYER_SEPARATE_DRY2, SEPath.SE_PLAYER_SEPARATE_DRY3, SEPath.SE_PLAYER_SEPARATE_DRY4 };
            separateRainSEPaht = new string[4] { SEPath.SE_PLAYER_SEPARATE_WET1, SEPath.SE_PLAYER_SEPARATE_WET2, SEPath.SE_PLAYER_SEPARATE_WET3, SEPath.SE_PLAYER_SEPARATE_WET4 };
            separateSnowSEPaht = new string[4] { SEPath.SE_PLAYER_SEPARATE_SNOW1, SEPath.SE_PLAYER_SEPARATE_SNOW2, SEPath.SE_PLAYER_SEPARATE_SNOW3, SEPath.SE_PLAYER_SEPARATE_SNOW4 };

            bitFlagStageType = 0;

            // bitの初期化
            var rain = GameObject.FindGameObjectsWithTag("Rain");

            // rainあったとき
            if (rain.Length != 0)
            {
                bitFlagStageType |= (byte)WalkMoad.Rain;
            }

        }

        [SerializeField]
        private LayerMask layer;

        // Update is called once per frame
        public void Update()
        {
            // 当たり判定前準備
            RaycastHit hit;
            Ray ray = new Ray(transform.position + new Vector3(0f, 3f, 0f), -Vector3.up);
            bool hitFlag = false;
            byte bitFlagChack = (byte)WalkMoad.Normal;

            hitFlag = Physics.Raycast(ray, out hit, 10f, layer);
            
            if (hitFlag)
            {
                // 追加あればここに
                Walk(hit, ref bitFlagChack);
            }

            // 地面の情報と環境の情報を合成
            bitFlag = (byte)(bitFlagChack | bitFlagStageType);
        }

        public void PlayWalkSound()
        {
            if (CheckFlag(WalkMoad.Normal))
            {
                Debug.Log("WalkManager:Normalなってます");
                Play(walkNormalSEPath);
            }
            if (CheckFlag(WalkMoad.Ground))
            {
                Debug.Log("WalkManager:Groundなってます");
                Play(walkGroundSEPath);
            }
            if (CheckFlag(WalkMoad.Grass))
            {
                Debug.Log("WalkManager:Grassなってます");
                Play(walkGrassSEPath);
                if (CheckFlag(WalkMoad.Rain))
                {
                    Play(separateDrySEPaht);
                }
                else
                {
                    Play(separateRainSEPaht);
                }
            }
            if (CheckFlag(WalkMoad.Rain))
            {
                Debug.Log("WalkManager:Rainなってます");
                Play(walkRainSEPath);
            }
            if (CheckFlag(WalkMoad.Snow))
            {
                Debug.Log("WalkManager:Snowなってます");
                Play(walkSnowSEPaht);
                Play(separateSnowSEPaht);
            }
            if (CheckFlag(WalkMoad.Ice))
            {
                Debug.Log("WalkManager:Iceなってます");
                Play(walkIceSEPaht);
            }
        }

        private bool CheckFlag(WalkMoad _moad)
        {
            return (bitFlag & (byte)_moad) == (byte)_moad;
        }


        private void Walk(RaycastHit _hit, ref byte _bitFlag)
        {
            switch (_hit.collider.tag)
            {
                case "Ground":  // Groundの時
                    _bitFlag |= (byte)WalkMoad.Ground;
                    break;

                case "Grass":   // Grassの時
                    _bitFlag |= (byte)WalkMoad.Grass;
                    break;

                case "Snow":    // Snowの時
                    _bitFlag |= (byte)WalkMoad.Snow;
                    break;

                case "Water":   // Waternの時
                    _bitFlag |= (byte)WalkMoad.Water;
                    break;

                case "IceGimmick":   // Iceの時
                    _bitFlag |= (byte)WalkMoad.Ice;
                    break;
                case "Wood":   // Woodの時
                    _bitFlag |= (byte)WalkMoad.Wood;
                    break;
            }
        }

        // 鳴らす
        private void Play(string[] _sePaht)
        {
            int rand = Random.Range(0, 4);

            SEManager.Instance.Play(_sePaht[rand]);
        }
    }
}