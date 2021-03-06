﻿/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar (https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World. If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using System.Windows.Forms;

namespace BriefingRoom4DCSWorld
{
    /// <summary>
    /// Stores the various paths to the files used by BriefingRoom.
    /// </summary>
    public static class BRPaths
    {
        /// <summary>
        /// Path to the application.
        /// </summary>
        public static string ROOT { get; } = Toolbox.NormalizeDirectoryPath(Application.StartupPath);

        /// <summary>
        /// Path to the database subdirectory.
        /// </summary>
        public static string DATABASE { get; } = $"{ROOT}Database\\";

#if DEBUG
        /// <summary>
        /// Path to the debug output directory
        /// </summary>
        public static string DEBUGOUTPUT { get; } = $"{ROOT}DebugOutput\\";
#endif

        /// <summary>
        /// Path to the Include subdirectory.
        /// </summary>
        public static string INCLUDE { get; } = $"{ROOT}Include\\";

        /// <summary>
        /// Path to the Include\Lua subdirectory.
        /// </summary>
        public static string INCLUDE_LUA { get; } = $"{INCLUDE}Lua\\";

        /// <summary>
        /// Path to the Include\Lua\IncludedScripts subdirectory.
        /// </summary>
        public static string INCLUDE_LUA_INCLUDEDSCRIPTS { get; } = $"{INCLUDE_LUA}IncludedScripts\\";

        /// <summary>
        /// Path to the Include\Lua\Mission subdirectory.
        /// </summary>
        public static string INCLUDE_LUA_MISSION { get; } = $"{INCLUDE_LUA}Mission\\";

        /// <summary>
        /// Path to the Include\Lua\Units subdirectory.
        /// </summary>
        public static string INCLUDE_LUA_UNITS { get; } = $"{INCLUDE_LUA}Units\\";
        
        /// <summary>
        /// Path to the Include\Lua subdirectory.
        /// </summary>
        public static string INCLUDE_OGG { get; } = $"{INCLUDE}Ogg\\";

        /// <summary>
        /// Path to the Media subdirectory.
        /// </summary>
        public static string MEDIA { get; } = $"{ROOT}Media\\";

        /// <summary>
        /// Path to the Templates subdirectory.
        /// </summary>
        public static string TEMPLATES { get; } = $"{ROOT}Templates\\";
    }
}
