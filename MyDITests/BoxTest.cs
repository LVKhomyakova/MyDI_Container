using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FluentAssertions;
using Moq;
using MyDI;
using System.Collections;

namespace MyDITests
{
    [TestFixture]
    public class BoxTest
    {
        //-------------------------------Registration------------------------------
        [Test, Category("Registration")]
        public void Test_Bind()
        {
            Box box = new Box();
            box.Bind(typeof(IEnumerable)).Root
                .Should().Contain(typeof(IEnumerable), null);
        }

        [Test, Category("Registration")]
        public void Test_Bind_To()
        {
            Box box = new Box();
            box.Bind(typeof(IEnumerable)).To(typeof(Stack));
            box.Root.Should().Contain(typeof(IEnumerable), typeof(Stack));
        }

        [Test, Category("Registration")]
        public void Test_Bind_Call_Twice()
        {
            Box box = new Box();
            box.Bind(typeof(IEnumerable));
            box.Bind(typeof(ICollection)).Root
                .Should().Contain(typeof(ICollection),null);
        }

        [Test, Category("Registration")]
        public void Test_Bind_ToSelf()
        {
            Box box = new Box();
            box.Bind(typeof(ICollection)).ToSelf();
            box.Root.Should().Contain(typeof(ICollection), typeof(ICollection));
        }

        [Test, Category("Registration")]
        public void Test_To_Before_Bind()
        {
            Box box = new Box();
            Action act = ()=> box.To(typeof(Stack));
            act.Should().Throw<Exception>()
                .WithMessage("Зависимость не найдена, необходимо выполнить команду Bind");
        }

        [Test, Category("Registration")]
        public void Test_Wrong_Injection()
        {
            Box box = new Box();
            box.Bind(typeof(IEnumerable));
            Action act = () => box.To(typeof(Box));
            act.Should().Throw<InvalidCastException>()
                .WithMessage($"Тип {typeof(Box)} не является потомком типа {typeof(IEnumerable)}");
        }

        //-------------------------------Resolving------------------------------
        [Test, Category("Resolving")]
        public void Test_Get()
        {
            Box box = new Box();
            box.Bind(typeof(IEnumerable)).To(typeof(Stack));

            box.Get(typeof(IEnumerable)).Should().BeOfType(typeof(Stack));
        }

        [Test, Category("Resolving")]
        public void Test_Get_No_Default_Conctructor()
        {
            Box box = new Box();
            box.Bind(typeof(ILogger)).To(typeof(NoCtorLogger));

            Action act = ()=> box.Get(typeof(ILogger));
            act.Should().Throw<Exception>();
        }

        [Test, Category("Resolving")]
        public void Test_Get_Nested_Call__One_Parametr()
        {
            Box box = new Box();
            box.Bind(typeof(Calculator)).ToSelf();
            box.Bind(typeof(ILogger)).To(typeof(CalcLogger));

            var calc = (Calculator)box.Get(typeof(Calculator));
            calc.Logger.Log("Resolving").Should().Be("Resolving Calclogger");
        }

        [Test, Category("Resolving")]
        public void Test_Get_Nested_Call__Two_parametrs_Generic()
        {
            Box box = new Box();
            box.Bind(typeof(Foo)).ToSelf();
            box.Bind(typeof(ILogger)).To(typeof(FooLogger));
            box.Bind(typeof(IEnumerable)).To(typeof(Stack));

            var foo = (Foo)box.Get(typeof(Foo));
            foo.Logger.Log("Resolving").Should().Be("Resolving FooLogger");
            foo.Stack.Should().BeOfType(typeof(Stack));
        }

        //-------------------------------Generic method------------------------------
        [Test, Category("Generic method")]
        public void Test_Bind_Generic()
        {
            Box box = new Box();
            box.Bind<IEnumerable>().Root
                .Should().Contain(typeof(IEnumerable), null);
        }

        [Test, Category("Generic method")]
        public void Test_Bind_To_Generic()
        {
            Box box = new Box();
            box.Bind<IEnumerable>().To<Stack>();
            box.Root.Should().Contain(typeof(IEnumerable), typeof(Stack));
        }

        [Test, Category("Generic method")]
        public void Test_Bind_Call_Twice_Generic()
        {
            Box box = new Box();
            box.Bind<IEnumerable>();
            box.Bind<ICollection>().Root
                .Should().Contain(typeof(ICollection), null);
        }

        [Test, Category("Generic method")]
        public void Test_To_Before_Bind_Generic()
        {
            Box box = new Box();
            Action act = () => box.To<Stack>();
            act.Should().Throw<Exception>()
                .WithMessage("Зависимость не найдена, необходимо выполнить команду Bind");
        }

        [Test, Category("Generic method")]
        public void Test_Wrong_Injection_Generic()
        {
            Box box = new Box();
            box.Bind<IEnumerable>();
            Action act = () => box.To<Box>();
            act.Should().Throw<InvalidCastException>()
                .WithMessage($"Тип {typeof(Box)} не является потомком типа {typeof(IEnumerable)}");
        }

        [Test, Category("Generic method")]
        public void Test_Get_Generic()
        {
            Box box = new Box();
            box.Bind<IEnumerable>().To<Stack>();

            box.Get<IEnumerable>().Should().BeOfType(typeof(Stack));
        }

        [Test, Category("Generic method")]
        public void Test_Get_No_Default_Conctructor_Generic
            ()
        {
            Box box = new Box();
            box.Bind<ILogger>().To<NoCtorLogger>();

            Action act = () => box.Get<ILogger>();
            act.Should().Throw<Exception>();
        }

        [Test, Category("Generic method")]
        public void Test_Get_Nested_Call__One_Parametr_Generic()
        {
            Box box = new Box();
            box.Bind<Calculator>().ToSelf();
            box.Bind<ILogger>().To<CalcLogger>();

            var calc = box.Get<Calculator>();
            calc.Logger.Log("Resolving").Should().Be("Resolving Calclogger");
        }

        [Test, Category("Generic method")]
        public void Test_Get_Nested_Call__Two_Parametrs_Generic()
        {
            Box box = new Box();
            box.Bind<Foo>().ToSelf();
            box.Bind<ILogger>().To<FooLogger>();
            box.Bind<IEnumerable>().To<Stack>();

            var foo = box.Get<Foo>();
            foo.Logger.Log("Resolving").Should().Be("Resolving FooLogger");
            foo.Stack.Should().BeOfType(typeof(Stack));
        }

    }
}
