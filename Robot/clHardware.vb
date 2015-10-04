Imports System.IO.Ports
Imports System.Linq

Public Class clHardware
    Public Event PositionChanged()
    Public Event Message(strMsg)

    Dim WithEvents oSerial As SerialPort
	Public RobotPos As New Point(0, 0)
	Public Heading As Decimal = 0
	Public BodySize As Point() = {New Point(-30, -5), New Point(-10, 30), New Point(10, 30), New Point(30, -5)}
	Dim oReader = New System.Threading.Thread(Sub()
                                                  While True
                                                      Try
                                                          ParseData(oSerial.ReadLine)
                                                      Catch ex As Exception
                                                          Exit While
                                                      End Try
                                                  End While
                                              End Sub)

    Public Sub New(strPort As String)
        oSerial = New SerialPort(strPort, 115200)
        oSerial.Open()
        oReader.Start()
    End Sub

    Public Sub ParseData(strData As String)
        If String.IsNullOrWhiteSpace(strData) Then Return
        Dim arrData = strData.Split(vbTab)

        Select Case arrData(0)
            Case "o"
				RobotPos.X = Double.Parse(arrData(1)) * 100
				RobotPos.Y = Double.Parse(arrData(2)) * 100
				Heading = Double.Parse(arrData(4))
                RaiseEvent PositionChanged()

            Case "InitializeDriveGeometry"
                SendCommand("DriveGeometry", -3, 220, -3, 460, 588 * 2)
            Case "InitializeSpeedController"
                SendCommand("SpeedControllerParams", -1, 3, 0, 2, -1, 3, 0, 2, 0, 1)
            Case "InitializeBatteryMonitor"
                SendCommand("BatteryMonitorParams", -1, 1)
            Case Else
                RaiseEvent Message(strData)

        End Select
    End Sub

    Public Sub Drive(nSpeed As Decimal, nTurn As Decimal)
        Dim nExpDrive = -2
        Dim nSpeedDrive = Math.Floor(nSpeed * 100)
        Dim nExpTurn = -2
        Dim nSpeedTurn = Math.Floor(nTurn * 100)
        SendCommand("s", nExpDrive, nSpeedDrive, nExpTurn, nSpeedTurn)
    End Sub

    Public Sub SendCommand(strCmd As String, ParamArray arrInt() As Integer)
        Try
            oSerial.Write(strCmd & " ")
            Array.ForEach(arrInt, Sub(i) oSerial.Write(i.ToString & " "))
            oSerial.WriteLine("#")
        Catch ex As Exception

        End Try
    End Sub

    Friend Sub Close()
        oReader.Abort
    End Sub
End Class
