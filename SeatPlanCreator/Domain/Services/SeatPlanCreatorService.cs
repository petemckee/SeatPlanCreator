using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using SeatPlanCreatorWeb.Domain.Pocos;
using Spire.Presentation;

namespace SeatPlanCreatorWeb.Domain.Services
{
	public sealed class SeatPlanCreatorService
	{
		public SeatingPlanResult Create(HttpPostedFileBase planData, HttpPostedFileBase template)
		{
			var spreadyService = new SpreadsheetService();
			Presentation presentation = new Presentation();

			if (template != null)
			{

				presentation.LoadFromStream(template.InputStream, FileFormat.Pptx2010);

			}
			else
			{
				var templateLocation = System.AppDomain.CurrentDomain.BaseDirectory + "ClassroomTemplate.pptx";
				presentation.LoadFromFile(templateLocation, FileFormat.Pptx2010);
			}

			var worksheetData = spreadyService.LoadWorksheetInDataTable(planData, "Sheet1");

			var templateSlide = presentation.Slides[0];
			presentation.Slides.Append(templateSlide);

			EditSlide(presentation, 0, worksheetData, true);
			EditSlide(presentation, 1, worksheetData);

			var fileName = $"seating-plan-{worksheetData.GroupName}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.pptx";

			return new SeatingPlanResult(fileName, presentation, true);
		}

		private IAutoShape GetSlideNameShape(ISlide slide, string desiredContent = "classname")
		{
			foreach (var shape in slide.Shapes)
			{
				var autoShape = shape as IAutoShape;

				if (autoShape == null)
					continue;

				var text = autoShape.TextFrame.Paragraphs[0].Text;

				if (String.IsNullOrWhiteSpace(text) || text.ToLower() != desiredContent)
					continue;

				return autoShape;
			}

			return null;
		}

		private void EditSlide(Presentation presentation, int slideIndex, WorkSheetData worksheetData, bool includeStudentData = false)
		{
			var slide = presentation.Slides[slideIndex];

			var boxPositions = this.GetBoxPositions(slide);
			var positions = boxPositions.ToArray();

			var groupNameShape = this.GetSlideNameShape(slide);

			if (groupNameShape != null)
				groupNameShape.TextFrame.Paragraphs[0].Text = worksheetData.GroupName;

			var studentArray = worksheetData.Students.ToArray();

			for (var i = 0; i < positions.Length; i++)
			{
				var textBoxShape = positions[i];

				//-- Remove shape if run out of students
				if (i >= studentArray.Length)
				{
					slide.Shapes.Remove(textBoxShape.AutoShape);
					continue;
				}

				var student = studentArray[i];

				var paragraphs = textBoxShape.AutoShape.TextFrame.Paragraphs[0];
				paragraphs.Text = GetName(student, studentArray);
				////set the Font fill style of text  
				TextRange textRange = textBoxShape.AutoShape.TextFrame.TextRange;
				textRange.Format.FontHeight = 14;
				textRange.Fill.FillType = Spire.Presentation.Drawing.FillFormatType.Solid;
				textRange.Fill.SolidColor.Color = Color.Black;
				textRange.LatinFont = new TextFont("Arial");
				textRange.Paragraph.Alignment = TextAlignmentType.Left;
				textRange.Paragraph.FontAlignment = FontAlignmentType.Top;

				if (!includeStudentData)
					continue;

				var sen = !String.IsNullOrEmpty(student.Sen) ? student.Sen + " " : null;
				var pp = !String.IsNullOrEmpty(student.PupilPremium) && student.PupilPremium == "Y" ? "PP " : null;

				var studentDetails = String.Format("{0}T: {1} {2}{0}{3}{4}", Environment.NewLine, student.TargetGrade, student.CurrentGrade, pp, sen);
				var detailsRange = new TextRange(studentDetails);
				detailsRange.Format.FontHeight = 10;
				detailsRange.Fill.FillType = Spire.Presentation.Drawing.FillFormatType.Solid;
				detailsRange.Fill.SolidColor.Color = Color.Black;
				detailsRange.LatinFont = new TextFont("Arial");
				textRange.Paragraph.TextRanges.Append(detailsRange);
			}
		}

		/// <summary>
		/// Check for firstname dupes and include first letter of surname
		/// </summary>
		/// <param name="student"></param>
		/// <param name="students"></param>
		/// <returns></returns>
		private string GetName(Student student, IEnumerable<Student> students)
		{
			var name = student.FirstName;

			if (students.Count(s => s.FirstName == student.FirstName) > 1)
			{
				// TODO:  check for also firstname first letter of surname dupes
				name += " " + student.LastName[0];
			}

			return name;
		}

		private const string PositionPrefix = "pos";

		private IEnumerable<TextBoxShape> GetBoxPositions(ISlide slide)
		{
			var textBoxShapes = new List<TextBoxShape>();

			foreach (var shape in slide.Shapes)
			{
				var autoShape = shape as IAutoShape;

				if (autoShape == null)
					continue;

				var text = autoShape.TextFrame.Paragraphs[0].Text;

				if (String.IsNullOrWhiteSpace(text) || !text.ToLower().StartsWith(PositionPrefix))
					continue;

				var posIndex = -1;
				int.TryParse(text.Replace(PositionPrefix, ""), out posIndex);

				if (posIndex == -1)
					continue;

				textBoxShapes.Add(new TextBoxShape(autoShape, posIndex));
			}

			return textBoxShapes.OrderBy(s => s.Index);
		}










		public SeatingPlanResult CreateWithInfo(HttpPostedFileBase planData, HttpPostedFileBase template)
		{
			var spreadyService = new SpreadsheetService();
			Presentation presentation = new Presentation();

			if (template != null)
			{
				presentation.LoadFromStream(template.InputStream, FileFormat.Pptx2010);
			}
			else
			{
				var templateLocation = System.AppDomain.CurrentDomain.BaseDirectory + "ClassroomTemplate.pptx";
				presentation.LoadFromFile(templateLocation, FileFormat.Pptx2010);
			}

			var worksheetData = spreadyService.LoadWorksheetInDataTable(planData, "Sheet1");

			var templateSlide = presentation.Slides[0];
			presentation.Slides.Append(templateSlide);

			EditSlide(presentation, 0, worksheetData, true);
			EditSlide(presentation, 1, worksheetData);

			var fileName = $"seating-plan-{worksheetData.GroupName}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.pptx";

			return new SeatingPlanResult(fileName, presentation, true);
		}
	}
}