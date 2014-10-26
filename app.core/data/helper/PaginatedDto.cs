using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.data.helper
{
    public class PaginatedDto<TDto>
    {
        public int PageSize { get; set; }

        public int TotalRecords
        {
            get
            {
                if (DataSet == null)
                    return 0;
                return DataSet.Count();
            }
        }

        public int CurrentPage { get; set; }
        public IEnumerable<TDto> DataSet { get; set; }

        public static PaginatedDto<TDto> Transform(IEnumerable<TDto> data, int pageSize = 1, int currentPage = 1)
        {
            return new PaginatedDto<TDto>()
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                DataSet = data
            };
        }

        public void MoveNext()
        {
            if (!HasNextRecord)
                return;
            CurrentPage++;
        }

        public void MovePrevious()
        {
            if (!HasPreviousPage)
                return;
            CurrentPage--;
        }

        public bool HasNextRecord
        {
            get { return CurrentPage < TotalRecords; }
        }

        public bool HasPreviousPage
        {
            get { return CurrentPage > 1; }
        }

        public IEnumerable<TDto> CurrentPagedData
        {
            get
            {
                if (DataSet == null)
                    return null;

                var result = DataSet.Skip((PageSize) * (CurrentPage - 1)).Take(PageSize);
                return result;
            }
        }
    }
}
