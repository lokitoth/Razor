// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Text;

namespace Microsoft.AspNetCore.Razor.Tools
{
    internal class LazyStringWriter : TextWriter
    {
        // Arbitrary value
        private const int DefaultMaxCapacity = 10000;

        private readonly int _maxCapacity;
        private int _writtenCount = 0;
        private StringWriter _writer;

        public LazyStringWriter() : this(DefaultMaxCapacity)
        {
        }

        public LazyStringWriter(int maxCapacity)
        {
            if (maxCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCapacity));
            }

            _maxCapacity = maxCapacity;
        }

        public override Encoding Encoding => _writer.Encoding;

        public override void Write(char value)
        {
            EnsureStringWriter();

            if (_writtenCount++ < _maxCapacity)
            {
                _writer.Write(value);
            }
        }

        public override void Write(char[] buffer, int index, int count)
        {
            EnsureStringWriter();

            if (_writtenCount + buffer.Length < _maxCapacity)
            {
                _writer.Write(buffer, index, count);
                _writtenCount += buffer.Length;
            }
        }

        public override void Write(string value)
        {
            EnsureStringWriter();

            if (_writtenCount + value.Length < _maxCapacity)
            {
                _writer.Write(value);
                _writtenCount += value.Length;
            }
        }

        public override string ToString()
        {
            if (_writer != null)
            {
                return _writer.ToString();
            }

            return string.Empty;
        }

        protected override void Dispose(bool disposing)
        {
            if (_writer != null)
            {
                _writer.Dispose();
            }

            base.Dispose(disposing);
        }

        private void EnsureStringWriter()
        {
            if (_writer == null)
            {
                _writer = new StringWriter();
            }
        }
    }
}
