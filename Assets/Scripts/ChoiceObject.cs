using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceObject : MonoBehaviour
{
    [SerializeField, Range(1, 5)]
    [Header("階層の入力")]
    private int floor = 1;

    public int Floor { get { return floor; } }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
