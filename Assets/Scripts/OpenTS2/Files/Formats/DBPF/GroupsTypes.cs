﻿/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at
 * http://mozilla.org/MPL/2.0/. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTS2.Files.Formats.DBPF
{
    //Could be enums, but I kinda like this better.
    public static class TypeIDs
    {
        public const uint STR = 0x53545223;
        public const uint IMG = 0x856DDBAC;
        public const uint IMG2 = 0x8C3CE95A;
        public const uint DIR = 0xE86B1EEF;
        public const uint EFFECT = 0xEA5118B0;
        /// <summary>
        /// Can be XA or MP3.
        /// </summary>
        public const uint MP3 = 0x2026960B;
        public const uint OBJD = 0x4F424A44;
        /// dynamiclinklibrary CRC32 hash
        public const uint DLL = 0x7582DEC6;
        public const uint CTSS = 0x43545353;
    }
    public static class GroupIDs
    {
        /// <summary>
        /// Local package file group - gets converted to a hash of the package filename.
        /// </summary>
        public const uint Local = 0xFFFFFFFF;
        public const uint DIR = 0xE86B1EEF;
        public const uint Scenegraph = 0x1C050000;
        public const uint Pop = 0xFF23C3BF;
    }
}
