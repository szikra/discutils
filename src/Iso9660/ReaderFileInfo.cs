﻿//
// Copyright (c) 2008, Kenneth Bell
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;

namespace DiscUtils.Iso9660
{
    internal class ReaderFileInfo : DiscFileInfo
    {
        private CDReader _reader;
        private DirectoryRecord _record;
        private ReaderDirectoryInfo _parent;

        public ReaderFileInfo(CDReader reader, ReaderDirectoryInfo parent, DirectoryRecord record)
        {
            _reader = reader;
            _record = record;
            _parent = parent;
        }

        public override string Name
        {
            get { return _record.FileIdentifier; }
        }

        public override string FullName
        {
            get { return _parent.FullName + Name; }
        }

        public override FileAttributes Attributes
        {
            get
            {
                FileAttributes attrs = FileAttributes.ReadOnly;
                if ((_record.Flags & FileFlags.Hidden) != 0) { attrs |= FileAttributes.Hidden; }
                return attrs;
            }
        }

        public override DiscDirectoryInfo Parent
        {
            get { return _parent; }
        }

        public override bool Exists
        {
            // We don't support arbitrary FileInfo's (yet) - they always represent a real file.
            get { return true; }
        }

        public override DateTime CreationTime
        {
            get { return _record.RecordingDateAndTime.ToLocalTime(); }
        }

        public override DateTime CreationTimeUtc
        {
            get { return _record.RecordingDateAndTime; }
        }

        public override DateTime LastAccessTime
        {
            get { return CreationTime; }
        }

        public override DateTime LastAccessTimeUtc
        {
            get { return CreationTimeUtc; }
        }

        public override DateTime LastWriteTime
        {
            get { return CreationTime; }
        }

        public override DateTime LastWriteTimeUtc
        {
            get { return CreationTimeUtc; }
        }

        public override long Length
        {
            get { return _record.DataLength; }
        }

        public override Stream Open(FileMode mode)
        {
            return Open(mode, FileAccess.Read);
        }

        public override Stream Open(FileMode mode, FileAccess access)
        {
            if (mode != FileMode.Open)
            {
                throw new NotSupportedException("Only existing files can be opened");
            }

            if (access != FileAccess.Read)
            {
                throw new NotSupportedException("Files cannot be opened for write");
            }

            return _reader.GetExtentStream(_record);
        }
    }
}