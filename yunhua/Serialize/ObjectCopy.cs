using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRAPBase.Entities;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace IRAPBase.Serialize
{
    public static class ObjectCopy
    {

        public static T CopyTo<T>(this BaseEntity source)
        {
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

 
        /// <summary>
        /// 传入类型B的对象b，将b与a相同名称的值进行赋值给创建的a中
        /// </summary>
        /// <typeparam name="A">类型A</typeparam>
        /// <typeparam name="B">类型B</typeparam>
        /// <param name="b">类型为B的参数b</param>
        /// <returns>拷贝b中相同属性的值的a</returns>
        public static A Mapper<A>(this BaseEntity b)
        {
            A a = Activator.CreateInstance<A>();
            try
            {
                Type Typeb = b.GetType();//获得类型  
                Type Typea = typeof(A);
                foreach (PropertyInfo sp in Typeb.GetProperties())//获得类型的属性字段  
                {
                    foreach (PropertyInfo ap in Typea.GetProperties())
                    {
                        if (ap.Name == sp.Name)//判断属性名是否相同  
                        {
                            ap.SetValue(a, sp.GetValue(b, null), null);//获得b对象属性的值复制给a对象的属性  
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return a;
        }


        public static void CopyTo(this BaseEntity b,BaseEntity a)
        {
            try
            {
                Type Typeb = b.GetType();//获得类型  
                foreach (PropertyInfo sp in Typeb.GetProperties())//获得类型的属性字段  
                {
                    foreach (PropertyInfo ap in Typeb.GetProperties())
                    {
                        if (ap.Name == sp.Name)//判断属性名是否相同  
                        {
                            ap.SetValue(a, sp.GetValue(b, null), null);//获得b对象属性的值复制给a对象的属性  
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetTBLName(Type type)
        {
           // Type type = a.GetType();
            try
            {
                TableAttribute temp = (TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), false).First();
                return temp.Name;
            }
            catch 
            {
                return "";
            }
        }
    }
}
