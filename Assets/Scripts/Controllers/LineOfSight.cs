using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    List<Transform> targetsInRange = new List<Transform>();

    IEnumerator SightCheck()
    {
        yield return null;
    }

}
