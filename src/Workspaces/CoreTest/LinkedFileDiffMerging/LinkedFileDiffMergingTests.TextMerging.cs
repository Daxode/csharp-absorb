﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.UnitTests.LinkedFileDiffMerging
{
    [Trait(Traits.Feature, Traits.Features.LinkedFileDiffMerging)]
    public partial class LinkedFileDiffMergingTests
    {
        [Fact]
        public void TestIdenticalChanges()
        {
            TestLinkedFileSet(
                "x",
                new List<string> { "y", "y" },
                @"y",
                LanguageNames.CSharp);
        }

        [Fact]
        public void TestChangesInOnlyOneFile()
        {
            TestLinkedFileSet(
                "a b c d e",
                new List<string> { "a b c d e", "a z c z e" },
                @"a z c z e",
                LanguageNames.CSharp);
        }

        [Fact]
        public void TestIsolatedChangesInBothFiles()
        {
            TestLinkedFileSet(
                "a b c d e",
                new List<string> { "a z c d e", "a b c z e" },
                @"a z c z e",
                LanguageNames.CSharp);
        }

        [Fact]
        [WorkItem(44423, "https://github.com/dotnet/roslyn/issues/44423")]
        public void TestIdenticalEditAfterIsolatedChanges()
        {
            TestLinkedFileSet(
                "a; b; c; d; e;",
                new List<string> { "a; zzz; c; xx; e;", "a; b; c; xx; e;" },
                @"a; zzz; c; xx; e;",
                LanguageNames.CSharp);
        }

        [Fact]
        public void TestOneConflict()
        {
            TestLinkedFileSet(
                "a b c d e",
                new List<string> { "a b y d e", "a b z d e" },
                @"
/* " + string.Format(WorkspacesResources.Unmerged_change_from_project_0, "ProjectName1") + @"
" + WorkspacesResources.Before_colon + @"
a b c d e
" + WorkspacesResources.After_colon + @"
a b z d e
*/
a b y d e",
                LanguageNames.CSharp);
        }

        [Fact]
        public void TestTwoConflictsOnSameLine()
        {
            TestLinkedFileSet(
                "a b c d e",
                new List<string> { "a q1 c z1 e", "a q2 c z2 e" },
                @"
/* " + string.Format(WorkspacesResources.Unmerged_change_from_project_0, "ProjectName1") + @"
" + WorkspacesResources.Before_colon + @"
a b c d e
" + WorkspacesResources.After_colon + @"
a q2 c z2 e
*/
a q1 c z1 e",
                LanguageNames.CSharp);
        }

        [Fact]
        public void TestTwoConflictsOnAdjacentLines()
        {
            TestLinkedFileSet(
                @"One
Two
Three
Four",
                new List<string>
                {
                    @"One
TwoY
ThreeY
Four",
                    @"One
TwoZ
ThreeZ
Four"
                },
                @"One

/* " + string.Format(WorkspacesResources.Unmerged_change_from_project_0, "ProjectName1") + @"
" + WorkspacesResources.Before_colon + @"
Two
Three
" + WorkspacesResources.After_colon + @"
TwoZ
ThreeZ
*/
TwoY
ThreeY
Four",
                LanguageNames.CSharp);
        }

        [Fact]
        [WorkItem(44423, "https://github.com/dotnet/roslyn/issues/44423")]
        public void TestTwoConflictsOnSeparatedLines()
        {
            TestLinkedFileSet(
                @"One;
Two;
Three;
Four;
Five;",
                new List<string>
                {
                    @"One;
TwoY;
Three;
FourY;
Five;",
                    @"One;
TwoZ;
Three;
FourZ;
Five;"
                },
                @"One;

/* " + string.Format(WorkspacesResources.Unmerged_change_from_project_0, "ProjectName1") + @"
" + WorkspacesResources.Before_colon + @"
Two;
" + WorkspacesResources.After_colon + @"
TwoZ;
*/
TwoY;
Three;

/* " + string.Format(WorkspacesResources.Unmerged_change_from_project_0, "ProjectName1") + @"
" + WorkspacesResources.Before_colon + @"
Four;
" + WorkspacesResources.After_colon + @"
FourZ;
*/
FourY;
Five;",
                LanguageNames.CSharp);
        }

        [Fact]
        public void TestManyLinkedFilesWithOverlappingChange()
        {
            TestLinkedFileSet(
                @"A",
                new List<string>
                {
                    @"A",
                    @"B",
                    @"C",
                    @"",
                },
                @"
/* " + string.Format(WorkspacesResources.Unmerged_change_from_project_0, "ProjectName2") + @"
" + WorkspacesResources.Before_colon + @"
A
" + WorkspacesResources.After_colon + @"
C
*/

/* " + string.Format(WorkspacesResources.Unmerged_change_from_project_0, "ProjectName3") + @"
" + WorkspacesResources.Removed_colon + @"
A
*/
B",
                LanguageNames.CSharp);
        }

        [Fact]
        public void TestCommentsAddedCodeCSharp()
        {
            TestLinkedFileSet(
                @"",
                new List<string>
                {
                    @"A",
                    @"B",
                },
                @"
/* " + string.Format(WorkspacesResources.Unmerged_change_from_project_0, "ProjectName1") + @"
" + WorkspacesResources.Added_colon + @"
B
*/
A",
                LanguageNames.CSharp);
        }

        [Fact]
        public void TestCommentsAddedCodeVB()
        {
            TestLinkedFileSet(
                @"",
                new List<string>
                {
                    @"A",
                    @"B",
                },
                @"
' " + string.Format(WorkspacesResources.Unmerged_change_from_project_0, "ProjectName1") + @" 
' " + WorkspacesResources.Added_colon + @"
' B
A",
                LanguageNames.VisualBasic);
        }

        [Fact]
        public void TestCommentsRemovedCodeCSharp()
        {
            TestLinkedFileSet(
                @"A",
                new List<string>
                {
                    @"B",
                    @"",
                },
                @"
/* " + string.Format(WorkspacesResources.Unmerged_change_from_project_0, "ProjectName1") + @"
" + WorkspacesResources.Removed_colon + @"
A
*/
B",
                LanguageNames.CSharp);
        }

        [Fact]
        public void TestCommentsRemovedCodeVB()
        {
            TestLinkedFileSet(
                @"A",
                new List<string>
                {
                    @"B",
                    @"",
                },
                @"
' " + string.Format(WorkspacesResources.Unmerged_change_from_project_0, "ProjectName1") + @" 
' " + WorkspacesResources.Removed_colon + @"
' A
B",
                LanguageNames.VisualBasic);
        }
    }
}
