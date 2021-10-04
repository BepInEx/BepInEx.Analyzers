﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BepInEx.Analyzers {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BepInEx.Analyzers.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Accessing a member that was not originally public.
        /// </summary>
        internal static string AccessPublicizedMemberAnalyzerDescription {
            get {
                return ResourceManager.GetString("AccessPublicizedMemberAnalyzerDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Forced access of {0} can lead to unintended behavior. Check for public members first..
        /// </summary>
        internal static string AccessPublicizedMemberAnalyzerMessageFormat {
            get {
                return ResourceManager.GetString("AccessPublicizedMemberAnalyzerMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Accessing a member that was not originally public.
        /// </summary>
        internal static string AccessPublicizedMemberAnalyzerTitle {
            get {
                return ResourceManager.GetString("AccessPublicizedMemberAnalyzerTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Classes inheriting from BaseUnityPlugin should have a BepInPlugin attribute..
        /// </summary>
        internal static string BepInExMissingAttributeAnalyzerDescription {
            get {
                return ResourceManager.GetString("BepInExMissingAttributeAnalyzerDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Class {0} inheriting from BaseUnityPlugin should have a BepInPlugin attribute.
        /// </summary>
        internal static string BepInExMissingAttributeAnalyzerMessageFormat {
            get {
                return ResourceManager.GetString("BepInExMissingAttributeAnalyzerMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Class inheriting from BaseUnityPlugin missing BepInPlugin attribute.
        /// </summary>
        internal static string BepInExMissingAttributeAnalyzerTitle {
            get {
                return ResourceManager.GetString("BepInExMissingAttributeAnalyzerTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Classes with BepInPlugin attribute must inherit from BaseUnityPlugin..
        /// </summary>
        internal static string BepInExMissingInheritanceAnalyzerDescription {
            get {
                return ResourceManager.GetString("BepInExMissingInheritanceAnalyzerDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Class {0} has BepInPlugin attribute but does not inherit from BaseUnityPlugin.
        /// </summary>
        internal static string BepInExMissingInheritanceAnalyzerMessageFormat {
            get {
                return ResourceManager.GetString("BepInExMissingInheritanceAnalyzerMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Classes with BepInPlugin attribute must inherit from BaseUnityPlugin.
        /// </summary>
        internal static string BepInExMissingInheritanceAnalyzerTitle {
            get {
                return ResourceManager.GetString("BepInExMissingInheritanceAnalyzerTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Harmony non-ref patch parameter modified.
        /// </summary>
        internal static string HarmonyMethodRefParametersAnalyzerDescription {
            get {
                return ResourceManager.GetString("HarmonyMethodRefParametersAnalyzerDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Harmony non-ref patch parameter {0} modified.
        /// </summary>
        internal static string HarmonyMethodRefParametersAnalyzerMessageFormat {
            get {
                return ResourceManager.GetString("HarmonyMethodRefParametersAnalyzerMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Harmony non-ref patch parameters modified.
        /// </summary>
        internal static string HarmonyMethodRefParametersAnalyzerTitle {
            get {
                return ResourceManager.GetString("HarmonyMethodRefParametersAnalyzerTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Don&apos;t flag private methods with HarmonyPatch attribute as unused..
        /// </summary>
        internal static string HarmonyPrivateMethodSuppressorJustification {
            get {
                return ResourceManager.GetString("HarmonyPrivateMethodSuppressorJustification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Patching properties by patching get_ or set_ is not recommended.
        /// </summary>
        internal static string HarmonyPropertyPatchAnalyzerDescription {
            get {
                return ResourceManager.GetString("HarmonyPropertyPatchAnalyzerDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Harmony patch should use MethodType.Getter or MethodType.Setter instead of get_ or set_ when patching properties.
        /// </summary>
        internal static string HarmonyPropertyPatchAnalyzerMessageFormat {
            get {
                return ResourceManager.GetString("HarmonyPropertyPatchAnalyzerMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Patching properties by patching get_ or set_ is not recommended.
        /// </summary>
        internal static string HarmonyPropertyPatchAnalyzerTitle {
            get {
                return ResourceManager.GetString("HarmonyPropertyPatchAnalyzerTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Harmony patches must have a &apos;static&apos; member modifier.
        /// </summary>
        internal static string HarmonyStaticMethodAnalyzerDescription {
            get {
                return ResourceManager.GetString("HarmonyStaticMethodAnalyzerDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Harmony patch method {0} must have a &apos;static&apos; member modifier.
        /// </summary>
        internal static string HarmonyStaticMethodAnalyzerMessageFormat {
            get {
                return ResourceManager.GetString("HarmonyStaticMethodAnalyzerMessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Harmony patch missing &apos;static&apos; member modifier.
        /// </summary>
        internal static string HarmonyStaticMethodAnalyzerTitle {
            get {
                return ResourceManager.GetString("HarmonyStaticMethodAnalyzerTitle", resourceCulture);
            }
        }
    }
}
