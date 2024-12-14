using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace ADUserGroupManager
{
    public static class GoogleSheetsHelper
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string ApplicationName = "ADUserGroupManager";
        private static SheetsService _service;
        private static readonly string LogFilePath = "GoogleSheetsHelperLog.txt";

        static GoogleSheetsHelper()
        {
            try
            {
                LogAction("Initializing Google Sheets service...");
                using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    var credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                    _service = new SheetsService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = ApplicationName,
                    });
                    LogAction("Google Sheets service initialized successfully.");
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error initializing Google Sheets service: {ex.Message}");
                throw;
            }
        }

        public static void CreateSheet(string spreadsheetId, string sheetName)
        {
            try
            {
                LogAction($"Creating sheet '{sheetName}' in spreadsheet '{spreadsheetId}'...");

                var spreadsheet = _service.Spreadsheets.Get(spreadsheetId).Execute();
                bool sheetExists = spreadsheet.Sheets.Any(s => s.Properties.Title.Equals(sheetName, StringComparison.OrdinalIgnoreCase));

                if (!sheetExists)
                {
                    var addSheetRequest = new AddSheetRequest
                    {
                        Properties = new SheetProperties { Title = sheetName }
                    };

                    var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
                    {
                        Requests = new List<Request>
                        {
                            new Request { AddSheet = addSheetRequest }
                        }
                    };

                    _service.Spreadsheets.BatchUpdate(batchUpdateRequest, spreadsheetId).Execute();
                    LogAction($"Sheet '{sheetName}' created successfully.");
                }
                else
                {
                    LogAction($"Sheet '{sheetName}' already exists. No need to create.");
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error creating sheet '{sheetName}': {ex.Message}");
                throw;
            }
        }

        public static void UpdateCell(string spreadsheetId, string cellRange, string value)
        {
            try
            {
                // Evitar logs para contraseñas en columna C
                if (cellRange.IndexOf("!C", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    LogAction($"Updating cell '{cellRange}' in spreadsheet '{spreadsheetId}'... (Password hidden)");
                }
                else
                {
                    LogAction($"Updating cell '{cellRange}' in spreadsheet '{spreadsheetId}' with value '{value}'...");
                }


                var valueRange = new ValueRange
                {
                    Values = new List<IList<object>> { new List<object> { value } }
                };

                var updateRequest = _service.Spreadsheets.Values.Update(valueRange, spreadsheetId, cellRange);
                updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                updateRequest.Execute();
                LogAction($"Cell '{cellRange}' updated successfully.");
            }
            catch (Exception ex)
            {
                LogAction($"Error updating cell '{cellRange}': {ex.Message}");
                throw;
            }
        }

        public static void SetCellBold(string spreadsheetId, string sheetName, string cellAddress, bool bold)
        {
            int sheetId = GetSheetId(spreadsheetId, sheetName);
            var (startRowIndex, startColumnIndex) = ConvertCellAddressToIndices(cellAddress);

            var request = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>
                {
                    new Request
                    {
                        RepeatCell = new RepeatCellRequest
                        {
                            Range = new GridRange
                            {
                                SheetId = sheetId,
                                StartRowIndex = startRowIndex,
                                EndRowIndex = startRowIndex + 1,
                                StartColumnIndex = startColumnIndex,
                                EndColumnIndex = startColumnIndex + 1
                            },
                            Cell = new CellData
                            {
                                UserEnteredFormat = new CellFormat
                                {
                                    TextFormat = new TextFormat
                                    {
                                        Bold = bold
                                    }
                                }
                            },
                            Fields = "userEnteredFormat.textFormat.bold"
                        }
                    }
                }
            };

            var service = GetSheetsService();
            service.Spreadsheets.BatchUpdate(request, spreadsheetId).Execute();
        }

        public static void AutoResizeColumns(string spreadsheetId, string sheetName, int startColumnIndex, int endColumnIndex)
        {
            int sheetId = GetSheetId(spreadsheetId, sheetName);
            var service = GetSheetsService();

            var autoResizeRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request>
                {
                    new Request
                    {
                        AutoResizeDimensions = new AutoResizeDimensionsRequest
                        {
                            Dimensions = new DimensionRange
                            {
                                SheetId = sheetId,
                                Dimension = "COLUMNS",
                                StartIndex = startColumnIndex,
                                EndIndex = endColumnIndex
                            }
                        }
                    }
                }
            };

            service.Spreadsheets.BatchUpdate(autoResizeRequest, spreadsheetId).Execute();
        }

        private static int GetSheetId(string spreadsheetId, string sheetName)
        {
            var spreadsheet = _service.Spreadsheets.Get(spreadsheetId).Execute();
            var sheet = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title.Equals(sheetName, StringComparison.OrdinalIgnoreCase));
            if (sheet == null)
                throw new Exception($"Sheet '{sheetName}' not found.");

            return (int)sheet.Properties.SheetId.Value;
        }

        private static SheetsService GetSheetsService()
        {
            return _service;
        }

        private static (int Row, int Col) ConvertCellAddressToIndices(string cellAddress)
        {
            string columnPart = new string(cellAddress.TakeWhile(char.IsLetter).ToArray());
            string rowPart = new string(cellAddress.SkipWhile(char.IsLetter).ToArray());

            int rowIndex = int.Parse(rowPart) - 1;
            int columnIndex = ColumnLetterToNumber(columnPart) - 1;

            return (rowIndex, columnIndex);
        }

        private static int ColumnLetterToNumber(string column)
        {
            int sum = 0;
            for (int i = 0; i < column.Length; i++)
            {
                sum *= 26;
                sum += (column[i] - 'A' + 1);
            }
            return sum;
        }

        public static int GetLastUsedRow(string spreadsheetId, string sheetName, string column)
        {
            try
            {
                string range = $"{sheetName}!{column}5:{column}";
                var service = GetSheetsService();

                var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
                var response = request.Execute();
                var values = response.Values;

                return values == null || values.Count == 0 ? 0 : 4 + values.Count;
            }
            catch (Exception ex)
            {
                LogAction($"Error en GetLastUsedRow: {ex.Message}");
                return 0;
            }
        }

        private static void LogAction(string message)
        {
            try
            {
                // Filtrar mensajes que contengan contraseñas
                if (message.IndexOf("!C", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    message = message.Replace("with value", "(Password hidden)");
                }


                using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
            }
            catch
            {
                // Ignorar errores de logging
            }
        }
    }
}
