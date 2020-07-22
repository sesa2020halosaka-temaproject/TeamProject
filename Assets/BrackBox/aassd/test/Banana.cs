using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Banana : MonoBehaviour
{
    private RectTransform ue;
    private RectTransform sita;
    GameObject FlagManager;
    TeamProject.StageStatusManager StageStatusManager;

    public float x, y, z;
    public float yy;
    public float speed=5.0f;
    void Start()
    {
        FlagManager = GameObject.Find("StageStatusManager");
        StageStatusManager = FlagManager.GetComponent<TeamProject.StageStatusManager>();
        ue = GameObject.Find("UE").GetComponent<RectTransform>();
        sita = GameObject.Find("SITA").GetComponent<RectTransform>();
        if (StageStatusManager.m_S1Flag)
        {
            ue.gameObject.SetActive(false);
            sita.gameObject.SetActive(false);
        }
    }

    void Update()
    {

        y = ue.localPosition.y;
        y += speed;
        yy = sita.localPosition.y;
        yy -= speed;

        ue.localPosition = new Vector3(x, y, z);
        sita.localPosition = new Vector3(x, yy, z);

        if(y > 950)
        {
            ue.gameObject.SetActive(false);
            sita.gameObject.SetActive(false);
        }

 
    }
}
