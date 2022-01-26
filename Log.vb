Imports System.IO
'
' LogFile = path.to.logfile.log
' LogLeversRef = ("debug", "info", "trace", "etc...")
' Write("debug", "message to be pushed into the log")
'
Namespace YOUR_NAMESPACE
    Public Class Log
        Private Disposed As Boolean = False
        Private LogWriter As StreamWriter
        Private LogLevels As List(Of String)
        Private LogLock As Object
        Private LogfileAvailable As Boolean = True

        Public Sub New(ByVal LogFile As String, ByRef LogLevelsRef As List(Of String))
            LogLock = New Object()
            Try
                LogWriter = New StreamWriter(LogFile, True, System.Text.Encoding.UTF8)
                LogLevels = LogLevelsRef
                Write("info", "Logging Init.")
            Catch ex As Exception
                '//obviously if we cant open it we cant write to it...
                LogfileAvailable = False
            End Try
        End Sub

        Public Sub Write(Level As String, Content As String)
            If Disposed Then Return
            If (LogfileAvailable = False) Then Return
            If (LogLevels.Contains(Level) = False) Then Return
            Dim stackTrace As StackTrace = New StackTrace()
            Dim messageout As String = ""
            SyncLock LogLock
                messageout = $"{DateTime.Now.ToString("MM-dd HH:mm:ss")}.{DateTime.Now.Millisecond.ToString("D3")} [{Level}] {stackTrace.GetFrame(1).GetMethod().Name}: {Content}"
                LogWriter.WriteLine(messageout)
                LogWriter.Flush()
            End SyncLock
        End Sub

        Public Sub Dispose()
            If Disposed Then Return
            LogWriter.Close()
            LogWriter.Dispose()
            LogLevels.Clear()
            LogLevels = Nothing
            LogLock = Nothing
            LogfileAvailable = False
            Disposed = True
            MyBase.Finalize()
        End Sub

    End Class
End Namespace
