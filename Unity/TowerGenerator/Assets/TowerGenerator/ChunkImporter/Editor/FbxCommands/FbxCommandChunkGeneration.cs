using TowerGenerator.ChunkImporter;
using UnityEngine;
using UnityEngine.Assertions;

namespace TowerGenerator.FbxCommands
{
    // Версия чанка или его поколение. Для детерминированной генерации нужно иметь возможность фильтравать множество всех доступных чанков по их поколению.
    // В дальнейшем, когда добавляется новый чанк с поздним номером поколения в уже существующий билд (деплой, релиз, версию) это не разрушит рандомную генерацию старых башен. 
    // Потому что генератор работает на заданном диапазоне пололений чанков.
    // Если же результат генерации предполагает включать в себя новые добавленные позже чанки, тогда в генераторе должен быть отключен фильтр по поколениям.

    public class FbxCommandChunkGeneration : FbxCommandBase
    {
        public uint Generation;
        public override string GetFbxCommandName()
        {
            return "ChunkGeneration";
        }

        public override int GetExecutionPriority()
        {
            return PriorityLowest;
        }

        public override void ParseParameters(string parameters, GameObject gameObject)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsFalse(string.IsNullOrEmpty(parameters));
            Generation = ConvertUInt(parameters);
        }

        public override void Execute(GameObject gameObject, ChunkCooker.ChunkImportInformation importInformation)
        {
            Assert.IsNotNull(gameObject, $"There must be an object for the command '{GetFbxCommandName()}'");
            Assert.IsNotNull(importInformation);
            var chunkController = gameObject.GetComponent<ChunkControllerBase>();
            Assert.IsNotNull(chunkController);

            importInformation.GenerationAttributeAmount++;
            chunkController.Generation = Generation;
            if (Generation > importInformation.MaxGeneration)
                importInformation.MaxGeneration = Generation;
        }
    }
}
       