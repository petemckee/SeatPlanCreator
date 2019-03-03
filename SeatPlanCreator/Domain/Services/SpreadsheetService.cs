using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Excel;
using SeatPlanCreatorWeb.Domain.Extensions;
using SeatPlanCreatorWeb.Domain.Pocos;

namespace SeatPlanCreatorWeb.Domain.Services
{
	public sealed class SpreadsheetService
	{
		public WorkSheetData LoadWorksheetInDataTable(HttpPostedFileBase planData, string sheetName)
		{
			var students = new List<Student>();
			var warnings = new List<string>();
			var errors = new List<string>();

			IExcelDataReader excelReader = this.GetReader(planData);

			if (excelReader == null)
			{
				return new WorkSheetData()
				{
					Errors = new List<string>() { "Invalid file" }
				};
			}

			excelReader.IsFirstRowAsColumnNames = true;
			DataSet result = excelReader.AsDataSet();
			excelReader.Close();

			var dt = result.Tables[0];

			var nameColIndex = 0;
			var senColIndex = this.GetColumnIndexByName(dt, "sen status", ref warnings);
			var pupilPremiumColIndex = this.GetColumnIndexByName(dt, "pupil premium indicator", ref warnings);
			int currentGradeColumnIndex = GetCurrentGradeColumnIndex(dt, ref warnings);
			int targetGradeColumnIndex = GetTargetGradeColumnIndex(dt, ref warnings);

			foreach (DataRow row in dt.Rows)
			{
				var student = new Student()
				{
					Name = row.GetString(nameColIndex),
					Sen = row.GetString(senColIndex),
					PupilPremium = row.GetString(pupilPremiumColIndex)
				};

				if (String.IsNullOrEmpty(student.Name))
				{
					break;
				}

				var nameArray = student.Name.Split(' ').Reverse().ToArray();
				student.FirstName = nameArray[0];
				student.LastName = nameArray[1];
				student.TargetGrade = row.GetString(targetGradeColumnIndex);
				student.CurrentGrade = row.GetString(currentGradeColumnIndex);

				students.Add(student);
			}

			var groupName = this.GetGroupName(dt, ref warnings);
			if (String.IsNullOrWhiteSpace(groupName))
				warnings.Add("Group name not found (CLASSNAME)");

			if (students.Count == 0)
				errors.Add("No student data found");

			return new WorkSheetData()
			{
				Students = students,
				GroupName = groupName
			};
		}

		private IExcelDataReader GetReader(HttpPostedFileBase planData)
		{
			var ext = planData.FileName.Split('.').Last();
			IExcelDataReader excelReader = null;

			if (ext == "xls")
				//1. Reading from a binary Excel file ('97-2003 format; *.xls)
				excelReader = ExcelReaderFactory.CreateBinaryReader(planData.InputStream);
			else
				//2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
				excelReader = ExcelReaderFactory.CreateOpenXmlReader(planData.InputStream);

			return excelReader;
		}

		private int GetColumnIndexByName(DataTable dt, string name, ref List<string> warnings)
		{
			int colIndex = -1;
			int i = 0;
			while (i < dt.Columns.Count)
			{
				var col = dt.Columns[i];
				if (col.ColumnName.ToLower() == name)
				{
					colIndex = i;
					break;
				}

				i++;
			}

			if (colIndex == -1)
				warnings.Add($"Column '{name}' not found");

			return colIndex;

		}
 
		private string GetGroupName(DataTable dt, ref List<string> warnings)
		{
			var name = string.Empty;
			var startsWith = "Group Name : ";

			foreach (DataRow row in dt.Rows)
			{
				var text = row.GetString(0);
				if (text.StartsWith(startsWith))
				{
					return text.Replace(startsWith, "");
				}
			}

			if (String.IsNullOrWhiteSpace(name))
				warnings.Add($"Cell with prefix '{startsWith}' not found");

			return name;
		}

		private int GetCurrentGradeColumnIndex(DataTable dt, ref List<string> warnings)
		{
			return GetGradeColumnIndex(dt, "Year", "Band", ref warnings);
		}

		private int GetTargetGradeColumnIndex(DataTable dt, ref List<string> warnings)
		{
			return GetGradeColumnIndex(dt, "Year", "Target", ref warnings);
		}

		private int GetGradeColumnIndex(DataTable dt, string startsWith, string contains, ref List<string> warnings)
		{
			int colIndex = -1;
			int i = 0;
			while (i < dt.Columns.Count)
			{
				var col = dt.Columns[i];
				if (col.ColumnName.StartsWith(startsWith) && col.ColumnName.Contains(contains))
				{
					colIndex = i;
					break;
				}

				i++;
			}

			if (colIndex == -1)
				warnings.Add($"Column with prefix '{startsWith}' and contains '{contains}' not found");

			return colIndex;
		}
	}
}