Public Class Form1
    Dim fff As String = ""
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        OpenFileDialog1.InitialDirectory = Application.StartupPath


        ' Me.ShowInTaskbar = False


    End Sub

    Sub leggibmp(f As String, splash As Boolean)
        Dim clr As Integer ' or string
        Dim xmax As Integer
        Dim ymax As Integer
        Dim x As Integer
        Dim y As Integer
        Dim bm As New Bitmap(f)
        Dim out As String = IO.Path.Combine(Application.StartupPath, "OUT")
        IO.Directory.CreateDirectory(out)
        If splash Then
            xmax = bm.Width - 1
            ymax = bm.Height - 1
            If xmax <> 127 Or ymax <> 63 Then
                MsgBox("Wrong image size, it must be 128x64 pixels...")
                End
            End If
            leggilo.Clear()
            IO.File.Delete(IO.Path.Combine(out, "TEXT_BMP.txt"))



            Dim bicolor As Integer = 0
            Using writer As IO.StreamWriter = New IO.StreamWriter(IO.Path.Combine(out, "TEXT_BMP.txt"), True)
                For y = 0 To ymax
                    Dim s As String = ""
                    For x = 0 To xmax '''Step 3
                        Dim bb = bm.GetPixel(x, y)
                        Dim color As Integer = (CInt(bb.R) + CInt(bb.G) + CInt(bb.B)) / 3
                        If color > CInt(NumericUpDown1.Value) Then
                            s = s + "."
                            bicolor = 0
                        ElseIf color < CInt(NumericUpDown3.Value) Then
                            s = s + "*"
                            bicolor = 0
                        Else
                            If bicolor Mod 2 = 0 Then
                                s = s + "."
                            Else
                                s = s + "*"
                            End If
                            bicolor = bicolor + 1
                        End If
                    Next x
                    leggilo.Add(s)

                    writer.WriteLine(s)

                Next y
            End Using
        Else
            xmax = bm.Width - 1
            ymax = bm.Height - 1
            If xmax <> 15 Or ymax <> 15 Then
                MsgBox("Wrong image size, it must be 16x16 pixels...")
                End
            End If
            leggilo.Clear()
            IO.File.Delete(IO.Path.Combine(out, "TEXT_BMP_ICON.txt"))

            Using writer As IO.StreamWriter = New IO.StreamWriter(IO.Path.Combine(out, "TEXT_BMP_ICON.txt"), True)
                writer.WriteLine("")
                writer.WriteLine("")
                writer.WriteLine("####  THIS IS A CUSTOM ICON FOR KLIPPER LCD   ####")
                writer.WriteLine("### Copy all and replace existing klipper icon or create new one")
                writer.WriteLine("")
                writer.WriteLine("")
                writer.WriteLine("data:")
                Dim bicolor As Integer = 0
                For y = 0 To ymax
                    Dim s As String = ""
                    For x = 0 To xmax
                        Dim bb = bm.GetPixel(x, y)
                        Dim color As Integer = (CInt(bb.R) + CInt(bb.G) + CInt(bb.B)) / 3
                        If color > CInt(NumericUpDown1.Value) Then
                            s = s + "."
                            bicolor = 0
                        ElseIf color < CInt(NumericUpDown3.Value) Then
                            s = s + "*"
                            bicolor = 0
                        Else
                            If bicolor Mod 2 = 0 Then
                                s = s + "."
                            Else
                                s = s + "*"
                            End If
                            bicolor = bicolor + 1
                        End If

                    Next x



                    writer.WriteLine(s)

                Next y
                writer.WriteLine("")
                writer.WriteLine("")
            End Using
        End If
        eseguitutto(splash)
    End Sub
    Dim leggilo As New List(Of String)
    Public Function ToBlackAndWhite3(bmp As Bitmap) As Bitmap
        bmp = New Bitmap(bmp) 'this makes sure the bitmap is 32-bit format.
        Dim gem As Integer
        Dim r, g, b As Integer
        Dim col As Integer


        r = (col And &HFF0000) >> 16 'get red byte
                g = (col And &HFF00) >> 8 'get green byte
                b = (col And &HFF) 'get blue byte
                gem = (r + g + b) \ 3

                If gem > 128 Then
            'pixels(i) = &HFFFFFFFF  'white
        Else
            ' pixels(i) = &HFF000000   'black
        End If

        Return bmp
    End Function

    Sub eseguitutto(splash As Boolean)
        Dim s As New List(Of String)
        Dim a As New List(Of String)
        s = leggilo
        Dim out As String = IO.Path.Combine(Application.StartupPath, "OUT")
        IO.Directory.CreateDirectory(out)

        If splash Then

            IO.File.Delete(IO.Path.Combine(out, "custom_printer.cfg"))
            IO.File.Delete(IO.Path.Combine(out, "custom_lcd.cfg"))

            Dim x1, y1, xx, yy As Integer
            xx = 0
            x1 = 0
            yy = 0
            y1 = 0

            xx = 0
            yy = 0
            Do While 1 = 1



                For x = 0 + xx To 16 + xx
                    For y = yy + 0 To yy + 15
                        Dim ss As String = s(y)
                        Dim sss = Mid(ss, (xx + 1), 16)
                        If sss = "" Or sss.Length <> 16 Then
                            Continue For
                        End If
                        a.Add(sss)
                    Next
                    xx = xx + 16
                Next
                yy = yy + 16
                xx = 0
                If yy >= 64 Then
                    Exit Do
                End If
            Loop

            xx = 0
            yy = 0

            Using writer As IO.StreamWriter = New IO.StreamWriter(IO.Path.Combine(out, "custom_printer.cfg"), True)
                writer.WriteLine("")
                writer.WriteLine("")
                writer.WriteLine("####  COPY all INTO Printer.cfg")
                writer.WriteLine("####   START COPY   ###")
                writer.WriteLine("")
                writer.WriteLine("[include custom_lcd.cfg] ")
                writer.WriteLine("")
                writer.WriteLine("[delayed_gcode START_LOGO]")
                writer.WriteLine("initial_duration: 0.001")
                writer.WriteLine("gcode:")
                writer.WriteLine("  {% set custom=" & Chr(34) & "_custom_start" & Chr(34) & " %}")
                writer.WriteLine("  SET_DISPLAY_GROUP GROUP={custom}")
                writer.WriteLine("  UPDATE_DELAYED_GCODE ID=clear_display DURATION=" & CStr(NumericUpDown2.Value) & "  #seconds to show splash screen")
                writer.WriteLine("")
                writer.WriteLine("")
                writer.WriteLine("#### CLEAR MESSAGE And SET DEFAULT DISPLAY GROUP")
                writer.WriteLine("[delayed_gcode clear_display]")
                writer.WriteLine("gcode:")
                writer.WriteLine("  #### CLEAR MESSAGE")
                writer.WriteLine("  M117")
                writer.WriteLine("  #### SET DEFAULT DISPLAY GROUP")
                writer.WriteLine("  SET_DISPLAY_GROUP GROUP={printer.configfile.settings.display.display_group}")
                writer.WriteLine("")
                writer.WriteLine("####   END COPY   ###")
                writer.WriteLine("")
                writer.WriteLine("")
                writer.WriteLine("")

            End Using

            Using writer As IO.StreamWriter = New IO.StreamWriter(IO.Path.Combine(out, "custom_lcd.cfg"), True)

                writer.WriteLine("")
                writer.WriteLine("")
                writer.WriteLine("")
                writer.WriteLine("####  copy me: (file: 'custom.lcd.cfg') in same 'printer.cfg' folder!                               ####")
                writer.WriteLine("####  or create new file inside same folder called 'custom_lcd.cfg' and paste all text inside it!   ####")
                writer.WriteLine("")
                writer.WriteLine("")
                ''''''''''''''''''''''''''''''''''''
                For x As Integer = 0 To 31
                    xx = (x Mod 8) * 2
                    yy = x \ 8
                    ' Append the first line of text to a new file.

                    writer.WriteLine("[display_data _custom_start start" & CStr(x + 1) & "]")
                    writer.WriteLine("position: " & CStr(yy) & ", " & CStr(xx))
                    writer.WriteLine("text:")
                    writer.WriteLine("  ~start" & CStr(x + 1) & "~")
                    writer.WriteLine("")

                    ''''''''''''''''''''''''''''''''''
                Next


                xx = 0
                yy = 0

                For x As Integer = 0 To 511 Step 16
                    xx = xx + 1
                    ' Append the first line of text to a new file.

                    writer.WriteLine("[display_glyph start" & CStr(xx) & "]")
                    writer.WriteLine("data:")

                    For y = 0 + x To 15 + x
                        Dim kk As Integer = y
                        Dim k As String = a(kk).Trim
                        k = k.Replace(Chr(13), "").Replace(Chr(10), "")
                        writer.WriteLine("  " & k)

                    Next
                    writer.WriteLine("")

                Next
                writer.WriteLine("")
            End Using
            leggilo.Clear()
            MsgBox("Files created successfull, follow instructions inside, Please!")
            Dim p As New ProcessStartInfo("Explorer.exe", out)
            p.WindowStyle = ProcessWindowStyle.Maximized
            Process.Start(p)
        Else
            MsgBox("ICON created successfull, copy file content into 'custom display' cfg file!")
            Dim p As New ProcessStartInfo("Explorer.exe", IO.Path.Combine(out, "TEXT_BMP_ICON.txt"))
            p.WindowStyle = ProcessWindowStyle.Maximized
            Process.Start(p)
        End If
        End
    End Sub
    Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk

    End Sub

    Dim filename As String = ""
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        PictureBox1.Visible = False
        PictureBox2.Visible = False

        OpenFileDialog1.FileName = "*.bmp"
        'OpenFileDialog1.Filter = "txt files (*.txt)|*.txt"
        OpenFileDialog1.Filter = "BMP files (*.bmp)|*.bmp"
        OpenFileDialog1.FilterIndex = 1
        OpenFileDialog1.RestoreDirectory = True
        filename = ""
        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Try
                fff = OpenFileDialog1.FileName().ToString
                If (fff IsNot Nothing) Then
                    '  eseguitutto(fff)
                    filename = (fff)
                    Dim bm As New Bitmap(filename)

                    If RadioButton1.Checked Then
                        Dim xmax = bm.Width - 1
                        Dim ymax = bm.Height - 1
                        Label3.Text = "IMAGE: Width: " & bm.Width & " pixels; Height: " & bm.Height & " pixels."
                        If xmax <> 127 Or ymax <> 63 Then
                            MsgBox("Wrong image size, it must be 128x64 pixels...")
                            Exit Sub
                        End If
                        PictureBox1.Image = bm
                        PictureBox1.Visible = True
                        PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
                    Else
                        Dim xmax = bm.Width - 1
                        Dim ymax = bm.Height - 1
                        Label3.Text = "IMAGE: Width: " & bm.Width & " pixels; Height: " & bm.Height & " pixels."
                        If xmax <> 15 Or ymax <> 15 Then
                            MsgBox("Wrong image size, it must be 16x16 pixels...")
                            Exit Sub
                        End If
                        PictureBox2.Image = bm
                        PictureBox2.Visible = True
                        PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage
                    End If


                End If
            Catch ex As Exception
                MsgBox("Cannot read file from disk. Original error: " & ex.Message)

            End Try
        End If

        '"................"
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If filename <> "" Then
            leggibmp(filename, RadioButton1.Checked)
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim a As New AboutBox1
        a.ShowDialog(Me)
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        PictureBox1.Visible = True
        PictureBox2.Visible = False
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        PictureBox1.Visible = False
        PictureBox2.Visible = True
    End Sub
End Class
