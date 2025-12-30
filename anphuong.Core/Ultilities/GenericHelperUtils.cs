using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Ultilities
{
    public static class GenericHelperUtils
    {
        // Reference-type (string) helper
        public static bool SetIfChanged<T>(T? newValue, Func<T?> getter, Action<T?> setter)
        {
            var oldValue = getter();
            if (newValue != null && !Equals(oldValue, newValue))
            {
                setter(newValue);
                return true;
            }
            return false;
        }

        // Non-nullable value-type helper (for product.Price, etc.)
        public static bool SetIfChangedValue<T>(T? newValue, Func<T> getter, Action<T> setter) where T : struct
        {
            if (newValue.HasValue && !EqualityComparer<T>.Default.Equals(getter(), newValue.Value))
            {
                setter(newValue.Value);
                return true;
            }
            return false;
        }

        // Nullable-target value-type helper (for product.DetailImageId, product.CategoryId, product.VariationId)
        public static bool SetIfChangedNullableValue<T>(T? newValue, Func<T?> getter, Action<T?> setter) where T : struct
        {
            var oldValue = getter();
            if (newValue.HasValue)
            {
                // update if old is null or different
                if (!oldValue.HasValue || !EqualityComparer<T>.Default.Equals(oldValue.Value, newValue.Value))
                {
                    setter(newValue); // set nullable
                    return true;
                }
            }
            return false;
        }
    }
}
