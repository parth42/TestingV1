using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using DevExpress.Spreadsheet;
using WordAutomationDemo.Models;
using System.IO;

namespace WordAutomationDemo.Common
{
    public class CellStyleHelper
    {
        static List<Worksheet> worksheets;

        public static void PrepareWorksheet(List<Worksheet> worksheetsToPrepare, string docName/*, List<int> rowNumbers, int maxCol*/)
        {
            worksheets = worksheetsToPrepare;
            Style styleGood = worksheets[0].Workbook.Styles[BuiltInStyleId.Good];
            //List<Cell> cells = worksheet.Cells.ToList();
            using (ReadyPortalDBEntities _entities = new ReadyPortalDBEntities())
            {
                var takenGuids = _entities.tblExcelRowMaps.Where(er => er.IsActive && er.MasterDocumentName == docName).Select(erm => erm.RowId).ToList();
                
                if (takenGuids.Count > 0)
                {
                    using (Aspose.Cells.Workbook masterWb = new Aspose.Cells.Workbook(Path.Combine(DirectoryManagmentUtils.InitialDemoFilesPathExcels, docName)))
                    {

                        foreach (var worksheet in worksheets)
                        {
                            var worksheetName = worksheet.Name;
                            var takenRows = new List<int>();

                            var masterWorksheet = masterWb.Worksheets[worksheetName];

                            var maxDataRow = masterWorksheet.Cells.MaxDataRow + 1;

                            for (int i = 1; i < maxDataRow; i++)
                            {
                                //worksheet.ClearFormats(worksheet.Rows[i]);
                                worksheet.ClearComments(worksheet.Rows[i]);
                                var guid = masterWorksheet.Cells.Rows[i].FirstCell.Value;
                                var validGuid = Guid.Empty;
                                if (Guid.TryParse(guid as string, out validGuid))
                                {
                                    if (takenGuids.Contains(validGuid))
                                    {
                                        worksheet.Rows[i].Style = styleGood;

                                        //ASPxSpreadsheet DOESN'T SUPPORT COMMENTS YET. Stuff below won't work.
                                        //var assignment = _entities.tblExcelRowMaps.FirstOrDefault(er => er.RowId == validGuid).tblAssignment;
                                        //var usernames = _entities.tblUserDepartments.Where(u => u.tblAssignmentMembers.Where(am => am.AssignmentID == assignment.AssignmentID).Count() > 0).Select(u => u.FullName).ToArray();
                                        //var isLocked = assignment.LockedByUserID != null;

                                        //if (isLocked)
                                        //{
                                        //    var lockedUserName = _entities.tblUserDepartments.FirstOrDefault(u => u.UserId == assignment.LockedByUserID).FullName;
                                        //    Comment comment = worksheet.Comments.Add(worksheet.Cells[i, 3], "test", "Assigned to:" + string.Join(", ", usernames) + " and locked by " + lockedUserName);
                                        //}
                                        //else
                                        //{
                                        //    Comment comment = worksheet.Comments.Add(worksheet.Cells[i, 3], "test", "Assigned to:" + string.Join(", ", usernames));
                                        //}
                                    }
                                        
                                }
                            }
                        }
                    }
                }                
            }
                
            //Style styleGood = worksheet.Workbook.Styles[BuiltInStyleId.Good];
            //worksheet.Rows[1].Style = styleGood;
            
        }

        static void FormatCell(Cell cell)
        {
            // Specify font settings (font name, color, size and style).
            cell.Font.Name = "MV Boli";
            cell.Font.Color = Color.Blue;
            cell.Font.Size = 14;
            cell.Font.FontStyle = DevExpress.Spreadsheet.SpreadsheetFontStyle.Bold;

            // Specify cell background color.
            cell.Fill.BackgroundColor = Color.LightSkyBlue;

            // Specify text alignment in the cell. 
            cell.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
            cell.Alignment.Horizontal = DevExpress.Spreadsheet.SpreadsheetHorizontalAlignment.Center;
        }
        
    }
}