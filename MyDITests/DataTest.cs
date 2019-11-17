using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDITests
{
    public interface ILogger
    {
        string Log(string str);
    }
    //--------------For test - No Default Conctructor---------------------
    public class NoCtorLogger : ILogger
    {
        string _str;
        public NoCtorLogger(string str)
        {
            _str = str;
        }
        public string Log(string str) => _str + " " + str;
    }
    //-----------------------For test - One Parametr-------------------
    public class CalcLogger : ILogger
    {
        public string Log(string str) => str + " Calclogger";
    }
    public class Calculator
    {
        public Calculator(ILogger logger)
        {
            Logger = logger;
        }
        public ILogger Logger { get; set; }
        public int Sum(int a, int b) => a + b;
    }
    //--------------------For test - Two parametrs------------------------ 
    public class FooLogger : ILogger
    {
        public string Log(string str) => str + " FooLogger";
    }
    public class Foo
    {
        public Foo(ILogger logger, IEnumerable stack)
        {
            Logger = logger;
            Stack = stack;
        }
        public ILogger Logger { get; set; }
        public IEnumerable Stack { get; set; }
    }
}
