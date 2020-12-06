using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sample behaviour for an NPC the player can interact with.
/// </summary>
public class CharacterBehaviour : MonoBehaviour, IConversable
{
    public Conversation ConversationA;

    public Conversation ConversationB;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Call StartConversation() on your conversation instance to initiate the conversation
            ConversationA.StartConversation();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Call StartConversation() on your conversation instance to initiate the conversation
            ConversationB.StartConversation();
        }
    }

    // Function definitions for function headers listed in the Dialog Editor Window must be defined in a class
    // implementing the IConversable interface.
    public void myFunction()
    {
        Debug.Log("myFunction executed!");
    }

} // end CharacterBehaviour class    
