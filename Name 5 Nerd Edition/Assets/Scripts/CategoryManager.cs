using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CategoryManager : MonoBehaviour
{
    public List<CategorySO> easyCategoryList = new List<CategorySO>(),
        mediumCategoryList = new List<CategorySO>(),
        hardCategoryList = new List<CategorySO>();

    public List<CategorySO> categorySOList = new List<CategorySO>();

    public List<CategorySO> usedCategoriesList = new List<CategorySO>();

    // Start is called before the first frame update
    void Start()
    {
        
    }
    

    //if all the categories are used during the game refill them from the usedCategoryList and empty the usedCategoryList
    public void RefillCategoryList()
    {
        if (categorySOList.Count <= 0)
        {
            for (int i = 0; i <usedCategoriesList.Count; i++)
            {
                categorySOList.Add(usedCategoriesList[i]);
            }

            for (int i = usedCategoriesList.Count - 1; i >= 0; i--)
            {
                usedCategoriesList.Remove(usedCategoriesList[i]);
            }
        }
    }

    //select a random category from CategorySOList
    public int PickCategory()
    {
        int randomCategory = Random.Range(0, categorySOList.Count);
        return randomCategory;
    }

    //move a categorySO from categorySOList to usedCategoryList
    public void MoveCategoryToUsedCategoryList(int categoryToBeMoved)
    {
        usedCategoriesList.Add(categorySOList[categoryToBeMoved]);
        categorySOList.Remove(categorySOList[categoryToBeMoved]);
    }

    public void LoadEasyCategories()
    {
        for (int i = 0; i < easyCategoryList.Count; i++)
        {
            //change list in the paretheseses
            categorySOList.Add(easyCategoryList[i]);
        } 
        
        categorySOList.Sort(delegate(CategorySO i1, CategorySO i2)
            {return String.Compare(i1.name, i2.name, StringComparison.Ordinal);});
        
        UIController.instance.difficultyMenu.SetActive(false);
        
        GameManager.instance.LoadMainGameScene();
        
    }

    public void LoadMediumCategories()
    {
        for (int i = 0; i < mediumCategoryList.Count; i++)
        {
            categorySOList.Add(mediumCategoryList[i]);
        } 
        
        categorySOList.Sort(delegate(CategorySO i1, CategorySO i2)
            {return String.Compare(i1.name, i2.name, StringComparison.Ordinal);});
        
        UIController.instance.difficultyMenu.SetActive(false);
        
        GameManager.instance.LoadMainGameScene();
    }

    public void LoadHardCategories()
    {
        for (int i = 0; i < hardCategoryList.Count; i++)
        {
            categorySOList.Add(hardCategoryList[i]);
        } 
        
        categorySOList.Sort(delegate(CategorySO i1, CategorySO i2)
            {return String.Compare(i1.name, i2.name, StringComparison.Ordinal);});
        
        UIController.instance.difficultyMenu.SetActive(false);
        
        GameManager.instance.LoadMainGameScene();
    }
}
