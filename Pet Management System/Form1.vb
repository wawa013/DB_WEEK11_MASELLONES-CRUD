Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient

Public Class frmPet
    Dim query As String = ""
    Private tbl_pet As New DataTable()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializePetDataTable()
    End Sub

    Private Sub InitializePetDataTable()
        tbl_pet.Columns.Add("ID", GetType(Integer))
        tbl_pet.Columns.Add("Name", GetType(String))
        tbl_pet.Columns.Add("Gender", GetType(String))
        tbl_pet.Columns.Add("Age", GetType(Integer))
        tbl_pet.Columns.Add("Breed", GetType(String))
        tbl_pet.Columns.Add("Price", GetType(Decimal))
        tbl_pet.Columns.Add("OwnerID", GetType(Integer))
    End Sub

    Private Sub frmPet_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        Call dbConnection()
        query = "SELECT *from tbl_pet"
        displayRecords(query, DGPet)

        loadToComboBox("SELECT *from tbl_owner", cboOwner)
        loadToComboBox("SELECT *from tbl_Type", cboType)
        loadToComboBox("SELECT *from tbl_breed", cboBreed)
        loadToComboBox("SELECT *from tbl_gender", cboGender)

        txtID.Text = GetNextAvailablePetID().ToString()
    End Sub

    Private Function GetNextAvailablePetID() As Integer
        Using connection As New MySqlConnection("Server=localhost;Port=3307;Database=dbms_pet;Uid=root;Pwd=masellones;")
            connection.Open()

            Dim countQuery As String = "SELECT COUNT(*) FROM tbl_pet"
            Using cmd As New MySqlCommand(countQuery, connection)
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                If count = 0 Then
                    Return 1000
                Else
                    Dim maxIDQuery As String = "SELECT MAX(pet_id) FROM tbl_pet"
                    Using maxIDCmd As New MySqlCommand(maxIDQuery, connection)
                        Dim maxID As Integer = Convert.ToInt32(maxIDCmd.ExecuteScalar())
                        Return maxID + 1
                    End Using
                End If
            End Using
        End Using
    End Function

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            Using connection As New MySqlConnection("Server=localhost;Port=3307;Database=dbms_pet;Uid=root;Pwd=masellones;")
                connection.Open()

                Dim query As String = "INSERT INTO tbl_pet (pet_id, pet_name, pet_gender, pet_age, pet_breed, pet_price, status_id, owner_id) VALUES (@ID, @Name, @GenderID, @Age, @BreedID, @Price, @StatusID, @OwnerID)"

                Using cmd As New MySqlCommand(query, connection)
                    cmd.Parameters.AddWithValue("@ID", Integer.Parse(txtID.Text))
                    cmd.Parameters.AddWithValue("@Name", txtName.Text)
                    cmd.Parameters.AddWithValue("@GenderID", Convert.ToInt32(cboGender.SelectedValue))
                    cmd.Parameters.AddWithValue("@Age", Integer.Parse(txtAge.Text))
                    cmd.Parameters.AddWithValue("@BreedID", Convert.ToInt32(cboBreed.SelectedValue))
                    cmd.Parameters.AddWithValue("@Price", Decimal.Parse(txtPrice.Text))
                    cmd.Parameters.AddWithValue("@StatusID", 1)
                    cmd.Parameters.AddWithValue("@OwnerID", Convert.ToInt32(cboOwner.SelectedValue))

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            clear()

            MessageBox.Show("Data saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Function IsFormValid() As Boolean
        If String.IsNullOrWhiteSpace(txtID.Text) OrElse
       String.IsNullOrWhiteSpace(txtName.Text) OrElse
       cboGender.SelectedItem Is Nothing OrElse
       String.IsNullOrWhiteSpace(txtAge.Text) OrElse
       cboBreed.SelectedItem Is Nothing OrElse
       String.IsNullOrWhiteSpace(txtPrice.Text) OrElse
       cboOwner.SelectedValue Is Nothing Then
            Return False
        End If

        Return True
    End Function




    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        clear()
    End Sub

    Public Sub clear()
        SQLManager(query)
        displayRecords("SELECT *from tbl_pet", DGPet)

        txtID.Clear()
        txtName.Clear()
        cboGender.SelectedIndex = 0
        txtAge.Clear()
        cboBreed.SelectedIndex = 0
        txtPrice.Clear()

        txtID.Enabled = True
        btnSave.Enabled = True
        btnUpdate.Enabled = False
        btnDelete.Enabled = False
    End Sub
    Private Sub DGPet_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DGPet.CellClick
        Dim i As Integer = e.RowIndex
        If i >= 0 AndAlso i < DGPet.Rows.Count Then
            With DGPet.Rows(i)
                txtID.Text = .Cells("pet_id").Value.ToString()
                txtName.Text = .Cells("pet_name").Value.ToString()
                cboGender.SelectedItem = .Cells("pet_gender").Value.ToString()
                If Integer.TryParse(.Cells("pet_age").Value.ToString(), Nothing) Then
                    txtAge.Text = .Cells("pet_age").Value.ToString()
                Else
                    txtAge.Text = ""
                End If
                cboBreed.SelectedItem = .Cells("pet_breed").Value.ToString()
                If Decimal.TryParse(.Cells("pet_price").Value.ToString(), Nothing) Then
                    txtPrice.Text = .Cells("pet_price").Value.ToString()
                Else
                    txtPrice.Text = ""
                End If
                If .Cells("owner_id").Value IsNot Nothing Then
                    If Integer.TryParse(.Cells("owner_id").Value.ToString(), Nothing) Then
                        cboOwner.SelectedValue = .Cells("owner_id").Value.ToString()
                    Else
                        cboOwner.SelectedIndex = -1
                    End If
                Else
                    cboOwner.SelectedIndex = -1
                End If
            End With

            txtID.Enabled = False
            btnSave.Enabled = False
            btnUpdate.Enabled = True
            btnDelete.Enabled = True
        End If
    End Sub




    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Try
            Dim petID As Integer
            If Not Integer.TryParse(txtID.Text, petID) OrElse petID <= 0 Then
                MessageBox.Show("Please select a valid pet ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            If Not IsFormValid() Then
                MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim breedID As Integer = Convert.ToInt32(cboBreed.SelectedValue)
            Dim genderID As Integer = Convert.ToInt32(cboGender.SelectedValue)

            Dim query As String = "UPDATE tbl_pet SET pet_name = @Name, pet_gender = @GenderID, pet_age = @Age, pet_breed = @BreedID, pet_price = @Price, owner_id = @OwnerID WHERE pet_id = @PetID"

            Using connection As New MySqlConnection("Server=localhost;Port=3307;Database=dbms_pet;Uid=root;Pwd=masellones;")
                connection.Open()

                Using cmd As New MySqlCommand(query, connection)
                    cmd.Parameters.AddWithValue("@Name", txtName.Text)
                    cmd.Parameters.AddWithValue("@GenderID", genderID)
                    cmd.Parameters.AddWithValue("@Age", Integer.Parse(txtAge.Text))
                    cmd.Parameters.AddWithValue("@BreedID", breedID)
                    cmd.Parameters.AddWithValue("@Price", Decimal.Parse(txtPrice.Text))
                    cmd.Parameters.AddWithValue("@OwnerID", Convert.ToInt32(cboOwner.SelectedValue))
                    cmd.Parameters.AddWithValue("@PetID", petID)

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            clear()

            MessageBox.Show("Data updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error occurred: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub




    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        query = "DELETE from tbl_pet WHERE pet_id=" & txtID.Text & ""
        clear()

    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        query = "SELECT *from tbl_pet WHERE pet_id LIKE '%" & txtSearch.Text & "%' OR pet_name LIKE '%" & txtSearch.Text & "%'"
        displayRecords(query, DGPet)
    End Sub

    Private Sub btnAddGender_Click(sender As Object, e As EventArgs) Handles btnAddGender.Click
        Dim newForm As New frmGender
        newForm.ShowDialog()

    End Sub

    Private Sub txtName_TextChanged(sender As Object, e As EventArgs) Handles txtName.TextChanged

    End Sub

    Private Sub DGPet_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DGPet.CellContentClick

    End Sub

    Private Sub cboType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboType.SelectedIndexChanged

    End Sub

    Private Sub Label8_Click(sender As Object, e As EventArgs) Handles Label8.Click

    End Sub
End Class
