using SynchronizationLibrary;

namespace SecondTask.Tests
{
    /// <summary>
    /// Тесты должны выполняться последовательно из-за того что класс-сервер статический как следствие он 1 на все тесты
    /// </summary>
    [Collection("Sequential")]
    public class AsyncCounterServiceTests
    {
        // Сброс состояния перед каждым тестом
        public AsyncCounterServiceTests()
        {
            ResetAsyncCounterService();
        }

        private static void ResetAsyncCounterService()
        {
            var type = typeof(AsyncCounterService);
            var countField = type.GetField("_count",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var lockField = type.GetField("_lock",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            countField?.SetValue(null, 0);
            
            // 2. Теперь этот cast будет работать, так как мы добавили using SynchronizationLibrary
            //var oldLock = lockField?.GetValue(null) as AsyncReaderWriterLock;
            lockField?.SetValue(null, new AsyncReaderWriterLock());
        }

        [Fact]
        public async Task GetCount_ReturnsZero_Initially()
        {
            var count = await AsyncCounterService.GetCount();
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task AddToCount_IncrementsCount()
        {
            var countzero = await AsyncCounterService.GetCount();
            Assert.Equal(0, countzero);

            await AsyncCounterService.AddToCount(5);
            var count = await AsyncCounterService.GetCount();
            Assert.Equal(5, count);
        }

        [Fact]
        public async Task MultipleAdds_AreAccumulated()
        {
            await AsyncCounterService.AddToCount(10);
            await AsyncCounterService.AddToCount(-3);
            await AsyncCounterService.AddToCount(7);

            var count = await AsyncCounterService.GetCount();
            Assert.Equal(14, count);
        }

        [Fact]
        public async Task ConcurrentReaders_CanReadSimultaneously()
        {
            const int initialValue = 42;
            await AsyncCounterService.AddToCount(initialValue);

            const int readerCount = 50;
            var tasks = new Task<int>[readerCount];
            for (int i = 0; i < readerCount; i++)
            {
                tasks[i] = AsyncCounterService.GetCount();
            }

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                Assert.Equal(initialValue, result);
            }
        }

        [Fact]
        public async Task ConcurrentWriters_ExecuteSequentially_WithoutRaceCondition()
        {
            const int iterations = 500;
            var tasks = new Task[iterations];
            for (int i = 0; i < iterations; i++)
            {
                tasks[i] = AsyncCounterService.AddToCount(1);
            }

            await Task.WhenAll(tasks);

            var finalCount = await AsyncCounterService.GetCount();
            Assert.Equal(iterations, finalCount); // если гонка — будет < 500
        }

        [Fact]
        public async Task ReadersWaitForWriterToComplete()
        {
            // Запускаем писателя, который удерживает блокировку ~100 мс
            var writerTask = Task.Run(async () =>
            {
                await AsyncCounterService.AddToCount(100); // внутри — await DelaySimulation()
            });

            // Даём время начать запись
            await Task.Delay(10);

            // Запускаем чтение — оно должно ждать завершения записи
            var readTask = AsyncCounterService.GetCount();

            // Убеждаемся, что чтение ещё не завершилось
            // (надёжнее — проверить через короткий таймаут)
            var completed = await Task.WhenAny(readTask, Task.Delay(50)) == readTask;
            Assert.False(completed, "Чтение завершилось до окончания записи!");

            // Ждём окончания записи
            await writerTask;

            // Теперь чтение должно завершиться
            var result = await readTask;
            Assert.Equal(100, result);
        }

        [Fact]
        public async Task MixedReadersAndWriters_ProduceConsistentResults()
        {
            const int writerCount = 100;
            const int readerCount = 100;

            var tasks = new List<Task>();

            // Запускаем писателей
            for (int i = 0; i < writerCount; i++)
            {
                tasks.Add(AsyncCounterService.AddToCount(1));
            }

            var readerResults = new int[readerCount];
            for (int i = 0; i < readerCount; i++)
            {
                int index = i;
                tasks.Add(Task.Run(async () =>
                {
                    readerResults[index] = await AsyncCounterService.GetCount();
                }));
            }

            await Task.WhenAll(tasks);

            var finalCount = await AsyncCounterService.GetCount();
            Assert.Equal(writerCount, finalCount);

            foreach (var r in readerResults)
            {
                Assert.InRange(r, 0, writerCount);
            }
        }
    }

    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class SequentialCollection : ICollectionFixture<SequentialFixture>
    {
    }

    public class SequentialFixture : IDisposable
    {
        public void Dispose() { }
    }
}

