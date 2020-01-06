using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;
using static ShowWhenAttribute;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/M_Material")]
public class M_Material : ScriptableObject
{
    public MaterialType materialType;

    [Header("Material properties for bullets")]

    [SerializeField]
    public MaterialType[] matBulletTypeResistence;



    [DocumentationSorting(DocumentationSortingAttribute.Level.UserRef)]
    public enum MaterialType
    {
        Metal = 0,
        Wood = 1,
        Glass = 2
    }
}


