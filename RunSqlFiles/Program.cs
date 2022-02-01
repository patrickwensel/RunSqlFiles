using System.Data;
using System.Data.SqlClient;

string folderName;
string connectionString;
string table;


Console.WriteLine("Run SQL Files");
Console.WriteLine("Enter the file path: ");
folderName = Console.ReadLine();
Console.WriteLine("Enter the database connection string: ");
connectionString = Console.ReadLine();

if (!string.IsNullOrWhiteSpace(folderName) && !string.IsNullOrWhiteSpace(connectionString))
{
    var doesItWork = Run(folderName, connectionString);
}

Console.ReadLine();

static bool Run(string folderName, string connectionString)
{
    //string fileName = null;

    string[] files = Directory.GetFiles(folderName, "*.sql");
    foreach (var fileName in files)
    {
        bool didSQLRunOk = true;
        string sql = null;
        List<string> allLinesText = File.ReadAllLines(fileName).ToList();

        allLinesText = allLinesText.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

        var firstline = allLinesText.First();
        string firstLine20 = firstline.Substring(0, 20);
        string lastLine = allLinesText.Last();
        string lastLine20 = firstline.Substring(0, 20);

        if (firstLine20 == "SET IDENTITY_INSERT ")
        {
            var listLength = allLinesText.Count() - 2;

            //start at line #2 through listLenght - 1
            // If it is under 100 lines, just run it, if not, break it into chunks

            if (listLength < 100)
            {
                foreach (var line in allLinesText)
                {
                    sql = sql + Environment.NewLine + line;
                }
                didSQLRunOk = RunSQLCommand(sql, connectionString);
            }
            else
            {
                int howManyIterations = listLength / 100;
                int howManyLeftOver = listLength - howManyIterations * 100;
                int k = 1;

                for (int j = 1; j < howManyIterations + 1; j++)
                {
                    if (j < howManyIterations)
                    {
                        sql = firstline;
                        for (int i = k; i < j * 100 + 1; i++)
                        {
                            sql = sql + Environment.NewLine + allLinesText[i];
                        }
                        sql = sql + Environment.NewLine + lastLine;

                        //var r = sql;
                        didSQLRunOk = RunSQLCommand(sql, connectionString);
                    }
                    k = k + 100;
                }

                var e = listLength - k;

                sql = firstline;
                for (int i = k - 100; i < listLength + 1; i++)
                {
                    sql = sql + Environment.NewLine + allLinesText[i];
                }
                sql = sql + Environment.NewLine + lastLine;

                didSQLRunOk = RunSQLCommand(sql, connectionString);

                var x = sql;
            }

        }
        else
        {
            var listLength = allLinesText.Count();
            if (listLength < 100)
            {
                foreach (var line in allLinesText)
                {
                    sql = sql + Environment.NewLine + line;
                }
                didSQLRunOk = RunSQLCommand(sql, connectionString);
            }
            else
            {
                int howManyIterations = listLength / 100;
                int howManyLeftOver = listLength - howManyIterations * 100;
                int k = 1;

                for (int j = 1; j < howManyIterations + 1; j++)
                {
                    if (j < howManyIterations)
                    {
                        sql = null;
                        for (int i = k; i < j * 100 + 1; i++)
                        {
                            sql = sql + Environment.NewLine + allLinesText[i];
                        }

                        //var r = sql;
                        didSQLRunOk = RunSQLCommand(sql, connectionString);
                    }
                    k = k + 100;
                }

                var e = listLength - k;

                for (int i = k - 100; i < listLength; i++)
                {
                    sql = sql + Environment.NewLine + allLinesText[i];
                }

                didSQLRunOk = RunSQLCommand(sql, connectionString);

                var x = sql;
            }
        }
    }
    return true;
}

static bool RunSQLCommand(string sql, string connectionString)
{

    SqlConnection conn = new SqlConnection(connectionString);
    SqlCommand cmd = new SqlCommand();
    cmd.CommandTimeout = 60; //  
    cmd.Connection = conn; // Sets wich connection you want to use  
    cmd.CommandType = CommandType.Text; // Sets the command type to use  
    cmd.CommandText = sql;

    try
    {
        conn.Open();
        if (conn.State == ConnectionState.Open)
        {
            Console.WriteLine("\r\n");
            cmd.ExecuteScalar();
            Console.WriteLine("Successfully executed: ");

        }
    }
    catch (Exception exp)
    {
        Console.WriteLine("Error executing: ");
        Console.Write(exp.Message);
        return false;
    }
    finally
    {
        conn.Close();
    }




    return true;
}
