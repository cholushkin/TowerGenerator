using System;
using UnityEngine;


namespace TowerGenerator
{
    public class MetaProviderPopulatorTagExpression : MetaProviderPopulatorBase
    {
        [TextArea(3,10)]
        public string TagExpression;

        public override void FindMetas()
        {
            throw new NotFiniteNumberException();
        }
    }
}
