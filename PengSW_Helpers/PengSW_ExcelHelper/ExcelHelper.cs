using Microsoft.Office.Interop.Excel;

namespace PengSW.ExcelHelper
{
    public static class ExcelHelper
    {
        public static string GetCellValue(this Worksheet aWorksheet, int aRow, int aCol)
        {
            Range aCell = aWorksheet.Cells[aRow, aCol] as Range;
            return aCell.Value?.ToString();
        }

        public static string GetCellText(this Worksheet aWorksheet, int aRow, int aCol)
        {
            Range aCell = aWorksheet.Cells[aRow, aCol] as Range;
            return aCell.Text.ToString();
        }

        public static void SetCellValue(this Worksheet aWorksheet, bool aCondition, int aRow, int aCol, object aValue, string aFormat = null)
        {
            if (aCondition && aValue != null) aWorksheet.SetCellValue(aRow, aCol, aValue, aFormat);
        }

        public static void SetCellValue(Worksheet aWorksheet, bool aCondition, int aRow, int aCol, double aValue, string aFormat = null)
        {
            if (aCondition && aValue > 0) aWorksheet.SetCellValue(aRow, aCol, aValue, aFormat);
        }

        public static void SetCellValue(this Worksheet aWorksheet, int aRow, int aCol, object aValue, string aFormat = null)
        {
            if (aValue == null) return;
            Range aCell = aWorksheet.Cells[aRow, aCol] as Range;
            aCell.set_Value(value: aValue);
            if (!string.IsNullOrWhiteSpace(aFormat)) aCell.NumberFormatLocal = aFormat;
        }

        public static void SetCellFont(this Worksheet aWorksheet, int aRow, int aCol, int aFontSize, bool aBold)
        {
            Range aCell = aWorksheet.Cells[aRow, aCol] as Range;
            aCell.Font.Size = aFontSize;
            aCell.Font.Bold = aBold;
        }

        public static void SetRowHeight(this Worksheet aWorksheet, int aRow, double aHeight)
        {
            (aWorksheet.Rows[aRow] as Range).RowHeight = aHeight;
        }

        public static void SetColumnWidth(this Worksheet aWorksheet, int aCol, double aWidth)
        {
            (aWorksheet.Columns[aCol] as Range).ColumnWidth = aWidth;
        }

        public static void SetRowValues(this Worksheet aWorksheet, int aRow, int aCol, params object[] aValues)
        {
            for (int i = 0; i < aValues.Length; i++)
            {
                aWorksheet.SetCellValue(aRow, aCol + i, aValues[i]);
            }
        }

        public static void SetTableHead(this Worksheet aWorksheet, string aName, string aTitle, string[] aColumnHeaders, double[] aColumnWidths)
        {
            aWorksheet.Name = aName;
            aWorksheet.SetRowHeight(1, 32);
            for (int i = 1; i <= aColumnWidths.Length; i++) aWorksheet.SetColumnWidth(i, aColumnWidths[i - 1]);
            aWorksheet.SetCellValue(1, 1, aTitle);
            aWorksheet.SetCellFont(1, 1, 16, true);
            aWorksheet.Range[aWorksheet.Cells[1, 1], aWorksheet.Cells[1, aColumnHeaders.Length]].Merge();
            aWorksheet.SetRowValues(2, 1, aColumnHeaders);
        }

        public static void SetCellBackground(this Worksheet aWorksheet, int aRow, int aCol, int aColor)
        {
            (aWorksheet.Cells[aRow, aCol] as Range).Interior.Color = aColor;
        }

        public static void SetCellColor(this Worksheet aWorksheet, int aRow, int aCol, int aColor)
        {
            (aWorksheet.Cells[aRow, aCol] as Range).Font.Color = aColor;
        }

        public static void SetCellBorder(this Worksheet aWorksheet, int aRow, int aCol)
        {
            Range aCell = aWorksheet.Cells[aRow, aCol] as Range;
            aCell.Borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            aCell.Borders[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
            aCell.Borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
            aCell.Borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
        }

        public static void SetCellBorder(this Worksheet aWorksheet, int aRow0, int aCol0, int aRow1, int aCol1)
        {
            Range aRange = aWorksheet.Range[aWorksheet.Cells[aRow0, aCol0], aWorksheet.Cells[aRow1, aCol1]] as Range;
            aRange.Borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            aRange.Borders[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
            aRange.Borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
            aRange.Borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
            aRange.Borders[XlBordersIndex.xlInsideVertical].LineStyle = XlLineStyle.xlContinuous;
            aRange.Borders[XlBordersIndex.xlInsideHorizontal].LineStyle = XlLineStyle.xlContinuous;
        }

        public static void SetRangeMerge(this Worksheet aWorksheet, int aRow0, int aCol0, int aRow1, int aCol1)
        {
            aWorksheet.Range[aWorksheet.Cells[aRow0, aCol0], aWorksheet.Cells[aRow1, aCol1]].Merge();
        }

        public static void SetCellFormulaR1C1(this Worksheet aWorksheet, int aRow, int aCol, string aFormulaR1C1)
        {
            (aWorksheet.Cells[aRow, aCol] as Range).FormulaR1C1 = aFormulaR1C1;
        }

        public static void SetCellComment(this Worksheet aWorksheet, int aRow, int aCol, string aComment)
        {
            Range aCell = aWorksheet.Cells[aRow, aCol] as Range;
            aCell.AddComment(aComment).Visible = false;
        }
    }
}
