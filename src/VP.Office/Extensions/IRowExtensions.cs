using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VP.Office.Extensions
{
    public static class IRowExtensions
    {
        public static ICell GetOrCreateCell(this IRow row,int colIndex)
        {
            return row.GetCell(colIndex)??row.CreateCell(colIndex);
        }
    }
}
