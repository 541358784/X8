using UnityEngine;

public class MonoBehaviourBridge : MonoBehaviour
{
    bool inited = false;

    void Awake()
    {
        if (!Application.isPlaying || inited)
        {
            return;
        }

        gameObject.SetActive(false);

        MonoBehaviourBridge[] monoBehaviourBridges = gameObject.GetComponents<MonoBehaviourBridge>();
        foreach (MonoBehaviourBridge monoBehaviourBridge in monoBehaviourBridges)
        {
            Component component = monoBehaviourBridge.AddTargetComponent();
            MonoBehaviour monoBehaviour = component as MonoBehaviour;
            if (monoBehaviour)
            {
                monoBehaviour.enabled = monoBehaviourBridge.enabled;
            }
            monoBehaviourBridge.inited = true;
            Destroy(monoBehaviourBridge);
        }

        gameObject.SetActive(true);
    }

    public virtual Component AddTargetComponent()
    {
        return null;
    }

    void Start()
    {

    }
}
