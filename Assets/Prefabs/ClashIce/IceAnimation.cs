using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamProject
{
    public class IceAnimation : MonoBehaviour
    {
        private IceMatGet[] iceMatGet;
        // Start is called before the first frame update
        void Start()
        {
            iceMatGet = GetComponentsInChildren<IceMatGet>();
        }

        void StratAnime()
        {
            foreach(var itr in iceMatGet)
            {
                itr.On();
            }
        }
    }
}