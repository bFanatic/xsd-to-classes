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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BlueToque.XsdToClasses
{
    /// <summary>
    /// This class presents the Xml serializer and deserializer wrapped using 
    /// generic methods that introduce strong type checking.
    /// 
    /// This class also caches the serializers that were created so they can be re-used
    /// </summary>
    public static class Serializer
    {
        class ExceptionManager
        {
            public static bool CriticalException(Exception ex)
            {
                return true;
            }
        }
        #region fields
        private static Dictionary<Type, XmlSerializer> m_serializers = new Dictionary<Type, XmlSerializer>();
        private static bool m_cacheSerializers = true;
        #endregion

        /// <summary>
        /// A flag indicating weather to cache the serializers.
        /// Serializers are cached by default. This means that any calls to
        /// Serialize or Deserialize will create a class, and this class will 
        /// be saved in a dictionary so that further calls to serialize or 
        /// deserialize the class will be more efficient
        /// </summary>
        public static bool CacheSerializers
        {
            get { return Serializer.m_cacheSerializers; }
            set { Serializer.m_cacheSerializers = value; }
        }

        #region Deserialize
        /// <summary>
        /// Serialize an object of the given type to a string
        /// </summary>
        /// <param name="type">Type to Deserialize</param>
        /// <param name="reader">XmlReader to use</param>
        /// <returns>string containing the serialized obejct</returns>
        public static object Deserialize(Type type, XmlReader reader)
        {
            try
            {
                Trace.Assert(type != null);
                XmlSerializer serializer = CreateSerializer(type);
                return serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                if (ExceptionManager.CriticalException(ex))
                    throw;
            }
            return null;
        }


        /// <summary>
        /// This method deserializes the contents ot a file into the object of the given type
        /// This method is used in preference to the base XmlSerializers because it's a generic 
        /// version that introduces strong type checking, and caches the serializers.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="fileName">file name to read</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeFromFile<T>(string fileName) where T : class
        {
            // using the type given as a parameter, create a deserializer and 
            // load the XML from the file reference
            T data = default(T);
            try
            {
                XmlSerializer ser = CreateSerializer(typeof(T));

                using (XmlTextReader reader = new XmlTextReader(fileName))
                    data = ser.Deserialize(reader) as T;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                //if (ExceptionManager.CriticalException(ex))
                    throw;
            }

            // return the configuration
            return data;
        }

        /// <summary>
        /// Deserialize an instance of type T from the given xml string
        /// </summary>
        /// <typeparam name="T">the type to deserialize</typeparam>
        /// <param name="xmlString">the xml string</param>
        /// <returns>an instance of the type or NULL if any errors in the XML</returns>
        public static T Deserialize<T>(string xmlString) where T : class
        {
            return Deserialize<T>(xmlString, null);
        }

        /// <summary>
        /// Deserialize an instance of type T from the given xml string, and using 
        /// supporting types.
        /// Caches the serializer so that further instances of this
        /// call will be more efficient.
        /// </summary>
        /// <typeparam name="T">the type to deserialize</typeparam>
        /// <param name="xmlString">the xml string</param>
        /// <param name="types">additional types to use for the deserialization</param>
        /// <returns></returns>
        public static T Deserialize<T>(string xmlString, Type[] types) where T : class
        {
            // using the type given as a parameter, create a deserializer and 
            // load the XML from the file reference
            T data = default(T);
            StringReader sr = new StringReader(xmlString);
            try
            {
                XmlSerializer ser = CreateSerializer(typeof(T), types);
                using (XmlTextReader reader = new XmlTextReader(sr))
                    data = ser.Deserialize(reader) as T;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception when trying to deserialize from file: {0}", ex.ToString());
                //if (ExceptionManager.CriticalException(ex))
                    throw;
            }

            // return the configuration
            return data;
        }

        /// <summary>
        /// Deserialize an instance of type T from the given XmlElement
        /// Caches the serializer so that further instances of this
        /// call will be more efficient.
        /// </summary>
        /// <typeparam name="T">the type to deserialize</typeparam>
        /// <param name="element">an XmlElement that contains the node to deserialize</param>
        /// <returns>an instance of T deserialized from the XmlElement</returns>
        public static T Deserialize<T>(XmlElement element) where T : class
        {
            return Deserialize<T>(element.InnerXml);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">the type to deserialize</typeparam>
        /// <param name="element">an XmlElement that contains the node to deserialize</param>
        /// <param name="types">additional types to use for the deserialization</param>
        /// <returns>an instance of T deserialized from the XmlElement</returns>
        public static T Deserialize<T>(XmlElement element, Type[] types) where T : class
        {
            return Deserialize<T>(element.InnerXml, types);
        }
        #endregion

        #region Serialize

        public static void Serialize(XmlWriter writer, object obj)
        {
            try
            {
                Trace.Assert(obj != null);
                XmlSerializer serializer = CreateSerializer(obj.GetType());
                serializer.Serialize(writer, obj);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                //if (ExceptionManager.CriticalException(ex))
                    throw;
            }
        }

        /// <summary>
        /// Serialize an object of the given type to a string
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>string containing the serialized obejct</returns>
        public static string Serialize(object obj)
        {
            try
            {
                Trace.Assert(obj != null);
                XmlSerializer serializer = CreateSerializer(obj.GetType());
                StringWriter sw = new StringWriter();
                serializer.Serialize(sw, obj);
                return sw.ToString();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                if (ExceptionManager.CriticalException(ex))
                    throw;
            }
            return null;
        }

        /// <summary>
        /// Serialize the contents of a class into a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>string containing the serialized object</returns>
        public static string Serialize<T>(T obj) where T : class
        {
            return Serialize<T>(obj, null, null);
        }

        /// <summary>
        /// Serialize the object to the given file
        /// </summary>
        /// <typeparam name="T">type of object to serialize</typeparam>
        /// <param name="obj">the object to serialize</param>
        /// <param name="fileName">file to serialize to</param>
        public static void SerializeToFile<T>(T obj, string fileName) where T : class
        {
            SerializeToFile<T>(obj, fileName, null);
        }

        public static void SerializeToFile<T>(T obj, string fileName, XmlWriterSettings settings) where T : class
        {
            try
            {
                XmlSerializer serializer = CreateSerializer(typeof(T));

                if (settings == null)
                    settings = DefaultXmlWriterSettings();

                using (XmlWriter writer = XmlWriter.Create(fileName, settings))
                    serializer.Serialize(writer, obj);

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                //if (ExceptionManager.CriticalException(ex))
                    throw;
            }

        }

        /// <summary>
        /// Serialize the object into a string.
        /// </summary>
        /// <typeparam name="T">type of object to serialize</typeparam>
        /// <param name="obj">the object to serialize</param>
        /// <param name="settings">XmlWriter settings to use for writing the xml</param>
        /// <returns>xml file of serialized object, formatted using XmlWriterSettings</returns>
        public static string Serialize<T>(T obj, XmlWriterSettings settings) where T : class
        {
            return Serialize<T>(obj, null, settings);
        }

        /// <summary>
        /// Serialize the object into an XmlElement with the given name.
        /// </summary>
        /// <typeparam name="T">type of object to serialize</typeparam>
        /// <param name="obj">the object to serialize</param>
        /// <param name="elementName">the name of the XmlElement to produce</param>
        /// <returns>XmlElement containing the serialized object</returns>
        public static XmlElement SerializeToElement<T>(T obj, string elementName) where T : class
        {
            return SerializeToElement<T>(obj, null, elementName);
        }

        /// <summary>
        /// Serialize the object into an XmlElement with the given name.
        /// </summary>
        /// <typeparam name="T">type of object to serialize</typeparam>
        /// <param name="obj">the object to serialize</param>
        /// <param name="types">additional types</param>
        /// <param name="elementName">the name of the XmlElement to produce</param>
        /// <returns>XmlElement containing the serialized object</returns>
        public static XmlElement SerializeToElement<T>(T obj, Type[] types, string elementName) where T : class
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.CloseOutput = true;
                settings.Indent = true;

                string xmlString = Serialize<T>(obj, types, settings);

                XmlDocument doc = new XmlDocument();
                XmlElement element = doc.CreateElement(elementName);
                element.InnerXml = xmlString;

                return element;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                if (ExceptionManager.CriticalException(ex))
                    throw;
            }
            return null;
        }

        #endregion

        #region private methods

        private static XmlWriterSettings DefaultXmlWriterSettings()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.Unicode;
            settings.NewLineChars = Environment.NewLine;
            settings.NewLineOnAttributes = true;
            settings.NewLineHandling = NewLineHandling.Replace;
            settings.Indent = true;
            return settings;
        }

        /// <summary>
        /// Serialize the object into a string.
        /// </summary>
        /// <typeparam name="T">type of object to serialize</typeparam>
        /// <param name="obj">the object to serialize</param>
        /// <param name="types">additional types</param>
        /// <param name="settings">XmlWriter settings to use for writing the xml</param>
        /// <returns>xml file of serialized object, formatted using XmlWriterSettings</returns>
        private static string Serialize<T>(object obj, Type[] types, XmlWriterSettings settings)
        {
            if (obj == null)
                throw new ArgumentNullException("Object is null when trying to serialize");

            StringWriter sw = new StringWriter();

            try
            {
                XmlSerializer ser = CreateSerializer(typeof(T), types);

                if (settings == null)
                    settings = DefaultXmlWriterSettings();

                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                    ser.Serialize(writer, obj);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                //if (ExceptionManager.CriticalException(ex))
                    throw;
            }

            return sw.ToString();
        }

        #endregion

        #region Serializer cache

        /// <summary>
        /// Cache serializers that do not use the default constructor in a hashtable
        /// There can be a memory leak when using any serializer that does not use the 
        /// default constructor new XmlSerializer(type);
        /// to fix this leak, create a hash table, and find a way to get a unique ID
        /// to look up the serializer if you need it again, otherwise every time you
        /// serialize using the "new XmlSerializer(typeof(T), types);" it will create a 
        /// new class/assembly that never gets unloaded.
        /// This solution caches all serializers since we use them a lot.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static XmlSerializer CreateSerializer(Type type) { return CreateSerializer(type, null); }

        /// <summary>
        /// Create a serializer with the given supporting types
        /// </summary>
        /// <param name="type">the type to create the serializer from</param>
        /// <param name="types">the supporting types to use when constructing the serializer</param>
        /// <returns>an XmlSerializer</returns>
        static XmlSerializer CreateSerializer(Type type, Type[] types)
        {
            XmlSerializer ser;
            if (m_serializers.ContainsKey(type))
            {
                Trace.TraceInformation("Retrieving serializer {0} from cache.", type.FullName);
                ser = m_serializers[type];
            }
            else
            {
                Trace.TraceInformation("Creating serializer {0}.", type.FullName);
                ser = (types == null || types.Length == 0) ?
                    new XmlSerializer(type) :
                    new XmlSerializer(type, types);

                // if we're configured to cache the serializer, then store it in the table
                if (Serializer.CacheSerializers)
                {
                    Trace.TraceInformation("Adding serializer {0} to cache.", type.FullName);
                    m_serializers.Add(type, ser);
                }
            }
            return ser;
        }

        #endregion
    }
}
