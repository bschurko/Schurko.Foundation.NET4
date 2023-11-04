using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class ImageExtensions
    {
        private static volatile ConstructorInfo _propertyItemConstructor;
        private static readonly object _propertyItemConstructorLock = new object();

        public static PropertyItem CreatePropertyItem(this Image img, int? itemId)
        {
            PropertyItem propitem = null;
            if (itemId.HasValue)
                propitem = img.PropertyItems.FirstOrDefault(p => p.Id == itemId.Value);
            if (propitem == null)
            {
                propitem = CreatePropertyItem(itemId);
                img.SetPropertyItem(propitem);
            }
            return propitem;
        }

        private static PropertyItem CreatePropertyItem(int? itemId)
        {
            if (_propertyItemConstructor == null)
            {
                lock (_propertyItemConstructorLock)
                {
                    if (_propertyItemConstructor == null)
                    {
                        ConstructorInfo constructor = typeof(PropertyItem).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], null);
                        if (constructor != null)
                            _propertyItemConstructor = constructor;
                    }
                }
            }
            if (!(_propertyItemConstructor != null))
                return null;
            PropertyItem propertyItem = (PropertyItem)_propertyItemConstructor.Invoke((object[])null);
            if (itemId.HasValue)
                propertyItem.Id = itemId.Value;
            return propertyItem;
        }
    }
}
