// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigFileParser.cs">
//
//  The MIT License (MIT)
//
//  Copyright (c) 2014 Matt Channer
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgt.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml.Linq;

    using Widgt.Core.Exceptions;
    using Widgt.Core.Model;

    /// <summary>
    /// A parser for widget archives
    /// </summary>
    public class ConfigFileParser
    {
        /// <summary> The namespace for widgets </summary>
        private static readonly XNamespace WidgetNS = "http://www.w3.org/ns/widgets";

        /// <summary>
        /// Parses an input stream to return a new <see cref="Widget"/> instance
        /// </summary>
        /// <param name="configFileStream"> The configuration file input stream. </param>
        /// <returns> The <see cref="Widget"/> represented by the configuration file. </returns>
        /// <exception cref="ArgumentNullException">Raised when the configFile is null</exception>
        /// <exception cref="ConfigFileParseException">General exception raised when the document is malformed</exception>
        public Widget Parse(Stream configFileStream)
        {
            Throwable.ThrowIfNull(configFileStream, "configFileStream");

            try
            {
                return Parse(XDocument.Load(configFileStream));
            }
            catch (InvalidManifestFileException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ConfigFileParseException("An exception occurred trying to read from the input stream", ex);
            }
        }

        /// <summary>
        /// Parses the XML document to return a new <see cref="Widget"/> instance
        /// </summary>
        /// <param name="configFile"> The configuration file. </param>
        /// <returns> The <see cref="Widget"/> represented by the configuration file. </returns>
        /// <exception cref="ArgumentNullException">Raised when the configFile is null</exception>
        /// <exception cref="ConfigFileParseException">General exception raised when the document is malformed</exception>
        public Widget Parse(XDocument configFile)
        {
            Throwable.ThrowIfNull(configFile, "configFile");

            try
            {
                XElement root = configFile.Root;
                if (root == null) throw new InvalidManifestFileException("Empty document");

                Ensure(root, WidgetNS + "widget");

                Widget widget = new Widget();

                widget.WidgetId = AttributeString(root, "id");
                widget.Version = AttributeString(root, "version");
                widget.Height = AttributeInt(root, "height");
                widget.Width = AttributeInt(root, "width");
                widget.DefaultLocale = AttributeString(root, "defaultlocale");

                AttributeStringList(widget.ViewModes, root, "viewmodes");

                ParseNames(root, widget);
                ParseAuthor(root, widget);
                ParseIcons(root, widget);
                ParseContent(root, widget);
                ParseLicense(root, widget);
                ParseDescription(root, widget);
                ParseFeatures(root, widget);
                ParsePreferences(root, widget);
                ParseAccessRequests(root, widget);

                return widget;
            }
            catch (ConfigFileParseException)
            {
                throw;
            }
            catch (Exception inner)
            {
                throw new ConfigFileParseException(
                    "An unknown exception occurred parsing the configuration file",
                    inner);
            }
        }

        /// <summary>
        /// Parses the preferences section of the configuration file
        /// </summary>
        /// <param name="root">The root element to read from</param>
        /// <param name="widget">The widget to write to</param>
        private static void ParsePreferences(XElement root, Widget widget)
        {
            foreach (XElement preferenceEl in root.Elements(WidgetNS + "preference"))
            {
                var preference = new Preference
                {
                    Widget = widget,
                    Name = AttributeString(preferenceEl, "name"),
                    Value = AttributeString(preferenceEl, "value"),
                    Readonly = AttributeString(preferenceEl, "readonly") == "true"
                };

                ReadLanguage(preferenceEl, preference);
                widget.Preferences.Add(preference);
            }
        }

        /// <summary>
        /// Parses the features section of the configuration file
        /// </summary>
        /// <param name="root">The root element to read from</param>
        /// <param name="widget">The widget to write to</param>
        private static void ParseFeatures(XElement root, Widget widget)
        {
            foreach (XElement featureEl in root.Elements(WidgetNS + "feature"))
            {
                Feature feature = new Feature
                {
                    Widget = widget,
                    Required = AttributeString(featureEl, "required") != "false",
                    Name = AttributeString(featureEl, "name")
                };

                ReadLanguage(featureEl, feature);
                ReadFeatureParameters(featureEl, feature);

                widget.Features.Add(feature);
            }
        }

        /// <summary>
        /// Reads the feature parameters from the parent feature node
        /// </summary>
        /// <param name="featureEl">The feature element to read from</param>
        /// <param name="feature">The feature to assign the parameters to</param>
        private static void ReadFeatureParameters(XElement featureEl, Feature feature)
        {
            foreach (XElement paramEl in featureEl.Elements(WidgetNS + "param"))
            {
                var p = new FeatureParameter
                {
                    Feature = feature,
                    Name = AttributeString(paramEl, "name"),
                    Value = AttributeString(paramEl, "value")
                };

                feature.Parameters.Add(p);
            }
        }

        /// <summary>
        /// Parses the description section of the configuration file
        /// </summary>
        /// <param name="root">The root element to read from</param>
        /// <param name="widget">The widget to write to</param>
        private static void ParseDescription(XElement root, Widget widget)
        {
            foreach (XElement descriptionEl in root.Elements(WidgetNS + "description"))
            {
                var description = new Description { Text = descriptionEl.Value.Trim(), Widget = widget };
                ReadLanguage(descriptionEl, description);
                widget.Descriptions.Add(description);
            }
        }

        /// <summary>
        /// Parses the license section of the configuration file
        /// </summary>
        /// <param name="root">The root element to read from</param>
        /// <param name="widget">The widget to write to</param>
        private static void ParseLicense(XElement root, Widget widget)
        {
            foreach (XElement licenseEl in root.Elements(WidgetNS + "license"))
            {
                License license = new License
                {
                    Widget = widget,
                    HRef = AttributeString(licenseEl, "href"),
                    Contents = licenseEl.Value.Trim()
                };

                widget.Licenses.Add(license);
            }
        }

        /// <summary>
        /// Parses the content section of the configuration file
        /// </summary>
        /// <param name="root">The root element to read from</param>
        /// <param name="widget">The widget to write to</param>
        private static void ParseContent(XElement root, Widget widget)
        {
            foreach (XElement contentEl in root.Elements(WidgetNS + "content"))
            {
                var content = new Content
                {
                    Widget = widget,
                    Encoding = AttributeString(contentEl, "encoding"),
                    Src = AttributeString(contentEl, "src"),
                    Type = AttributeString(contentEl, "type")
                };

                // Do not add invalid content
                if (string.IsNullOrEmpty(content.Src)) continue;

                widget.Contents.Add(content);
            }
        }

        /// <summary>
        /// Parses the list of access requests from the configuration file
        /// </summary>
        /// <param name="el">The root element to read from</param>
        /// <param name="widget">The widget to assign values to</param>
        private static void ParseAccessRequests(XElement el, Widget widget)
        {
            foreach (XElement accessElement in el.Elements(WidgetNS + "access"))
            {
                string origin = AttributeString(accessElement, "origin");
                if (string.IsNullOrEmpty(origin)) continue;

                bool subdomains = AttributeString(accessElement, "subdomains") == "true";
                widget.AccessRequests.Add(
                    new AccessRequest { Widget = widget, Origin = origin, Subdomains = subdomains });
            }
        }

        /// <summary>
        /// Parses the widget name element and assigns it to the widget.  The name instance is
        /// not assigned if the node is not present
        /// </summary>
        /// <param name="root">The root element to read from</param>
        /// <param name="widget">The widget to assign the name to</param>
        private static void ParseNames(XElement root, Widget widget)
        {
            foreach (XElement nameElement in root.Elements(WidgetNS + "name"))
            {
                var name = new Name
                {
                    Widget = widget,
                    Short = AttributeString(nameElement, "short"),
                    Value = nameElement.Value.Trim()
                };

                // Skip over empty content
                if (string.IsNullOrEmpty(name.Value) && string.IsNullOrEmpty(name.Short)) continue;

                ReadLanguage(nameElement, name);

                widget.Names.Add(name);
            }
        }

        /// <summary>
        /// Parses the Author element from the supplied root XML element
        /// </summary>
        /// <param name="root">The root element to read from</param>
        /// <param name="widget">The widget to write to</param>
        private static void ParseAuthor(XElement root, Widget widget)
        {
            XElement authorEl = root.Element(WidgetNS + "author");
            if (authorEl != null)
            {
                widget.Author = new Author
                {
                    Widget = widget,
                    Email = AttributeString(authorEl, "email"),
                    HRef = AttributeString(authorEl, "href"),
                    Contents = authorEl.Value.Trim()
                };
            }
        }

        /// <summary>
        /// Parses each icon in the document
        /// </summary>
        /// <param name="root">The root element</param>
        /// <param name="widget">The widget instance</param>
        private static void ParseIcons(XElement root, Widget widget)
        {
            foreach (XElement iconElement in root.Elements(WidgetNS + "icon"))
            {
                var icon = new Icon { Src = AttributeString(iconElement, "src"), Widget = widget };

                // Skip over empty icon element
                if (string.IsNullOrEmpty(icon.Src)) continue;

                ReadLanguage(iconElement, icon);

                widget.Icons.Add(icon);
            }
        }

        /// <summary>
        /// Reads a language attribute and assigns it to the language property on the language
        /// aware element
        /// </summary>
        /// <param name="el">The element to read the attribute from</param>
        /// <param name="languageNode">The language aware node</param>
        private static void ReadLanguage(XElement el, ILanguageAware languageNode)
        {
            languageNode.Language = AttributeString(el, XNamespace.Xml + "lang") ?? string.Empty;
        }

        /// <summary>
        /// Ensures an element is named with the expected name, throwing a <see cref="ConfigFileParseException"/> if not the same
        /// </summary>
        /// <param name="el">The element to verify</param>
        /// <param name="expected">The expected name of the element</param>
        private static void Ensure(XElement el, XName expected)
        {
            if (el.Name != expected)
                throw new InvalidManifestFileException(
                    string.Format("Unexpected element.  Expected {0}, got {1}", expected, el.Name));
        }

        /// <summary>
        /// Splits the value of an attribute based on a comma delimiter, and assigns each value to the
        /// supplied list of strings.
        /// </summary>
        /// <param name="stringList">The string list to append to</param>
        /// <param name="el">The element to read from</param>
        /// <param name="attributeName">The name of the attribute</param>
        private static void AttributeStringList(
            ICollection<string> stringList, XElement el, XName attributeName)
        {
            string attributeValue = AttributeString(el, attributeName);
            if (string.IsNullOrEmpty(attributeValue)) return;

            string[] segments = attributeValue.Trim().Split(new[] { ',' });
            foreach (string segment in segments)
                stringList.Add(segment);
        }

        /// <summary>
        /// Returns the integer value represented by an attribute if present, otherwise the
        /// defaultValue is returned instead.
        /// </summary>
        /// <param name="el">The element to test</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>The result</returns>
        private static int? AttributeInt(XElement el, XName attributeName)
        {
            string attributeString = AttributeString(el, attributeName);
            if (string.IsNullOrEmpty(attributeString)) return null;

            int parsed;
            int? result = null;
            if (int.TryParse(attributeString, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed))
            {
                result = parsed;
            }

            return result;
        }

        /// <summary>
        /// Returns the value of an attribute if present, or an empty string if not
        /// </summary>
        /// <param name="el">The element to look up the attribute on</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>The attribute value</returns>
        private static string AttributeString(XElement el, XName attributeName)
        {
            XAttribute attr = el.Attribute(attributeName);
            return attr == null ? string.Empty : attr.Value;
        }
    }
}
