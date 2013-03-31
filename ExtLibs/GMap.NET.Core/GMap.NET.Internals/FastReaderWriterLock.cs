
namespace GMap.NET.Internals
{
   using System;
   using System.Threading;
#if !MONO
   using System.Runtime.InteropServices;
#endif

   /// <summary>
   /// custom ReaderWriterLock
   /// in Vista and later uses integrated Slim Reader/Writer (SRW) Lock
   /// http://msdn.microsoft.com/en-us/library/aa904937(VS.85).aspx
   /// http://msdn.microsoft.com/en-us/magazine/cc163405.aspx#S2
   /// </summary>
   public sealed class FastReaderWriterLock
   {
#if !MONO
      private static class NativeMethods
      {
         // Methods
         [DllImport("Kernel32", ExactSpelling=true)]
         internal static extern void AcquireSRWLockExclusive(ref IntPtr srw);
         [DllImport("Kernel32", ExactSpelling=true)]
         internal static extern void AcquireSRWLockShared(ref IntPtr srw);
         [DllImport("Kernel32", ExactSpelling=true)]
         internal static extern void InitializeSRWLock(out IntPtr srw);
         [DllImport("Kernel32", ExactSpelling=true)]
         internal static extern void ReleaseSRWLockExclusive(ref IntPtr srw);
         [DllImport("Kernel32", ExactSpelling=true)]
         internal static extern void ReleaseSRWLockShared(ref IntPtr srw);
      }

      IntPtr LockSRW;

      public FastReaderWriterLock()
      {
         if(UseNativeSRWLock)
         {
            NativeMethods.InitializeSRWLock(out this.LockSRW);
         }
      }
#endif

      static readonly bool UseNativeSRWLock = Stuff.IsRunningOnVistaOrLater() && IntPtr.Size == 4; // works only in 32-bit mode, any ideas on native 64-bit support? 
      Int32 busy = 0;
      Int32 readCount = 0;

      public void AcquireReaderLock()
      {
#if !MONO
         if(UseNativeSRWLock)
         {
            NativeMethods.AcquireSRWLockShared(ref LockSRW);
         }
         else
#endif
         {
            Thread.BeginCriticalRegion();

            while(Interlocked.CompareExchange(ref busy, 1, 0) != 0)
            {
               Thread.Sleep(1);
            }

            Interlocked.Increment(ref readCount);

            // somehow this fix deadlock on heavy reads
            Thread.Sleep(0);
            Thread.Sleep(0);
            Thread.Sleep(0);
            Thread.Sleep(0);
            Thread.Sleep(0);
            Thread.Sleep(0);
            Thread.Sleep(0);

            Interlocked.Exchange(ref busy, 0);
         }
      }

      public void ReleaseReaderLock()
      {
#if !MONO
         if(UseNativeSRWLock)
         {
            NativeMethods.ReleaseSRWLockShared(ref LockSRW);
         }
         else
#endif
         {
            Interlocked.Decrement(ref readCount);
            Thread.EndCriticalRegion();
         }
      }

      public void AcquireWriterLock()
      {
#if !MONO
         if(UseNativeSRWLock)
         {
            NativeMethods.AcquireSRWLockExclusive(ref LockSRW);
         }
         else
#endif
         {
            Thread.BeginCriticalRegion();

            while(Interlocked.CompareExchange(ref busy, 1, 0) != 0)
            {
               Thread.Sleep(1);
            }

            while(Interlocked.CompareExchange(ref readCount, 0, 0) != 0)
            {
               Thread.Sleep(1);
            }
         }
      }

      public void ReleaseWriterLock()
      {
#if !MONO
         if(UseNativeSRWLock)
         {
            NativeMethods.ReleaseSRWLockExclusive(ref LockSRW);
         }
         else
#endif
         {
            Interlocked.Exchange(ref busy, 0);
            Thread.EndCriticalRegion();
         }
      }
   }
}
