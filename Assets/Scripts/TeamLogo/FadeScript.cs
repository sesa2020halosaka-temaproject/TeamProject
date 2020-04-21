using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;     //UIを使用可能にする

//フェードイン・フェードアウト用
public class FadeScript : MonoBehaviour
{
    public float speed = 0.01f;  //透明化の速さ
    float alfa;    //A値を操作するための変数
    float red, green, blue;    //RGBを操作するための変数
    public bool Fade_IO_Flag;//フェードインアウト切り替え（true:IN,false:OUT）

    // Start is called before the first frame update
    void Start()
    {
        //Panelの色を取得
        red = GetComponent<Image>().color.r;
        green = GetComponent<Image>().color.g;
        blue = GetComponent<Image>().color.b;
        if (Fade_IO_Flag)
        {
            alfa = 0.0f;
        }
        else
        {
            alfa = 1.0f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Image>().color = new Color(red, green, blue, alfa);
        if (Fade_IO_Flag)
        {
            FadeIn();

        }
        else
        {
            FadeOut();
        }
    }

    public void FadeIn()
    {
        alfa += speed;

    }
    public void FadeOut()
    {
        alfa -= speed;

    }
}
