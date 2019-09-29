using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace GeometryDashAutoPilot
{
    public class LevelProfile
    {
        private readonly uint _level;

        private List<(long, bool)> _operations;

        private readonly List<(long, bool)> _processedOperations;

        public int Count => _processedOperations.Count;

        public void LoadOperationsFromFile()
        {
            var filename = $"{_level}.txt";
            if (!File.Exists(filename))
            {
                Log.Information("The level profile does not exist.");
            }
            else
            {
                var json = File.ReadAllText($"{_level}.txt");
                _operations = JsonConvert.DeserializeObject<List<(long, bool)>>(json);
                ProcessOperations();
                Log.Information("Level profile loaded/reloaded.");
            }
        }

        public void SaveOperationsToFile()
        {
            var json = JsonConvert.SerializeObject(_operations);
            File.WriteAllText($"{_level}.txt", json);
            Log.Information("Level profile saved.");
        }

        public void AddOperation(long elapsedTicks, bool isStartPressing)
        {
            _operations.Add((elapsedTicks, isStartPressing));
        }

        public void AddOperations(ICollection<(long, bool)> operations)
        {
            _operations.AddRange(operations);
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

        /**
         * Fill the durations with key downs or it won't behave properly in the flight mode.
         */
        public void ProcessOperations()
        {
            _processedOperations.Clear();

            // Oh, it's just magic...
            const long fillingStep = 154375L;
            var lastTips = 0L;
            foreach (var operation in _operations)
            {
                if (operation.Item2)
                {
                    lastTips = operation.Item1;
                }
                else
                {
                    for (var tips = lastTips + fillingStep; tips < operation.Item1; tips += fillingStep)
                    {
                        _processedOperations.Add((tips, true));
                    }
                }
                _processedOperations.Add(operation);
            }
        }

        public (long, bool) this[int index] => _processedOperations[index];

        public LevelProfile(uint level)
        {
            _level = level;
            _operations = new List<(long, bool)>();
            _processedOperations = new List<(long, bool)>();
        }
    }
}
