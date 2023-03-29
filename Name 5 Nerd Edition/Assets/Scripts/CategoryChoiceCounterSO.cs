using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CategoryChoiceCounterSO : ScriptableObject
{
    public int categoriesChosenInt = 0;

    public void ResetCategoriesChosenInt()
    {
        categoriesChosenInt = 0;
    }

    public void IncreaseCategoriesChosenCounterInt()
    {
        categoriesChosenInt++;
    }
}
