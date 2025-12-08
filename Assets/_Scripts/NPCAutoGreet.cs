using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Convai.Scripts.Runtime.Core;
public class NPCAutoGreet : MonoBehaviour
{
    // Start is called before the first frame update
    public ConvaiNPC npcComponent;
    void Start()
    {
        if (npcComponent != null) return;
        npcComponent = GetComponent<ConvaiNPC>();
        TriggerGreeting();
    }

    private void TriggerGreeting()
    {
        if (npcComponent != null)
        {
            string contextTrigger = "(The player just broke the ice wall and rescued you. Thank them enthusiastically, compliment them on how nice and cute they are and ask for their name immediately!)";
            npcComponent.SendTextDataAsync(contextTrigger);
            Debug.Log("Send sucssesful!:" + contextTrigger);
        }
    }
}
