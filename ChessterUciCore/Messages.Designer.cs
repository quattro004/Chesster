﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChessterUciCore {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///    A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        internal Messages() {
        }
        
        /// <summary>
        ///    Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ChessterUciCore.Messages", typeof(Messages).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///    Overrides the current thread's CurrentUICulture property for all
        ///    resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to The chess engine didn&apos;t respond with the uciok command during initialization within the specified time period..
        /// </summary>
        public static string ChessEngineDidntInitialize {
            get {
                return ResourceManager.GetString("ChessEngineDidntInitialize", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to The chess engine is not currently running..
        /// </summary>
        public static string ChessEngineNotRunning {
            get {
                return ResourceManager.GetString("ChessEngineNotRunning", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to The chess engine executable path was not supplied..
        /// </summary>
        public static string ChessEnginePathNotSupplied {
            get {
                return ResourceManager.GetString("ChessEnginePathNotSupplied", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Specify a time period greater than zero..
        /// </summary>
        public static string InvalidTimePeriod {
            get {
                return ResourceManager.GetString("InvalidTimePeriod", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to The command cannot be null..
        /// </summary>
        public static string NullCommand {
            get {
                return ResourceManager.GetString("NullCommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to The engine controller cannot be null..
        /// </summary>
        public static string NullEngineController {
            get {
                return ResourceManager.GetString("NullEngineController", resourceCulture);
            }
        }
    }
}