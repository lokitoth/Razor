// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace Microsoft.AspNetCore.Mvc.Razor.Extensions
{
    internal class RazorCompiledItemMetadataAttributeIntermediateNode : ExtensionIntermediateNode
    {
        private const string AttributeName = "global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemMetadataAttribute";

        public override IntermediateNodeCollection Children => IntermediateNodeCollection.ReadOnly;

        public RazorCompiledItemMetadataAttributeIntermediateNode(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpy, nameof(key));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpy, nameof(value));
            }

            Key = key;
            Value = value;
        }

        public string Key { get; }

        public string Value { get; }

        public override void Accept(IntermediateNodeVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            AcceptExtensionNode(this, visitor);
        }

        public override void WriteNode(CodeTarget target, CodeRenderingContext context)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // [global::...RazorCompiledItemMetadataAttribute(@"{Key}", @"{Value}")]
            context.CodeWriter.Write("[");
            context.CodeWriter.Write(AttributeName);
            context.CodeWriter.Write("(@\"");
            context.CodeWriter.Write(Key);
            context.CodeWriter.Write("\", @\"");
            context.CodeWriter.Write(Value);
            context.CodeWriter.WriteLine("\")]");
        }
    }
}
