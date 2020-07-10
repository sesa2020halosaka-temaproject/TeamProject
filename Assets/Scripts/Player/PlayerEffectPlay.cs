using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using KanKikuchi.AudioManager;

namespace TeamProject
{
    public class PlayerEffectPlay : MonoBehaviour
    {
        [SerializeField]
        private GameObject grassEffectObject;
        [SerializeField]
        private GameObject grassAutEffectObject;
        [SerializeField]
        private GameObject grassSneowEffectObject;

        [SerializeField]
        private GameObject particl;

        private GameObject grass;
        private VisualEffect effect;

        [SerializeField]
        private GameObject playerStartParticl;
        [SerializeField]
        private GameObject playerWinterStartParticl;
        // Start is called before the first frame update
        void Start()
        {
            var worldNum = StageStatusManager.Instance.CurrentWorld;
            switch ((IN_WORLD_NO)worldNum)
            {
                case IN_WORLD_NO.S1:
                    effect = grassEffectObject.GetComponent<VisualEffect>();
                    grass = grassEffectObject;
                    break;
                case IN_WORLD_NO.S2:
                    effect = grassAutEffectObject.GetComponent<VisualEffect>();
                    grass = grassAutEffectObject;
                    break;
                case IN_WORLD_NO.S3:
                    effect = grassSneowEffectObject.GetComponent<VisualEffect>();
                    grass = grassSneowEffectObject;
                    break;
                case IN_WORLD_NO.S4:
                    effect = grassEffectObject.GetComponent<VisualEffect>();
                    grass = grassEffectObject;
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void GrassEffectPlay()
        {
            if (grass == null) return;
            grass.SetActive(true);
            effect.Play();
        }

        private void SEFall()
        {
            string[] lan = { SEPath.SE_PLAYER_LANDING1, SEPath.SE_PLAYER_LANDING2, SEPath.SE_PLAYER_LANDING3, SEPath.SE_PLAYER_LANDING4 };
            var rand = Random.Range(0, 3);
            SEManager.Instance.Play(lan[rand]);
        }

        void QuestionParticl()
        {
            Instantiate(particl, transform.parent.position, transform.parent.rotation, transform.parent);
        }

        private void PlayerStartParticle()
        {
            var stageSeason = (WORLD_NO)StageStatusManager.Instance.CurrentWorld;
            Debug.Log(stageSeason+"gijeaorgjeoriigjoiaerjgoiraej");
            if (stageSeason != WORLD_NO.W3)
            {
                Instantiate(playerStartParticl, transform.parent.position, transform.parent.rotation);
            }
            else
            {
                Instantiate(playerWinterStartParticl, transform.parent.position, transform.parent.rotation);
            }
        }
    }
}