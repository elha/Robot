Imports System.Runtime.InteropServices
Imports System.Drawing.Imaging
Imports OpenNIWrapper
Imports Map

Public Class clRGBDCam
    Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (ByVal pDst As IntPtr, ByVal pSrc As IntPtr, ByVal ByteLen As Long)

    Dim mDepthStream As VideoStream
    Dim mMap As clMap

    Dim xSize As Integer = 320
    Dim ySize As Integer = 240

    Dim ProcessNthFrame As Integer = 30
    Dim nMaxIncline As Double = 60 '[0.1mm]
    Dim sensorHeight As Double = 545.0 '[mm]
    Dim sensorAngle As Double = 24 / 180 * Math.PI '[Grad -> Rad]

    Dim Buffer(ySize - 1, xSize - 1) As UInt16

    Dim fovH As Double = 58.0 / 180 * Math.PI
    Dim fovV As Double = 45.0 / 180 * Math.PI
    Dim xzFactor As Double = Math.Tan(fovH / 2) * 2
    Dim yzFactor As Double = Math.Tan(fovV / 2) * 2
    Dim coeffX As Double = xSize / xzFactor
    Dim coeffY As Double = ySize / yzFactor
    Dim cosSensor As Double = Math.Cos(sensorAngle)
    Dim sinSensor As Double = Math.Sin(sensorAngle)

    Public Sub New()
        Dim status As OpenNI.Status = OpenNI.Initialize()
        If Not HandleError(status) Then Throw New Exception("Error initializing OpenNI")

        Dim devices As DeviceInfo() = OpenNI.EnumerateDevices()
        If devices.Length = 0 Then Throw New Exception("No RGBD Device found")

        Dim device As Device = devices(0).OpenDevice()
        mDepthStream = device.CreateVideoStream(Device.SensorType.Depth)
        mDepthStream.Mirroring = False

        Dim vm = New VideoMode()
        vm.Resolution = New Size(xSize, ySize)
        vm.DataPixelFormat = VideoMode.PixelFormat.Depth100Um
        vm.Fps = 30
        mDepthStream.VideoMode = vm

        device.ImageRegistration = Device.ImageRegistrationMode.DepthToColor
        device.DepthColorSyncEnabled = False
    End Sub

    Friend Sub Start(oMap As clMap)
        If mDepthStream.IsValid Then
            If Not HandleError(mDepthStream.Start()) Then
                Throw New Exception("could not start RGBD Stream")
            End If
            AddHandler mDepthStream.OnNewFrame, AddressOf ProcessFrame
        End If

        mMap = oMap
    End Sub

    Public Sub Cancel()
        OpenNI.Shutdown()
    End Sub

    Function HandleError(status As OpenNI.Status) As Boolean
        If (status = OpenNI.Status.Ok) Then
            Return True
        End If
        Console.WriteLine("Error: " + status.ToString() + " - " + OpenNI.LastError)
        Console.ReadLine()
        Return False
    End Function

    Public Sub ProcessFrame(vStream As VideoStream)
        Static nCnt As Integer = 0 : nCnt += 1
        If nCnt < ProcessNthFrame Then Return
        nCnt = 0
        Dim depthFrame As VideoFrameRef = vStream.ReadFrame()
        FindLineOfSight(depthFrame)
        depthFrame.Release()
    End Sub

    Private Shared Sub CopyData(ptr As IntPtr, len As Long, dest As UInt16(,))
        Dim handle = GCHandle.Alloc(dest, GCHandleType.Pinned)
        Try
            Dim ptrdest = handle.AddrOfPinnedObject()
            CopyMemory(ptrdest, ptr, len)
        Finally
            If handle.IsAllocated Then handle.Free()
        End Try
    End Sub

    Private Function ConvertToWorld(x As Integer, y As Integer, depthz As UInt16) As Double()
        Dim orig As Double() = {(x / xSize - 0.5F) * depthz * xzFactor / 10,
                                      (0.5F - y / ySize) * depthz * yzFactor / -10,
                                      depthz / 10}
        Dim result = Me.Rotate(orig)
        result(1) = result(1) + sensorHeight
        Return result
    End Function

    Private Sub FindLineOfSight(depthFrame As VideoFrameRef)
        CopyData(depthFrame.Data, depthFrame.DataSize, Buffer)
        mMap.Clear()

        For x = 3 To xSize - 3 Step 5
            Dim arrIncline(4) As Double
            Dim arrLength(4) As Double
            Dim n = 0
            Dim pLast As Double() = Nothing
            Dim y = 0
            Dim pObstacle As Double() = Nothing
            Dim pLineOfSight As Double() = Nothing
            While y < ySize
                Dim nCur = Median3(Buffer(y, x - 1), Buffer(y, x), Buffer(y, x + 1))
                If nCur > 0 Then
                    Dim Point = ConvertToWorld(x, y, nCur)
                    If pLast IsNot Nothing Then
                        arrIncline(n Mod 5) = Point(1) - pLast(1)
                        arrLength(n Mod 5) = Point(2) - pLast(2)
                        n += 1
                        Dim nIncline = arrIncline(0) + arrIncline(1) + arrIncline(2) + arrIncline(3) + arrIncline(4)
                        Dim nLength = arrLength(0) + arrLength(1) + arrLength(2) + arrLength(3) + arrLength(4)

                        If pObstacle IsNot Nothing Then
                            'Hindernis: grob nach oben scannen ob noch näher
                            If pObstacle(2) > Point(2) Then pObstacle = Point
                            y += 4
                        ElseIf Math.Abs(nIncline) > nMaxIncline OrElse nLength < 0 Then
                            'Steigung > 3cm oder Objekt überhalb des Bodens
                            pObstacle = pLast
                        ElseIf nLength > 600
                            'wenn die Punkte zu weit auseinenander liegen dann wirds ungenau
                            Exit While
                        Else
                            'so weit sieht man
                            pLineOfSight = Point
                        End If
                    End If
                    pLast = Point
                End If
                y += 1
            End While
            If pLineOfSight IsNot Nothing Then mMap.SetPath(New Point(pLineOfSight(0) / 10, pLineOfSight(2) / 10))
            If pObstacle IsNot Nothing Then mMap.SetObstacle(New Point(pObstacle(0) / 10, pObstacle(2) / 10))
        Next
        mMap.UpdateUI()
    End Sub

    Public Function Rotate(vector As Double()) As Double()
        Return {vector(0),
                vector(1) * cosSensor - vector(2) * sinSensor,
                vector(1) * sinSensor + vector(2) * cosSensor}
    End Function

    Public Function Median3(n1 As Integer, n2 As Integer, n3 As Integer) As Integer
        'Sortieren
        If n2 < n1 Then n2 = n1
        If n3 < n2 Then Return n3
        Return n2
    End Function
End Class
