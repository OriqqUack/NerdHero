using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderlineTitleAttribute : PropertyAttribute
{
    // Title Text
    public string Title { get; private set; }
    // ���� GUI�� ����� ����
    public int Space { get; private set; }

    public UnderlineTitleAttribute(string title, int space = 12)
    {
        Title = title;
        Space = space;
    }
}
