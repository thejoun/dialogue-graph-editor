using UnityEngine;

namespace DialogueSystem
{
    public abstract class TriggerHandler : MonoBehaviour
    {
        public abstract void Handle(string trigger);
    }
}
