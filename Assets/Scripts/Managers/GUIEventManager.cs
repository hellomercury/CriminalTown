using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIEventManager : MonoBehaviour
{
    public delegate void DataEvent(Character character);

    public event DataEvent OnKickEvent = delegate { };

    public event DataEvent OnAddEvent = delegate { };

    private void Awake()
    {
        
    }

    public void RemoveCharacter(Character character)
    {
        OnKickEvent(character);
        DataScript.chData.RemoveCharacter(character);
    }

    public void AddCharacter(Character character)
    {
        OnAddEvent(character);
        DataScript.chData.AddCharacter(character);
    }
}
