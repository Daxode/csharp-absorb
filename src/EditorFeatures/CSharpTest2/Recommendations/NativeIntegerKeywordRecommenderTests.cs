﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Completion.KeywordRecommenders;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.Recommendations
{
    [Trait(Traits.Feature, Traits.Features.KeywordRecommending)]
    public abstract class NativeIntegerKeywordRecommenderTests : RecommenderTests
    {
        private protected abstract AbstractNativeIntegerKeywordRecommender Recommender { get; }

        protected NativeIntegerKeywordRecommenderTests()
        {
            RecommendKeywordsAsync = (position, context) => Task.FromResult(Recommender.RecommendKeywords(position, context, CancellationToken.None));
        }

        [Fact]
        public async Task TestInLocalDeclaration()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"$$"));
        }

        [Fact]
        public async Task TestInParameterList()
        {
            await VerifyKeywordAsync(
@"class C
{
    void F($$");
        }

        [Fact]
        public async Task TestInLambdaParameterListFirst()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"F(($$"));
        }

        [Fact]
        public async Task TestInLambdaParameterListLater()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"F((int x, $$"));
        }

        [Fact]
        public async Task TestAfterConst()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"const $$"));
        }

        [Fact]
        public async Task TestInFixedStatement()
        {
            await VerifyKeywordAsync(
@"fixed ($$");
        }

        [Fact]
        public async Task TestInRef()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"ref $$"));
        }

        [Fact]
        public async Task TestInMemberType()
        {
            await VerifyKeywordAsync(
@"class C
{
    private $$");
        }

        [Fact]
        public async Task TestInOperatorType()
        {
            await VerifyKeywordAsync(
@"class C
{
    static implicit operator $$");
        }

        [Fact]
        public async Task TestInEnumUnderlyingType()
        {
            await VerifyAbsenceAsync(
@"enum E : $$");
        }

        [WorkItem(30784, "https://github.com/dotnet/roslyn/issues/30784")]
        [Fact]
        public async Task TestNotInTypeParameterConstraint_TypeDeclaration1()
        {
            await VerifyAbsenceAsync(
@"class C<T> where T : $$");
        }

        [WorkItem(30784, "https://github.com/dotnet/roslyn/issues/30784")]
        [Fact]
        public async Task TestInTypeParameterConstraint_TypeDeclaration_WhenNotDirectlyInConstraint1()
        {
            await VerifyKeywordAsync(
@"class C<T> where T : IList<$$");
        }

        [WorkItem(30784, "https://github.com/dotnet/roslyn/issues/30784")]
        [Fact]
        public async Task TestNotInTypeParameterConstraint_TypeDeclaration2()
        {
            await VerifyAbsenceAsync(
@"class C<T>
        where T : $$
        where U : U");
        }

        [WorkItem(30784, "https://github.com/dotnet/roslyn/issues/30784")]
        [Fact]
        public async Task TestInTypeParameterConstraint_TypeDeclaration_WhenNotDirectlyInConstraint2()
        {
            await VerifyKeywordAsync(
@"class C<T>
        where T : IList<$$
        where U : U");
        }

        [WorkItem(30784, "https://github.com/dotnet/roslyn/issues/30784")]
        [Fact]
        public async Task TestNotInTypeParameterConstraint_MethodDeclaration1()
        {
            await VerifyAbsenceAsync(
@"class C
{
    public void M<T>() where T : $$");
        }

        [WorkItem(30784, "https://github.com/dotnet/roslyn/issues/30784")]
        [Fact]
        public async Task TestInTypeParameterConstraint_MethodDeclaration_WhenNotDirectlyInConstraint1()
        {
            await VerifyKeywordAsync(
@"class C
{
    public void M<T>() where T : IList<$$");
        }

        [WorkItem(30784, "https://github.com/dotnet/roslyn/issues/30784")]
        [Fact]
        public async Task TestNotInTypeParameterConstraint_MethodDeclaration2()
        {
            await VerifyAbsenceAsync(
@"class C
{
    public void M<T>()
        where T : $$
        where U : T");
        }

        [WorkItem(30784, "https://github.com/dotnet/roslyn/issues/30784")]
        [Fact]
        public async Task TestNotInTypeParameterConstraint_MethodDeclaration_WhenNotDirectlyInConstraint2()
        {
            await VerifyKeywordAsync(
@"class C
{
    public void M<T>()
        where T : IList<$$
        where U : T");
        }

        [Fact]
        public async Task TestInExpression()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"var v = 1 + $$"));
        }

        [Fact]
        public async Task TestInDefault()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"_ = default($$"));
        }

        [Fact]
        public async Task TestInCastType()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"var v = (($$"));
        }

        [Fact]
        public async Task TestInNew()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"_ = new $$"));
        }

        [Fact]
        public async Task TestAfterAs()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"object x = null;
var y = x as $$"));
        }

        [Fact]
        public async Task TestAfterIs()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"object x = null;
if (x is $$"));
        }

        [Fact]
        public async Task TestAfterStackAlloc()
        {
            await VerifyKeywordAsync(
@"class C
{
    nint* p = stackalloc $$");
        }

        [Fact]
        public async Task TestInUsingAliasFirst()
        {
            // Ideally, keywords should not be recommended as first token in target.
            await VerifyKeywordAsync(
@"using A = $$");
        }

        [Fact]
        public async Task TestInGlobalUsingAliasFirst()
        {
            // Ideally, keywords should not be recommended as first token in target.
            await VerifyKeywordAsync(
@"global using A = $$");
        }

        [Fact]
        public async Task TestInUsingAliasLater()
        {
            await VerifyKeywordAsync(
@"using A = List<$$");
        }

        [Fact]
        public async Task TestInGlobalUsingAliasLater()
        {
            await VerifyKeywordAsync(
@"global using A = List<$$");
        }

        [Fact]
        public async Task TestInNameOf()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"_ = nameof($$"));
        }

        [Fact]
        public async Task TestInSizeOf()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"_ = sizeof($$"));
        }

        [Fact]
        public async Task TestInCRef()
        {
            await VerifyAbsenceAsync(AddInsideMethod(
@"/// <see cref=""$$"));
        }

        [Fact]
        public async Task TestInTupleWithinType()
        {
            await VerifyKeywordAsync(@"
class Program
{
    ($$
}");
        }

        [Fact]
        public async Task TestInTupleWithinMember()
        {
            await VerifyKeywordAsync(@"
class Program
{
    void Method()
    {
        ($$
    }
}");
        }

        [Fact]
        public async Task TestPatternInSwitch()
        {
            await VerifyKeywordAsync(AddInsideMethod(
@"switch(o)
{
    case $$
}"));
        }

        [Fact]
        public async Task TestInFunctionPointerType()
        {
            await VerifyKeywordAsync(@"
class C
{
    delegate*<$$");
        }

        [Fact]
        public async Task TestInFunctionPointerTypeAfterComma()
        {
            await VerifyKeywordAsync(@"
class C
{
    delegate*<int, $$");
        }

        [Fact]
        public async Task TestInFunctionPointerTypeAfterModifier()
        {
            await VerifyKeywordAsync(@"
class C
{
    delegate*<ref $$");
        }

        [WorkItem(60341, "https://github.com/dotnet/roslyn/issues/60341")]
        [Fact]
        public async Task TestNotAfterAsync()
        {
            await VerifyAbsenceAsync(@"
class C
{
    async $$");
        }

        [Fact]
        public async Task TestNotAfterDelegateAsterisk()
        {
            await VerifyAbsenceAsync(@"
class C
{
    delegate*$$");
        }

        [WorkItem(53585, "https://github.com/dotnet/roslyn/issues/53585")]
        [Theory]
        [ClassData(typeof(TheoryDataKeywordsIndicatingLocalFunctionWithoutAsync))]
        public async Task TestAfterKeywordIndicatingLocalFunctionWithoutAsync(string keyword)
        {
            await VerifyKeywordAsync(AddInsideMethod($@"
{keyword} $$"));
        }

        [WorkItem(60341, "https://github.com/dotnet/roslyn/issues/60341")]
        [Theory]
        [ClassData(typeof(TheoryDataKeywordsIndicatingLocalFunctionWithAsync))]
        public async Task TestNotAfterKeywordIndicatingLocalFunctionWithAsync(string keyword)
        {
            await VerifyAbsenceAsync(AddInsideMethod($@"
{keyword} $$"));
        }

        [WorkItem(49743, "https://github.com/dotnet/roslyn/issues/49743")]
        [Fact]
        public async Task TestNotInPreprocessorDirective()
        {
            await VerifyAbsenceAsync(
@"#$$");
        }
    }
}
