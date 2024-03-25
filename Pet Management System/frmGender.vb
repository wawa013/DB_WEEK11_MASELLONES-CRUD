Public Class frmGender
    Dim strQuery As String = ""

    Private Sub frmGender_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        strQuery = "SELECT * FROM tbl_gender"
        displayRecords(strQuery, DGGender)
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        strQuery = "INSERT INTO tbl_gender VALUES (" & txtID.Text & ", '" & txtGender.Text & "')"
        SQLManager(strQuery)

        strQuery = "SELECT * FROM tbl_gender"
        displayRecords(strQuery, DGGender)
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If Not String.IsNullOrEmpty(txtID.Text) Then
            strQuery = "DELETE FROM tbl_gender WHERE Gender_id=" & txtID.Text
            SQLManager(strQuery)

            strQuery = "SELECT * FROM tbl_gender"
            displayRecords(strQuery, DGGender)
        Else
            MessageBox.Show("Please enter a valid ID to delete.")
        End If
    End Sub
End Class
