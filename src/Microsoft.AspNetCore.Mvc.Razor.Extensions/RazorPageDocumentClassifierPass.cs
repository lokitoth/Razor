﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace Microsoft.AspNetCore.Mvc.Razor.Extensions
{
    public class RazorPageDocumentClassifierPass : DocumentClassifierPassBase
    {
        public static readonly string RazorPageDocumentKind = "mvc.1.0.razor-page";
        private static readonly RazorEngine LeadingDirectiveParsingEngine = RazorEngine.Create(builder =>
        {
            for (var i = builder.Phases.Count - 1; i >= 0; i--)
            {
                var phase = builder.Phases[i];
                builder.Phases.RemoveAt(i);
                if (phase is IRazorDocumentClassifierPhase)
                {
                    break;
                }
            }

            RazorExtensions.Register(builder);
            builder.Features.Add(new LeadingDirectiveParserOptionsFeature());
        });

        protected override string DocumentKind => RazorPageDocumentKind;

        protected override bool IsMatch(RazorCodeDocument codeDocument, DocumentIntermediateNode documentNode)
        {
            return PageDirective.TryGetPageDirective(documentNode, out var pageDirective);
        }

        protected override void OnDocumentStructureCreated(
            RazorCodeDocument codeDocument,
            NamespaceDeclarationIntermediateNode @namespace,
            ClassDeclarationIntermediateNode @class,
            MethodDeclarationIntermediateNode method)
        {
            base.OnDocumentStructureCreated(codeDocument, @namespace, @class, method);

            @namespace.Content = "AspNetCore";

            @class.BaseType = "global::Microsoft.AspNetCore.Mvc.RazorPages.Page";

            var filePath = codeDocument.Source.RelativePath ?? codeDocument.Source.FilePath;
            @class.ClassName = CSharpIdentifier.GetClassNameFromPath(filePath);

            @class.Modifiers.Clear();
            @class.Modifiers.Add("public");

            method.MethodName = "ExecuteAsync";
            method.Modifiers.Clear();
            method.Modifiers.Add("public");
            method.Modifiers.Add("async");
            method.Modifiers.Add("override");
            method.ReturnType = $"global::{typeof(System.Threading.Tasks.Task).FullName}";

            EnsureValidPageDirective(codeDocument);
        }

        private void EnsureValidPageDirective(RazorCodeDocument codeDocument)
        {
            var document = codeDocument.GetDocumentIntermediateNode();
            PageDirective.TryGetPageDirective(document, out var pageDirective);

            Debug.Assert(pageDirective != null);

            if (pageDirective.DirectiveNode.IsImported())
            {
                pageDirective.DirectiveNode.Diagnostics.Add(
                    RazorExtensionsDiagnosticFactory.CreatePageDirective_CannotBeImported(pageDirective.DirectiveNode.Source.Value));
            }
            else
            {
                // The document contains a page directive and it is not imported.
                // We now want to make sure this page directive exists at the top of the file.
                var sourceDocument = codeDocument.Source;
                var leadingDirectiveCodeDocument = RazorCodeDocument.Create(sourceDocument);
                LeadingDirectiveParsingEngine.Process(leadingDirectiveCodeDocument);

                var documentIRNode = leadingDirectiveCodeDocument.GetDocumentIntermediateNode();
                if (!PageDirective.TryGetPageDirective(documentIRNode, out var _))
                {
                    // The page directive is not the leading directive. Add an error.
                    pageDirective.DirectiveNode.Diagnostics.Add(
                        RazorExtensionsDiagnosticFactory.CreatePageDirective_MustExistAtTheTopOfFile(pageDirective.DirectiveNode.Source.Value));
                }
            }
        }

        private class LeadingDirectiveParserOptionsFeature : RazorEngineFeatureBase, IConfigureRazorParserOptionsFeature
        {
            public int Order { get; }

            public void Configure(RazorParserOptionsBuilder options)
            {
                options.ParseLeadingDirectives = true;
            }
        }
    }
}
