﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace usis.ApplicationServer.Administration {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("usis.ApplicationServer.Administration.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Name.
        /// </summary>
        internal static string ApplicationListColumnName {
            get {
                return ResourceManager.GetString("ApplicationListColumnName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Status.
        /// </summary>
        internal static string ApplicationListColumnStatus {
            get {
                return ResourceManager.GetString("ApplicationListColumnStatus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Applications.
        /// </summary>
        internal static string Applications {
            get {
                return ResourceManager.GetString("Applications", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Applications.
        /// </summary>
        internal static string ApplicationsNode {
            get {
                return ResourceManager.GetString("ApplicationsNode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pause.
        /// </summary>
        internal static string PauseApplicationAction {
            get {
                return ResourceManager.GetString("PauseApplicationAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pauses all operations performed by the application and its snap-ins..
        /// </summary>
        internal static string PauseApplicationDescription {
            get {
                return ResourceManager.GetString("PauseApplicationDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resume.
        /// </summary>
        internal static string ResumeApplicationAction {
            get {
                return ResourceManager.GetString("ResumeApplicationAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resumes all operations performed by the application and its snap-ins..
        /// </summary>
        internal static string ResumeApplicationDescription {
            get {
                return ResourceManager.GetString("ResumeApplicationDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Start.
        /// </summary>
        internal static string StartApplicationAction {
            get {
                return ResourceManager.GetString("StartApplicationAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Starts the selected application..
        /// </summary>
        internal static string StartApplicationDescription {
            get {
                return ResourceManager.GetString("StartApplicationDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stop.
        /// </summary>
        internal static string StopApplicationAction {
            get {
                return ResourceManager.GetString("StopApplicationAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stops the selected application..
        /// </summary>
        internal static string StopApplicationDescription {
            get {
                return ResourceManager.GetString("StopApplicationDescription", resourceCulture);
            }
        }
    }
}
