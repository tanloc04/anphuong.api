using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Ultilities
{
    public static class ExpressionUtils
    {
        /// <summary>
        /// Combines two expression filters using a logical AND.
        /// </summary>
        /// <typeparam name="T">The type of the entity being filtered.</typeparam>
        /// <param name="existingFilter">The existing filter expression.</param>
        /// <param name="newFilter">The new filter expression to be combined with the existing one.</param>
        /// <returns>A new expression representing the combined filters.</returns>
        public static Expression<Func<T, bool>> AddFilter<T>(
            Expression<Func<T, bool>> existingFilter,
            Expression<Func<T, bool>> newFilter)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            var combined = Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(
                    Expression.Invoke(existingFilter, parameter),
                    Expression.Invoke(newFilter, parameter)
                ),
                parameter
            );

            return combined;
        }

        /// <summary>
        /// Combines multiple expression filters using a logical AND.
        /// </summary>
        /// <typeparam name="T">The type of the entity being filtered.</typeparam>
        /// <param name="filters">An array of filters to be combined.</param>
        /// <returns>A new expression representing the combined filters.</returns>
        public static Expression<Func<T, bool>> CombineFilters<T>(params Expression<Func<T, bool>>[] filters)
        {
            if (filters == null || filters.Length == 0)
                throw new ArgumentException("At least one filter must be provided.");

            Expression<Func<T, bool>> combinedFilter = filters[0];

            for (int i = 1; i < filters.Length; i++)
            {
                combinedFilter = AddFilter(combinedFilter, filters[i]);
            }

            return combinedFilter;
        }
    }
}
