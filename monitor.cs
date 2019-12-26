using System;
using System.IO;
using System.Threading;
using mtail;
using System.Diagnostics;

namespace Monitor
{
    public class MonitorEventArgs : EventArgs
    {
        public MonitorEventArgs(string text) { this.Text = text; }
        public string Text { get; private set; }
    }

    public delegate void MonitorEventHandler(Object sender, MonitorEventArgs e);

    /// <summary>
    /// Summary description for Monitor.
    /// </summary>
    public class Monitor : IDisposable
    {
        private Thread watcher = null;
        public string FilePath { get; private set; }
        public bool Monitoring { get; private set; }
        public event MonitorEventHandler OnTextAppended;

        public Monitor(string FilePath)
        {
            this.FilePath = FilePath;
        }

        public void StartMonitoring()
        {
            if (this.Monitoring)
                throw new ApplicationException("Already monitoring.");

            var w = new Thread(new ThreadStart(DoMonitor));
            w.Start();
            watcher = w;
            this.Monitoring = true;
        }

        public void StopMonitoring()
        {
            if (!this.Monitoring)
                throw new ApplicationException("Not monitoring.");

            var w = watcher;
            watcher = null;
            this.Monitoring = false;

            if (w != null && w.IsAlive)
            {
                var evt = new AutoResetEvent(false);
                w.Abort(evt);
                evt.WaitOne(2000); //wait for the watcher to terminate.
            }
        }

        private void DoMonitor()
        {
            try
            {
                // Open the file...
                var fi = new FileInfo(this.FilePath);
                var fs = fi.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                //Move near the end...
                if (fs.CanSeek && fs.Length > 1000)
                    fs.Position = fs.Length - 1000;

                using (var sr = new StreamReader(fs))
                {
                    while (sr.BaseStream.CanRead)
                    {
                        var input = sr.ReadToEnd();
                        try
                        {
                            if (OnTextAppended != null && !string.IsNullOrEmpty(input))
                                OnTextAppended(this, new MonitorEventArgs(input));
                        }
                        catch (ThreadAbortException)
                        {
                            throw;
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.ToString());
                            //keep going
                        }

                        //pause
                        Thread.Sleep(500);

                    }//while
                }
            }
            catch (ThreadAbortException e)
            {
                var evt = e.ExceptionState as AutoResetEvent;
                if (evt != null)
                    evt.Set();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (this.Monitoring)
                    StopMonitoring();
            }
            catch
            {
            }
        }

        #endregion
    } // class monitor
} // namespace Monitor
