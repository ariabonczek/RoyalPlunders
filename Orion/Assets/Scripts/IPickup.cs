using UnityEngine;
using System.Collections;

public interface IPickup
{
    void Pickup(Player player);

    void Throw(Player player);
}
