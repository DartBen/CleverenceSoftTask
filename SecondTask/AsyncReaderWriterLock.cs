using System;
using System.Threading;
using System.Threading.Tasks;

namespace SynchronizationLibrary
{


    public class AsyncReaderWriterLock
    {
        private readonly SemaphoreSlim _writerSemaphore = new(1, 1);
        private readonly SemaphoreSlim _readerGateSemaphore = new(1, 1);
        private int _readerCount;
        private int _writerCount;

        public async Task<IDisposable> ReadLockAsync()
        {
            await _readerGateSemaphore.WaitAsync();
            try
            {
                if (Interlocked.Increment(ref _readerCount) == 1)
                {
                    await _writerSemaphore.WaitAsync();
                }
            }
            finally
            {
                _readerGateSemaphore.Release();
            }

            return new ReadLockReleaser(this);
        }

        public async Task<IDisposable> WriteLockAsync()
        {
            bool isFirstWriter = Interlocked.Increment(ref _writerCount) == 1;

            try
            {
                if (isFirstWriter)
                {
                    await _readerGateSemaphore.WaitAsync();
                }

                await _writerSemaphore.WaitAsync();
            }
            catch
            {
                if (Interlocked.Decrement(ref _writerCount) == 0)
                {
                    _readerGateSemaphore.Release();
                }
                throw;
            }

            return new WriteLockReleaser(this);
        }

        private void ReleaseRead()
        {
            if (Interlocked.Decrement(ref _readerCount) == 0)
            {
                _writerSemaphore.Release();
            }
        }

        private void ReleaseWrite()
        {
            _writerSemaphore.Release();

            if (Interlocked.Decrement(ref _writerCount) == 0)
            {
                _readerGateSemaphore.Release();
            }
        }

        private readonly struct ReadLockReleaser : IDisposable
        {
            private readonly AsyncReaderWriterLock _lock;
            public ReadLockReleaser(AsyncReaderWriterLock @lock) => _lock = @lock;
            public void Dispose() => _lock.ReleaseRead();
        }

        private readonly struct WriteLockReleaser : IDisposable
        {
            private readonly AsyncReaderWriterLock _lock;
            public WriteLockReleaser(AsyncReaderWriterLock @lock) => _lock = @lock;
            public void Dispose() => _lock.ReleaseWrite();
        }
    }
}