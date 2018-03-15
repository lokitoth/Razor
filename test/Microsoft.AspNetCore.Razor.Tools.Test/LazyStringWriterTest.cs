// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;

namespace Microsoft.AspNetCore.Razor.Tools
{
    public class LazyStringWriterTest
    {
        [Fact]
        public void Writer_WritesStringAsExpected()
        {
            using (var writer = new LazyStringWriter())
            {
                // Arrange
                var expected = "Hello World!";

                // Act
                writer.Write(expected);

                // Assert
                var result = writer.ToString();
                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void Writer_MaxCapacityReached_DoesNotWriteString()
        {
            using (var writer = new LazyStringWriter(maxCapacity: 5))
            {
                // Arrange
                var expected = "Hello World!";

                // Act
                writer.Write(expected);

                // Assert
                var result = writer.ToString();
                Assert.Empty(result);
            }
        }
    }
}
