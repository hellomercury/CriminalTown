using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharactersContainer
{
    void OnAddReaction(Character character);
    void OnRemoveReaction(Character character);
}
