using DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem.Runtime.Data;
using DialogueSystem.Runtime.Logic;
using UnityEngine;

public class TestDialogue : MonoBehaviour
{
    [Header("Test")]
    [SerializeField]
    private Dialogue testDialogue;

    private void Update()
    {
        // TEST
        if (Input.GetKeyDown(KeyCode.T))
        {
            DialogueController.Instance.StartDialogue(testDialogue);
        }
    }
}
