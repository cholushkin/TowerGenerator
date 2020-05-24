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

            // todo:
            // get first config recursively and ask for recommended establishing 

            var protoPointer = Prototype;
            while (protoPointer.GeneratorNodes.GetNext())
            {
                
            }
            Prototype.GeneratorNodes.GetNext();
        }
    }
}
