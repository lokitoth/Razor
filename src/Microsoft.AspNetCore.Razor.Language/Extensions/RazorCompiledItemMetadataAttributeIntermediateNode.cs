// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace Microsoft.AspNetCore.Razor.Language.Extensions
{
    public class RazorCompiledItemMetadataAttributeIntermediateNode : ExtensionIntermediateNode
    {
        public override IntermediateNodeCollection Children => IntermediateNodeCollection.ReadOnly;

        public RazorCompiledItemMetadataAttributeIntermediateNode(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpty, nameof(key));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpty, nameof(value));
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

            var extension = target.GetExtension<IMetadataAttributeTargetExtension>();
            if (extension == null)
            {
                ReportMissingCodeTargetExtension<IMetadataAttributeTargetExtension>(context);
                return;
            }

            extension.WriteRazorCompiledItemMetadataAttribute(context, this);
        }
    }
}
