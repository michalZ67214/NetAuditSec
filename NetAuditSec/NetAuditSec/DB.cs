using MySql.Data.MySqlClient;

namespace NetAuditSec
{
    internal class DB
    {
        MySqlConnection connection = new MySqlConnection();
        MySqlCommand query;
        string Server = "localhost";
        string User = "root";
        string Pass = "toor";
        string DbName = "netauditsec";

        public void GetMySqlConnection()
        {
            try
            {
                connection.ConnectionString = $"server={this.Server};uid={this.User};pwd={this.Pass};database={this.DbName}";

                this.connection.Open();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }
        }

        public void CloseMySqlConnection()
        {
            try
            {
                this.connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void InsertScan(List<string> devices_or_ports, string[] table_names, string ip = null)
        {
            try
            {
                string date_now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                string sql = $"INSERT INTO {table_names[0]} VALUES (NULL, '{date_now}'{((ip == null) ? "" : $", '{ip}'")})";

                this.query = new MySqlCommand(sql, this.connection);

                query.ExecuteNonQuery();

                sql = $"SELECT * FROM {table_names[0]} WHERE date='{date_now}'";
                this.query = new MySqlCommand(sql, this.connection);
                MySqlDataReader rdr = this.query.ExecuteReader();

                string id_of_scan = "";
                while (rdr.Read())
                {
                    id_of_scan = rdr[0].ToString();
                }
                rdr.Close();

                foreach (string el in devices_or_ports)
                {
                    sql = $"INSERT INTO {table_names[1]} VALUES (NULL, '{el}', {id_of_scan})";

                    this.query = new MySqlCommand(sql, this.connection);

                    query.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public List<string> GetDatesOfScans(string table_name)
        {
            List<string> dates = new List<string>();
            try
            {
                string sql = $"SELECT DISTINCT DATE(date) AS day FROM {table_name} ORDER BY day";
                this.query = new MySqlCommand(sql, this.connection);
                MySqlDataReader rdr = this.query.ExecuteReader();

                string[] rdr_arr;
                string day;
                while (rdr.Read())
                {
                    rdr_arr = rdr[0].ToString().Split(" ");
                    day = rdr_arr[0];
                    dates.Add(day);
                }
                rdr.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return dates;
        }

        public List<string[]> GetScansOfDay(string table_name, string day, bool display_ip)
        {
            List<string[]> scans = new List<string[]>();
            try
            {
                string sql = $"SELECT * FROM {table_name} WHERE date LIKE '%{day}%'";
                this.query = new MySqlCommand(sql, this.connection);
                MySqlDataReader rdr = this.query.ExecuteReader();

                while (rdr.Read())
                {
                    string[] scan = { rdr[0].ToString(), rdr[1].ToString(), display_ip ? rdr[2].ToString() : null };
                    scans.Add(scan);
                }
                rdr.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return scans;
        }

        public List<string> GetElementsFromScan(string column_name, string table_name, string scan_id)
        {
            List<string> ips = new List<string>();
            try
            {
                string sql = $"SELECT {column_name} FROM {table_name} WHERE id_scan = {scan_id}";
                this.query = new MySqlCommand(sql, this.connection);
                MySqlDataReader rdr = this.query.ExecuteReader();

                while (rdr.Read())
                {
                    ips.Add(rdr[0].ToString());
                }
                rdr.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return ips;
        }

        public void DeleteAllScansFromDay(string table_name, string day)
        {
            try
            {
                string sql = $"DELETE FROM {table_name} WHERE date LIKE '%{day}%'";
                this.query = new MySqlCommand(sql, this.connection);

                query.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void DeleteScanById(string table_name, string id)
        {
            try
            {
                string sql = $"DELETE FROM {table_name} WHERE id={id}";
                this.query = new MySqlCommand(sql, this.connection);

                query.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
