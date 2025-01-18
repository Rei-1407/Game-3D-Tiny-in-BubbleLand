using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnMyPool : MonoBehaviour
{
   public MyPool myPool;

   private void OnDisable() {
        myPool.AddToPool(this.gameObject);
   }
}