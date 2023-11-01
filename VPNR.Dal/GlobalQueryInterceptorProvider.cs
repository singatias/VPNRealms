using System.Collections.Generic;
using System.Linq;
using PoweredSoft.DynamicQuery.Core;

namespace VPNR.Dal
{
    public class GlobalQueryInterceptorProvider : IQueryInterceptorProvider
    {
        public IEnumerable<IQueryInterceptor> GetInterceptors<TSource, TResult>(IQueryCriteria queryCriteria, IQueryable<TSource> queryable)
        {
            yield return new GlobalFilterInterceptor();
        }
    }
    
    public class GlobalFilterInterceptor : IFilterInterceptor
    {
        public IFilter InterceptFilter(IFilter filter)
        {
            if (filter is ISimpleFilter simpleFilter)
            {
                if (simpleFilter.Type == FilterType.Contains || 
                    simpleFilter.Type == FilterType.StartsWith ||
                    simpleFilter.Type == FilterType.EndsWith)
                {
                    simpleFilter.CaseInsensitive = true;
                }
            }

            return filter;
        }
    }
}