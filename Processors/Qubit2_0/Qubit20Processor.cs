﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using PluginBase;
using OfficeOpenXml;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Qubit2_0
{
    public class Qubit20Processor : Processor
    {

        public override string UniqueId { get => "qubit2.0_version1.0"; }

        public override string Name { get => "Qubit2.0"; }

        public override string Description { get => "Processor used for Qubit2.0 translation to universal template"; }

        public override string InstrumentFileType { get => ".xlsx"; }

        public override string InputFile { get; set; }

        public override string Path { get; set; }
        

        public Qubit20Processor()
        {
        }

        public override DataTableResponseMessage Execute()
        {
            DataTableResponseMessage rm = new DataTableResponseMessage();
            try
            {
                rm = VerifyInputFile();
                FileInfo fi = new FileInfo(InputFile);

                using (var package = new ExcelPackage(fi))
                {
                    //Data is in the 2nd sheet
                    var worksheet = package.Workbook.Worksheets[1]; //Worksheets are zero-based index
                    string name = worksheet.Name;
                    int startRow = worksheet.Dimension.Start.Row;
                    int startCol = worksheet.Dimension.Start.Column;
                    int numRows = worksheet.Dimension.End.Row;
                    int numCols = worksheet.Dimension.End.Column;

                    DataTable dt_template = GetDataTable();                    
                    dt_template.TableName = System.IO.Path.GetFileNameWithoutExtension(fi.FullName);
                    TemplateField[] fields = Fields;


                    //The columns in the data file are as follows through column J
                    //  A        B          C             D            E       F             G        H          I                   J
                    //SED_ID	Name	  Date/Time	  Assay Conc.	Units	Stock Conc.	  Units	  Assay Type   Sample Vol (µL)	 Dilution Factor
                    //^^^^^^^   ^^^^^     ^^^^^^^^    ^^^^^^^^^                                                                  ^^^^^^^^^^^^^^^

                    //Aliquot  AssayType  Analysis    Measured                                                                   Dilution Factor   
                    //ID                  Date/Time   Value

                    for (int row = 2; row <= numRows; row++)
                    {

                        string aliquot_id = GetStringValue(worksheet.Cells[row, 1]);

                        DateTime analysis_datetime = GetDateTimeValue(worksheet.Cells[row, 3]);

                        double measured_val = default;
                        ExcelRange rng_meas_val = worksheet.Cells[row, 4];
                        if (rng_meas_val != null && rng_meas_val.Value != null)
                        {
                            string msr_val = rng_meas_val.Value.ToString().Trim();
                            if (string.Compare(msr_val, "<0.50") == 0)
                                measured_val = default;
                            else
                                measured_val = GetDoubleValue(worksheet.Cells[row, 4]);
                        }


                        string analyte_id = GetStringValue(worksheet.Cells[row, 8]);

                        double dilution_factor = GetDoubleValue(worksheet.Cells[row, 10]);

                        DataRow dr = dt_template.NewRow();
                        dr[0] = aliquot_id;
                        dr[5] = analysis_datetime;
                        dr[2] = measured_val;
                        dr[1] = analyte_id;
                        dr[4] = dilution_factor;

                        dt_template.Rows.Add(dr);
                    }

                    rm.TemplateData = dt_template;
                }
            }
            catch(Exception ex)
            {
                rm.AddErrorAndLogMessage(string.Format("Problem transferring data file {0}  to template file", InputFile));
            }

            return rm;           
        }

        private double GetDoubleValue(ExcelRange cell)
        {
            double retVal = 0.0;
            if (cell == null || cell.Value == null)
                retVal = default;
            else
                retVal = Convert.ToDouble(cell.Value.ToString().Trim());

            return retVal;
        }
        private string GetStringValue(ExcelRange cell)
        {
            string retVal = "";
            if (cell == null || cell.Value == null)
                retVal = default;
            else
                retVal = Convert.ToString(cell.Value.ToString().Trim());

            return retVal;
        }

        private DateTime GetDateTimeValue(ExcelRange cell)
        {
            DateTime retVal = default;
            if (cell == null || cell.Value == null)
                retVal = default;
            else
                retVal = Convert.ToDateTime(cell.Value.ToString().Trim());

            return retVal;
        }
    }
}