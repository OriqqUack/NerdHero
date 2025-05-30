using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimatorParameterType
{
    Bool,
    Trigger
}

public enum LayerIndex
{
    Zero,
    One,
    Two,
    Three
}

[System.Serializable]
public struct AnimatorParameter
{
    public AnimatorParameterType type;
    public LayerIndex index;
    public Stat stat;
    public string name;
    
    private int hash;

    public bool IsValid => !string.IsNullOrEmpty(name);
    public int Hash
    {
        get
        {
            if (hash == 0 && IsValid)
                hash = Animator.StringToHash(name);
            return hash;
        }
    }
}
