using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryManager : MonoBehaviour
{
    public List<CategorySO> categorySOList = new List<CategorySO>();

    // Start is called before the first frame update
    void Start()
    {
        categorySOList.Sort(delegate(CategorySO i1, CategorySO i2)
            {return String.Compare(i1.name, i2.name, StringComparison.Ordinal);});
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
