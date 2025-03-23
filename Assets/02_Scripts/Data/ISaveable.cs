using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    void Save(SaveData data);
    void Load(SaveData data);
}
