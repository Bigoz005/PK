using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Pod przyszłe stworki (ewentualnie)
public delegate void HealthChanged(float health);

public delegate void CharacterRemoved();

public class NPC : Characters, IInteractable
{

    public virtual void Interact()
    {
        Debug.Log("Hello");
    }

    public virtual void StopInteract()
    {
        Debug.Log("Bye!");
    }
}
