using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalEffectFlour : MonoBehaviour
{
    [SerializeField]
    private GameObject goalEffectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GoalEffect()
    {
        Instantiate(goalEffectPrefab, transform.position, Quaternion.identity);
    }
}
