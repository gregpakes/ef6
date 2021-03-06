﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Data.Entity.Infrastructure.Interception
{
    using System.Data.Entity.Core.Objects;
    using System.Threading.Tasks;
    using Xunit;
    using Xunit.Extensions;

    public class DbConnectionInterceptionContextTests : TestBase
    {
        public class Generic
        {
            [Fact]
            public void Constructor_throws_on_null()
            {
                Assert.Equal(
                    "copyFrom",
                    Assert.Throws<ArgumentNullException>(() => new DbConnectionInterceptionContext<int>(null)).ParamName);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Initially_has_no_state(bool useObsoleteState)
            {
                var interceptionContext = new DbConnectionInterceptionContext<int>();

                Assert.Empty(interceptionContext.DbContexts);
                Assert.Null(interceptionContext.Exception);
                Assert.False(interceptionContext.IsAsync);
                Assert.False(interceptionContext.IsExecutionSuppressed);
                Assert.Empty(interceptionContext.ObjectContexts);
                Assert.Null(interceptionContext.OriginalException);
                Assert.Equal(0, interceptionContext.OriginalResult);
                Assert.Equal(0, interceptionContext.Result);
                Assert.Equal((TaskStatus)0, interceptionContext.TaskStatus);
                if (useObsoleteState)
                {
#pragma warning disable 618
                    Assert.Null(interceptionContext.UserState);
#pragma warning restore 618
                }
                else
                {
                    Assert.Null(interceptionContext.FindUserState("A"));
                    Assert.Null(interceptionContext.FindUserState("B"));
                }
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Cloning_the_interception_context_preserves_contextual_information_but_not_mutable_state(bool useObsoleteState)
            {
                var objectContext = new ObjectContext();
                var dbContext = DbContextMockHelper.CreateDbContext(objectContext);

                var interceptionContext = new DbConnectionInterceptionContext<int>();
                interceptionContext.Exception = new Exception("Cheez Whiz");
                interceptionContext.Result = 23;
                if (useObsoleteState)
                {
#pragma warning disable 618
                    interceptionContext.UserState = "Cheddar";
#pragma warning restore 618
                }
                else
                {
                    interceptionContext.SetUserState("A", "AState");
                    interceptionContext.SetUserState("B", "BState");
                }

                interceptionContext = interceptionContext
                    .WithDbContext(dbContext)
                    .WithObjectContext(objectContext)
                    .AsAsync();

                Assert.Equal(new[] { objectContext }, interceptionContext.ObjectContexts);
                Assert.Equal(new[] { dbContext }, interceptionContext.DbContexts);
                Assert.True(interceptionContext.IsAsync);

                Assert.Equal(0, interceptionContext.Result);
                Assert.Equal(0, interceptionContext.OriginalResult);
                Assert.Null(interceptionContext.Exception);
                Assert.Null(interceptionContext.OriginalException);
                Assert.False(interceptionContext.IsExecutionSuppressed);
                if (useObsoleteState)
                {
#pragma warning disable 618
                    Assert.Null(interceptionContext.UserState);
#pragma warning restore 618
                }
                else
                {
                    Assert.Null(interceptionContext.FindUserState("A"));
                    Assert.Null(interceptionContext.FindUserState("B"));
                }
            }

            [Fact]
            public void Association_the_base_with_a_null_ObjectContext_or_DbContext_throws()
            {
                Assert.Equal(
                    "context",
                    Assert.Throws<ArgumentNullException>(() => new DbConnectionInterceptionContext<int>().WithObjectContext(null)).ParamName);

                Assert.Equal(
                    "context",
                    Assert.Throws<ArgumentNullException>(() => new DbConnectionInterceptionContext<int>().WithDbContext(null)).ParamName);
            }
        }

        public class NonGeneric
        {
            [Fact]
            public void Constructor_throws_on_null()
            {
                Assert.Equal(
                    "copyFrom",
                    Assert.Throws<ArgumentNullException>(() => new DbConnectionInterceptionContext(null)).ParamName);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Initially_has_no_state(bool useObsoleteState)
            {
                var interceptionContext = new DbConnectionInterceptionContext();

                Assert.Empty(interceptionContext.DbContexts);
                Assert.Null(interceptionContext.Exception);
                Assert.False(interceptionContext.IsAsync);
                Assert.False(interceptionContext.IsExecutionSuppressed);
                Assert.Empty(interceptionContext.ObjectContexts);
                Assert.Null(interceptionContext.OriginalException);
                Assert.Equal((TaskStatus)0, interceptionContext.TaskStatus);
                if (useObsoleteState)
                {
#pragma warning disable 618
                    Assert.Null(interceptionContext.UserState);
#pragma warning restore 618
                }
                else
                {
                    Assert.Null(interceptionContext.FindUserState("A"));
                    Assert.Null(interceptionContext.FindUserState("B"));
                }
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Cloning_the_interception_context_preserves_contextual_information_but_not_mutable_state(bool useObsoleteState)
            {
                var objectContext = new ObjectContext();
                var dbContext = DbContextMockHelper.CreateDbContext(objectContext);

                var interceptionContext = new DbConnectionInterceptionContext();

                var mutableData = ((IDbMutableInterceptionContext)interceptionContext).MutableData;
                mutableData.SetExceptionThrown(new Exception("Cheez Whiz"));
                if (useObsoleteState)
                {
#pragma warning disable 618
                    interceptionContext.UserState = "Cheddar";
#pragma warning restore 618
                }
                else
                {
                    interceptionContext.SetUserState("A", "AState");
                    interceptionContext.SetUserState("B", "BState");
                }

                interceptionContext = interceptionContext
                    .WithDbContext(dbContext)
                    .WithObjectContext(objectContext)
                    .AsAsync();

                Assert.Equal(new[] { objectContext }, interceptionContext.ObjectContexts);
                Assert.Equal(new[] { dbContext }, interceptionContext.DbContexts);
                Assert.True(interceptionContext.IsAsync);

                Assert.Null(interceptionContext.Exception);
                Assert.Null(interceptionContext.OriginalException);
                Assert.False(interceptionContext.IsExecutionSuppressed);
                if (useObsoleteState)
                {
#pragma warning disable 618
                    Assert.Null(interceptionContext.UserState);
#pragma warning restore 618
                }
                else
                {
                    Assert.Null(interceptionContext.FindUserState("A"));
                    Assert.Null(interceptionContext.FindUserState("B"));
                }
            }

            [Fact]
            public void Association_the_base_with_a_null_ObjectContext_or_DbContext_throws()
            {
                Assert.Equal(
                    "context",
                    Assert.Throws<ArgumentNullException>(() => new DbConnectionInterceptionContext().WithObjectContext(null)).ParamName);

                Assert.Equal(
                    "context",
                    Assert.Throws<ArgumentNullException>(() => new DbConnectionInterceptionContext().WithDbContext(null)).ParamName);
            }
        }
    }
}
