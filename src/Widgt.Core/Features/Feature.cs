// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Feature.cs">
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

namespace Widgt.Core.Features
{
    using System.Collections.Generic;
    using System.IO;

    using Widgt.Features.Model;

    /// <summary>
    /// Represents a deployed feature on the server
    /// </summary>
    public class Feature
    {
        /// <summary> The manifest file for the feature </summary>
        private readonly FileInfo featureFile;

        /// <summary> A checksum used for determining whether the file has changed </summary>
        private readonly string checksum;

        /// <summary>
        /// Initializes a new instance of the <see cref="Feature"/> class.
        /// </summary>
        /// <param name="featureFile"> The feature file. </param>
        /// <param name="checksum"> The checksum value for the supplied file. </param>
        public Feature(FileInfo featureFile, string checksum)
        {
            this.checksum = checksum;
            this.featureFile = featureFile;
            this.Includes = new List<FeatureInclude>();
        }

        /// <summary> Gets the checksum for the feature file </summary>
        public string Checksum
        {
            get
            {
                return checksum;
            }
        }

        /// <summary>
        /// Gets the path to the feature.xml file represented by this feature instance
        /// </summary>
        public FileInfo FeatureFile
        {
            get
            {
                return featureFile;
            }
        }

        /// <summary>
        /// Gets the directory 
        /// </summary>
        public DirectoryInfo FeatureDirectory
        {
            get
            {
                return this.FeatureFile.Directory;
            }
        }

        /// <summary>
        /// Gets or sets the Id of the feature
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the list of resources to include
        /// </summary>
        public IList<FeatureInclude> Includes { get; private set; }
    }
}
