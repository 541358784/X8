using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Asset;
using Makeover;
using UnityEngine;
using UnityEngine.UI;

public partial class UIGameMainController
{
    private Slider operateStatusSlider;
     private List<GameObject> operateStatusGameObjects = new List<GameObject>();
     
     public void InitStatus()
     {
         operateStatusGroup.SetActive(true);

         operateStatusSlider = operateStatusGroup.transform.Find("Slider").GetComponent<Slider>();
         operateStatusSlider.maxValue = 1;
         operateStatusSlider.value = 0.0f;
         
         GameObject prefab = operateStatusGroup.transform.Find("Operate_1").gameObject;
         prefab.SetActive(false);
         
         var steps = MakeoverConfigManager.Instance.stepList.FindAll( a => a.levelId == _curLevelConfig.stepLevelId);
         if (_curLevelConfig.newLevel)
         {
             for (var i = 0; i < _curLevelConfig.groupIds.Length; i++)
             {
                 var gameObject = GameObject.Instantiate(prefab, operateStatusGroup.transform);
                 gameObject.SetActive(true);
                 operateStatusGameObjects.Add(gameObject);
                 gameObject.transform.Find(i == 0 ? "Status_2" : "Status_1").gameObject.SetActive(true);
                 for (int j = 1; j <= 2; j++)
                 {
                     gameObject.transform.Find($"Status_{j}/Title").GetComponent<LocalizeTextMeshProUGUI>().SetText((i+1) + "/" + (_curLevelConfig.groupIds.Length));
                 }
             }
         }
         else
         {
             for (int i = 0; i < steps.Count; i++)
             {
                 GameObject gameObject = GameObject.Instantiate(prefab, operateStatusGroup.transform);
                 gameObject.SetActive(true);
                 operateStatusGameObjects.Add(gameObject);
                 gameObject.transform.Find(i == 0 ? "Status_2" : "Status_1").gameObject.SetActive(true);
                 for (int j = 1; j <= 2; j++)
                 {
                     gameObject.transform.Find($"Status_{j}/Title").GetComponent<LocalizeTextMeshProUGUI>().SetText((i+1) + "/" + (steps.Count));
                 }
             }
         }
     }

     public async Task FinishStatus(int treatIndex)
     {
         treatIndex = Math.Min(treatIndex, operateStatusGameObjects.Count - 1);
         {
             operateStatusGameObjects[treatIndex].transform.Find("Status_2").gameObject.SetActive(false);
             operateStatusGameObjects[treatIndex].transform.Find("Status_3").gameObject.SetActive(true);
         }
         
         //progress
         List<float> lengths = new List<float>();
         float sumLenght = Mathf.Abs(operateStatusGameObjects[operateStatusGameObjects.Count - 1].transform.localPosition.x - 
                           operateStatusGameObjects[0].transform.localPosition.x);
         for (int i = 1; i < operateStatusGameObjects.Count; i++)
         {
             lengths.Add(Mathf.Abs(operateStatusGameObjects[i].transform.localPosition.x - operateStatusGameObjects[i - 1].transform.localPosition.x));
         }
         lengths.Add(0.0f);
         float curLenght = 0.0f;
         for (int i = 0; i < treatIndex + 1; i++) curLenght += lengths[i];
         float sliderValue = 0.0f;
         if (sumLenght > 0) sliderValue = curLenght / sumLenght;
     
         if (sliderValue > 0 && Mathf.Abs(sliderValue - operateStatusSlider.value) > 0.001f)
         {
             operateStatusSlider.DOValue(sliderValue, 1.0f);
             await Task.Delay(800);
         }
         else if(treatIndex + 1 == operateStatusGameObjects.Count)
         {
             await Task.Delay(400);
         }
     
         //next
         if (treatIndex + 1 < operateStatusGameObjects.Count)
         {
             operateStatusGameObjects[treatIndex + 1].transform.Find("Status_1").gameObject.SetActive(false);
             operateStatusGameObjects[treatIndex + 1].transform.Find("Status_2").gameObject.SetActive(true);
         }
     }

}