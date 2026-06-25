using SynchronizationLibrary;

namespace SecondTask
{
    public static class AsyncCounterService
    {
        private static int _count;
        private static AsyncReaderWriterLock _lock;

        static AsyncCounterService()
        {
            _count = 0;
            _lock = new AsyncReaderWriterLock();
        }

        public static async Task AddToCount(int value)
        {
            // Писатель получает эксклюзивный доступ
            using (await _lock.WriteLockAsync())
            {
                await DelaySimulation();
                _count += value;
                Console.WriteLine($"[Writer] Added {value}. Current count: {_count}");
            }
        }

        public static async Task<int> GetCount()
        {
            // Читатели могут заходить параллельно
            using (await _lock.ReadLockAsync())
            {
                await DelaySimulation();
                int current = _count;
                Console.WriteLine($"[Reader] Read value: {current}");
                return current;
            }
        }

        // для имитации задержек - важно для тестов 
        private static Task DelaySimulation()
        {
            var r = new Random();

            return Task.Delay(r.Next(1, 100));
        }
    }
}
