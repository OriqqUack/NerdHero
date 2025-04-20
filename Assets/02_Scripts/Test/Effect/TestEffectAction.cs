using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    [System.Serializable]
    public class TestEffectAction : EffectAction
    {
        public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
        {
            return true;
        }

        public override object Clone()
        {
            return new TestEffectAction();
        }
    }
}