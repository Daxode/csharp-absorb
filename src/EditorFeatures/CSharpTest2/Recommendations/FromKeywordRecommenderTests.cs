﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Recommendations
{
    [Trait(Traits.Feature, Traits.Features.KeywordRecommending)]
    public class FromKeywordRecommenderTests : KeywordRecommenderTests
    {
        [Fact]
        public async Task TestAtRoot_Interactive()
        {
            await VerifyKeywordAsync(SourceCodeKind.Script,
@"$$");
        }

        [Fact]
        public async Task TestAfterClass_Interactive()
        {
            await VerifyKeywordAsync(SourceCodeKind.Script,
@"class C { }
$$");
        }

        [Fact]
        public async Task TestAfterGlobalStatement_Interactive()
        {
            await VerifyKeywordAsync(SourceCodeKind.Script,
@"System.Console.WriteLine();
$$");
        }

        [Fact]
        public async Task TestAfterGlobalVariableDeclaration_Interactive()
        {
            await VerifyKeywordAsync(SourceCodeKind.Script,
@"int i = 0;
$$");
        }

        [Fact]
        public async Task TestNotInUsingAlias()
        {
            await VerifyAbsenceAsync(
@"using Goo = $$");
        }

        [Fact]
        public async Task TestNotInGlobalUsingAlias()
        {
            await VerifyAbsenceAsync(
@"global using Goo = $$");
        }

        [Fact]
        public async Task TestNotInEmptyStatement()
        {
            await VerifyAbsenceAsync(AddInsideMethod(
@"$$"));
        }

        [Fact]
        public async Task TestInEmptySpace()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"var v = $$"));
        }

        [Fact]
        public async Task TestAfterIdentifier()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"var v = a$$"));
        }

        [Fact]
        public async Task TestNestedInQueryExpression()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"var q = from x in $$"));
        }

        [Fact]
        public async Task TestAfterFrom()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"var v = from x in y
          $$"));
        }

        [Fact]
        public async Task TestAfterPreviousClause()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"var v = from x in y
          where x > y
          $$"));
        }

        [Fact]
        public async Task TestAfterPreviousContinuationClause()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"var v = from x in y
          group x by y into g
          $$"));
        }

        [Fact]
        public async Task TestNotAfterFrom1()
        {
            await VerifyAbsenceAsync(AddInsideMethod(
@"var v = from $$"));
        }

        [Fact]
        public async Task TestNotAfterFrom2()
        {
            await VerifyAbsenceAsync(AddInsideMethod(
@"var v = from a in y
          from $$"));
        }

        [Fact]
        public async Task TestBetweenClauses()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"var v = from x in y
          $$
          from z in w"));
        }

        [Fact]
        public async Task TestContinueInSelect()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"var v = from x in y
          select $$"));
        }

        [Fact]
        public async Task TestBetweenTokens()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"var v =$$;"));
        }

        [Fact]
        public async Task TestNotInDeclaration()
        {
            await VerifyAbsenceAsync(AddInsideMethod(
@"int $$"));
        }

        [WorkItem(538264, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/538264")]
        [Fact]
        public async Task TestNotInEnumMemberInitializer1()
        {
            await VerifyAbsenceAsync(
@"enum E {
    a = $$
}");
        }

        [WorkItem(538264, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/538264")]
        [Fact]
        public async Task TestNotInConstMemberInitializer1()
        {
            await VerifyAbsenceAsync(
@"class E {
    const int a = $$
}");
        }

        [WorkItem(538264, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/538264")]
        [Fact]
        public async Task TestInMemberInitializer1()
        {
            await VerifyKeywordAsync(
@"class E {
    int a = $$
}");
        }

        [WorkItem(538804, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/538804")]
        [Fact]
        public async Task TestNotInTypeOf()
        {
            await VerifyAbsenceAsync(AddInsideMethod(
@"typeof($$"));
        }

        [WorkItem(538804, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/538804")]
        [Fact]
        public async Task TestNotInDefault()
        {
            await VerifyAbsenceAsync(AddInsideMethod(
@"default($$"));
        }

        [WorkItem(538804, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/538804")]
        [Fact]
        public async Task TestNotInSizeOf()
        {
            await VerifyAbsenceAsync(AddInsideMethod(
@"sizeof($$"));
        }

        [WorkItem(544219, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/544219")]
        [Fact]
        public async Task TestNotInObjectInitializerMemberContext()
        {
            await VerifyAbsenceAsync(@"
class C
{
    public int x, y;
    void M()
    {
        var c = new C { x = 2, y = 3, $$");
        }

        [Fact]
        public async Task TestNotAfterOutInArgument()
        {
            var experimentalFeatures = new System.Collections.Generic.Dictionary<string, string>(); // no experimental features to enable
            await VerifyAbsenceAsync(@"
class C
{
    void M(out int x) { x = 42; }

    void N()
    {
        M(out var $$", options: Options.Regular.WithFeatures(experimentalFeatures), scriptOptions: Options.Script.WithFeatures(experimentalFeatures));
        }
    }
}
