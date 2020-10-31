using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using GameApi.Tests.Helpers.TestClasses;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Reflection;
using System.Linq.Expressions;

namespace GameApi.Tests.Helpers
{

    public static class TestFunctions
    {

        // Return a DbSet of the specified generic type with support for async operations
        public static Mock<DbSet<T>> GetDbSet<T>(IQueryable<T> TestData) where T : class
        {
            var MockSet = new Mock<DbSet<T>>();
            MockSet.As<IAsyncEnumerable<T>>().Setup(x => x.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<T>(TestData.GetEnumerator()));
            MockSet.As<IQueryable<T>>().Setup(x => x.Provider).Returns(new TestAsyncQueryProvider<T>(TestData.Provider));
            MockSet.As<IQueryable<T>>().Setup(x => x.Expression).Returns(TestData.Expression);
            MockSet.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(TestData.ElementType);
            MockSet.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(TestData.GetEnumerator());
            return MockSet;
        }

        public static MethodInfo MethodOf(Expression<System.Action> expression)
        {
            MethodCallExpression body = (MethodCallExpression)expression.Body;
            return body.Method;
        }

        public static bool MethodHasAttribute(Expression<System.Action> expression, Type attributeType)
        {
            var methodInfo = MethodOf(expression);

            const bool includeInherited = false;
            return methodInfo.GetCustomAttributes(attributeType, includeInherited).Any();
        }

    }
}