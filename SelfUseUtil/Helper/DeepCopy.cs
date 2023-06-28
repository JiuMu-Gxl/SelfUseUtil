using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SelfUseUtil.Helper
{
    public class DeepCopy
    {
        public async static Task GetDeepCopy()
        {
            var data = new TestObj
            {
                a = "1",
                b = "2",
                c = "3",
                d = new TestObj
                {
                    a = "4",
                    b = "5",
                    c = "6",
                    d = new TestObj
                    {
                        a = "7",
                        b = "8",
                        c = "9",
                    }
                }
            };

            var copyData = DeepCopyByBin(data);
            copyData.a = "10";
            copyData.d.a = "100";
            copyData.d.d.a = "1000";

            Console.WriteLine("------------------- data -------------------");
            Console.WriteLine(JsonConvert.SerializeObject(data));
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("------------------- data -------------------");
            Console.WriteLine(JsonConvert.SerializeObject(copyData));
            Console.WriteLine("--------------------------------------------");
        }

        // 重新映射 可进行深拷贝
        public static T DeepCopyByBin<T>(T obj)
        {
            //循环外创建MapperConfig
            var config = new MapperConfiguration(cfg => cfg.CreateMap<T, T>());
            var mapper = config.CreateMapper();

            //循环内调用
            T newInfo = mapper.Map<T>(obj);
            return newInfo;
        }

        /// <summary>
        /// 序列化反序列化实现深拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopySerializable<T>(T obj)
        {
            if (obj == null) { return default; }

            var res = (T)Activator.CreateInstance(obj.GetType());

            foreach (var field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.FieldType.IsValueType || field.FieldType.Equals(typeof(string)))
                {
                    field.SetValue(res, field.GetValue(obj));
                }
                else
                {
                    var fieldValue = field.GetValue(obj);
                    field.SetValue(res, fieldValue == null ? null : DeepCopySerializable(fieldValue));
                }
            }

            return res;
        }

        /// <summary>
        /// 反射实现深拷贝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopyReflex<T>(T obj)
        {
            if (obj == null) { return default; }

            var res = (T)Activator.CreateInstance(obj.GetType());

            foreach (var field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.FieldType.IsValueType || field.FieldType.Equals(typeof(string)))
                {
                    field.SetValue(res, field.GetValue(obj));
                }
                else
                {
                    var fieldValue = field.GetValue(obj);
                    field.SetValue(res, fieldValue == null ? null : DeepCopyReflex(fieldValue));
                }
            }

            return res;
        }
    }

    [Serializable]
    public class TestObj
    {
        public string a { get; set; }
        public string b { get; set; }
        public string c { get; set; }
        public TestObj d { get; set; }
    }
}
