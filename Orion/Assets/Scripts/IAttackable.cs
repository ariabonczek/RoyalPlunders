using UnityEngine;
using System.Collections;

public interface IAttackable 
{
    // Returns true if the object is still alive
    bool TakeDamage();
}
