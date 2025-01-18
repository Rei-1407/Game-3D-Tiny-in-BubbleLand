using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyPool 
{
    private GameObject objectPool;
    private Stack<GameObject> stackPooling=new Stack<GameObject>();
    private ReturnMyPool returnMyPool;
    private GameObject tmp;
    public MyPool(GameObject game){
        this.objectPool=game;
    }
    public GameObject Get(Transform parent){
        if(stackPooling.Count!=0){
            tmp=stackPooling.Pop();
            tmp.SetActive(true);
            return tmp;
        }
        tmp=GameObject.Instantiate(objectPool,parent);
        returnMyPool=tmp.AddComponent<ReturnMyPool>();
        returnMyPool.myPool=this;
        return tmp;
    }


    public void AddToPool(GameObject pushObject){
        stackPooling.Push(pushObject);
    }
}