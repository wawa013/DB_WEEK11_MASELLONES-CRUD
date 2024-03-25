Imports MySql.Data.MySqlClient

Module Pet
    Private dbConn As MySqlConnection
    Private sqlCommand As MySqlCommand
    Private da As MySqlDataAdapter
    Private dt As DataTable

    Dim strConn = "server=localhost; port=3307; user id=root; password=masellones; database=dbms_pet; Convert Zero Datetime=true"
    Public Sub dbConnection()
        Try
            dbConn = New MySqlConnection(strConn)
            'dbConn.ConnectionString = strConn
            dbConn.Open()
            'MessageBox.Show("Test connection successful")
        Catch ex As Exception
            MessageBox.Show("Error 101: DbConnection " & ex.Message)
            'MessageBox.Show("Error 101: DbConnection " & ex.ToString)
            Console.WriteLine(ex.ToString)
        Finally
            dbConn.Close()
        End Try
    End Sub

    Public Sub displayRecords(ByVal SQL As String, ByVal DG As DataGridView)
        dbConn.Open()
        da = New MySqlDataAdapter(SQL, dbConn)
        dt = New DataTable
        da.Fill(dt)
        DG.DataSource = dt

        dbConn.Close()
    End Sub

    Public Sub SQLManager(ByVal SQL As String)
        dbConn.Open()
        sqlCommand = New MySqlCommand(SQL, dbConn)

        With sqlCommand
            '.CommandText = SQL
            .CommandType = CommandType.Text
            '.Connection = dbConn
            .ExecuteNonQuery()
        End With

        dbConn.Close()
    End Sub

    Public Sub loadToComboBox(ByVal SQL As String, ByVal cbo As ComboBox)
        dbConn.Open()
        da = New MySqlDataAdapter(SQL, dbConn)
        dt = New DataTable
        da.Fill(dt)

        cbo.DataSource = dt
        'cbo.ValueMember = dt.Columns("owner_id").ToString ' value reference on your database
        'cbo.DisplayMember = dt.Columns("owner_name").ToString ' 
        cbo.ValueMember = dt.Columns(0).ToString ' value reference on your database
        cbo.DisplayMember = dt.Columns(1).ToString ' 

        dbConn.Close()
    End Sub

End Module
