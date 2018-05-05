using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WECdotNET
{
    public class Subscription : IDisposable
    {
        bool __disposed = false;

        IntPtr handle;

        public readonly string Name;
        public bool Open { get { return handle != IntPtr.Zero; } }

        /// <summary>
        /// Constructor for the Subscription wrapper class. Opens an existing subscription on the local machine
        /// </summary>
        /// <param name="subscriptionName">The name of the subscription</param>
        public Subscription(string subscriptionName) : this(subscriptionName, AccessMask.ReadAccess, Flags.OpenExisting)
        {
        }

        private Subscription(string subscriptionName, AccessMask accessMask, Flags flags)
        {
            Name = subscriptionName;
            handle = NativeMethods.EcOpenSubscription(Name, Convert.ToUInt32(accessMask), Convert.ToUInt32(flags));
            if (!Open)
                throw new Exception(Win32Helper.GetSystemMessage(Marshal.GetLastWin32Error()));
        }

        /// <summary>
        /// Enumerates all existing subscriptions on the local machine and returns their names.
        /// </summary>
        /// <returns>The names of all subscriptions on the local machine</returns>
        public static IEnumerable<string> EnumerateSubscriptions()
        {
            bool next = true;

            IntPtr enumerator = NativeMethods.EcOpenSubscriptionEnum(0);
            if (enumerator == IntPtr.Zero)
                throw new Exception(Win32Helper.GetSystemMessage(Marshal.GetLastWin32Error()));

            try
            {
                StringBuilder buffer = new StringBuilder(0x104);
                while (next)
                {
                    next = NativeMethods.EcEnumNextSubscription(enumerator, UInt16.MaxValue, buffer, out uint length);
                    if (next)
                        yield return buffer.ToString();

                    buffer.Clear();
                }
            }
            finally {
                NativeMethods.EcClose(enumerator);
                enumerator = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Enumerates all local subscriptions and returns a list of Subscription objects
        /// </summary>
        /// <returns>A List of Subscriptions on registered the local machine</returns>
        public static List<Subscription> GetSubscriptions()
        {
            List<Subscription> subscriptions = new List<Subscription>();

            foreach(string subscriptionName in EnumerateSubscriptions())
                subscriptions.Add(new Subscription(subscriptionName));

            return subscriptions;
        }

        /// <summary>
        /// Close subscription handles and dispose of managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (__disposed)
                return;

            if (disposing)
            {
            }

            try
            {
                if(Open)
                    NativeMethods.EcClose(handle);
                handle = IntPtr.Zero;
            }
            catch (Exception)
            {

            }

            __disposed = true;
        }

        ~Subscription()
        {
            Dispose(false);
        }
    }
}
