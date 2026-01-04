using Dapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using System.ComponentModel;

namespace BookSystem.Model
{
    public class BookService
    {
        /// <summary>
        /// 取得預設連線字串
        /// </summary>
        /// <returns></returns>
        private string GetDBConnectionString()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Ensure a non-null string is always returned, or throw a more explicit exception.
            // For now, we'll return an empty string if not found, to be handled by TestDbConnection.
            return config.GetConnectionString("DBConn") ?? string.Empty;
        }

        public string TestDbConnection()
        {
            string connectionString = GetDBConnectionString();
            if (string.IsNullOrEmpty(connectionString))
            {
                // This means 'DBConn' was not found in appsettings.json or was empty.
                return "錯誤：在 appsettings.json 中找不到 'DBConn' 連接字串，或其值為空。請檢查設定。";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    return "資料庫連線成功！";
                }
                catch (Exception ex)
                {
                    return $"資料庫連線失敗：{ex.Message}";
                }
                finally
                {
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }

        public List<Book> QueryBook(BookQueryArg arg)
        {
            var result = new List<Book>();
            using (SqlConnection conn = new SqlConnection(GetDBConnectionString()))
            {
                string sql = @"
                    Select 
	                    A.BOOK_ID As BookId,A.BOOK_NAME As BookName,
	                    A.BOOK_CLASS_ID As BookClassId,B.BOOK_CLASS_NAME As BookClassName,
                        A.BOOK_STATUS As BookStatusId, C.CODE_NAME As BookStatusName,
                        A.BOOK_KEEPER As BookKeeperId, D.USER_CNAME As BookKeeperCname, D.USER_ENAME As BookKeeperEname,
	                    Convert(VarChar(10),A.BOOK_BOUGHT_DATE,120) As BookBoughtDate,
                        A.BOOK_AUTHOR As BookAuthor, A.BOOK_PUBLISHER As BookPublisher, A.BOOK_NOTE As BookNote
                    From BOOK_DATA As A
	                    Inner Join BOOK_CLASS As B On A.BOOK_CLASS_ID=B.BOOK_CLASS_ID
	                    Left Join BOOK_CODE As C On A.BOOK_STATUS=C.CODE_ID And C.CODE_TYPE = 'BOOK_STATUS'
                        Left Join MEMBER_M As D On A.BOOK_KEEPER = D.USER_ID
                    Where (A.BOOK_NAME LIKE @BookName Or @BookName = '%%')
                      And (A.BOOK_CLASS_ID = @BookClassId Or @BookClassId = '')
                      And (A.BOOK_STATUS = @BookStatusId Or @BookStatusId = '')
                      And (A.BOOK_KEEPER = @BookKeeperId Or @BookKeeperId = '')";

                var parameters = new
                {
                    BookName = "%" + (arg.BookName ?? "") + "%",
                    BookClassId = arg.BookClassId ?? "",
                    BookStatusId = arg.BookStatusId ?? "",
                    BookKeeperId = arg.BookKeeperId ?? ""
                };

                result = conn.Query<Book>(sql, parameters).ToList();
            }
            return result;
        }

        public void AddBook(Book book)
        {

            using (SqlConnection conn = new SqlConnection(GetDBConnectionString()))
            {
                string sql = @"
                Insert Into BOOK_DATA
                (
	                BOOK_NAME,BOOK_CLASS_ID,
	                BOOK_AUTHOR,BOOK_BOUGHT_DATE,
	                BOOK_PUBLISHER,BOOK_NOTE,
	                BOOK_STATUS,BOOK_KEEPER,
	                BOOK_AMOUNT,
	                CREATE_DATE,CREATE_USER,MODIFY_DATE,MODIFY_USER
                )
                Select 
	                @BOOK_NAME,@BOOK_CLASS_ID,
	                @BOOK_AUTHOR,@BOOK_BOUGHT_DATE,
	                @BOOK_PUBLISHER,@BOOK_NOTE,
	                @BOOK_STATUS,@BOOK_KEEPER,
	                0 As BOOK_AMOUNT,
	                GetDate() As CREATE_DATE,'Admin' As CREATE_USER,GetDate() As MODIFY_DATE,'Admin' As MODIFY_USER";

                Dictionary<string, Object> parameter = new Dictionary<string, object>();
                parameter.Add("@BOOK_NAME", book.BookName);
                parameter.Add("@BOOK_CLASS_ID", book.BookClassId);
                parameter.Add("@BOOK_AUTHOR", book.BookAuthor);
                parameter.Add("@BOOK_BOUGHT_DATE", book.BookBoughtDate);
                parameter.Add("@BOOK_PUBLISHER", book.BookPublisher);
                parameter.Add("@BOOK_NOTE", book.BookNote);
                parameter.Add("@BOOK_STATUS", "A");
                parameter.Add("@BOOK_KEEPER", book.BookKeeperId);

                conn.Execute(sql, parameter);
            }
        }

        public void UpdateBook(Book book)
        {
            using (SqlConnection conn = new SqlConnection(GetDBConnectionString()))
            {
                try
                {
                    //TODO:Update SQL
                    string sql = @"";

                    Dictionary<string, Object> parameter = new Dictionary<string, object>();
                    parameter.Add("@BOOK_NAME", book.BookName);
                    parameter.Add("@BOOK_CLASS_ID", book.BookClassId);
                    parameter.Add("@BOOK_AUTHOR", book.BookAuthor);
                    parameter.Add("@BOOK_BOUGHT_DATE", book.BookBoughtDate);
                    parameter.Add("@BOOK_PUBLISHER", book.BookPublisher);
                    parameter.Add("@BOOK_NOTE", book.BookNote);
                    parameter.Add("@BOOK_STATUS", book.BookStatusId);
                    parameter.Add("@BOOK_KEEPER", book.BookKeeperId);
                    parameter.Add("BOOK_ID", book.BookId);

                    conn.Execute(sql, parameter);

                    if (book.BookStatusId == "B" || book.BookStatusId == "C")
                    {
                        sql = @"
                                Insert Into BOOK_LEND_RECORD
                                (
                                    BOOK_ID,KEEPER_ID,LEND_DATE,
                                    CRE_DATE,CRE_USR,MOD_DATE,MOD_USR
                                )
                                ";
                        parameter.Clear();
                        parameter.Add("@BOOK_ID", book.BookId);

                        conn.Execute(sql, parameter);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void DeleteBookById(int bookId)
        {
            using (SqlConnection conn = new SqlConnection(GetDBConnectionString()))
            {
                string sql = @"Delete From BOOK_DATA Where BOOK_ID=@BOOK_ID";

                Dictionary<string, Object> parameter = new Dictionary<string, object>();
                parameter.Add("BOOK_ID", bookId);

                conn.Execute(sql, parameter);
            }
        }
    }
}
