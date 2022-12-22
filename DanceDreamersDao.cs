using Dance_Dreamers_Administartor.model;
using Npgsql;
using OfficeOpenXml;

namespace Dance_Dreamers_Administartor
{
    public class DanceDreamersDao
    {
        public delegate void FileErrorEventHandler(string errorMsg);
        
        public event FileErrorEventHandler FileError;

        private readonly NpgsqlConnection db_connection;

        public DanceDreamersDao(string connectionString)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            db_connection = new NpgsqlConnection(connectionString);
        }

        public bool Authenticate(string password)
        {
            db_connection.Open();
            string query = @"SELECT password FROM admins";
            NpgsqlCommand command = new NpgsqlCommand(query, db_connection);
            command.ExecuteNonQuery();
            NpgsqlDataReader reader = command.ExecuteReader();
            reader.Read();
            bool res = reader.GetString(0).Trim().Equals(password);
            db_connection.Close();
            return res;
        }

        public void InsertEvent(Event danceEvent)
        {
            string startDateStr = string.Format("{0}-{1}-{2}", danceEvent.StartDate.Year, danceEvent.StartDate.Month, danceEvent.StartDate.Day);
            string endDateStr = "null";
            if (danceEvent.EndDate != null)
            {
                endDateStr = string.Format("{0}-{1}-{2}", danceEvent.EndDate.Value.Year, danceEvent.EndDate.Value.Month, danceEvent.EndDate.Value.Day);
            }
            db_connection.Open();
            string query;
            if (danceEvent.AdditionalInfoUrl == null&&danceEvent.EndDate==null)
            {
                query = string.Format(@"INSERT INTO events(event_name, date_of_issue, town, place) VALUES('{0}', '{1}', '{2}', '{3}')",
               danceEvent.Name, startDateStr, danceEvent.Town, danceEvent.Place);
            }
            else if(danceEvent.AdditionalInfoUrl == null&&danceEvent.EndDate!=null)
            {
                query = string.Format(@"INSERT INTO events(event_name, date_of_issue, date_until, town, place) VALUES('{0}', '{1}', '{2}', '{3}', '{4}')",
               danceEvent.Name, startDateStr, endDateStr, danceEvent.Town, danceEvent.Place);
            }
            else
            {
                query = string.Format(@"INSERT INTO events(event_name, date_of_issue, date_until, town, place, info_url) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5'})",
               danceEvent.Name, startDateStr, endDateStr, danceEvent.Town, danceEvent.Place, danceEvent.AdditionalInfoUrl);
            }
            NpgsqlCommand command = new NpgsqlCommand(query, db_connection);
            command.ExecuteNonQuery();
            db_connection.Close();
        }

        public void DeleteEvent(int eventId)
        {
            db_connection.Open();
            string query = string.Format(@"DELETE FROM enrollments WHERE event_id = {0}", eventId);
            query = string.Format(@"DELETE FROM events WHERE id = {0}", eventId);
            NpgsqlCommand command = new NpgsqlCommand(query, db_connection);
            command.ExecuteNonQuery();
            db_connection.Close();
        }

        public List<Event> FetchEventsFromDB()
        {
            List<Event> events = new List<Event>();
            db_connection.Open();
            string query = @"SELECT * FROM events";
            NpgsqlCommand command = new NpgsqlCommand(query, db_connection);
            command.ExecuteNonQuery();
            NpgsqlDataReader reader = command.ExecuteReader();
            Event danceEvent;
            while (reader.Read())
            {
                if (reader.GetValue(3)is not DBNull && reader.GetValue(6) is not DBNull)
                {
                    danceEvent = new Event(
                        reader.GetInt32(0),
                        reader.GetString(1).Trim(),
                        reader.GetDateTime(2), reader.GetDateTime(3).Date, reader.GetString(4).Trim(), reader.GetString(5).Trim(), reader.GetString(6).Trim());
                }
                else if (reader.GetValue(6) is DBNull && reader.GetValue(3) is not DBNull)
                {
                    danceEvent = new Event(
                       reader.GetInt32(0),
                       reader.GetString(1).Trim(),
                       reader.GetDateTime(2), reader.GetDateTime(3).Date, reader.GetString(4).Trim(), reader.GetString(5).Trim(), null);
                }
                else
                {
                    danceEvent = new Event(
                       reader.GetInt32(0),
                       reader.GetString(1).Trim(),
                       reader.GetDateTime(2), null, reader.GetString(4).Trim(), reader.GetString(5).Trim(), null);
                }
                events.Add(danceEvent);
            }
            db_connection.Close();
            return events;
        }

        public bool GetEnrollmentsFromDBAndWriteToFiles(string filename, int eventId)
        {
            try
            {
                List<string> programs = new List<string>();
                List<string> age_categories = new List<string>();

                string query = @"SELECT DISTINCT dance_program FROM enrollments WHERE filled = true AND dance_program IS NOT null";
                GetValuesToList(query, programs);
                query = @"SELECT DISTINCT age_category FROM enrollments WHERE filled = true AND age_category IS NOT null";
                GetValuesToList(query, age_categories);

                using (ExcelPackage excel = new ExcelPackage())
                {
                    ExcelWorksheet worksheet;
                    FileInfo excelFile = new FileInfo(filename);
                    foreach (string category in age_categories)
                    {
                        foreach (string program in programs)
                        {
                            worksheet = excel.Workbook.Worksheets.Add(category + " " + program);
                            WriteDancers(eventId, program, category, worksheet);
                        }
                    }
                    worksheet = excel.Workbook.Worksheets.Add("Тренери");
                    WriteCoaches(eventId, worksheet);
                    excel.SaveAs(excelFile);
                    return true;
                }
            }
            catch (Exception e)
            {
                FileError("Не вдалося відкрити файл:\n" +
                    "Можливо ви відкрили його в іншому додатку");
                return false;
            }
        }

        private void WriteCoaches(int eventId, ExcelWorksheet worksheet)
        {
            string query = string.Format(@"SELECT DISTINCT participant_name, town, club,phone_number FROM enrollments 
                WHERE event_id = {0} AND filled = true AND participant_type = 'Тренер' ORDER BY participant_name", eventId);
            WriteToSheet(query, worksheet);
        }

        private void WriteDancers(int eventId, string program, string age_category, ExcelWorksheet worksheet)
        {
            string query = string.Format(
                @"SELECT DISTINCT participant_name, participant_type, dance_class, town, club, coach, phone_number
                FROM enrollments WHERE event_id = {0} AND filled = true AND dance_program = '{1}' AND age_category = '{2}' ORDER BY participant_name",
                    eventId, program.ToString(), age_category.ToString());
            WriteToSheet(query, worksheet);
        }

        private void WriteToSheet(string query, ExcelWorksheet worksheet)
        {
            db_connection.Open();

            NpgsqlCommand command = new NpgsqlCommand(query, db_connection);
            command.ExecuteNonQuery();
            NpgsqlDataReader reader = command.ExecuteReader();
            string[] row;
            int count = 0;

            var Data = new List<string[]>()
            {

            };

            while (reader.Read())
            {
                count++;
                row = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[i] = reader.GetString(i).Trim().Replace("\n", " ");
                }
                Data.Add(row);

            }

            worksheet.Cells[1, 1].LoadFromArrays(Data);
            worksheet.Columns.AutoFit();
            db_connection.Close();
        }

        private void GetValuesToList(string query, List<string> values)
        {
            db_connection.Open();
            NpgsqlCommand command = new NpgsqlCommand(query, db_connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            string value;
            while (reader.Read())
            {
                value = reader.GetString(0);
                values.Add(value.Trim());
            }
            db_connection.Close();
        }
    }
}
