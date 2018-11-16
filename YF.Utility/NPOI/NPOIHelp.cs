using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace YF.Utility.NPOI
{
    public static class NPOIHelp
    {

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, string SheetName, int HeaderRowIndex)
        {
            //HSSFWorkbook workbook;
            IWorkbook wb;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                wb = WorkbookFactory.Create(file);
            }
            ISheet sheet = wb.GetSheet(SheetName);
            DataTable table = new DataTable();
            table = ImportDt(sheet, HeaderRowIndex, true);
            //ExcelFileStream.Close();
            //workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, int SheetIndex, int HeaderRowIndex)
        {
            //HSSFWorkbook workbook;
            IWorkbook wb;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                wb = WorkbookFactory.Create(file);
            }
            ISheet isheet = wb.GetSheetAt(SheetIndex);
            DataTable table = new DataTable();
            table = ImportDt(isheet, HeaderRowIndex, true);
            //ExcelFileStream.Close();
            //workbook = null;
            isheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, string SheetName, int HeaderRowIndex, bool needHeader)
        {
            //HSSFWorkbook workbook;
            IWorkbook wb;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                wb = WorkbookFactory.Create(file);
            }
            ISheet sheet = wb.GetSheet(SheetName);
            DataTable table = new DataTable();
            table = ImportDt(sheet, HeaderRowIndex, needHeader);
            //ExcelFileStream.Close();
            //workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 读取excel
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet序号</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, int SheetIndex, int HeaderRowIndex, bool needHeader)
        {
            //HSSFWorkbook workbook;
            IWorkbook wb;
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                wb = WorkbookFactory.Create(file);
            }
            ISheet sheet = wb.GetSheetAt(SheetIndex);
            DataTable table = new DataTable();
            table = ImportDt(sheet, HeaderRowIndex, needHeader);
            //ExcelFileStream.Close();
            //workbook = null;
            sheet = null;
            return table;
        }

        /// <summary>
        /// 将制定sheet中的数据导出到datatable中
        /// </summary>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <returns></returns>
        static DataTable ImportDt(ISheet sheet, int HeaderRowIndex, bool needHeader)
        {
            DataTable table = new DataTable();
            IRow headerRow;
            int cellCount;
            try
            {
                if (HeaderRowIndex < 0 || !needHeader)
                {
                    headerRow = sheet.GetRow(0);
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        DataColumn column = new DataColumn(Convert.ToString(i));
                        table.Columns.Add(column);
                    }
                }
                else
                {
                    headerRow = sheet.GetRow(HeaderRowIndex);
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        if (headerRow.GetCell(i) == null)
                        {
                            if (table.Columns.IndexOf(Convert.ToString(i)) > 0)
                            {
                                DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                                table.Columns.Add(column);
                            }
                            else
                            {
                                DataColumn column = new DataColumn(Convert.ToString(i));
                                table.Columns.Add(column);
                            }

                        }
                        else if (table.Columns.IndexOf(headerRow.GetCell(i).ToString()) > 0)
                        {
                            DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                            table.Columns.Add(column);
                        }
                        else
                        {
                            DataColumn column = new DataColumn(headerRow.GetCell(i).ToString());
                            table.Columns.Add(column);
                        }
                    }
                }
                int rowCount = sheet.LastRowNum;
                for (int i = (HeaderRowIndex + 1); i <= sheet.LastRowNum; i++)
                {
                    try
                    {
                        IRow row;
                        if (sheet.GetRow(i) == null)
                        {
                            row = sheet.CreateRow(i);
                        }
                        else
                        {
                            row = sheet.GetRow(i);
                        }

                        DataRow dataRow = table.NewRow();

                        var cellIndex = 0;
                        for (int j = row.FirstCellNum; j <= cellCount; j++)
                        {


                            try
                            {
                                if (row.GetCell(j) != null)
                                {
                                    switch (row.GetCell(j).CellType)
                                    {
                                        case CellType.String:
                                            string str = row.GetCell(j).StringCellValue;
                                            if (str != null && str.Length > 0)
                                            {
                                                dataRow[cellIndex] = str.ToString();
                                            }
                                            else
                                            {
                                                dataRow[cellIndex] = null;
                                            }
                                            break;
                                        case CellType.Numeric:
                                            if (DateUtil.IsCellDateFormatted(row.GetCell(j)))
                                            {
                                                dataRow[cellIndex] = DateTime.FromOADate(row.GetCell(j).NumericCellValue);
                                            }
                                            else
                                            {
                                                dataRow[cellIndex] = Convert.ToDecimal(row.GetCell(j).NumericCellValue);
                                            }
                                            break;
                                        case CellType.Boolean:
                                            dataRow[cellIndex] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                            break;
                                        case CellType.Error:
                                            dataRow[cellIndex] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                            break;
                                        case CellType.Formula:
                                            switch (row.GetCell(j).CachedFormulaResultType)
                                            {
                                                case CellType.String:
                                                    string strFORMULA = row.GetCell(j).StringCellValue;
                                                    if (strFORMULA != null && strFORMULA.Length > 0)
                                                    {
                                                        dataRow[cellIndex] = strFORMULA.ToString();
                                                    }
                                                    else
                                                    {
                                                        dataRow[cellIndex] = null;
                                                    }
                                                    break;
                                                case CellType.Numeric:
                                                    dataRow[cellIndex] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                                    break;
                                                case CellType.Boolean:
                                                    dataRow[cellIndex] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                                    break;
                                                case CellType.Error:
                                                    dataRow[cellIndex] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                                    break;
                                                default:
                                                    dataRow[cellIndex] = "";
                                                    break;
                                            }
                                            break;
                                        default:
                                            dataRow[cellIndex] = "";
                                            break;
                                    }
                                }

                            }
                            catch (Exception exception)
                            {
                                //  wl.WriteLogs(exception.ToString());
                                throw exception;
                            }
                            cellIndex++;
                        }
                        table.Rows.Add(dataRow);
                    }
                    catch (Exception exception)
                    {
                        //wl.WriteLogs(exception.ToString());
                        throw exception;
                    }
                }
            }
            catch (Exception exception)
            {
                //wl.WriteLogs(exception.ToString());
                throw exception;
            }
            return table;
        }


        /// <summary>
        /// 验证导入的Excel是否有数据
        /// </summary>
        /// <param name="excelFileStream"></param>
        /// <returns></returns>
        public static bool HasData(Stream excelFileStream)
        {
            using (excelFileStream)
            {
                IWorkbook workBook = WorkbookFactory.Create(excelFileStream);
                if (workBook.NumberOfSheets > 0)
                {
                    ISheet sheet = workBook.GetSheetAt(0);
                    return sheet.PhysicalNumberOfRows > 0;
                }
            }
            return false;
        }


        /// <summary>
        /// 从模板导出Excel 2018/09/11 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dtoList"></param>
        /// <param name="tPath"></param>
        /// <param name="sheet"></param>
        /// <param name="headerIndex"></param>
        /// <param name="templateIndex"></param>
        /// <param name="rowDenormaliser"></param>
        /// <returns></returns>
        public static MemoryStream ExportByTemplate<T>(List<T> dtoList, string tPath, string sheet = "Sheet1", int headerIndex = 0, int templateIndex = 1, bool rowDenormaliser = false)
        {
            try
            {
                IWorkbook book;
                //打开模板文件到文件流中
                using (FileStream file = new FileStream(tPath, FileMode.Open, FileAccess.Read))
                {
                    //将文件流中模板加载到工作簿对象中
                    //  hssfworkbook = new XSSFWorkbook(file);
                    book = WorkbookFactory.Create(file);
                }

                if (dtoList != null)
                {
                    book.SetDataSource(dtoList, sheet, headerIndex, templateIndex, rowDenormaliser);
                }


                MemoryStream ms = new MemoryStream();
                //将工作簿的内容放到内存流中
                book.Write(ms);
                ms.Seek(0, SeekOrigin.Begin);
                //将内存流转换成字节数组发送到客户端
                return ms;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #region  扩展模板导出 余庆元:20150301  ,20181115迁移到 .net core


        public static bool isNumeric(String message, out double result)
        {
            Regex rex = new Regex(@"^[-]?\d+[.]?\d*$");
            result = -1;
            if (rex.IsMatch(message))
            {
                result = double.Parse(message);
                return true;
            }
            else
                return false;

        }

        public enum TemplateType { DtField, Formula, Constant, Variable, Null };

        public struct Template
        {
            public string TemplateVal;
            public ICellStyle CellStyle;
        }

        public struct TemplateFormat
        {
            public TemplateType Type;
            public string TemplateVal;
            public ICellStyle CellStyle;
        }



        /// <summary>
        /// 根据模板填充数据
        /// </summary>
        /// <param name="workbook">当前Excel对象</param>
        /// <param name="dt">数据源</param>
        /// <param name="headerIndex">表头开始索引</param>
        /// <param name="templateIndex">模板开始索引</param>
        /// <param name="rowDenormaliser">是否行转列展示</param>
        public static void SetDataSource(this IWorkbook workbook, DataTable dt, string sheetName = "Sheet1", int headerIndex = 0, int templateIndex = 1, bool rowDenormaliser = false)
        {
            List<Template> template = new List<Template>();
            //Dictionary<string, ICellStyle> template = new Dictionary<string, ICellStyle>();
            ISheet sheet = workbook.GetSheet(sheetName);
            float rowheigh = 50;

            if (rowDenormaliser)
            {
                int rowCount = sheet.LastRowNum;
                for (int i = 0; i <= rowCount; i++)
                {
                    try
                    {
                        IRow templateRow;
                        templateRow = sheet.GetRow(i) ?? sheet.GetRow(i);
                        rowheigh = templateRow.HeightInPoints;
                        Template t = new Template();
                        if (templateRow.GetCell(templateIndex) != null)
                        {
                            string str = templateRow.GetCell(templateIndex).StringCellValue;

                            t.TemplateVal = str ?? string.Empty;
                            t.CellStyle = templateRow.GetCell(templateIndex).CellStyle;
                            template.Add(t);

                        }
                        else
                        {
                            t.TemplateVal = string.Empty;
                            t.CellStyle = workbook.CreateCellStyle();
                            template.Add(t);
                            // template.Add(string.Empty, null);
                        }

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                try
                {
                    IRow headerRow;
                    headerRow = sheet.GetRow(headerIndex);
                    IRow templateRow = sheet.GetRow(templateIndex);
                    rowheigh = templateRow.HeightInPoints;
                    int cellCount = 0;
                    cellCount = headerRow.LastCellNum >= templateRow.LastCellNum ?
                        headerRow.LastCellNum :
                        templateRow.LastCellNum;
                    for (int i = 0; i < cellCount; i++)
                    {
                        string str = templateRow.GetCell(i).StringCellValue;
                        //template.Add(str ?? string.Empty, templateRow.GetCell(i).CellStyle);
                        Template t = new Template();
                        t.TemplateVal = str ?? string.Empty;
                        t.CellStyle = templateRow.GetCell(i).CellStyle;
                        template.Add(t);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            int writeIndex = templateIndex;
            int dtrowindex = 0;

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < template.Count; i++)
                    {
                        try
                        {
                            ICell cell;
                            if (rowDenormaliser)
                            {
                                cell = sheet.GetRow(i).CreateCell(writeIndex);
                            }
                            else
                            {
                                IRow row = sheet.GetRow(writeIndex) ?? sheet.CreateRow(writeIndex);
                                row.HeightInPoints = rowheigh;
                                cell = row.CreateCell(i);
                            }
                            string dtColumn;
                            TemplateFormat tFormat = checkTemplateType(template[i], dr, out dtColumn, rowDenormaliser ? i : dtrowindex + 1);
                            string[] tpl = tFormat.TemplateVal.Split('#');
                            string cellFormatStr = "";
                            if (tpl.Length == 2)
                            {
                                cellFormatStr = tpl[1];
                            }
                            switch (tFormat.Type)
                            {
                                case TemplateType.DtField:
                                    //dr[]
                                    DataColumn column = dt.Columns[dtColumn];
                                    if (column == null)
                                    {
                                        //cell.SetCellValue(string.Format("数据源不存在列  {0}", dtColumn));
                                        cell.SetCellValue("");
                                        break;
                                    }
                                    //formatCellTypeByDT(column, workbook, cell, dr[string.Format(tpl[0], dtColumn)].ToString().Trim(), tFormat.cellStyle, cellFormatStr);
                                    formatCellTypeByDT(column, workbook, cell, tpl[0].ToString().Trim(), tFormat.CellStyle, cellFormatStr);

                                    break;
                                case TemplateType.Constant:
                                    double intV = 0;

                                    if (isNumeric(tpl[0], out intV))
                                    {
                                        // double.TryParse(drValue, out intV);
                                        cell.SetCellValue(intV);
                                    }
                                    else
                                    {
                                        cell.SetCellValue(tpl[0]);
                                    }
                                    //cell.CellStyle = workbook.CreateCellStyle();
                                    cell.CellStyle.CloneStyleFrom(tFormat.CellStyle);
                                    cell.CellStyle.CellStyleDataFormat(workbook, formatCellsty(cellFormatStr));
                                    //if(string.IsNullOrEmpty(cellFormatStr)
                                    // cell.CellStyle = Getcellstyle(workbook, formatCellsty(cellsty));//格式化显示
                                    break;
                                case TemplateType.Formula:
                                    // cell.SetCellFormula();
                                    // string formula = string.IsNullOrEmpty(dtColumn) ? tpl[0] : tpl[0].Replace("{" + dtColumn + "}", dr[dtColumn].ToString().Trim());

                                    formatCellFormula(workbook, cell, tFormat.CellStyle, tpl[0], cellFormatStr,
                                        rowDenormaliser == true ? writeIndex : i,
                                         rowDenormaliser == true ? i + 1 : writeIndex + 1);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    dtrowindex++;
                    writeIndex++;
                }
            }
            else
            {
                for (int i = 0; i < template.Count; i++)
                {
                    try
                    {
                        ICell cell;
                        if (rowDenormaliser)
                        {
                            cell = sheet.GetRow(i).CreateCell(writeIndex);
                        }
                        else
                        {
                            IRow row = sheet.GetRow(writeIndex) ?? sheet.CreateRow(writeIndex);
                            row.HeightInPoints = rowheigh;
                            cell = row.CreateCell(i);
                        }

                        cell.SetCellValue(i == 0 ? "暂无相关数据！" : "");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }


            //强制Excel重新计算表中所有的公式
            sheet.ForceFormulaRecalculation = true;
            //sheet.GetRow(1).CreateCell(1).SetCellValue(200200);
            // return sheet;
        }


        public static void SetDataSource<T>(this IWorkbook workbook, List<T> lists, string sheetName = "Sheet1", int headerIndex = 0, int templateIndex = 1, bool rowDenormaliser = false)
        {
            // ====== list to Datable 转化
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable dt = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in lists)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = props[i].GetValue(item) ?? DBNull.Value;
                dt.Rows.Add(values);
            }
            // ====== list to Datable 转化 End

            List<Template> template = new List<Template>();
            //Dictionary<string, ICellStyle> template = new Dictionary<string, ICellStyle>();
            ISheet sheet = workbook.GetSheet(sheetName);
            float rowheigh = 50;

            if (rowDenormaliser)
            {
                int rowCount = sheet.LastRowNum;
                for (int i = 0; i <= rowCount; i++)
                {
                    try
                    {
                        IRow templateRow;
                        templateRow = sheet.GetRow(i) ?? sheet.GetRow(i);
                        rowheigh = templateRow.HeightInPoints;
                        Template t = new Template();
                        if (templateRow.GetCell(templateIndex) != null)
                        {
                            string str = templateRow.GetCell(templateIndex).StringCellValue;

                            t.TemplateVal = str ?? string.Empty;
                            t.CellStyle = templateRow.GetCell(templateIndex).CellStyle;
                            template.Add(t);

                        }
                        else
                        {
                            t.TemplateVal = string.Empty;
                            t.CellStyle = workbook.CreateCellStyle();
                            template.Add(t);
                            // template.Add(string.Empty, null);
                        }

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
            {
                try
                {
                    IRow headerRow;
                    headerRow = sheet.GetRow(headerIndex);
                    IRow templateRow = sheet.GetRow(templateIndex);
                    rowheigh = templateRow.HeightInPoints;
                    int cellCount = 0;
                    cellCount = headerRow.LastCellNum >= templateRow.LastCellNum ?
                        headerRow.LastCellNum :
                        templateRow.LastCellNum;
                    for (int i = 0; i < cellCount; i++)
                    {
                        string str = templateRow.GetCell(i).StringCellValue;
                        //template.Add(str ?? string.Empty, templateRow.GetCell(i).CellStyle);
                        Template t = new Template();
                        t.TemplateVal = str ?? string.Empty;
                        t.CellStyle = templateRow.GetCell(i).CellStyle;
                        template.Add(t);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            int writeIndex = templateIndex;
            int dtrowindex = 0;

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < template.Count; i++)
                    {
                        try
                        {
                            ICell cell;
                            if (rowDenormaliser)
                            {
                                cell = sheet.GetRow(i).CreateCell(writeIndex);
                            }
                            else
                            {
                                IRow row = sheet.GetRow(writeIndex) ?? sheet.CreateRow(writeIndex);
                                row.HeightInPoints = rowheigh;
                                cell = row.CreateCell(i);
                            }
                            string dtColumn;
                            TemplateFormat tFormat = checkTemplateType(template[i], dr, out dtColumn, rowDenormaliser ? i : dtrowindex + 1);
                            string[] tpl = tFormat.TemplateVal.Split('#');
                            string cellFormatStr = "";
                            if (tpl.Length == 2)
                            {
                                cellFormatStr = tpl[1];
                            }
                            switch (tFormat.Type)
                            {
                                case TemplateType.DtField:
                                    //dr[]
                                    DataColumn column = dt.Columns[dtColumn];
                                    if (column == null)
                                    {
                                        // cell.SetCellValue(string.Format("数据源不存在列  {0}", dtColumn));
                                        cell.SetCellValue("");
                                        break;
                                    }
                                    //formatCellTypeByDT(column, workbook, cell, dr[string.Format(tpl[0], dtColumn)].ToString().Trim(), tFormat.cellStyle, cellFormatStr);
                                    formatCellTypeByDT(column, workbook, cell, tpl[0].ToString().Trim(), tFormat.CellStyle, cellFormatStr);

                                    break;
                                case TemplateType.Constant:
                                    double intV = 0;

                                    if (isNumeric(tpl[0], out intV))
                                    {
                                        // double.TryParse(drValue, out intV);
                                        cell.SetCellValue(intV);
                                    }
                                    else
                                    {
                                        cell.SetCellValue(tpl[0]);
                                    }
                                    //cell.CellStyle = workbook.CreateCellStyle();
                                    cell.CellStyle.CloneStyleFrom(tFormat.CellStyle);
                                    cell.CellStyle.CellStyleDataFormat(workbook, formatCellsty(cellFormatStr));
                                    //if(string.IsNullOrEmpty(cellFormatStr)
                                    // cell.CellStyle = Getcellstyle(workbook, formatCellsty(cellsty));//格式化显示
                                    break;
                                case TemplateType.Formula:
                                    // cell.SetCellFormula();
                                    // string formula = string.IsNullOrEmpty(dtColumn) ? tpl[0] : tpl[0].Replace("{" + dtColumn + "}", dr[dtColumn].ToString().Trim());

                                    formatCellFormula(workbook, cell, tFormat.CellStyle, tpl[0], cellFormatStr,
                                        rowDenormaliser == true ? writeIndex : i,
                                         rowDenormaliser == true ? i + 1 : writeIndex + 1);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    dtrowindex++;
                    writeIndex++;
                }
            }
            else
            {
                for (int i = 0; i < template.Count; i++)
                {
                    try
                    {
                        ICell cell;
                        if (rowDenormaliser)
                        {
                            cell = sheet.GetRow(i).CreateCell(writeIndex);
                        }
                        else
                        {
                            IRow row = sheet.GetRow(writeIndex) ?? sheet.CreateRow(writeIndex);
                            row.HeightInPoints = rowheigh;
                            cell = row.CreateCell(i);
                        }

                        cell.SetCellValue(i == 0 ? "暂无相关数据！" : "");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }


            //强制Excel重新计算表中所有的公式
            sheet.ForceFormulaRecalculation = true;
            //sheet.GetRow(1).CreateCell(1).SetCellValue(200200);
            // return sheet;
        }

        public static void SetFixedData(this IWorkbook workbook, NPOIDataSource nds, string sheetName = "Sheet1")
        {
            ISheet sheet = workbook.GetSheet(sheetName);

            int rowCount = sheet.LastRowNum;
            float rowheigh = 50;
            //  int cellCount =

            for (int i = 0; i <= rowCount; i++)
            {
                try
                {
                    IRow templateRow;
                    templateRow = sheet.GetRow(i) ?? sheet.GetRow(i);
                    rowheigh = templateRow.HeightInPoints;

                    int cellCount = templateRow.LastCellNum;

                    for (int j = 0; j < cellCount; j++)
                    {
                        ICell cell = templateRow.GetCell(j);
                        string str = GetCellValue(cell).ToString();


                        if (!string.IsNullOrEmpty(str))
                        {
                            Template t = new Template();
                            t.TemplateVal = str ?? string.Empty;
                            // t.CellStyle = cell.CellStyle;
                            // template.Add(t);
                            DataRow dr = null;
                            string dtColumn = "";
                            TemplateFormat tFormat = checkTemplateType(t, dr, out dtColumn, i);
                            string[] tpl = tFormat.TemplateVal.Split('#');
                            string cellFormatStr = "";
                            if (tpl.Length == 2)
                            {
                                cellFormatStr = tpl[1];
                            }
                            switch (tFormat.Type)
                            {
                                case TemplateType.Constant:
                                    double intV = 0;

                                    if (isNumeric(tpl[0], out intV))
                                    {
                                        // double.TryParse(drValue, out intV);
                                        cell.SetCellValue(intV);
                                    }
                                    else
                                    {
                                        cell.SetCellValue(tpl[0]);
                                    }
                                    //cell.CellStyle = workbook.CreateCellStyle();
                                    // cell.CellStyle.CloneStyleFrom(tFormat.CellStyle);
                                    cell.CellStyle.CellStyleDataFormat(workbook, formatCellsty(cellFormatStr));
                                    //if(string.IsNullOrEmpty(cellFormatStr)
                                    // cell.CellStyle = Getcellstyle(workbook, formatCellsty(cellsty));//格式化显示
                                    break;
                                case TemplateType.Variable:
                                    cell.SetCellValue(nds.Source[tpl[0]].ToString());
                                    // cell.CellStyle.CloneStyleFrom(tFormat.CellStyle);
                                    cell.CellStyle.CellStyleDataFormat(workbook, formatCellsty(cellFormatStr));
                                    break;
                                case TemplateType.Formula:
                                    // cell.SetCellFormula();
                                    // string formula = string.IsNullOrEmpty(dtColumn) ? tpl[0] : tpl[0].Replace("{" + dtColumn + "}", dr[dtColumn].ToString().Trim());
                                    //tpl[0].Replace("",nds.Source[tpl[0]].ToString())

                                    foreach (var item in nds.Source)
                                    {
                                        tpl[0].Replace("{" + item.Key + "}", item.Value.ToString());
                                    }

                                    formatCellFormula(workbook, cell, tFormat.CellStyle, tpl[0], cellFormatStr, j, i);
                                    break;
                            }
                        }
                        //template.Add(str ?? string.Empty, templateRow.GetCell(i).CellStyle);




                        //  templateRow.GetCell(j).SetCellValue("");
                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }


        }

        //public static void ListToExcel<T>(this List<T> list, IWorkbook workbook, string sheetName = "Sheet1", int headerIndex = 0, int templateIndex = 1, bool rowDenormaliser = false)
        // {

        // }

        /// <summary>
        /// 格式化样式
        /// </summary>
        /// <param name="cellsty">样式字符串</param>
        /// <returns></returns>
        private static stylexls formatCellsty(string cellsty)
        {
            stylexls sty = stylexls.thedefault;
            if (!string.IsNullOrEmpty(cellsty))
            {
                switch (cellsty)
                {
                    case "[h]":
                        sty = stylexls.head;
                        break;
                    case "[u]":
                        sty = stylexls.url;
                        break;
                    case "[d]":
                        sty = stylexls.datetime;
                        break;
                    case "[n]":
                        sty = stylexls.number;
                        break;
                    case "[$]":
                        sty = stylexls.money;
                        break;
                    case "[%]":
                        sty = stylexls.percent;
                        break;
                    case "[c]":
                        sty = stylexls.chbig;
                        break;
                    case "[e]":
                        sty = stylexls.scientificnotation;
                        break;
                    default:
                        sty = stylexls.thedefault;
                        break;

                }
            }
            return sty;
        }

        /// <summary>
        /// 根据Datatable数据格式，格式化Cell 格式
        /// </summary>
        /// <param name="column">数据列</param>
        /// <param name="workbook">工作簿</param>
        /// <param name="newCell">单元格</param>
        /// <param name="drValue">赋值</param>
        private static void formatCellTypeByDT(DataColumn column, IWorkbook workbook, ICell newCell, string drValue, ICellStyle cellStyle, string cellFroamtStr = "")
        {
            try
            {


                //HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                //HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
                //dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
                // newCell.CellStyle = workbook.CreateCellStyle();
                newCell.CellStyle.CloneStyleFrom(cellStyle);
                switch (column.DataType.ToString())
                {
                    case "System.String"://字符串类型

                        newCell.SetCellValue(drValue);

                        break;
                    case "System.DateTime"://日期类型

                        if (string.IsNullOrEmpty(drValue))
                        {
                            newCell.SetCellValue(drValue);
                        }
                        else
                        {
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);
                        }


                        newCell.CellStyle.CellStyleDataFormat(workbook, stylexls.datetime);
                        break;
                    case "System.Boolean"://布尔型
                        bool boolV = false;
                        bool.TryParse(drValue, out boolV);
                        newCell.SetCellValue(boolV);
                        break;
                    case "System.Int16"://整型
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Byte":
                        int intV = 0;
                        int.TryParse(drValue, out intV);

                        if (string.IsNullOrEmpty(drValue))
                        {
                            newCell.SetCellValue(drValue);
                        }
                        else
                        {
                            newCell.SetCellValue(intV);
                        }

                        break;
                    case "System.Decimal"://浮点型
                    case "System.Double":
                        double doubV = 0;
                        double.TryParse(drValue, out doubV);
                        if (string.IsNullOrEmpty(drValue))
                        {
                            newCell.SetCellValue(drValue);
                        }
                        else
                        {
                            newCell.SetCellValue(doubV);
                        }


                        break;
                    case "System.DBNull"://空值处理
                        newCell.SetCellValue("");

                        break;
                    default:
                        newCell.SetCellValue("");

                        break;
                }

                if (!string.IsNullOrEmpty(cellFroamtStr))
                {
                    newCell.CellStyle.CellStyleDataFormat(workbook, formatCellsty(cellFroamtStr));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        /// <summary>
        /// 格式化公式
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="cellindex"></param>
        /// <param name="rowindex"></param>
        /// <returns></returns>
        private static void formatCellFormula(IWorkbook workbook, ICell newCell, ICellStyle cellStyle, string formula, string cellFromatSty = "", int cellindex = 0, int rowindex = 0)
        {
            try
            {
                // newCell.CellStyle = workbook.CreateCellStyle();
                newCell.CellStyle.CloneStyleFrom(cellStyle);
                // formula.rep
                if (formula.IndexOf("{c}") > -1)
                {
                    formula = formula.Replace("{c}", ExcelConvert.ToName(cellindex));
                }

                if (formula.IndexOf("{r}") > -1)
                {
                    formula = formula.Replace("{r}", rowindex.ToString());
                }
                newCell.SetCellFormula(formula);

                newCell.CellStyle.CellStyleDataFormat(workbook, formatCellsty(cellFromatSty));//格式化显示

            }
            catch (Exception ex)
            {
                throw ex;
            }


            // return formula;
        }

        //public static ICellStyle CreateCellStyle(IWorkbook wb, ICellStyle fromStyle)
        //{
        //    ICellStyle newStyle = styleCache[fromStyle];
        //    if (newStyle == null)
        //    {
        //        newStyle = wb.CreateCellStyle();
        //        styleCache[newStyle] = newStyle;
        //    }
        //    //ICellStyle newStyle = wb.CreateCellStyle();   
        //    return newStyle;
        //}  

        #region 定义单元格常用到样式的枚举
        public enum stylexls
        {
            head,
            url,
            datetime,
            number,
            money,
            percent,
            chbig,
            scientificnotation,
            thedefault
        }
        #endregion


        #region 定义单元格常用到样式
        //private   static ICellStyle Getcellstyle(IWorkbook wb, stylexls str)
        //  {
        //      ICellStyle cellStyle = wb.CreateCellStyle();

        //      //定义几种字体  
        //      //也可以一种字体，写一些公共属性，然后在下面需要时加特殊的  
        //      IFont font12 = wb.CreateFont();
        //      font12.FontHeightInPoints = 12;
        //     // font12.FontName = "微软雅黑";
        //      font12.Boldweight = (short)FontBoldWeight.Bold;


        //      IFont font = wb.CreateFont();
        //      font12.FontHeightInPoints = 10;
        //      //font.FontName = "微软雅黑";
        //      //font.Underline = 1;下划线  


        //      IFont fontcolorblue = wb.CreateFont();

        //      fontcolorblue.Color = HSSFColor.OliveGreen.Blue.Index;
        //      fontcolorblue.IsItalic = true;//下划线  
        //      fontcolorblue.FontName = "微软雅黑";


        //      ////边框  
        //      //cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Dotted;
        //      //cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Hair;
        //      //cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Hair;
        //      //cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Dotted;
        //      ////边框颜色  
        //      //cellStyle.BottomBorderColor = HSSFColor.OliveGreen.Blue.Index;
        //      //cellStyle.TopBorderColor = HSSFColor.OliveGreen.Blue.Index;

        //      ////背景图形，我没有用到过。感觉很丑  
        //      ////cellStyle.FillBackgroundColor = HSSFColor.OLIVE_GREEN.BLUE.index;  
        //      ////cellStyle.FillForegroundColor = HSSFColor.OLIVE_GREEN.BLUE.index;  
        //      //cellStyle.FillForegroundColor = HSSFColor.White.Index;
        //      //// cellStyle.FillPattern = FillPatternType.NO_FILL;  
        //      //cellStyle.FillBackgroundColor = HSSFColor.Blue.Index;

        //      ////水平对齐  
        //      //cellStyle.Alignment = HorizontalAlignment.Center;

        //      ////垂直对齐  
        //      //cellStyle.VerticalAlignment = VerticalAlignment.Center;

        //      ////自动换行  
        //      //cellStyle.WrapText = true;


        //      ////缩进;当设置为1时，前面留的空白太大了。希旺官网改进。或者是我设置的不对  
        //      //cellStyle.Indention = 0;

        //      //上面基本都是设共公的设置  
        //      //下面列出了常用的字段类型  
        //      switch (str)
        //      {
        //          case stylexls.head:
        //              // cellStyle.FillPattern = FillPatternType.LEAST_DOTS;  
        //              cellStyle.SetFont(font12);
        //              break;
        //          case stylexls.datetime:
        //              IDataFormat datastyle = wb.CreateDataFormat();

        //              cellStyle.DataFormat = datastyle.GetFormat("yyyy/mm/dd");
        //             // cellStyle.SetFont(font);
        //              break;
        //          case stylexls.number:
        //              cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
        //             // cellStyle.SetFont(font);
        //              break;
        //          case stylexls.money:
        //              IDataFormat format = wb.CreateDataFormat();
        //              cellStyle.DataFormat = format.GetFormat("￥#,##0");
        //             // cellStyle.SetFont(font);
        //              break;
        //          case stylexls.url:
        //              fontcolorblue.Underline = FontUnderlineType.Single;
        //              cellStyle.SetFont(fontcolorblue);
        //              break;
        //          case stylexls.percent:
        //              cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
        //             // cellStyle.SetFont(font);
        //              break;
        //          case stylexls.chbig:
        //              IDataFormat format1 = wb.CreateDataFormat();
        //              cellStyle.DataFormat = format1.GetFormat("[DbNum2][$-804]0");
        //             // cellStyle.SetFont(font);
        //              break;
        //          case stylexls.scientificnotation:
        //              cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00E+00");
        //             // cellStyle.SetFont(font);
        //              break;
        //          case stylexls.thedefault:
        //             // cellStyle.SetFont(font);
        //              break;
        //      }
        //      return cellStyle;


        //  }

        private static void CellStyleDataFormat(this ICellStyle cellStyle, IWorkbook wb, stylexls str)
        {
            IFont baseFont = cellStyle.GetFont(wb);
            //IFont font12 = wb.CreateFont();
            IFont font12 = wb.CreateFont();
            font12.FontName = baseFont.FontName;
            font12.FontHeightInPoints = 12;
            //font12.FontName = "微软雅黑";
            font12.Boldweight = (short)FontBoldWeight.Bold;
            font12.Color = baseFont.Color;
            // cellStyle.GetFont(wb);

            //IFont font = wb.CreateFont();
            //font.FontHeightInPoints = 10;
            //font.FontName = baseFont.FontName;
            ////font.Underline = 1;下划线  


            IFont fontcolorblue = wb.CreateFont();

            fontcolorblue.Color = HSSFColor.OliveGreen.Blue.Index;
            fontcolorblue.IsItalic = true;//下划线  
            fontcolorblue.FontName = baseFont.FontName;
            switch (str)
            {
                case stylexls.head:
                    // cellStyle.FillPattern = FillPatternType.LEAST_DOTS;  
                    cellStyle.SetFont(font12);
                    break;
                case stylexls.datetime:
                    IDataFormat datastyle = wb.CreateDataFormat();

                    cellStyle.DataFormat = datastyle.GetFormat("yyyy/m/d");
                    // cellStyle.SetFont(font);
                    break;
                case stylexls.number:
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
                    // cellStyle.SetFont(font);
                    break;
                case stylexls.money:
                    IDataFormat format = wb.CreateDataFormat();
                    cellStyle.DataFormat = format.GetFormat("¥#,##0;¥-#,##0");
                    // cellStyle.SetFont(font);
                    break;
                case stylexls.url:
                    fontcolorblue.Underline = FontUnderlineType.Single;
                    cellStyle.SetFont(fontcolorblue);
                    break;
                case stylexls.percent:
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
                    // cellStyle.SetFont(font);
                    break;
                case stylexls.chbig:
                    IDataFormat format1 = wb.CreateDataFormat();
                    cellStyle.DataFormat = format1.GetFormat("[DbNum2][$-804]0");
                    // cellStyle.SetFont(font);
                    break;
                case stylexls.scientificnotation:
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00E+00");
                    // cellStyle.SetFont(font);
                    break;
                case stylexls.thedefault:
                    // cellStyle.SetFont(font);
                    break;
            }
        }

        #endregion

        /// <summary>
        /// 检查模板数据类型，并格式化
        /// </summary>
        /// <param name="tplVal"></param>
        /// <param name="dtColumn"></param>
        /// <param name="AutoNo"></param>
        /// <returns></returns>
        private static TemplateFormat checkTemplateType(Template tplVal, DataRow dr, out string dtColumn, int AutoNo = 1)
        {
            // return "";
            TemplateFormat tReq = new TemplateFormat();
            //tReq.cellStyle=new 
            tplVal.TemplateVal = tplVal.TemplateVal.Replace("{n}", AutoNo.ToString());
            tReq.CellStyle = tplVal.CellStyle;
            if (tplVal.TemplateVal.IndexOf("#=") == 0)
            {
                tReq.Type = TemplateType.Constant;
                tReq.TemplateVal = tplVal.TemplateVal.Replace("#=", "");

                dtColumn = null;
            }
            else if (tplVal.TemplateVal.IndexOf("&=") == 0)
            {
                tReq.Type = TemplateType.DtField;
                string[] tpl = tplVal.TemplateVal.Split('#');
                dtColumn = tpl[0].Replace("&=", "");

                DataColumn column = dr.Table.Columns[dtColumn];

                tReq.TemplateVal = column == null ?
                        tplVal.TemplateVal.Replace("&=" + dtColumn, "数据表不存在列 " + dtColumn) :
                        tReq.TemplateVal = tplVal.TemplateVal.Replace("&=" + dtColumn, dr[dtColumn].ToString());

            }
            else if (tplVal.TemplateVal.IndexOf("&&=") == 0)
            {
                tReq.Type = TemplateType.Formula;
                tReq.TemplateVal = tplVal.TemplateVal.Replace("&&=", "");

                //IF(B1==0,"N/A",{&=ACR})
                List<string> Keys = new List<string>();
                if (tReq.TemplateVal.IndexOf("&=") > -1 && dr != null)
                {
                    Keys = GetValue(tReq.TemplateVal, "{&=", "}");
                    // tReq.templateVal = tReq.templateVal.Replace("&=" , "");

                    foreach (string str in Keys)
                    {
                        DataColumn column = dr.Table.Columns[str];
                        if (column == null)
                        {
                            tReq.Type = TemplateType.Constant;
                            tReq.TemplateVal = "数据表不存在列 " + str;
                            break;
                        }
                        else
                        {
                            tReq.TemplateVal = tReq.TemplateVal.Replace("{&=" + str + "}", dr[str].ToString());
                        }
                        // tReq.templateVal = column == null ?
                        //"数据表不存在列" + str :
                        //tReq.templateVal = tReq.templateVal.Replace("{&=" + str + "}", dr[str].ToString());
                    }
                    dtColumn = null;
                }
                else if (tplVal.TemplateVal.IndexOf("&$=") == 0)
                {
                    Keys = GetValue(tReq.TemplateVal, "{&$=", "}");
                    //tReq.Type = TemplateType.Variable;
                    foreach (string str in Keys)
                    {
                        tReq.TemplateVal = tplVal.TemplateVal.Replace("{&$=" + str + "}", "{" + str + "}"); ;
                    }

                    dtColumn = null;
                }
                else
                {
                    dtColumn = null;
                }

            }
            else if (tplVal.TemplateVal.IndexOf("&$=") == 0)
            {
                tReq.Type = TemplateType.Variable;
                tReq.TemplateVal = tplVal.TemplateVal.Replace("&$=", ""); ;
                dtColumn = null;
            }
            else
            {
                tReq.Type = TemplateType.Null;
                tReq.TemplateVal = "";

                dtColumn = null;
            }

            return tReq;
        }

        private static object GetCellValue(ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Blank:
                case CellType.Formula:
                case CellType.String:
                case CellType.Unknown:
                    return cell.StringCellValue;
                case CellType.Numeric:
                    return cell.NumericCellValue;
                case CellType.Error:
                    return cell.ErrorCellValue;
                default:
                    return cell.StringCellValue;
            }
        }

        /// <summary>
        /// 获得字符串中开始和结束字符串中间得值
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="s">开始</param>
        /// <param name="e">结束</param>
        /// <returns></returns> 
        private static List<string> GetValue(string str, string s, string e)
        {
            Regex rg = new Regex("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            Match mat = rg.Match(str);
            List<string> reqstr = new List<string>();
            while (mat.Success)
            {

                reqstr.Add(mat.Value);
                // MessageBox.Show(mat.Index.ToString());//位置
                mat = rg.Match(str, mat.Index + mat.Length);
            }

            return reqstr.Distinct().ToList(); ;
            // return rg.Match(str).Value;
        }

        #endregion


        #region 提取图片

        public static List<PicturesInfo> GetAllPictureInfos(this ISheet sheet)
        {
            return sheet.GetAllPictureInfos(null, null, null, null);
        }

        public static List<PicturesInfo> GetAllPictureInfos(this ISheet sheet, int? minRow, int? maxRow, int? minCol, int? maxCol, bool onlyInternal = true)
        {
            if (sheet is HSSFSheet)
            {
                return GetAllPictureInfos((HSSFSheet)sheet, minRow, maxRow, minCol, maxCol, onlyInternal);
            }
            else if (sheet is XSSFSheet)
            {
                return GetAllPictureInfos((XSSFSheet)sheet, minRow, maxRow, minCol, maxCol, onlyInternal);
            }
            else
            {
                throw new Exception("未处理类型，没有为该类型添加：GetAllPicturesInfos()扩展方法！");
            }
        }

        private static List<PicturesInfo> GetAllPictureInfos(HSSFSheet sheet, int? minRow, int? maxRow, int? minCol, int? maxCol, bool onlyInternal)
        {
            List<PicturesInfo> picturesInfoList = new List<PicturesInfo>();

            var shapeContainer = sheet.DrawingPatriarch as HSSFShapeContainer;
            if (null != shapeContainer)
            {
                var shapeList = shapeContainer.Children;
                foreach (var shape in shapeList)
                {
                    if (shape is HSSFPicture && shape.Anchor is HSSFClientAnchor)
                    {
                        var picture = (HSSFPicture)shape;
                        var anchor = (HSSFClientAnchor)shape.Anchor;

                        if (IsInternalOrIntersect(minRow, maxRow, minCol, maxCol, anchor.Row1, anchor.Row2, anchor.Col1, anchor.Col2, onlyInternal))
                        {
                            picturesInfoList.Add(new PicturesInfo(anchor.Row1, anchor.Row2, anchor.Col1, anchor.Col2, picture.PictureData.Data));
                        }
                    }
                }
            }

            return picturesInfoList;
        }

        private static List<PicturesInfo> GetAllPictureInfos(XSSFSheet sheet, int? minRow, int? maxRow, int? minCol, int? maxCol, bool onlyInternal)
        {
            List<PicturesInfo> picturesInfoList = new List<PicturesInfo>();

            var documentPartList = sheet.GetRelations();
            foreach (var documentPart in documentPartList)
            {
                if (documentPart is XSSFDrawing)
                {
                    var drawing = (XSSFDrawing)documentPart;
                    var shapeList = drawing.GetShapes();
                    foreach (var shape in shapeList)
                    {
                        if (shape is XSSFPicture)
                        {
                            var picture = (XSSFPicture)shape;
                            var anchor = picture.GetPreferredSize();

                            if (IsInternalOrIntersect(minRow, maxRow, minCol, maxCol, anchor.Row1, anchor.Row2, anchor.Col1, anchor.Col2, onlyInternal))
                            {
                                picturesInfoList.Add(new PicturesInfo(anchor.Row1, anchor.Row2, anchor.Col1, anchor.Col2, picture.PictureData.Data));
                            }
                        }
                    }
                }
            }

            return picturesInfoList;
        }

        private static bool IsInternalOrIntersect(int? rangeMinRow, int? rangeMaxRow, int? rangeMinCol, int? rangeMaxCol,
            int pictureMinRow, int pictureMaxRow, int pictureMinCol, int pictureMaxCol, bool onlyInternal)
        {
            int _rangeMinRow = rangeMinRow ?? pictureMinRow;
            int _rangeMaxRow = rangeMaxRow ?? pictureMaxRow;
            int _rangeMinCol = rangeMinCol ?? pictureMinCol;
            int _rangeMaxCol = rangeMaxCol ?? pictureMaxCol;

            if (onlyInternal)
            {
                return (_rangeMinRow <= pictureMinRow && _rangeMaxRow >= pictureMaxRow &&
                        _rangeMinCol <= pictureMinCol && _rangeMaxCol >= pictureMaxCol);
            }
            else
            {
                return ((Math.Abs(_rangeMaxRow - _rangeMinRow) + Math.Abs(pictureMaxRow - pictureMinRow) >= Math.Abs(_rangeMaxRow + _rangeMinRow - pictureMaxRow - pictureMinRow)) &&
                (Math.Abs(_rangeMaxCol - _rangeMinCol) + Math.Abs(pictureMaxCol - pictureMinCol) >= Math.Abs(_rangeMaxCol + _rangeMinCol - pictureMaxCol - pictureMinCol)));
            }
        }

        #endregion


        public static void RemoveEmpty(DataTable dt)
        {
            List<DataRow> removelist = new List<DataRow>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool rowdataisnull = true;
                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString().Trim()))
                    {

                        rowdataisnull = false;
                    }

                }
                if (rowdataisnull)
                {
                    removelist.Add(dt.Rows[i]);
                }

            }
            foreach (var t in removelist)
            {
                dt.Rows.Remove(t);
            }
        }

    }

    public class PicturesInfo
    {
        public int MinRow { get; set; }
        public int MaxRow { get; set; }
        public int MinCol { get; set; }
        public int MaxCol { get; set; }
        public Byte[] PictureData { get; private set; }

        public PicturesInfo(int minRow, int maxRow, int minCol, int maxCol, Byte[] pictureData)
        {
            this.MinRow = minRow;
            this.MaxRow = maxRow;
            this.MinCol = minCol;
            this.MaxCol = maxCol;
            this.PictureData = pictureData;
        }
    }

    public static class ExcelConvert
    {
        #region - 由数字转换为Excel中的列字母 -

        public static int ToIndex(string columnName)
        {
            if (!Regex.IsMatch(columnName.ToUpper(), @"[A-Z]+")) { throw new Exception("invalid parameter"); }
            int index = 0;
            char[] chars = columnName.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }
            return index - 1;
        }

        public static string ToName(int index)
        {
            if (index < 0) { throw new Exception("invalid parameter"); }
            List<string> chars = new List<string>();
            do
            {
                if (chars.Count > 0) index--;
                chars.Insert(0, ((char)(index % 26 + (int)'A')).ToString());
                index = (int)((index - index % 26) / 26);
            } while (index > 0);
            return String.Join(string.Empty, chars.ToArray());
        }
        #endregion
    }

    public class NPOIDataSource
    {
        public NPOIDataSource()
        {
            Source = new Dictionary<string, object>();
        }
        public Dictionary<string, object> Source { get; set; }

        public void Add(DataTable dt)
        {
            Source.Add(dt.TableName, dt);
        }
        public void Add(string name, object val)
        {
            Source.Add(name, val);
        }

    }
}
