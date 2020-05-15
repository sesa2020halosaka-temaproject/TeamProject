using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerEffectPlay : MonoBehaviour
{

    [SerializeField]
    private GameObject grassEffectObject;

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
}
