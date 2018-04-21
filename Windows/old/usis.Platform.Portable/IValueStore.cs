﻿//
//  @(#) IValueStore.cs
//
//  Project:    usis Platform
//  System:     Microsoft Visual Studio 2015
//  Author:     Udo Schäfer
//
//  Copyright (c) 2015-2017 usis GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace usis.Platform.Portable
{
    #region IValueStore interface

    //  ---------------------
    //  IValueStore interface
    //  ---------------------

    /// <summary>
    /// Defines the properties and methods that have to be implemented
    /// by a class to provide a store for named values.
    /// </summary>

    [Obsolete("Use type from usis.Platform namespace instead.")]
    public interface IValueStore
    {
        //  -------------
        //  Name property
        //  -------------

        /// <summary>
        /// Gets the name of the value store.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>

        string Name { get; }

        //  -------------------
        //  ValueNames property
        //  -------------------

        /// <summary>
        /// Gets an enumerator to iterate all value names in the store.
        /// </summary>
        /// <value>
        /// An enumerator to iterate all value names in the store.
        /// </value>

        IEnumerable<string> ValueNames { get; }

        //  ---------------
        //  Values property
        //  ---------------

        /// <summary>
        /// Gets an enumerator to iterate all named values in the store.
        /// </summary>
        /// <value>
        /// An enumerator to iterate all named values in the store.
        /// </value>

        IEnumerable<INamedValue> Values { get; }

        //  ---------------
        //  GetValue method
        //  ---------------

        /// <summary>
        /// Retrieves the value with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the value to retrieve.
        /// </param>
        /// <returns>
        /// A type that implements <b>INamedValue</b> and represents the specified value,
        /// or <b>null</b> if the value does not exists.
        /// </returns>

        INamedValue GetValue(string name);

        //  ---------------
        //  SetValue method
        //  ---------------

        /// <summary>
        /// Saves the specified named value in the store.
        /// </summary>
        /// <param name="value">
        /// The named value to save.
        /// </param>

        void SetValue(INamedValue value);

        //  ------------------
        //  DeleteValue method
        //  ------------------

        /// <summary>
        /// Deletes the value with the specified name.
        /// </summary>
        /// <param name="name">The name of the value to delete.</param>
        /// <returns>
        /// <b>true</b> when value was deleted
        /// or
        /// <b>false</b> when a value with the specified name does not exist.
        /// </returns>

        bool DeleteValue(string name);
    }

    #endregion IValueStore interface

    #region ValueStore class

    //  ----------------
    //  ValueStore class
    //  ----------------

    /// <summary>
    /// Implements the interface <see cref="IValueStore"/> as a value store
    /// that keeps all values in memory.
    /// </summary>

    [Obsolete("Use type from usis.Platform namespace instead.")]
    [DataContract]
    public class ValueStore : IValueStore
    {
        #region fields

        private Dictionary<string, INamedValue> values = new Dictionary<string, INamedValue>(StringComparer.OrdinalIgnoreCase);

        #endregion fields

        #region IValueStore members

        //  -------------
        //  Name property
        //  -------------

        /// <summary>
        /// Gets the name of the value store.
        /// </summary>
        /// <value>
        /// The name of the value store..
        /// </value>

        [DataMember]
        public string Name { get; set; }

        //  -------------------
        //  ValueNames property
        //  -------------------

        /// <summary>
        /// Gets an enumerator to iterate all value names in the store.
        /// </summary>
        /// <value>
        /// An enumerator to iterate all value names in the store.
        /// </value>

        public IEnumerable<string> ValueNames { get { foreach (var name in values.Keys) yield return name; } }

        //  ---------------
        //  Values property
        //  ---------------

        /// <summary>
        /// Gets an enumerator to iterate all named values in the store.
        /// </summary>
        /// <value>
        /// An enumerator to iterate all named values in the store.
        /// </value>

        public IEnumerable<INamedValue> Values { get { foreach (var item in values.Values) yield return item; } }

        //  ---------------
        //  GetValue method
        //  ---------------

        /// <summary>
        /// Retrieves the value with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of the value to retrieve.
        /// </param>
        /// <returns>
        /// A type that implements <b>INamedValue</b> and represents the specified value.
        /// </returns>

        public INamedValue GetValue(string name)
        {
            INamedValue namedValue = null;
            if (values.TryGetValue(name, out namedValue))
            {
                return namedValue;
            }
            else return null;
        }

        //  ---------------
        //  SetValue method
        //  ---------------

        /// <summary>
        /// Saves the specified named value in the store.
        /// </summary>
        /// <param name="value">The named value to save.</param>
        /// <exception cref="ArgumentNullException">
        /// <c>value</c> is a null reference.
        /// </exception>

        public void SetValue(INamedValue value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            values[value.Name] = value;
        }

        //  ------------------
        //  DeleteValue method
        //  ------------------

        /// <summary>
        /// Deletes the value with the specified name.
        /// </summary>
        /// <param name="name">The name of the value to delete.</param>
        /// <returns>
        /// <b>true</b> when value was deleted
        /// or
        /// <b>false</b> when a value with the specified name does not exist.
        /// </returns>

        public bool DeleteValue(string name)
        {
            return values.Remove(name);
        }

        #endregion IValueStore members
    }

    #endregion ValueStore class

    #region ValueStoreInterfaceExtensions class

    //  -----------------------------------
    //  ValueStoreInterfaceExtensions class
    //  -----------------------------------

    /// <summary>
    /// Provides extension to class that implement the
    /// <see cref="IValueStore"/> interface.
    /// </summary>

    [Obsolete("Use type from usis.Platform namespace instead.")]
    public static class ValueStoreInterfaceExtensions
    {
        //  -------------
        //  Get<T> method
        //  -------------

        /// <summary>
        /// Gets value with the specified name.
        /// </summary>
        /// <typeparam name="T">The type of the value to get.</typeparam>
        /// <param name="store">The store to get the value from.</param>
        /// <param name="name">The name of the value to get.</param>
        /// <returns>
        /// The value with the specified name as type T.
        /// </returns>
        /// <exception cref="ArgumentNullException"><i>store</i> is a null reference (<b>Nothing</b> in Visual Basic).</exception>

        public static T Get<T>(this IValueStore store, string name)
        {
            return store.Get(name, default(T));
        }

        /// <summary>
        /// Gets value with the specified name.
        /// </summary>
        /// <typeparam name="T">The type of the value to get.</typeparam>
        /// <param name="store">The store to get the value from.</param>
        /// <param name="name">The name of the value to get.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The value with the specified name as type T.
        /// </returns>
        /// <exception cref="ArgumentNullException"><i>store</i> is a null reference (<b>Nothing</b> in Visual Basic).</exception>

        public static T Get<T>(this IValueStore store, string name, T defaultValue)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            var namedValue = store.GetValue(name);
            if (namedValue == null) return defaultValue;
            return namedValue.Get(defaultValue);
        }

        //  --------------
        //  GetByte method
        //  --------------

        /// <summary>
        /// Gets the value with the specified name as a <see cref="byte"/>.
        /// </summary>
        /// <param name="store">
        /// The store to get the value from.
        /// </param>
        /// <param name="name">
        /// The name of the value to get.
        /// </param>
        /// <param name="defaultValue">The default value to return when the specified value does not exist.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The value with the specified name as <see cref="byte"/> or <i>defaultValue</i> if the value does not exists.</returns>
        /// <exception cref="ArgumentNullException"><i>store</i> is a null reference (<b>Nothing</b> in Visual Basic).</exception>

        public static byte GetByte(this IValueStore store, string name, byte defaultValue, IFormatProvider provider)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            var namedValue = store.GetValue(name);
            if (namedValue == null) return defaultValue;
            return namedValue.GetByte(provider);
        }

        /// <summary>
        /// Gets the value with the specified name as a <see cref="byte"/>.
        /// </summary>
        /// <param name="store">
        /// The store to get the value from.
        /// </param>
        /// <param name="name">
        /// The name of the value to get.
        /// </param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The value with the specified name as <see cref="byte"/> or 0 if the value does not exists.</returns>
        /// <exception cref="ArgumentNullException"><i>store</i> is a null reference (<b>Nothing</b> in Visual Basic).</exception>

        public static byte GetByte(this IValueStore store, string name, IFormatProvider provider)
        {
            return store.GetByte(name, 0, provider);
        }

        //  ---------------
        //  GetInt32 method
        //  ---------------

        /// <summary>
        /// Gets the value with the specified name as a <see cref="int"/>.
        /// </summary>
        /// <param name="store">
        /// The store to get the value from.
        /// </param>
        /// <param name="name">
        /// The name of the value to get.
        /// </param>
        /// <param name="defaultValue">The default value to return when the specified value does not exist.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The value with the specified name as <see cref="int"/> or <i>defaultValue</i> if the value does not exists.</returns>
        /// <exception cref="ArgumentNullException"><i>store</i> is a null reference (<b>Nothing</b> in Visual Basic).</exception>

        public static int GetInt32(this IValueStore store, string name, int defaultValue, IFormatProvider provider)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            var namedValue = store.GetValue(name);
            if (namedValue == null) return defaultValue;
            return namedValue.GetInt32(provider);
        }

        /// <summary>
        /// Gets the value with the specified name as a <see cref="int"/>.
        /// </summary>
        /// <param name="store">
        /// The store to get the value from.
        /// </param>
        /// <param name="name">
        /// The name of the value to get.
        /// </param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The value with the specified name as <see cref="int"/> or 0 if the value does not exists.</returns>
        /// <exception cref="ArgumentNullException"><i>store</i> is a null reference (<b>Nothing</b> in Visual Basic).</exception>

        public static int GetInt32(this IValueStore store, string name, IFormatProvider provider)
        {
            return store.GetInt32(name, 0, provider);
        }

        //  ---------------
        //  GetInt64 method
        //  ---------------

        /// <summary>
        /// Gets the value with the specified name as a <see cref="long"/>.
        /// </summary>
        /// <param name="store">
        /// The store to get the value from.
        /// </param>
        /// <param name="name">
        /// The name of the value to get.
        /// </param>
        /// <param name="defaultValue">The default value to return when the specified value does not exist.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The value with the specified name as <see cref="long"/> or <i>defaultValue</i> if the value does not exists.</returns>
        /// <exception cref="ArgumentNullException"><i>store</i> is a null reference (<b>Nothing</b> in Visual Basic).</exception>

        public static long GetInt64(this IValueStore store, string name, long defaultValue, IFormatProvider provider)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            var namedValue = store.GetValue(name);
            if (namedValue == null) return defaultValue;
            return namedValue.GetInt64(provider);
        }

        /// <summary>
        /// Gets the value with the specified name as a <see cref="long" />.
        /// </summary>
        /// <param name="store">The store to get the value from.</param>
        /// <param name="name">The name of the value to get.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>
        /// The value with the specified name as <see cref="long" /> or 0 if the value does not exists.
        /// </returns>
        /// <exception cref="ArgumentNullException"><i>store</i> is a null reference (<b>Nothing</b> in Visual Basic).</exception>

        public static long GetInt64(this IValueStore store, string name, IFormatProvider provider)
        {
            return store.GetInt64(name, 0, provider);
        }

        //  ----------------
        //  GetString method
        //  ----------------

        /// <summary>
        /// Gets the value with the specified name as a <see cref="string" />.
        /// </summary>
        /// <param name="store">The store to get the value from.</param>
        /// <param name="name">The name of the value to get.</param>
        /// <returns>
        /// The value with the specified name.
        /// If a value with the specified name does not exists an empty string is returned.
        /// </returns>

        public static string GetString(this IValueStore store, string name)
        {
            return store.Get(name, string.Empty);
        }

        //  ----------------
        //  SetString method
        //  ----------------

        /// <summary>
        /// Sets the <see cref="string"/> value with the specified name.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="name">The name of the value to set.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException"></exception>

        [Obsolete("Use SetValue() instead.")]
        public static void SetString(this IValueStore store, string name, string value)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            store.SetValue(new NamedValue(name, value));
        }

        //  ---------------
        //  SetValue method
        //  ---------------

        /// <summary>
        /// Sets the value with the specified name.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="name">The name of the value to set.</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="ArgumentNullException">
        /// <c>store</c> is a null reference.
        /// </exception>

        public static void SetValue(this IValueStore store, string name, object value)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            store.SetValue(new NamedValue(name, value));
        }
    }

    #endregion ValueStoreInterfaceExtensions class
}

// eof "IValueStore.cs"
