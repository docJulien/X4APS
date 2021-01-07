
using System.Collections.Generic;
using System.Linq;
using Kendo.Mvc;

namespace APS.Extensions
{
    public static class Extensions
    {
        public static List<FilterDescriptor> ToFilterDescriptor(this IList<IFilterDescriptor> filters)
        {
            var result = new List<FilterDescriptor>();
            if (filters.Any())
            {
                foreach (var filter in filters)
                {
                    switch (filter)
                    {
                        case FilterDescriptor descriptor:
                            result.Add(descriptor);
                            break;
                        case CompositeFilterDescriptor compositeFilterDescriptor:
                            result.AddRange(compositeFilterDescriptor.FilterDescriptors.ToFilterDescriptor());
                            break;
                    }
                }
            }
            return result;
        }
    }
}