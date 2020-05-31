using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using KanKikuchi.AudioManager;

public class PlayerEffectPlay : MonoBehaviour
{

    [SerializeField]
    private GameObject grassEffectObject;

    [SerializeField]
    private GameObject particl;

    private VisualEffect effect;
    // Start is called before the first frame update
    void Start()
    {
        effect = grassEffectObject.GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GrassEffectPlay()
    {
        grassEffectObject.SetActive(true);
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
}
