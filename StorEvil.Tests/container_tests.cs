using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Funq;
using NUnit.Framework;

namespace StorEvil
{

    [TestFixture]
    public class funq_easy_registration
    {
        [Test]
        public void should_be_able_to_get_service_impl()
        {
            var c = new Container();
            c.EasyRegister<IFoo, Foo>();

            Assert.IsInstanceOfType(typeof(Foo), c.Resolve<IFoo>());
        }

        [Test]
        public void should_be_able_to_inject_dependency()
        {
            var c = new Container();
            c.EasyRegister<IFoo, Foo>();
            c.EasyRegister<IBar, Bar>();

            var bar = c.Resolve<IBar>() as Bar;

            Assert.IsNotNull(bar.Foo);
        }

        [Test]
        public void should_be_able_to_chain_dependencies()
        {
            var c = new Container();
            var testFoo = new Foo();
            c.Register<IFoo>(testFoo);
            c.EasyRegister<IBar, Bar>();
            c.EasyRegister<IBaz, Baz>();
            var baz = c.Resolve<IBaz>() as Baz;

            var bar = baz.Bar as Bar;
            Assert.AreSame(bar.Foo, testFoo);
        }
    }

    public interface IFoo { }
    public class Foo : IFoo { }

    public interface IBar { }
    public class Bar : IBar
    {
        public IFoo Foo { get; set; }

        public Bar(IFoo foo)
        {
            Foo = foo;
        }
    }

    public interface IBaz { }
    public class Baz : IBaz
    {
        public IBar Bar { get; set; }

        public Baz(IBar bar)
        {
            Bar = bar;
        }
    }
 
    /// <summary>
    /// Funq helper for easy registration.
    /// </summary>
    public static class FunqEasyRegistrationHelper
    {
        /// <summary>
        /// Register a service with the default, look-up-all_dependencies-from-the-container behavior.
        /// </summary>
        /// <typeparam name="interfaceT">interface type</typeparam>
        /// <typeparam name="implT">implementing type</typeparam>
        /// <param name="container">Funq container</param>
        public static void EasyRegister<interfaceT, implT>(this Container container) where implT : interfaceT
        {
            var lambdaParam = Expression.Parameter(typeof (Container), "ref_to_the_container_passed_into_the_lambda");

            var constructorExpression = BuildImplConstructorExpression<implT>(lambdaParam);
            var compiledExpression = CompileInterfaceConstructor<interfaceT>(lambdaParam, constructorExpression);

            container.Register(compiledExpression);
        }

        private static readonly MethodInfo FunqContainerResolveMethod;

        static FunqEasyRegistrationHelper()
        {
            FunqContainerResolveMethod = typeof(Container).GetMethod("Resolve", new Type[0]);
        }

        private static NewExpression BuildImplConstructorExpression<implT>(Expression lambdaParam)
        {
            var ctorWithMostParameters = GetConstructorWithMostParameters<implT>();

            var constructorParameterInfos = ctorWithMostParameters.GetParameters();
            var regParams = constructorParameterInfos.Select(pi => GetParameterCreationExpression(pi, lambdaParam));

            return Expression.New(ctorWithMostParameters, regParams.ToArray());
        }

        private static Func<Container, interfaceT> CompileInterfaceConstructor<interfaceT>(ParameterExpression lambdaParam, Expression constructorExpression)
        {
            var constructorLambda = Expression.Lambda<Func<Container, interfaceT>>(constructorExpression, lambdaParam);
            return constructorLambda.Compile();
        }

        private static ConstructorInfo GetConstructorWithMostParameters<implT>()
        {
            return typeof(implT)
                .GetConstructors()
                .OrderBy(x => x.GetParameters().Length)
                .Where(ctor => !ctor.IsStatic)
                .Last();
        }

        private static MethodCallExpression GetParameterCreationExpression(ParameterInfo pi, Expression lambdaParam)
        {
            var method = FunqContainerResolveMethod.MakeGenericMethod(pi.ParameterType);
            return Expression.Call(lambdaParam, method);
        }  
    }
}