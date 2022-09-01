using System;
using System.Collections;
using System.Linq;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator
{
    public class ShowRoomChunksController : MonoBehaviour
    {
        public MetaProvider MetaProvider;
        public EntityPlace EntityPlace;
        public float Delay;
        private int _currentIndex;
        private IPseudoRandomNumberGenerator _rnd = RandomHelper.CreateRandomNumberGenerator();

        void Start()
        {
            Assert.IsNotNull(MetaProvider);
            StartCoroutine(ProcessShowing());
        }

        private IEnumerator ProcessShowing()
        {
            while (true)
            {
                var nextMetaToShow = GetNextMeta();
                var rndState = _rnd.GetState();
                Assert.IsNotNull(nextMetaToShow);
                Debug.Log($"Seed&Meta: {rndState.AsNumber()} {nextMetaToShow.ChunkName}");

                EntityPlace.Place(nextMetaToShow, rndState);
                _rnd.Next();
                yield return new WaitForSeconds(Delay);

            }
        }

        private MetaBase GetNextMeta()
        {
            var meta = MetaProvider.Metas[_currentIndex++];
            if (_currentIndex > MetaProvider.Metas.Count - 1)
            {
                _currentIndex = 0;
            }
            return meta;
        }
    }
}

