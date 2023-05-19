using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class MeshCuttingTest : MonoBehaviour
{
    public GameObject target;
    public Transform cuttingPlane;

    GameObject[] pieces = new GameObject[2];

    private void Start()
    {
        pieces = target.SliceInstantiate(target.transform.position, cuttingPlane.up);
        target.SetActive(false);
    }

}
