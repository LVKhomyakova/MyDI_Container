using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDI
{
    public class Box
    {
        protected Type _currentKeyType;
        public Box()
        {
            _currentKeyType = null;
            Root = new ConcurrentDictionary<Type, Type>();
        }
        public ConcurrentDictionary<Type, Type> Root { get; }

        /// <summary>
        /// Регистрация внедряемой зависимости
        /// </summary>
        /// <param name="type">Тип внедряемой зависимости</param>
        /// <returns></returns>
        public Box Bind(Type type)
        {
            if (!Root.ContainsKey(type))
            {
                Root.TryAdd(type, null);
                _currentKeyType = type;
            }
            return this;
        }

        /// <summary>
        /// Регистрация внедряемой зависимости
        /// </summary>
        /// <typeparam name="T">Тип внедряемой зависимости</typeparam>
        /// <returns></returns>
        public Box Bind<T>() where T: class
        {
            return Bind(typeof(T));
        }

        /// <summary>
        /// Регистрация реализации для зависимости
        /// </summary>
        /// <param name="type">Тип реализации</param>
        public void To(Type type)
        {
            if(_currentKeyType == null)
                throw new Exception("Зависимость не найдена, необходимо выполнить команду Bind");

            if (!IsCorrectInjection(_currentKeyType, type))
                throw new InvalidCastException($"Тип {type} не является потомком типа {_currentKeyType}");

            if (Root.ContainsKey(_currentKeyType))
                Root[_currentKeyType] = type;
            else
                throw new Exception("Зависимость не найдена, необходимо выполнить команду Bind");
        }

        /// <summary>
        /// Регистрация реализации для зависимости
        /// </summary>
        /// <typeparam name="T">Тип реализации</typeparam>
        public void To<T>() where T : class
        {
            To(typeof(T));
        }

        /// <summary>
        /// Регистрация реализации для зависимости (саму на себя)
        /// </summary>
        public void ToSelf()
        {
            if (_currentKeyType != null && Root.ContainsKey(_currentKeyType))
                Root[_currentKeyType] = _currentKeyType;
            else
                throw new Exception("Зависимость не найдена, необходимо выполнить команду Bind");
        }

        /// <summary>
        /// Получить обьект ассоциируемый с внедряемой зависимостью
        /// </summary>
        /// <param name="type">Тип внедряемой зависимости</param>
        /// <returns></returns>
        public object Get(Type type)
        {
            if (Root.ContainsKey(type))
            {
                var ctor = Root[type].GetConstructors();
                var paramsCtor = ctor.First().GetParameters();
                if (paramsCtor.Count() > 0)
                {
                    object[] paramsArray = new object[paramsCtor.Count()];
                    for (int i = 0; i < paramsCtor.Count(); i++)
                    {
                        if (Root.ContainsKey(paramsCtor[i].ParameterType))
                            paramsArray[i] = this.Get(paramsCtor[i].ParameterType);
                        else
                            paramsArray[i] = paramsCtor[i].ParameterType;
                    }
                    return Activator.CreateInstance(Root[type], paramsArray);
                }
                else
                    return Activator.CreateInstance(Root[type]);
            }
            else
                throw new Exception("Зависимость не найдена, необходимо выполнить команду Bind");
        }

        /// <summary>
        /// Получить обьект ассоциируемый с внедряемой зависимостью
        /// </summary>
        /// <typeparam name="T">Тип внедряемой зависимости</typeparam>
        /// <returns></returns>
        public T Get<T>() where T : class
        {
            return (T)Get(typeof(T));
        }

        /// <summary>
        /// Проверка на корректность инъекции 
        /// </summary>
        /// <param name="keyType">Тип внедряемой зависимости</param>
        /// <param name="valueType">Тип реализации</param>
        /// <returns></returns>
        bool IsCorrectInjection(Type keyType, Type valueType)
        {
            if (keyType.IsInterface && valueType.GetInterfaces().Any(t => t == keyType))
                return true;
            else
                return false;
        }
    }
}
