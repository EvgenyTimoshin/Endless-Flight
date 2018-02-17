using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickUp
{

    void PickUpInteraction(Collider other);
    void PickUpDestroy();

}
