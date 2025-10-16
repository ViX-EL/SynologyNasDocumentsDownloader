using SynologyNas.Api;

namespace SynologyNas.Search
{
    public class TaskManager
    {
        public async Task<string[]?> AccumulateSearchTasks(string nasFolderPath, List<string> patternBatches, string extension, SearchApiService searchClient)
        {
            string[]? taskIds = new string[patternBatches.Count];
            for (int i = 0; i < patternBatches.Count; i++)
            {
                string? taskId = await searchClient.StartSearchAsync(nasFolderPath, patternBatches[i], extension);
                if (taskId == null)
                {
                    Console.WriteLine($"Не удалось получить номер задачи для паттерна {patternBatches[i]}!");
                    return null;
                }
                taskIds[i] = taskId;
            }
            return taskIds;
        }
    }
}