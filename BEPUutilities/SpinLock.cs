using System;
using System.Threading;


namespace BEPUutilities
{
    /// <summary>
    /// Synchronizes using a busy wait.  Take care when using this; if the critical section is long or there's any doubt about the use of a busy wait, consider using Monitor locks or other approaches instead.
    /// Replaces the .NET SpinLock on PC and provides its functionality on the Xbox360.
    /// </summary>
    public class SpinLock
    {
        private const int MaximumSpinWait = 15;
        private const int SleepInterval = 10;
        private int owner = -1;

        private ManualResetEvent sleepResetEvent = new ManualResetEvent(false);
        private System.Threading.SpinLock spinLock = new System.Threading.SpinLock();

        /// <summary>
        /// Enters the critical section.  A thread cannot attempt to enter the spinlock if it already owns the spinlock.
        /// </summary>
        public void Enter()
        {
            bool taken = false;
            spinLock.Enter(ref taken);
            return;

            int count = 0;
            while (Interlocked.CompareExchange(ref owner, 0, -1) != -1)
            {
                //Lock is owned by someone else.
                count++;
                WaitBriefly(ref count);
            }
            //It's my lock now!
        }

        /// <summary>
        /// Attempts to enters the critical section.  A thread cannot attempt to enter the spinlock if it already owns the spinlock.
        /// </summary>
        public bool TryEnter()
        {
            bool taken = false;
            spinLock.TryEnter(ref taken);
            return taken;

            return Interlocked.CompareExchange(ref owner, 0, -1) == -1;
        }

        /// <summary>
        /// Exits the critical section.  This can only be safely called from the same
        /// thread of execution after a corresponding Enter.
        /// </summary>
        public void Exit()
        {
            spinLock.Exit();
            return;

            //To be safe, technically should check the identity of the exiter.
            //But since this is a very low-level, restricted access class,
            //assume that enter has to succeed before exit is tried.
            owner = -1;
        }

        internal void WaitBriefly(ref int attempt)
        {
            if (attempt == SleepInterval)
            {
                sleepResetEvent.WaitOne(0);
                attempt -= SleepInterval;
            }
            else
            {
                sleepResetEvent.WaitOne(Math.Min(3 << attempt, MaximumSpinWait));
            }
        }
    }
}