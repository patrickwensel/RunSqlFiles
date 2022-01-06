using System.Data;
using System.Data.SqlClient;

string fileName;
string connectionString;
string table;

Console.WriteLine("Run SQL Files");
Console.WriteLine("Enter the file path and name: ");
fileName = Console.ReadLine();
Console.WriteLine("Enter the database connection string: ");
connectionString = Console.ReadLine();

if (!string.IsNullOrWhiteSpace(fileName) && !string.IsNullOrWhiteSpace(connectionString))
{
    Run(fileName, connectionString);
}

Console.ReadLine();

static void Run(string fileName, string connectionString)
{
    List<string> allLinesText = File.ReadAllLines(fileName).ToList();

    foreach (string line in allLinesText)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 60; //  
            cmd.Connection = conn; // Sets wich connection you want to use  
            cmd.CommandType = CommandType.Text; // Sets the command type to use  
            cmd.CommandText = line;

            try
            {
                conn.Open(); 
                if (conn.State == ConnectionState.Open)
                {
                    cmd.ExecuteScalar();
                    Console.WriteLine("Successfully executed: " + line);
                    Console.WriteLine("\r\n");
                }
            }
            catch (Exception exp)
            {
                Console.Write(exp.Message);
            }
            finally
            {
                conn.Close(); 
            }

        }
    }
}
