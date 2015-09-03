Public Class Form1
    Private WithEvents hw As clHardware
    Private cPixPerM = 100
    Private W_OffsetRobot As New PointF(3.3, 3.1)
    'Private W_OffsetRobotHeading As Double = 0.0F
    Private Pen = New Pen(Color.Red)
    Private WithEvents Timer As New Timer
    'haus steht 0.17 rad

    Dim W_Pos As PointF
    Dim W_WayPoint As PointF
    Dim W_WayPoints As New Generic.Queue(Of PointF)
    Dim nTolerance As Decimal = 0.6

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim arrPort = System.IO.Ports.SerialPort.GetPortNames
        If arrPort.Length = 0 Then Throw New Exception("no port")
        hw = New clHardware(arrPort(arrPort.GetUpperBound(0)))
        Timer.Interval = 100
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'end drive
        W_WayPoint = W_Pos
        hw.Drive(0.01, 0)
        Timer.Enabled = False
    End Sub

    Private Sub hw_Message(strMsg As Object) Handles hw.Message
        If Me.InvokeRequired Then Me.BeginInvoke(Sub() hw_Message(strMsg)) : Return
        Trace.Items.Add(strMsg)
    End Sub

    Public Enum enCS
        World
        Robot
        Visual
    End Enum

    Private Function ConvPoint(Pin As PointF, SourceCS As enCS, DestCS As enCS) As PointF
        Dim W_P As PointF = Pin
        Select Case SourceCS
            Case enCS.Robot
                W_P = New PointF(Pin.X + W_OffsetRobot.X, Pin.Y + W_OffsetRobot.Y)
            Case enCS.Visual
                'Dezimeter, y Spiegeln
                W_P = New PointF(Pin.X / cPixPerM, (610 - Pin.Y) / cPixPerM)
        End Select

        Select Case DestCS
            Case enCS.Robot
                Return New PointF(W_P.X - W_OffsetRobot.X, W_P.Y - W_OffsetRobot.Y)
            Case enCS.Visual
                Return New PointF(W_P.X * cPixPerM, 610 - W_P.Y * cPixPerM)
            Case enCS.World
                Return W_P
        End Select
    End Function

    Private Sub hw_PositionChanged() Handles hw.PositionChanged
        If Me.InvokeRequired Then Me.BeginInvoke(Sub() hw_PositionChanged()) : Return

        Using g As Graphics = Map.CreateGraphics
            Dim W_CurPos = ConvPoint(hw.RobotPos, enCS.Robot, enCS.World)

            Dim V_OldPos = ConvPoint(W_Pos, enCS.World, enCS.Visual)
            Dim V_Pos = ConvPoint(W_CurPos, enCS.World, enCS.Visual)
            g.DrawLine(Pen, V_OldPos, V_Pos)
            Sprite.Location = New Point(V_Pos.X - Sprite.Width \ 2, V_Pos.Y - Sprite.Height \ 2)
            Status.Text = hw.Heading.ToString("N2") &
                " Coord " & hw.RobotPos.X.ToString("N2") & "," & hw.RobotPos.Y.ToString("N2")

            W_Pos = W_CurPos
        End Using

        'init drive system
        If W_WayPoint.X = 0 Then W_WayPoint = W_Pos
        Application.DoEvents()
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles Timer.Tick
        Dim nDiffY = CDbl(W_WayPoint.Y - W_Pos.Y)
        Dim nDiffX = CDbl(W_WayPoint.X - W_Pos.X)
        If nDiffX = 0 Then nDiffX = 0.01

        Dim nSoll As Double = Math.Atan(nDiffY / nDiffX) +
            If(nDiffX < 0 And nDiffY >= 0, Math.PI, 0) +
            If(nDiffX < 0 And nDiffY < 0, -Math.PI, 0)


        Dim nDist = Math.Sqrt(nDiffX ^ 2 + nDiffY ^ 2)

        'Rückwärtsfahren geht auch
        'If Math.Abs(nDiffHeading) > 2.5 Then
        '    If nDiffHeading <= 0 Then nDiffHeading += Math.PI
        '    If nDiffHeading > 0 Then nDiffHeading -= Math.PI
        '    nSpeed *= -1
        'End If
        Dim nIst = hw.Heading
        Dim nDiffHeading As Double = nSoll - nIst
        If nDiffHeading < -Math.PI Then nDiffHeading += 2 * Math.PI
        If nDiffHeading > Math.PI Then nDiffHeading -= 2 * Math.PI

        If nDist < nTolerance Then
            Trace.Items.Insert(0, "goal")
            hw.Drive(0.01, 0)
            If W_WayPoints.Count > 0 Then
                W_WayPoint = W_WayPoints.Dequeue
            Else
                Timer.Enabled = False
                hw.Drive(0.01, 0)
            End If
        Else
            Dim nSpeed = 1.0F
            If nDist < 2 Then nSpeed = 0.5
            Dim nSpeedTurn = nDiffHeading
            If Math.Abs(nSpeedTurn) < 0.2 Then nSpeedTurn = 0
            If Math.Abs(nSpeedTurn) > 2 Then nSpeedTurn = Math.Sign(nSpeedTurn) * 1
            hw.Drive(nSpeed, nSpeedTurn)
            Trace.Items.Insert(0, " Soll/Ist " &
                                nSoll.ToString("N2") & "rad  " &
                                nIst.ToString("N1") & "rad  " & vbTab &
                                " Delta " &
                                nDist.ToString("N2") & "m  " &
                                nDiffHeading.ToString("N1") & "rad  " & vbTab &
                                " Command " &
                                nSpeed.ToString("N1") & "m/s  " &
                                nSpeedTurn.ToString("N1") & "rad/s")

        End If
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        hw.Close()
    End Sub

    Private Sub Map_DoubleClick(sender As Object, e As MouseEventArgs) Handles Map.MouseDown
        If e.Button = MouseButtons.Right Then
            W_WayPoint = New PointF(0, 0)
            W_WayPoints.Clear()
            Dim Ist_W_Robot = ConvPoint(hw.RobotPos, enCS.Robot, enCS.World)
            Dim Soll_W_Robot = ConvPoint(e.Location, enCS.Visual, enCS.World)
            W_OffsetRobot = New PointF(W_OffsetRobot.X + Soll_W_Robot.X - Ist_W_Robot.X,
                                       W_OffsetRobot.Y + Soll_W_Robot.Y - Ist_W_Robot.Y)
            W_Pos = Soll_W_Robot
            'W_OffsetRobotHeading = hw.Heading
        Else
            W_WayPoints.Enqueue(ConvPoint(e.Location, enCS.Visual, enCS.World))
            Timer.Enabled = True
        End If
    End Sub

    Private Sub cmdL_Click(sender As Object, e As EventArgs) Handles cmdL.Click
        hw.Drive(0.3, 0.3)
    End Sub

    Private Sub cmdF_Click(sender As Object, e As EventArgs) Handles cmdF.Click
        hw.Drive(0.3, 0)
    End Sub

    Private Sub cmdR_Click(sender As Object, e As EventArgs) Handles cmdR.Click
        hw.Drive(0.3, -0.3)
    End Sub

    Private Sub cmdB_Click(sender As Object, e As EventArgs) Handles cmdB.Click
        hw.Drive(-0.3, 0)
    End Sub
End Class
