﻿// <copyright file="Extensions.cs" company="FC">
// Copyright (c) 2011 Fraser Chapman
// </copyright>
// <author>Fraser Chapman</author>
// <email>fraser.chapman@gmail.com</email>
// <date>2011-03-06</date>
// <summary>This file is part of FC.GEPluginCtrls
// FC.GEPluginCtrls is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.
// </summary>
namespace FC.GEPluginCtrls
{
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    /// Extension helper methods for the control libray
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Gets the internal ID for a Layer
        /// </summary>
        /// <param name="input">Layer type</param>
        /// <returns>The layer ID or an empty string</returns>
        internal static string GetId(this Layer input)
        {
            FieldInfo fi = input.GetType().GetField(input.ToString());
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            return string.Empty;
        }
    }
}