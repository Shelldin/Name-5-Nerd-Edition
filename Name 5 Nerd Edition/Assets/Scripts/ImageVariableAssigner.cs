using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageVariableAssigner : MonoBehaviour
{
    public Image imageToBeAssigned;

    private void Start()
    {
        if (imageToBeAssigned == null)
        {
            imageToBeAssigned = gameObject.GetComponent(typeof(Image)) as Image;
        }
    }

    public void AssignColorButtonImageVariable()
    {
        UIController.instance.colorButtonImage = imageToBeAssigned;
    }
}
