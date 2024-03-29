//=============================================================================
//
// Copyright (C) 2007 Michael Coyle, Blue Toque
// http://www.BlueToque.ca/Products/XsdToClasses.html
// michael.coyle@BlueToque.ca
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// http://www.gnu.org/licenses/gpl.txt
//
//=============================================================================
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using BlueToque.XsdToClasses.Properties;
using EnvDTE;
using Microsoft.Win32;
using VSLangProj;

namespace BlueToque.XsdToClasses
{
	/// <summary>
	///     Base class for all custom tools.
	/// </summary>
	/// <remarks>
	///     Inheriting classes must provide a <see cref="GuidAttribute"/>, static 
	///     methods with <see cref="ComRegisterFunctionAttribute"/> 
    ///     and <see cref="ComUnregisterFunctionAttribute"/>, 
	///     which should call this class <see cref="Register"/> and <see cref="UnRegister"/> 
	///     methods, passing the required parameters.
	/// </remarks>
	public abstract class CustomTool : VisualStudio.BaseCodeGeneratorWithSite
	{
		#region Constants

		/// <summary>
		/// {0}=VsVersion.Major
        /// {1}=VsVersion.Minor
        /// {2}=CategoryGuid
        /// {3}=CustomTool
		/// </summary>
		const string RegistryKey = @"SOFTWARE\Microsoft\{4}\{0}.{1}\Generators\{2}\{3}";

		/// <summary>
		/// {0}=Custom Tool Name
		/// {1}=Tool version
		/// {2}=.NET Runtime version
        /// {3}=current datetime
		/// </summary>
		const string TemplateAutogenerated = 
@"//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by the {0} tool.
//     Tool Version:    {1}
//     Runtime Version: {2}
//     Generated on:    {3}
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------
";

		#endregion Constants

        /// <summary>
        ///     This is the core of the code.
        ///     This gets called to generate the code from the source file name
        /// </summary>
        /// <param name="inputFileName"></param>
        /// <param name="inputFileContent"></param>
        /// <returns></returns>
		protected override sealed byte[] GenerateCode(string inputFileName, string inputFileContent)
		{
			try 
			{
				string code = OnGenerateCode(inputFileContent, inputFileContent);

				return ConvertStringToBytes(code);

			}
			catch (Exception ex)
			{
				if (ex is TargetInvocationException)
				{
					ex = ex.InnerException;
				}

                return ConvertStringToBytes(
                    string.Format(  CultureInfo.CurrentCulture,
					                Resources.CustomTool_GeneralError, 
                                    ex));
			}
		}

		private byte[] ConvertStringToBytes(string code)
		{
			return Encoding.UTF8.GetBytes(code);
		}

		protected abstract string OnGenerateCode(string inputFileName, string inputFileContent);

		public static string GetToolGeneratedCodeWarning(Type customToolType)
		{
			CustomToolAttribute attribute = (CustomToolAttribute)
                Attribute.GetCustomAttribute(   
                    customToolType, 
                    typeof(CustomToolAttribute), 
                    true);

			if (attribute == null)
			{
				throw new ArgumentException(
                    string.Format(
					    CultureInfo.CurrentCulture,
					    Properties.Resources.CustomTool_ToolRequiredAttributeMissing,
					    customToolType, 
                        typeof(CustomToolAttribute)));
			}

			return string.Format(
                TemplateAutogenerated,
				attribute.Name,
				ThisAssembly.P.Version,
				Environment.Version,
                DateTime.Now);
		}

		private static object CustomToolAttribute(CustomToolAttribute customToolAttribute)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#region Properties

		/// <summary>
        ///     Provides access to the current project item selected.
        /// </summary>
		protected ProjectItem CurrentItem
		{
			get 
			{	
				return base.GetService(typeof(ProjectItem))  as ProjectItem;
			}
		} 

		/// <summary>
        ///     Provides access to the current project item selected.
        /// </summary>
		protected VSProject CurrentProject
		{
			get 
			{
				if (CurrentItem != null)
                    return CurrentItem.ContainingProject.Object as VSProject;
			
				return null;
			}
		}

        /// <summary>
        /// Provides access to the current project
        /// </summary>
        protected Project Project
        {
            get
            {
                if (CurrentItem != null)
                    return CurrentItem.ContainingProject;

                return null;
            }
        }
		#endregion Properties

		#region Service access

		/// <summary>
		///     Provides access to services.
		/// </summary>
		/// <param name="serviceType">Service to retrieve.</param>
		/// <returns>The service object or null.</returns>
		protected override object GetService(Type serviceType)
		{
			object svc = base.GetService(serviceType);
			
            // Try the root environment.
			if (svc == null && CurrentItem == null) return null;

			VisualStudio.IOleServiceProvider ole = CurrentItem.DTE as VisualStudio.IOleServiceProvider;
			if (ole != null) 
				return new VisualStudio.ServiceProvider(ole).GetService(serviceType);

			return null;
		}

		#endregion Service access

		#region Registration and Installation

		/// <summary>
		///     Registers the custom tool.
		/// </summary>
        public static void Register(Type type)
        {
            Guid generator;
            CustomToolAttribute tool;
            VersionSupportAttribute[] versions;
            CategorySupportAttribute[] categories;

            GetAttributes(type, out generator, out tool, out versions, out categories);

            foreach (VersionSupportAttribute version in versions)
            {
                foreach (CategorySupportAttribute category in categories)
                {
                    RegisterCustomTool(
                        generator,
                        category.Guid,
                        version.Version,
                        tool.Description,
                        tool.Name,
                        tool.GeneratesDesignTimeCode, 
                        "VisualStudio");

                    RegisterCustomTool(
                        generator,
                        category.Guid,
                        version.Version,
                        tool.Description,
                        tool.Name,
                        tool.GeneratesDesignTimeCode, 
                        "VCSExpress");
                }
            }

        }

		/// <summary>
		///     Unregisters the custom tool.
		/// </summary>
		public static void UnRegister(Type type) 
		{
			Guid generator;
			CustomToolAttribute tool;
			VersionSupportAttribute[] versions;
			CategorySupportAttribute[] categories;
			
            GetAttributes(type, out generator, out tool, out versions, out categories);

			foreach (VersionSupportAttribute version in versions)
			{
				foreach (CategorySupportAttribute category in categories)
				{
					UnRegisterCustomTool(category.Guid, version.Version, tool.Name, "VisualStudio");
                    UnRegisterCustomTool(category.Guid, version.Version, tool.Name, "VCSExpress");
                       
				}
			}
		}

		#endregion Registration and Installation

		#region Helper methods

		/// <summary>
		///     Registers the custom tool.
		/// </summary>
		private static void RegisterCustomTool(
            Guid generator, 
            Guid category, 
            Version vsVersion, 
			string description, 
            string toolName, 
            bool generatesDesignTimeCode,
            string productName)
		{
            /*
             * [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\[productName]\[vsVersion]\Generators\[category]\[toolName]]
             * @="[description]"
             * "CLSID"="[category]"
             * "GeneratesDesignTimeSource"=[generatesDesignTimeCode]
             */
            string keypath =  String.Format(
                RegistryKey, 
                vsVersion.Major, 
                vsVersion.Minor, 
				category.ToString("B"), 
                toolName,
                productName);

			using(RegistryKey key = Registry.LocalMachine.CreateSubKey(keypath)) 
			{
				key.SetValue("", description);
				key.SetValue("CLSID", generator.ToString("B"));
				key.SetValue("GeneratesDesignTimeSource", generatesDesignTimeCode ? 1 : 0);
			}

		}

		/// <summary>
		///     Unregisters the custom tool.
		/// </summary>
		private static void UnRegisterCustomTool(Guid category, Version vsVersion, string toolName, string productName)
		{
			string key = String.Format(
                RegistryKey,
                vsVersion.Major,
                vsVersion.Minor,
                category.ToString("B"),
                toolName,
                productName);
            Registry.LocalMachine.DeleteSubKey(key, false);
		}

        /// <summary>
        ///     Get the attribites from the class/type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="generator"></param>
        /// <param name="tool"></param>
        /// <param name="versions"></param>
        /// <param name="categories"></param>
		private static void GetAttributes(
            Type type, 
            out Guid generator, 
            out CustomToolAttribute tool,
			out VersionSupportAttribute[] versions, 
            out CategorySupportAttribute[] categories)
		{
			object[] attrs;

			// Retrieve the GUID associated with the generator class.
			attrs = type.GetCustomAttributes(typeof(GuidAttribute), false);
            if (attrs.Length == 0)
            {
                throw new ArgumentException(String.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.Tool_AttributeMissing,
                            type, typeof(GuidAttribute)));
            }
			generator = new Guid(((GuidAttribute)attrs[0]).Value);

			// Retrieve the custom tool information.
			attrs = type.GetCustomAttributes(typeof(CustomToolAttribute), false);
            if (attrs.Length == 0)
            {
                throw new ArgumentException(String.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.Tool_AttributeMissing,
                            type, typeof(CustomToolAttribute)));
            }
			tool = (CustomToolAttribute) attrs[0];

			// Retrieve the VS.NET versions supported. Can be inherited.
			attrs = type.GetCustomAttributes(typeof(VersionSupportAttribute), true);
            if (attrs.Length == 0)
            {
                throw new ArgumentException(String.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.Tool_AttributeMissing,
                            type, typeof(VersionSupportAttribute)));
            }
			versions = (VersionSupportAttribute[]) attrs;

			// Retrieve the VS.NET generator categories supported. Can be inherited.
			attrs = type.GetCustomAttributes(typeof(CategorySupportAttribute), true);
            if (attrs.Length == 0)
            {
                throw new ArgumentException(String.Format(
                            CultureInfo.CurrentCulture,
                            Properties.Resources.Tool_AttributeMissing,
                            type, typeof(CategorySupportAttribute)));
            }
			categories = (CategorySupportAttribute[]) attrs;
		}

		#endregion Helper methods

	}
}