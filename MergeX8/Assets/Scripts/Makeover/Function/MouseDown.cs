using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ASMR
{
    public class MouseDown : MonoBehaviour
    {
        public float interval = 1.0f;
        float totalTime = 0;
        // Update is called once per frame
        void Update()
        {
            
            totalTime += Time.deltaTime;
            if(totalTime<interval)return;
            if(ASMR.Model.Instance.TaskRuning)return;
            
            if(Input.GetMouseButton(0))
			{
                totalTime = 0;
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				bool success = true;
				RaycastHit hitInfo;
				if(Physics.Raycast(ray, out hitInfo))
				{
					var operateTarget_script = hitInfo.transform.GetComponent<OperateTarget>();
					if(operateTarget_script != null)
                    {
                        operateTarget_script.ExcuteTask();
                        //Debug.Log($"Trigger name is triggerObj={triggerObj.name}");
                    }
						
				}
			}
        }
    }
}

