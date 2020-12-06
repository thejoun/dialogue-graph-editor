using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Acts as a tag for character classes that may define functions that are triggered by dialog.
/// For instance, it would be common for a generic character behaviour class to implement this interface
/// and then provide definitions for functions listed in the Dialog Editor Window in the class implementing 
/// this interface. 
/// To invoke methods on the NPC, one can directly use member variables in the class implementing this
/// interface that is attached to an NPC game object, or access scripts in the local NPC hierarchy.
/// To invoke methods on the Player, one can use a reference to the Player that would 
/// exist in the class attached to an NPC that implements interface.
/// </summary>
public interface IConversable {}
