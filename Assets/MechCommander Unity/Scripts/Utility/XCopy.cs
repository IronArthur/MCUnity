using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

internal class XCopy
{
	private enum CopyProgressResult : uint
	{
		PROGRESS_CONTINUE,
		PROGRESS_CANCEL,
		PROGRESS_STOP,
		PROGRESS_QUIET
	}

	private enum CopyProgressCallbackReason : uint
	{
		CALLBACK_CHUNK_FINISHED,
		CALLBACK_STREAM_SWITCH
	}

	[Flags]
	private enum CopyFileFlags : uint
	{
		COPY_FILE_FAIL_IF_EXISTS = 1u,
		COPY_FILE_NO_BUFFERING = 4096u,
		COPY_FILE_RESTARTABLE = 2u,
		COPY_FILE_OPEN_SOURCE_FOR_WRITE = 4u,
		COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 8u
	}

	internal delegate void ErrorHandler(Exception ex);

	private delegate XCopy.CopyProgressResult CopyProgressRoutine(long totalFileSize, long totalBytesTransferred, long streamSize, long streamBytesTransferred, uint dwStreamNumber, XCopy.CopyProgressCallbackReason dwCallbackReason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData);

	private int _isCancelled;

	private int _filePercent;

	internal string Source;

	private string _dst;

	private XCopy()
	{
		this._isCancelled = 0;
	}

	private event EventHandler Completed;

	private event EventHandler<ProgressChangedEventArgs> ProgressChanged;

	internal static void Copy(string source, string dest, bool overwrite, bool isLargeTransfer = false, EventHandler<ProgressChangedEventArgs> progressChangedHandler = null, EventHandler completedHandler = null, XCopy.ErrorHandler errorHandler = null)
	{
		new XCopy().CopyInternal(source, dest, overwrite, isLargeTransfer, progressChangedHandler, completedHandler, errorHandler);
	}

	private void CopyInternal(string source, string dest, bool overwrite, bool isLargeTransfer, EventHandler<ProgressChangedEventArgs> progressChangedHandler, EventHandler completedHandler, XCopy.ErrorHandler errorHandler)
	{
		try
		{
			XCopy.CopyFileFlags copyFileFlags = XCopy.CopyFileFlags.COPY_FILE_RESTARTABLE;
			if (!overwrite)
			{
				copyFileFlags |= XCopy.CopyFileFlags.COPY_FILE_FAIL_IF_EXISTS;
			}
			if (isLargeTransfer)
			{
				copyFileFlags |= XCopy.CopyFileFlags.COPY_FILE_NO_BUFFERING;
			}
			this.Source = source;
			this._dst = dest;
			if (progressChangedHandler != null)
			{
				this.ProgressChanged = (EventHandler<ProgressChangedEventArgs>)Delegate.Combine(this.ProgressChanged, progressChangedHandler);
			}
			if (completedHandler != null)
			{
				this.Completed = (EventHandler)Delegate.Combine(this.Completed, completedHandler);
			}
			if (!XCopy.CopyFileEx(this.Source, this._dst, new XCopy.CopyProgressRoutine(this.CopyProgressHandler), IntPtr.Zero, ref this._isCancelled, copyFileFlags))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}
		catch (Exception ex)
		{
			if (progressChangedHandler != null)
			{
				this.ProgressChanged = (EventHandler<ProgressChangedEventArgs>)Delegate.Remove(this.ProgressChanged, progressChangedHandler);
			}
			if (completedHandler != null)
			{
				this.Completed = (EventHandler)Delegate.Remove(this.Completed, completedHandler);
			}
			if (errorHandler != null)
			{
				errorHandler(ex);
			}
		}
	}

	private void OnProgressChanged(double percent)
	{
		if ((int)percent > this._filePercent)
		{
			this._filePercent = (int)percent;
			EventHandler<ProgressChangedEventArgs> progressChanged = this.ProgressChanged;
			if (progressChanged != null)
			{
				progressChanged(this, new ProgressChangedEventArgs(this._filePercent, this.Source));
			}
		}
	}

	private void OnCompleted()
	{
		EventHandler completed = this.Completed;
		if (completed != null)
		{
			completed(this, EventArgs.Empty);
		}
	}

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool CopyFileEx(string lpExistingFileName, string lpNewFileName, XCopy.CopyProgressRoutine lpProgressRoutine, IntPtr lpData, ref int pbCancel, XCopy.CopyFileFlags dwCopyFlags);

	private XCopy.CopyProgressResult CopyProgressHandler(long total, long transferred, long streamSize, long streamByteTrans, uint dwStreamNumber, XCopy.CopyProgressCallbackReason reason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData)
	{
		if (reason == XCopy.CopyProgressCallbackReason.CALLBACK_CHUNK_FINISHED)
		{
			this.OnProgressChanged((double)transferred / (double)total * 100.0);
		}
		if (transferred >= total)
		{
			this.OnCompleted();
		}
		return XCopy.CopyProgressResult.PROGRESS_CONTINUE;
	}
}
