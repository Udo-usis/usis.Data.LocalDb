﻿//
//  @(#) DataSourceApplicationExtension.cs
//
//  Project:    usis Framework
//  System:     Microsoft Visual Studio 2015
//  Author:     Udo Schäfer
//
//  Copyright (c) 2016,2017 usis GmbH. All rights reserved.

#if !WINDOWS_UWP
using System.Configuration;
#endif
using usis.Platform;
using usis.Platform.Data;

namespace usis.Framework.Data
{
    //  ------------------------------------
    //  DataSourceApplicationExtension class
    //  ------------------------------------

    /// <summary>
    /// Provides an application extension to access a single data source.
    /// </summary>

    public class DataSourceApplicationExtension : Extension<IApplication>
    {
		#region construction

		//  ------------
		//  construction
		//  ------------

#if !__IOS__ && !WINDOWS_UWP && !__MAC__

		/// <summary>
		/// Initializes a new instance of the <see cref="DataSourceApplicationExtension"/> class
		/// with the specified connection string.
		/// </summary>
		/// <param name="connectionStringName">Name of the connection string in the application configuration file.</param>

		public DataSourceApplicationExtension(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionString != null) DataSource = FromConnectionString(connectionString);
        }

#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceApplicationExtension"/> class
        /// with the specified data source
        /// </summary>
        /// <param name="dataSource">The data source.</param>

        public DataSourceApplicationExtension(DataSource dataSource) { DataSource = dataSource; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceApplicationExtension"/> class
        /// with the specified value store.
        /// </summary>
        /// <param name="valueStore">The value store that provides the data sources properties.</param>

        public DataSourceApplicationExtension(IValueStorage valueStore) { DataSource = new DataSource(valueStore); }

		#endregion construction

		#region methods

#if !__IOS__ && !WINDOWS_UWP && !__MAC__

		//  ---------------------------
		//  FromConnectionString method
		//  ---------------------------

		internal static DataSource FromConnectionString(ConnectionStringSettings connectionString)
        {
            var dataSource = new DataSource(connectionString.ProviderName)
            {
                ConnectionString = connectionString.ConnectionString
            };
            return dataSource;
        }
#endif
        #endregion methods

        #region properties

        //  -------------------
        //  DataSource property
        //  -------------------

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>
        /// The data source.
        /// </value>

        public DataSource DataSource { get; }

        #endregion properties
    }
}

// eof "DataSourceApplicationExtension.cs"
