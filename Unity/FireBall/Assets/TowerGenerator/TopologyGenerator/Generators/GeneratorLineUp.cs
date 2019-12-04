using System.Collections.Generic;
using GameLib.DataStructures;

namespace TowerGenerator
{
    // ----- Build algorithm:
    // * if there is no parent build island
    // * build trunk up
    // * try to generate islands based on chance from cfg for each side
    //   * based on chance end it up with opened segment or not
    // * for each opened segments from prev state
    //   * based on chance from cfg
    //      * grow it up
    //        * base on chance keep it opened for the next generator
    //      * close 
    public class GeneratorLineUp : GeneratorBase
    {
        public GeneratorLineUp(long seed, TreeNode<Blueprint.Segment> trunkNode, ConfigBase cfg, TopologyGeneratorsManifoldBase manifold) 
            : base(seed, trunkNode, cfg, manifold)
        {
        }

        public override IEnumerable<TopGenStep> GenerateTower()
        {
            throw new System.NotImplementedException();
        }
    }
}