using UnityEngine;

namespace DialogueSystem.Runtime.Logic
{

    /// <summary>
    /// This is a basic implementation of a trigger handler.
    /// Doesn't do much except for providing a way to read and filter triggers.
    /// You can either expand this or create your own handler.
    /// </summary>
    public class BasicTriggerHandler : TriggerHandler
    {
        public override void Handle(string trigger)
        {
            string[] split = trigger.Split(' ');

            if (split.Length <= 0)
            {
                Debug.LogError("Trigger is empty.");
                return;
            }
            else if (split.Length <= 1)
            {
                Debug.LogError("Trigger has no value.");
                return;
            }
            else
            {
                string key = split[0];
                string value = split[1];
                switch (key)
                {
                    case string k when k.Equals("music") || k.Equals("m"):
                        PlayMusic(value);
                        break;
                    case string k when k.Equals("sound") || k.Equals("s"):
                        PlaySound(value);
                        break;
                    default:
                        Debug.LogError($"Trigger key '{key}' not recognized.");
                        return;
                }
            }
        }

        private void PlayMusic(string value)
        {
            Debug.Log($"Playing music track {value}.");
        }

        private void PlaySound(string value)
        {
            Debug.Log($"Playing sound {value}.");
        }
    }
}