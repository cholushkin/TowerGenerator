using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class TowerGenerator : MonoBehaviour
    {
        public GeneratorProcessor Prototype;
        public bool DoGenerateOnStart;

        protected Blueprint _blueprint;
        protected GeneratorPointer _pointers;
        protected SegmentConstructor _constructor;

        // visualizer

        void Awake()
        {
            Assert.IsNotNull(Prototype);
        }

        void Start()
        {
            if (!Prototype)
                return;
            if (DoGenerateOnStart)
                Generate();
        }

        public void Generate()
        {
            PropagateSeeds();
            Establish();
            Prototype.Generate();
        }

        private void PropagateSeeds()
        {
        }

        // todo:
        // Gets all configs with 'establish' tag from prototype thus user can specify prototypes and configs that are fit better with establishing 
        // if there are no such prots or cfgs than just take next one according to _generatorNodes internal chooser
        public void Establish()
        {
            _pointers = new GeneratorPointer(_blueprint /*, MaxDistanceProgressToGenerator, MaxDistanceProgressToGarabageCollector*/);
            _blueprint = new Blueprint();
            //_constructor = new SegmentConstructor(_blueprint);

            // todo:
            // get first config recursively and ask for recommended establishing 

            var protoPointer = Prototype;
            while (protoPointer.GeneratorNodes.GetNext())
            {
                
            }
            Prototype.GeneratorNodes.GetNext();
        }


        //public virtual TopGenStep EstablishTower()
        //{
        //    SegmentBuilder segmentBuilder = new segmen(this, _rnd.ValueInt());

        //    segmentBuilder.Project(
        //        (TreeNode<SegmentBuilder.MemorySegment>)null,
        //        Range.One,
        //        Vector3.up,
        //        Vector3.zero,
        //        Config.GetPlacementConfig(TopologyType.ChunkIsland),
        //        null,
        //        null
        //    );

        //    Assert.IsTrue(segmentBuilder.GetProjectVariantsNumber() == 1);
        //    segmentBuilder.ApplyProject(0);

        //    var segment = segmentBuilder.Build().First();
        //    Assert.IsNotNull(segment);
        //    return TopGenStep.DoStep(segment, TopGenStep.Cmd.SegSpawn);
        //}
    }
}
