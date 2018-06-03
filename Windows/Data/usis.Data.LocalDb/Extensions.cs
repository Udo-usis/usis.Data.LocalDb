﻿//
//  @(#) Extensions.cs
//
//  Project:    usis.Data.LocalDb
//  System:     Microsoft Visual Studio 2017
//  Author:     Udo Schäfer
//
//  Copyright (c) 2018 usis GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace usis.Data.LocalDb
{
    //  ----------------
    //  Extensions class
    //  ----------------

    /// <summary>
    /// Provides extension methods for the <see cref="Manager"/> class.
    /// </summary>

    public static partial class Extensions
    {
        #region EnumerateVersions method

        //  ------------------------
        //  EnumerateVersions method
        //  ------------------------

        /// <summary>
        /// Returns all SQL Server Express LocalDB versions available on the computer.
        /// </summary>
        /// <param name="manager">The LocalDB manager object.</param>
        /// <returns>
        /// An enumerator to iterate through all version informations.
        /// </returns>

        public static IEnumerable<VersionInfo> EnumerateVersions(this Manager manager)
        {
            foreach (var version in manager.GetVersions())
            {
                yield return manager.GetVersionInfo(version);
            }
        }

        #endregion EnumerateVersions method

        #region EnumerateInstances methods

        //  --------------------------
        //  EnumerateInstances methods
        //  --------------------------

        /// <summary>
        /// Gets the informations about both named and default LocalDB instances on the user’s workstation.
        /// </summary>
        /// <param name="manager">The LocalDB manager object.</param>
        /// <returns>
        /// An enumerator to iterate through all instance informations.
        /// </returns>

        public static IEnumerable<InstanceInfo> EnumerateInstances(this Manager manager)
        {
            foreach (var instance in manager.GetInstances())
            {
                yield return manager.GetInstanceInfo(instance);
            }
        }

        #endregion EnumerateInstances methods

        #region CreateInstance method

        //  ---------------------
        //  CreateInstance method
        //  ---------------------

        /// <summary>
        /// Creates a new SQL Server Express LocalDB instance
        /// with the highest istalled version.
        /// </summary>
        /// <param name="manager">The LocalDB manager object.</param>
        /// <param name="instanceName">The name for the LocalDB instance to create.</param>
        /// <exception cref="ArgumentNullException"><paramref name="manager"/> is a <c>null</c> reference.</exception>

        public static void CreateInstance(this Manager manager, string instanceName)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            var version = manager.GetVersions().OrderByDescending(s => s).FirstOrDefault();
            if (version == null) throw new LocalDbException(Strings.NotInstalled);
            manager.CreateInstance(version, instanceName);
        }

        #endregion CreateInstance method

        #region StopInstance method

        //  -------------------
        //  StopInstance method
        //  -------------------

        /// <summary>
        /// Stops the specified SQL Server Express LocalDB instance from running.
        /// </summary>
        /// <param name="manager">The LocalDB manager object.</param>
        /// <param name="instanceName">The name of the LocalDB instance to stop.</param>
        /// <remarks>
        /// This function will return immediately without waiting for the LocalDB instance to stop.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="manager"/> is a <c>null</c> reference.</exception>

        public static void StopInstance(this Manager manager, string instanceName)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            manager.StopInstance(instanceName, StopInstanceOptions.None, new TimeSpan(0));
        }

        /// <summary>
        /// Stops the specified SQL Server Express LocalDB instance from running.
        /// </summary>
        /// <param name="manager">The LocalDB manager object.</param>
        /// <param name="instanceName">The name of the LocalDB instance to stop.</param>
        /// <param name="timeout">
        /// The time in seconds to wait for this operation to complete.
        /// If this value is 0, this function will return immediately without waiting for the LocalDB instance to stop.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="manager"/> is a <c>null</c> reference.</exception>

        public static void StopInstance(this Manager manager, string instanceName, int timeout)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            manager.StopInstance(instanceName, StopInstanceOptions.None, new TimeSpan(0, 0, timeout));
        }

        #endregion StopInstance method

        #region internal methods

        //  ------------------
        //  GetFunction method
        //  ------------------

        internal static T GetFunction<T>(this NativeLibraryHandle library, string procName, ref T function)
        {
            if (function == null)
            {
                var address = SafeNativeMethods.GetProcAddress(library, procName);
                if (address == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());
                function = Marshal.GetDelegateForFunctionPointer<T>(address);
            }
            return function;
        }

        //  ----------------------
        //  ValidateHResult method
        //  ----------------------

        internal static bool ValidateHResult(this uint hr, Func<uint, Exception> createException, params uint[] values)
        {
            foreach (var ok in values)
            {
                if (hr == ok) return (hr & 0x80000000) == 0;
            }
            if ((hr & 0x80000000) != 0) throw createException(hr);
            return true;
        }

        //  ----------------
        //  Enumerate method
        //  ----------------

        internal static IEnumerable<T> Enumerate<T>(this IntPtr pointer, int count, int size, Func<IntPtr, T> function)
        {
            for (int i = 0; i < count; i++)
            {
                var offset = new IntPtr(pointer.ToInt64() + size * i);
                yield return function(offset);
            }
        }

        //  ---------------
        //  ToString method
        //  ---------------

        internal static string ToString(this byte[] bytes, Encoding encoding) => new string(encoding.GetChars(bytes).TakeWhile(c => c != '\0').ToArray());

        //  -----------------
        //  ToDateTime method
        //  -----------------

        internal static DateTime ToDateTime(this System.Runtime.InteropServices.ComTypes.FILETIME fileTime) => DateTimeFromFileTime(fileTime.dwHighDateTime, fileTime.dwLowDateTime);

        #endregion internal methods

        #region private methods

        //  ---------------------------
        //  DateTimeFromFileTime method
        //  ---------------------------

        private static DateTime DateTimeFromFileTime(int high, int low)
        {
            if (high == 0 && low == 0) return DateTime.MinValue;

            long fileTime = ((long)high << 32) + (uint)low;
            return DateTime.FromFileTime(fileTime);
        }

        #endregion private methods
    }
}

// eof "Extensions.cs"
