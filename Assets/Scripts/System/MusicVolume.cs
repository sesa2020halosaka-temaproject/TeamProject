using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KanKikuchi.AudioManager;

public class MusicVolume : MonoBehaviour
{
    [Header("リニア値(0～1)")]
    public float BGMVolume; //BGM全体のボリューム(０～１)
    [Header("db単位 ここを調整する")]
    public float BGMVolumedb; //BGM全体のボリューム(デシベル単位)

    [Header("リニア値(0～1)")]
    public float SEVolume;//SE全体のボリューム(０～１)
    [Header("db単位 ここを調整する")]
    public float SEVolumedb;//SE全体のボリューム(デシベル単位)

    private float dbMin = -80.0f;//最低デシベル
    // Start is called before the first frame update
    void Start()
    {
        //if (BGMVolume <= 0)
        //{
        //    BGMVolume = 1.0f;

        //}
        //if (SEVolume <= 0)
        //{
        //    SEVolume = 1.0f;

        //}


    }

    // Update is called once per frame
    void Update()
    {
        /*volumeはfloat*/
        BGMVolume = FromDecibel(BGMVolumedb);
            //BGM全体のボリュームを変更
            BGMManager.Instance.ChangeBaseVolume(BGMVolume);
        BGMVolumedb = ToDecibel(BGMVolume, dbMin);

        if (BGMVolumedb >= 0)
        {
            BGMVolumedb = 0.0f;
        }


        SEVolume = FromDecibel(SEVolumedb);

        //SE全体のボリュームを変更
        SEManager.Instance.ChangeBaseVolume(SEVolume);
        SEVolumedb = ToDecibel(SEVolume, dbMin);

         if (SEVolumedb >= 0)
        {
            SEVolumedb = 0.0f;
        }
      
    }
    public  float ToDecibel(float linear, float dbMin)
    {
        var decibel = dbMin;
        if (linear > 0f)
        {
            decibel = 20f * Mathf.Log10(linear);
            decibel = Mathf.Max(decibel, dbMin);
        }
        return decibel;
    }

    public  float FromDecibel(float decibel)
    {
        return Mathf.Pow(10f, decibel / 20f);
    }
}
