// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Razor.Extensions
{
    public class RazorCompiledItemMetadataAttributeIntermediateNodeTest
    {
        [Fact]
        public void WriteNode_WritesAttribute()
        {
            // Arrange
            var expected = "[global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemMetadataAttribute(@\"test-key\", @\"test-\"value\")]";
            var node = new RazorCompiledItemMetadataAttributeIntermediateNode("test-key", "test-\"value");
            var codeDocument = TestRazorCodeDocument.CreateEmpty();
            var options = RazorCodeGenerationOptions.CreateDefault();

            var target = CodeTarget.CreateDefault(codeDocument, options);
            var context = TestCodeRenderingContext.CreateRuntime();

            // Act
            node.WriteNode(target, context);

            // Assert
            var actual = context.CodeWriter.GenerateCode().Trim();
            Assert.Equal(expected, actual);
        }
    }
}
