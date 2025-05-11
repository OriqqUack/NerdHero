using System.Collections.Generic;
using UnityEngine;

public interface ISaveable
{
    void Save(GameData data);
    void Load(GameData data);
}
