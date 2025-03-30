using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    [System.Serializable]
    public class TestSkillPreceidingAction : SkillPrecedingAction
    {
        private int count;

        public override void Release(Skill skill)
            => count = 0;

        public override bool Run(Skill skill)
        {
            return true;
        }

        public override object Clone() => new TestSkillPreceidingAction();
    }
}