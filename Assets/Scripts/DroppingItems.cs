using System;
using UnityEngine;

public class DroppingItems : MonoBehaviour
{
    public GameObject item;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    public void ItemDropped()
    {
        Instantiate(item, transform.position, Quaternion.identity);
        
    }

    
}
