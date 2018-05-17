using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DynamicUIForModels.Dynamic
{
    public class DynamicHelper
    {
        public static List<string> GetProperties(Type type)
        {
            List<string> all = new List<string>();

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))
                {
                    all.Add(property.Name);
                }
                else
                {
                    string prefix = property.Name + ".";

                    foreach (var inner in GetProperties(property.PropertyType))
                    {
                        all.Add(prefix + inner);
                    }
                }
            }

            return all;
        }


        public static void GetPropertiesWithValues(string prefix, Type type, object value, List<KeyValuePair<string, object>> target)
        {
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))
                {
                    target.Add(new KeyValuePair<string, object>(prefix + property.Name, property.GetValue(value)));
                }
                else
                {
                    GetPropertiesWithValues(property.Name + ".", property.PropertyType, property.GetValue(value), target);
                }
            }
        }

        public static void SetObjectProperty(string propertyName, object value, object obj)
        {
            string[] bits = propertyName.Split('.');

            if (bits.Length > 1)
            {
                PropertyInfo propertyToGet = obj.GetType().GetProperty(bits[0]);
                if (propertyToGet != null)
                {
                    var inner = propertyToGet.GetValue(obj, null) ?? Activator.CreateInstance(propertyToGet.PropertyType);
                    propertyToGet.SetValue(obj, inner, null);
                    SetObjectProperty(bits.Skip(1).Aggregate((x, y) => x + "." + y), value, inner);
                }
            }
            else if (bits.Length == 1)
            {
                PropertyInfo propertyToSet = obj.GetType().GetProperty(bits[0]);
                if (propertyToSet != null)
                {
                    //Convert.ChangeType does not handle conversion to nullable types
                    //if the property type is nullable, we need to get the underlying type of the property
                    var targetType = IsNullableType(propertyToSet.PropertyType)
                        ? Nullable.GetUnderlyingType(propertyToSet.PropertyType)
                        : propertyToSet.PropertyType;
                    if (targetType != null) value = Convert.ChangeType(value, targetType);
                    propertyToSet.SetValue(obj, value, null);
                }
            }
        }

        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static List<GenericModel> GetModel(Type type)
        {
            List<GenericModel> props = new List<GenericModel>();
            GetProperties(type).ForEach(x => { props.Add(new GenericModel { Key = x }); });
            return props;
        }

        public static List<GenericModel> GetModel<T>(T value)
        {
            List<GenericModel> props = new List<GenericModel>();
            var all = new List<KeyValuePair<string, object>>();
            GetPropertiesWithValues(string.Empty, typeof(T), value, all);
            all.ForEach(x => { props.Add(new GenericModel { Key = x.Key, Value = Convert.ToString(x.Value) }); });
            return props;
        }
    }
}
