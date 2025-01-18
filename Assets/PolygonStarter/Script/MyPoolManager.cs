using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPoolManager : MonoBehaviour
{
    public static MyPoolManager Instance{
        get;set;
    }
    Dictionary<object,MyPool>dicPools=new Dictionary<object, MyPool>();
    void Awake()
    {
        Instance=this;
    }

    public GameObject GetFromPool(GameObject obj,Transform parent){
        if(!dicPools.ContainsKey(obj)){
            dicPools.Add(obj,new MyPool(obj));
        }
        return dicPools[obj].Get(parent);
    }
}