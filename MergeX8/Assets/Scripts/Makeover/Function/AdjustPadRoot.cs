using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ASMR
{
    public class AdjustPadRoot : MonoBehaviour
    {
        public float FitScale = 0.5f;
        public void Awake()
        {
            if(CommonUtils.IsLE_16_10())
            {
                 this.transform.localScale = new Vector3(FitScale,FitScale,FitScale);
            }
           
        }
    }
}