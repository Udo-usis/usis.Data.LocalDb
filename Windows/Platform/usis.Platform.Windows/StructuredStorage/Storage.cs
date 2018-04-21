//
//  @(#) Storage.cs
//
//  Project:    usis Platform
//  System:     Microsoft Visual Studio 2017
//  Author:     Udo Sch�fer
//
//  Copyright (c) 2008-2017 usis GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using usis.Platform.Interop;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace usis.Platform.StructuredStorage
{
    //  -------------
    //  Storage class
    //  -------------

    /// <summary>
    /// Supports the creation and management of structured storage objects.
    /// </summary>

    public sealed class Storage : IDisposable
    {
        #region private constants

        private const uint STGFMT_STORAGE = 0;
        private const uint STGFMT_DOCFILE = 5;
        private const uint FILE_FLAG_NO_BUFFERING = 0x20000000;

        #endregion private constants

        #region fields

        private IStorage storageInterface;
        private ElementStatistics statistics;

        #endregion fields

        #region construction/destruction

        //  ------------------------
        //  construction/destruction
        //  ------------------------

        internal Storage(IStorage storage)
        {
            storageInterface = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        #region IDisposable members

        //  --------------
        //  Dispose method
        //  --------------

        /// <summary>
        /// Releases unmanaged resources.
        /// </summary>

        public void Dispose()
        {
            if (storageInterface != null)
            {
                Marshal.ReleaseComObject(storageInterface);
            }
            storageInterface = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This member overrides <see cref="object.Finalize"/>.
        /// </summary>

        ~Storage() { Dispose(); }

        #endregion IDisposable members

        #endregion construction/destruction

        #region private properties

        //  ------------------------------------
        //  PropertySetStorageInterface property
        //  ------------------------------------

        private IPropertySetStorage PropertySetStorageInterface => storageInterface as IPropertySetStorage;

        #endregion private properties

        #region public properties

        //  -------------------
        //  Statistics property
        //  -------------------

        /// <summary>
        /// Gets an <see cref="ElementStatistics"/> object that
        /// contains statistical information about the open storage element.
        /// </summary>
        /// <remarks>
        /// The statistical information is read the first time the
        /// <see cref="Statistics"/> property is accessed. Later changes to the
        /// storage are not reflected until the
        /// <see cref="Refresh"/> method is called.
        /// </remarks>

        [Category("Storage")]
        public ElementStatistics Statistics
        {
            get
            {
                if (statistics == null)
                {
                    statistics = new ElementStatistics(storageInterface);
                }
                return statistics;
            }
        }

        //  ----------------
        //  ClassId property
        //  ----------------

        /// <summary>
        /// Gets or sets the class identifier (<b>CLSID</b>)
        /// of the open storage element.
        /// </summary>

        [Obsolete("Remove when used.")]
        [Category("Storage")]
        public Guid ClassId
        {
            get => Statistics.ClassId;
            set
            {
                storageInterface.SetClass(ref value);
                if (statistics != null) statistics.ClassId = value;
            }
        }

        #region enumerators

        //  -----------------
        //  Elements property
        //  -----------------

        /// <summary>
        /// Gets an enumerator for all elements contained in this storage.
        /// </summary>

        [Category("Enumerators")]
        public IEnumerable<ElementStatistics> Elements => EnumerateElements(ElementType.None);

        //  -----------------
        //  Storages property
        //  -----------------

        /// <summary>
        /// Gets an enumerator for the storages contained in this storage.
        /// </summary>

        [Category("Enumerators")]
        public IEnumerable<ElementStatistics> Storages => EnumerateElements(ElementType.Storage);

        //  ----------------
        //  Streams property
        //  ----------------

        /// <summary>
        /// Gets an enumerator for the streams contained in this storage.
        /// </summary>

        [Category("Enumerators")]
        public IEnumerable<ElementStatistics> Streams => EnumerateElements(ElementType.Stream);

        //  ---------------------
        //  PropertySets property
        //  ---------------------

        /// <summary>
        /// Gets an enumerator for the property sets contained in this storage.
        /// </summary>

        [Category("Enumerators")]
        public IEnumerable<PropertySetStatistics> PropertySets => EnumeratePropertySets();

        #endregion enumerators

        #endregion public properties

        #region public static methods

        #region OpenStorageFile static method

        //  ----------------------
        //  OpenStorageFile method
        //  ----------------------

        /// <summary>
        /// Opens an existing root storage object in the file system.
        /// </summary>
        /// <param name="path">The path of file that contains the storage object.</param>
        /// <returns>
        /// The newly opened storage object.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="path" /> is a null reference (<c>Nothing</c> in Visual Basic).</exception>
        /// <overloads>
        /// Opens an existing root storage object in the file system.
        /// Use this function to open Compound Files and regular files.
        /// </overloads>

        [Obsolete("Remove when used.")]
        public static Storage OpenStorageFile(string path)
        {
            return OpenStorageFile(path, StorageModes.ShareDenyWrite);
        }

        /// <summary>
        /// Opens an existing root storage object in the file system,
        /// with the specified access mode.
        /// </summary>
        /// <param name="path">The path of file that contains the storage object.</param>
        /// <param name="storageMode">A value of that specifies the access mode to open the new storage object.</param>
        /// <returns>
        /// The newly opened storage object.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="path" /> is a null reference (<c>Nothing</c> in Visual Basic).</exception>

        [Obsolete("Remove when used.")]
        public static Storage OpenStorageFile(string path, StorageModes storageMode)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            Guid riid = new Guid(IID.IStorage);

            int hr = NativeMethods.StgOpenStorageEx(
                path,
                (int)storageMode,
                STGFMT_STORAGE,
                0,
                null,
                IntPtr.Zero,
                ref riid,
                out object storage);
            if (hr == HResult.StorageFileNotFound)
            {
                hr = HResult.FileNotFound;
            }
            Marshal.ThrowExceptionForHR(hr);

            return new Storage(storage as IStorage);
        }

        #endregion OpenStorageFile static method

        #region OpenCompoundFile static method

        //  -----------------------
        //  OpenCompoundFile method
        //  -----------------------

        /// <summary>
        /// Opens an existing Compound File's root storage object.
        /// </summary>
        /// <param name="path">The path of the Compound File.</param>
        /// <param name="storageMode">A value of that specifies the access mode to open the new storage object.</param>
        /// <returns>
        /// The newly opened storage object.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="path" /> is a null reference (<c>Nothing</c> in Visual Basic).</exception>
        /// <remarks>
        /// The storage object is opened a with a default sector size of 512 bytes.
        /// </remarks>

        public static Storage OpenCompoundFile(string path, StorageModes storageMode)
        {
            return OpenCompoundFile(path, storageMode, 512, false);
        }

        /// <summary>
        /// Opens an existing Compound File's root storage object with the
        /// specified sector size.
        /// </summary>
        /// <param name="path">The path of the Compound File.</param>
        /// <param name="storageMode">A value of that specifies the access mode to open the new storage object.</param>
        /// <param name="sectorSize">A value of that specifies the sector size to be used.</param>
        /// <param name="noBuffering">if set to <c>true</c> the file is being opened with no system caching for data reads and writes.</param>
        /// <returns>
        /// The newly opened storage object.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="path" /> is a null reference (<c>Nothing</c> in Visual Basic).</exception>

        public static Storage OpenCompoundFile(string path, StorageModes storageMode, int sectorSize, bool noBuffering)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            Guid riid = new Guid(IID.IStorage);
            STGOPTIONS options = new STGOPTIONS(1)
            {
                ulSectorSize = (uint)sectorSize
            };

            int hr = NativeMethods.StgOpenStorageEx(
                path,
                (int)storageMode,
                STGFMT_DOCFILE,
                noBuffering ? FILE_FLAG_NO_BUFFERING : 0,
                options,
                IntPtr.Zero,
                ref riid,
                out object storage);
            Marshal.ThrowExceptionForHR(hr);

            return new Storage(storage as IStorage);
        }

        #endregion OpenCompoundFile static method

        #region CreateStorageFile static method

        //  ------------------------
        //  CreateStorageFile method
        //  ------------------------

        /// <summary>
        /// Creates a new storage object.
        /// </summary>
        /// <param name="path">The path of the file to create.</param>
        /// <param name="storageMode">The access mode to use when opening the new storage object.</param>
        /// <returns>
        /// The newly created storage object.
        /// </returns>

        [Obsolete("Remove when used.")]
        public static Storage CreateStorageFile(string path, StorageModes storageMode)
        {
            Guid riid = new Guid(IID.IStorage);

            int hr = NativeMethods.StgCreateStorageEx(
                path,
                (int)storageMode,
                STGFMT_STORAGE,
                0,
                null,
                IntPtr.Zero,
                ref riid,
                out object storage);
            Marshal.ThrowExceptionForHR(hr);

            return new Storage(storage as IStorage);
        }

        #endregion CreateStorageFile static method

        #region CreateCompoundFile static method

        //  -------------------------
        //  CreateCompoundFile method
        //  -------------------------

        /// <summary>
        /// Creates a new compound file storage object using the COM-provided
        /// compound file implementation.
        /// </summary>
        /// <param name="path">The path of the file to create.
        /// It is passed uninterpreted to the file system.
        /// This can be a relative name or <c>null</c>.
        /// If <c>null</c>, a temporary file is allocated with a unique name.</param>
        /// <param name="storageMode">The access mode to use when opening the new storage object.</param>
        /// <returns>
        /// The newly created storage object.
        /// </returns>
        /// <remarks>
        /// The storage object is created a with a default sector size of 512 bytes.
        /// </remarks>

        public static Storage CreateCompoundFile(string path, StorageModes storageMode)
        {
            return CreateCompoundFile(path, storageMode, 512);
        }

        /// <summary>
        /// Creates a new compound file storage object using the COM-provided
        /// compound file implementation with a specified sector size.
        /// </summary>
        /// <param name="path">The path of the file to create.
        /// It is passed uninterpreted to the file system.
        /// This can be a relative name or <c>null</c>.
        /// If <c>null</c>, a temporary file is allocated with a unique name.</param>
        /// <param name="storageMode">The access mode to use when opening the new storage object.</param>
        /// <param name="sectorSize">The sector size to use.</param>
        /// <returns>
        /// The newly created storage object.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="sectorSize" /> is less than zero.</exception>
        /// <remarks>
        /// <p>
        /// The compound file implementation can be configured to use 512 or 4096 byte sectors,
        /// which is defined by the <paramref name="sectorSize" /> argument.
        /// </p>
        /// <p>
        /// The sector size determines the following limitations:
        /// <table>
        /// <tr><th>Limit</th><th>Compound file</th></tr>
        /// <tr><td>File size limits:</td><td>512: 2 gigabytes (GB)<p>4096: Up to file system limits</p></td></tr>
        /// <tr>
        /// <td>Maximum heap size required for open elements:</td>
        /// <td>512: 4 megabytes (MB)<p>4096: Up to virtual memory limits</p></td>
        /// </tr>
        /// <tr>
        /// <td>Concurrent root opens (opens of the same file):</td>
        /// <td>If <see cref="StorageModes.ShareDenyWrite" /> is specified, limits are dictated by the file-system limits.
        /// Otherwise, there is a limit of 20 concurrent root opens of the same file.</td>
        /// </tr>
        /// <tr>
        /// <td>Number of elements in a file:</td>
        /// <td>512: Unlimited, but performance may degrade if elements number in the thousands<p>4096: Unlimited</p></td>
        /// </tr>
        /// </table></p>
        /// </remarks>

        public static Storage CreateCompoundFile(string path, StorageModes storageMode, int sectorSize)
        {
            if (sectorSize < 0) throw new ArgumentOutOfRangeException(nameof(sectorSize));

            Guid riid = new Guid(IID.IStorage);
            STGOPTIONS options = new STGOPTIONS(1) { ulSectorSize = (uint)sectorSize };

            int hr = NativeMethods.StgCreateStorageEx(
                path,
                (int)storageMode,
                STGFMT_DOCFILE,
                0,
                options,
                IntPtr.Zero,
                ref riid,
                out object storage);
            Marshal.ThrowExceptionForHR(hr);

            return new Storage(storage as IStorage);
        }

        #endregion CreateCompoundFile static method

        #region Attach method

        //  -------------
        //  Attach method
        //  -------------

        /// <summary>
        /// Creates a new <see cref="Storage"/> object by attaching the specified storage.
        /// </summary>
        /// <param name="storage">
        /// A object that must implement the <c>IStorage</c> COM interface.
        /// </param>
        /// <returns>
        /// A newly created <see cref="Storage"/> object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <i>storage</i> is a null reference (<c>Nothing</c> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <i>storage</i> does not implement the <b>IStorage</b> COM interface.
        /// </exception>

        [Obsolete("TODO: check if needed.")]
        public static Storage Attach(object storage)
        {
            if (storage == null) throw new ArgumentNullException(nameof(storage));

            IStorage stg = storage as IStorage;
            if (stg == null) throw new ArgumentException(Resources.TextExceptionArgMustImplementIStorage, nameof(storage));

            return Attach(stg);
        }

        #region unused code

        ///// <summary>
        ///// Creates a new <b>Storage</b> object by attaching the specified
        ///// storage.
        ///// </summary>
        ///// <param name="storage">
        ///// A COM object that implements the <see cref="IStorage"/> interface.
        ///// </param>
        ///// <returns>
        ///// A newly created <see cref="Storage"/> object.
        ///// </returns>
        ///// <exception cref="ArgumentNullException">
        ///// <i>storage</i> is a null reference (<c>Nothing</c> in Visual Basic).
        ///// </exception>

        //internal static Storage Attach(IStorage storage, string path)
        //{
        //    if (storage == null) throw new ArgumentNullException("storage");

        //    return new Storage(storage, path);

        //} // Attach method

        #endregion unused code

        /// <summary>
        /// Creates a new <see cref="Storage" /> object by attaching the specified storage.
        /// </summary>
        /// <param name="storage">The storage to attach.</param>
        /// <returns>
        /// A newly created <see cref="Storage" /> object.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="storage" /> is a null reference (<c>Nothing</c> in Visual Basic).</exception>

        [Obsolete("TODO: check if needed.")]
        public static Storage Attach(Storage storage)
        {
            if (storage == null) throw new ArgumentNullException(nameof(storage));

            return new Storage(storage.storageInterface);
        }

        #endregion Attach method

        #region IsStorageFile method

        //  --------------------
        //  IsStorageFile method
        //  --------------------

        /// <summary>
        /// Indicates whether a particular disk file contains a storage object.
        /// </summary>
        /// <param name="fileName">The name of the disk file to be examined.</param>
        /// <returns>
        ///   <c>true</c>, when the file contains a storage object; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileName" /> is a null reference (<c>Nothing</c> in Visual Basic).</exception>
        /// <exception cref="FileNotFoundException">The specified file does not exist.</exception>
        /// <exception cref="Win32Exception">Another Windows error has occurred.</exception>

        [Obsolete("Remove when used.")]
        public static bool IsStorageFile(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));

            int hr = NativeMethods.StgIsStorageFile(fileName);
            if (!HResult.Succeeded(hr))
            {
                if (hr == HResult.StorageFileNotFound)
                {
                    throw new FileNotFoundException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.TextStorageFileNotFound,
                        fileName));
                }
                else throw new Win32Exception(hr);
            }
            else return hr == HResult.Ok;
        }

        #endregion IsStorageFile method

        #region WalkStorages method

        //  -------------------
        //  WalkStorages method
        //  -------------------        

        ///// <summary>
        ///// Walks the storage tree of a root storage.
        ///// </summary>
        ///// <param name="storage">
        ///// The root storage to iterate.
        ///// </param>
        ///// <returns>
        ///// An enumerable collection of sub storages of the given root storage.
        ///// </returns>

        //public static IEnumerable<Storage> WalkStorages(Storage storage)
        //{
        //    foreach (var item in storage.Storages)
        //    {
        //        using (Storage subStorage = storage.OpenStorage(item.Name, storage.Mode))
        //        {
        //            yield return subStorage;

        //            foreach (var stg in WalkStorages(subStorage))
        //            {
        //                yield return stg;
        //            }
        //        }
        //    }
        //}

        #endregion WalkStorages method

        #endregion public static methods

        #region WalkStorageStreams method

        //  -------------------------
        //  WalkStorageStreams method
        //  -------------------------        

        ///// <summary>
        ///// Walks thru all streams down the storage tree of a root storage.
        ///// </summary>
        ///// <param name="storage">
        ///// The root storage to iterate.
        ///// </param>
        ///// <returns>
        ///// An enumerable collection of all streams in all sub storages of the given root storage.
        ///// </returns>

        //public static IEnumerable<StorageStream> WalkStorageStreams(Storage storage)
        //{
        //    foreach (var stream in EnumerateStorageStreams(storage))
        //    {
        //        yield return stream;
        //    }
        //    foreach (var item in WalkStorages(storage))
        //    {
        //        foreach (var stream in EnumerateStorageStreams(item))
        //        {
        //            yield return stream;
        //        }
        //    }
        //}

        #endregion WalkStorageStreams method

        #region public methods

        #region StorageStream methods

        //  -------------------
        //  CreateStream method
        //  -------------------

        /// <summary>
        /// Creates and opens a stream object with the specified name
        /// contained in this storage object.
        /// All elements within a storage objects,
        /// both streams and other storage objects,
        /// are kept in the same name space.
        /// </summary>
        /// <param name="name">A string that contains the name of the newly created stream.
        /// The name can be used later to open or reopen the stream.
        /// The name must not exceed 31 characters in length.
        /// The 0x0 through 0x1f characters,
        /// serving as the first character of the stream/storage name,
        /// are reserved for use by OLE.
        /// This is a compound file restriction,
        /// not a structured storage restriction.</param>
        /// <param name="mode">Specifies the access mode to use when opening the newly created stream.</param>
        /// <returns>
        /// A <see cref="StorageStream" /> object the represents the newly created stream.
        /// </returns>

        [Obsolete("Remove when used.")]
        public StorageStream CreateStream(string name, StorageModes mode)
        {
            return new StorageStream(storageInterface.CreateStream(name, (int)mode, 0, 0));
        }

        //  -----------------
        //  OpenStream method
        //  -----------------

        /// <summary>
        /// Opens an existing stream object within this storage object
        /// in the specified access mode.
        /// </summary>
        /// <param name="name">A string that contains the name of the stream to open.
        /// The 0x0 through 0x1f characters,
        /// serving as the first character of the stream/storage name,
        /// are reserved for use by OLE.
        /// This is a compound file restriction,
        /// not a structured storage restriction.</param>
        /// <param name="mode">Specifies the access mode to be assigned to the open stream.</param>
        /// <returns>
        /// A <see cref="StorageStream" /> object the represents the newly opened stream.
        /// </returns>

        [Obsolete("Remove when used.")]
        public StorageStream OpenStream(string name, StorageModes mode)
        {
            return new StorageStream(storageInterface.OpenStream(name, IntPtr.Zero, (int)mode, 0));
        }

        #endregion StorageStream methods

        #region storage methods

        //  --------------------
        //  CreateStorage method
        //  --------------------

        /// <summary>
        /// Creates and opens a new storage object nested within
        /// this storage object with the specified name
        /// in the specified access mode.
        /// </summary>
        /// <param name="name">A string that contains the name of the newly created storage object.</param>
        /// <param name="mode">A value that specifies the access mode
        /// to use when opening the newly created storage object.</param>
        /// <returns>
        /// The newly created storage object.
        /// </returns>

        [Obsolete("Remove when used.")]
        public Storage CreateStorage(string name, StorageModes mode)
        {
            return new Storage(storageInterface.CreateStorage(name, (int)mode, 0, 0));
        }

        //  ------------------
        //  OpenStorage method
        //  ------------------

        /// <summary>
        /// Opens an existing storage object
        /// with the specified name.
        /// </summary>
        /// <param name="name">A string that contains the name of the storage object to open.</param>
        /// <returns>The opened storage object.</returns>
        /// <remarks>The storage is opened with the mode of the current storage.</remarks>

        public Storage OpenStorage(string name) { return OpenStorage(name, Statistics.Mode); }

        /// <summary>
        /// Opens an existing storage object
        /// with the specified name in the specified access mode.
        /// </summary>
        /// <param name="name">A string that contains the name of the storage object to open.</param>
        /// <param name="mode">A value that specifies the access mode to use when opening the storage object.</param>
        /// <returns>The opened storage object.</returns>

        public Storage OpenStorage(string name, StorageModes mode)
        {
            return new Storage(storageInterface.OpenStorage(name, null, (int)mode, IntPtr.Zero, 0));
        }

        //  -------------
        //  CopyTo method
        //  -------------

        /// <summary>
        /// Copies the entire contents of an open storage object
        /// to another storage object.
        /// </summary>
        /// <param name="destination">The open storage object
        /// into which this storage object is to be copied.</param>
        /// <exception cref="ArgumentNullException"><paramref name="destination" /> is a null reference (<c>Nothing</c> in Visual Basic).</exception>

        [Obsolete("Remove when used.")]
        public void CopyTo(Storage destination)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));

            storageInterface.CopyTo(0, IntPtr.Zero, IntPtr.Zero, destination.storageInterface);
        }

        //  -------------
        //  Commit method
        //  -------------

        /// <summary>
        /// Ensures that any changes made to a storage object
        /// open in transacted mode are reflected in the parent storage.
        /// For nonroot storage objects in direct mode,
        /// this method has no effect.
        /// For a root storage, it reflects the changes in the actual device;
        /// for example, a file on disk.
        /// </summary>

        public void Commit() { Commit(CommitConditions.None); }

        /// <summary>
        /// Ensures that any changes made to a storage object
        /// open in transacted mode are reflected in the parent storage.
        /// For nonroot storage objects in direct mode,
        /// this method has no effect.
        /// For a root storage, it reflects the changes in the actual device;
        /// for example, a file on disk.
        /// </summary>
        /// <param name="conditions">The conditions that control how the changes are committed to the storage object.</param>

        public void Commit(CommitConditions conditions) { storageInterface.Commit((int)conditions); }

        //  -------------
        //  Revert method
        //  -------------

        /// <summary>
        /// Discards all changes that have been made
        /// to the storage object since the last commit operation.
        /// </summary>

        [Obsolete("Remove when used.")]
        public void Revert() { storageInterface.Revert(); }

        //  -------------------
        //  SetStateBits method
        //  -------------------

        /// <summary>
        /// Stores up to 32 bits of state information in this storage object.
        /// This method is reserved for future use.
        /// </summary>
        /// <param name="stateBits">The new values of the bits to set.
        /// No legal values are defined for these bits;
        /// they are all reserved for future use
        /// and must not be used by applications.</param>
        /// <param name="mask">A binary mask indicating which bits in <i>stateBits</i>
        /// are significant in this call.</param>

        [Obsolete("Reserved for future use.")]
        public void SetStateBits(int stateBits, int mask)
        {
            storageInterface.SetStateBits(stateBits, mask);
        }

        #endregion storage methods

        #region element methods

        //  --------------------
        //  MoveElementTo method
        //  --------------------

        /// <summary>
        /// Moves a substorage or stream from this storage object
        /// to another storage object.
        /// </summary>
        /// <param name="name">String that contains the name of the element in this storage object
        /// to be moved.</param>
        /// <param name="destination">The destination storage object.</param>
        /// <param name="newName">String that contains the new name for the element
        /// in its new storage object.</param>

        [Obsolete("Remove when used.")]
        public void MoveElementTo(string name, Storage destination, string newName)
        {
            MoveElementTo(name, destination, newName, false);
        }

        /// <summary>
        /// Copies or moves a substorage or stream from this storage object
        /// to another storage object.
        /// </summary>
        /// <param name="name">String that contains the name of the element in this storage object
        /// to be moved or copied.</param>
        /// <param name="destination">The destination storage object.</param>
        /// <param name="newName">String that contains the new name for the element
        /// in its new storage object.</param>
        /// <param name="copy">Indicates whether the method should copy the data
        /// from the source to the destination.
        /// A copy is the same as a move except that the source element
        /// is not removed after copying the element to the destination.
        /// Copying an element on top of itself is undefined.</param>
        /// <exception cref="ArgumentNullException"><paramref name="destination" /> is a null reference (<c>Nothing</c> in Visual Basic).</exception>

        [Obsolete("Remove when used.")]
        public void MoveElementTo(string name, Storage destination, string newName, bool copy)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));

            storageInterface.MoveElementTo(name, destination.storageInterface, newName, copy ? 1 : 0);
        }

        //  ---------------------
        //  DestroyElement method
        //  ---------------------

        /// <summary>
        /// Removes the specified storage or stream from this storage object.
        /// </summary>
        /// <param name="name">A string that contains the name of the storage or stream to be removed.</param>

        [Obsolete("Remove when used.")]
        public void DestroyElement(string name) { storageInterface.DestroyElement(name); }

        //  --------------------
        //  RenameElement method
        //  --------------------

        /// <summary>
        /// Renames the specified substorage or stream in this storage object.
        /// </summary>
        /// <param name="oldName">A string that contains the name of the substorage or stream to be changed.</param>
        /// <param name="newName">A string that contains the new name for the specified substorage or stream.</param>

        [Obsolete("Remove when used.")]
        public void RenameElement(string oldName, string newName) { storageInterface.RenameElement(oldName, newName); }

        //  ----------------------
        //  SetElementTimes method
        //  ----------------------

        /// <summary>
        /// Sets the modification, access, and creation times
        /// of the specified storage element,
        /// if the underlying file system supports this method.
        /// </summary>
        /// <param name="name">The name of the storage object element whose times are to be modified.
        /// If <c>null</c> (<c>Nothing</c> in Visual Basic),
        /// the time is set on the root storage rather than one of its elements.</param>
        /// <param name="created">The new creation time for the element.</param>
        /// <param name="accessed">The new access time for the element.</param>
        /// <param name="modified">The new modification time for the element.</param>

        [Obsolete("Remove when used.")]
        public void SetElementTimes(
            string name,
            DateTime created,
            DateTime accessed,
            DateTime modified)
        {
            ComTypes.FILETIME ctime = created.ToFILETIME();
            ComTypes.FILETIME atime = accessed.ToFILETIME();
            ComTypes.FILETIME mtime = modified.ToFILETIME();

            storageInterface.SetElementTimes(name, ref ctime, ref atime, ref mtime);
        }

        #endregion element methods

        #region instance methods

        //  --------------
        //  Refresh method
        //  --------------

        /// <summary>
        /// Updates the storage's properties by re-reading
        /// the values from the underling storage object.
        /// </summary>

        [Obsolete("TODO: Check if needed.")]
        public void Refresh()
        {
            if (statistics != null) statistics = null;
        }

        //  -------------------
        //  SwitchToFile method
        //  -------------------

        /// <summary>
        /// Copies the current file associated with the storage object to a new file.
        /// The new file is then used for the storage object and any uncommitted changes.
        /// </summary>
        /// <param name="path">A string that specifies the file name for the new file.
        /// It cannot be the name of an existing file.
        /// If <c>null</c>, this method creates a temporary file with a unique name,
        /// and you can use the <see cref="ElementStatistics.Name" /> property
        /// to retrieve the name of the temporary file.</param>
        /// <exception cref="InvalidOperationException">This storage is not a root storage.</exception>

        public void SwitchToFile(string path)
        {
            var rootStorage = storageInterface as IRootStorage;
            if (rootStorage == null) throw new InvalidOperationException(Resources.TextExceptionNoRootStorage);
            rootStorage.SwitchToFile(path);
            statistics = null;
        }

        #endregion instance methods

        #region PropertySet methods

        //  ------------------------
        //  CreatePropertySet method
        //  ------------------------

        /// <summary>
        /// Creates and opens a new property set in the storage object.
        /// </summary>
        /// <param name="formatId">
        /// The format identifier (FMTID) of the property set to be created.
        /// </param>
        /// <param name="mode">
        /// An access mode in which the newly created property set is to be opened,
        /// taken from certain values from the
        /// <see cref="StorageModes"/> enumeration.
        /// </param>
        /// <param name="characteristics">
        /// The values from the <see cref="PropertySetCharacteristics"/> enumeration.
        /// </param>
        /// <returns>
        /// The newly created property set.
        /// </returns>

        [Obsolete("Remove when used.")]
        public PropertySet CreatePropertySet(
            Guid formatId,
            StorageModes mode,
            PropertySetCharacteristics characteristics)
        {
            IPropertyStorage ps = null;
            try
            {
                ps = PropertySetStorageInterface.Create(ref formatId, IntPtr.Zero, (int)characteristics, (int)mode);
                return PropertySet.Attach(ps);
            }
            finally
            {
                if (ps != null) Marshal.ReleaseComObject(ps);
            }
        }

        //  ----------------------
        //  OpenPropertySet method
        //  ----------------------

        /// <summary>
        /// Opens a property set contained in the storage object.
        /// </summary>
        /// <param name="formatId">
        /// The format identifier (FMTID) of the property set to be opened.
        /// </param>
        /// <returns>
        /// The newly opened property set.
        /// </returns>

        [Obsolete("Remove when used.")]
        public PropertySet OpenPropertySet(Guid formatId)
        {
            return OpenPropertySet(formatId, Statistics.Mode);
        }

        /// <summary>
        /// Opens a property set contained in the storage object with the
        /// specified access mode.
        /// </summary>
        /// <param name="formatId">
        /// The format identifier (FMTID) of the property set to be opened.
        /// </param>
        /// <param name="mode">
        /// The access mode in which the newly created property set is to be opened.
        /// These flags are taken from <see cref="StorageModes"/> enumeration.
        /// </param>
        /// <returns>
        /// The newly opened property set.
        /// </returns>

        [Obsolete("Remove when used.")]
        public PropertySet OpenPropertySet(Guid formatId, StorageModes mode)
        {
            IPropertyStorage ps = null;
            try
            {
                ps = PropertySetStorageInterface.Open(ref formatId, (int)mode);
                return PropertySet.Attach(ps);
            }
            finally
            {
                if (ps != null) Marshal.ReleaseComObject(ps);
            }
        }

        /// <summary>
        /// Opens a property set contained in the storage object with the
        /// specified access mode and optionally throws an exception
        /// when the property set does not exist.
        /// </summary>
        /// <param name="formatId">
        /// The format identifier (FMTID) of the property set to be opened.
        /// </param>
        /// <param name="mode">
        /// The access mode in which the newly created property set is to be opened.
        /// These flags are taken from <see cref="StorageModes"/> enumeration.
        /// </param>
        /// <param name="throwException">
        /// If set to <c>true</c> throws an exception,
        /// when the specified property set does not exist.
        /// </param>
        /// <returns>
        /// The newly opened property set.
        /// </returns>

        [Obsolete("Remove when used.")]
        public PropertySet OpenPropertySet(Guid formatId, StorageModes mode, bool throwException)
        {
            try
            {
                return OpenPropertySet(formatId, mode);
            }
            catch (COMException exception)
            {
                if (exception.ErrorCode == HResult.StorageFileNotFound && !throwException)
                {
                    return null;
                }
                else throw;
            }
        }

        //  ------------------------
        //  DeletePropertySet method
        //  ------------------------

        /// <summary>
        /// Deletes one of the property sets contained in the storage object.
        /// </summary>
        /// <param name="formatId">
        /// The format identifier (FMTID) of the property set to be deleted.
        /// </param>

        [Obsolete("Remove when used.")]
        public void DeletePropertySet(Guid formatId)
        {
            PropertySetStorageInterface.Delete(ref formatId);
        }

        #endregion PropertySet methods

        #endregion public methods

        #region private methods

        #region EnumerateElements method

        //  ------------------------
        //  EnumerateElements method
        //  ------------------------

        private IEnumerable<ElementStatistics> EnumerateElements(ElementType elementType, int bufferSize = 0x10)
        {
            // get enumerator
            IEnumSTATSTG enumerator = storageInterface.EnumElements(0, IntPtr.Zero, 0);
            try
            {
                // allocate buffer
                int fetched;
                ComTypes.STATSTG[] elements = new ComTypes.STATSTG[bufferSize];

                // enumerate elements
                enumerator.Reset();
                do
                {
                    fetched = enumerator.Next(bufferSize, elements);
                    for (int i = 0; i < fetched; i++)
                    {
                        ElementStatistics element = new ElementStatistics(elements[i]);
                        if (elementType == ElementType.None || element.ElementType == elementType) yield return element;
                    }
                }
                while (fetched != 0);
            }
            finally
            {
                if (enumerator != null) Marshal.ReleaseComObject(enumerator);
            }
        }

        #endregion EnumerateElements method

        #region EnumeratePropertySets method

        //  ----------------------------
        //  EnumeratePropertySets method
        //  ----------------------------

        private IEnumerable<PropertySetStatistics> EnumeratePropertySets(int bufferSize = 0x10)
        {
            // get interface
            IPropertySetStorage pss = PropertySetStorageInterface;
            if (pss == null) yield break;

            // get enumerator
            IEnumSTATPROPSETSTG enumerator = pss.Enum();
            try
            {
                // allocate buffer
                int fetched;
                STATPROPSETSTG[] elements = new STATPROPSETSTG[bufferSize];

                // enumerate elements
                enumerator.Reset();
                do
                {
                    fetched = enumerator.Next(bufferSize, elements);
                    for (int i = 0; i < fetched; i++)
                    {
                        yield return new PropertySetStatistics(elements[i]);
                    }
                }
                while (fetched != 0);
            }
            finally
            {
                if (enumerator != null) Marshal.ReleaseComObject(enumerator);
            }
        }

        #endregion EnumeratePropertySets method

        #region EnumerateStorageStreams methods

        //  -------------------------------
        //  EnumerateStorageStreams methods
        //  -------------------------------

        //[Obsolete]
        //private static IEnumerable<StorageStream> EnumerateStorageStreams(Storage storage)
        //{
        //    foreach (var item in storage.Streams)
        //    {
        //        using (var stream = storage.OpenStream(item.Name, storage.Mode))
        //        {
        //            yield return stream;
        //        }
        //    }
        //}

        #endregion EnumerateStorageStreams methods

        #endregion private methods
    }
}

// eof "Storage.cs"
