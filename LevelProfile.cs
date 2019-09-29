using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GeometryDashAutoPilot
{
    public class LevelProfile
    {
        private uint _level;

        private List<(long, bool)> _operations;

        public void LoadOperationsFromFile()
        {
            var json = File.ReadAllText($"{_level}.txt");
            _operations = JsonConvert.DeserializeObject<List<(long, bool)>>(json);
        }

        public void SaveOperationsFromFile()
        {
            var json = JsonConvert.SerializeObject(_operations);
            File.WriteAllText($"{_level}.txt", json);
        }

        public void AddOperation(long elapsedTicks, bool isStartPressing)
        {
            _operations.Add((elapsedTicks, isStartPressing));
        }

        public void RemoveOperationsSince(long elapsedTicks)
        {
            var filteredOperations = new List<(long, bool)>();
            foreach (var operation in _operations)
            {
                if (operation.Item1 < elapsedTicks)
                    filteredOperations.Add(operation);
            }

            _operations = filteredOperations;
        }

        public LevelProfile(uint level)
        {
            _level = level;
            _operations = new List<(long, bool)>();
        }
    }
}
