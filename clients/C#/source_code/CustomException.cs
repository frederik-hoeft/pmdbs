using System;

namespace pmdbs
{
    /// <summary>
    /// Provides a set of custom error messages.
    /// </summary>
    public static class CustomException
    {
        /// <summary>
        /// Provides a set of custom error messages.
        /// </summary>
        public struct ThrowNew
        {
            /// <summary>
            /// Creates a custom error message box for network exceptions.
            /// </summary>
            public static void NetworkException()
            {
                _ = WindowManager.Overlay.Spawn(GlobalVarPool.MainForm, (ErrorForm)GlobalVarPool.MainForm.Invoke(new Func<ErrorForm>(() => new ErrorForm("An unknown network error has occurred.", "Network Exception"))));
            }
            /// <summary>
            /// Creates a custom error message box for network exceptions.
            /// </summary>
            /// <param name="message">The error message that explains the reason for the exception.</param>
            public static void NetworkException(string message)
            {
                _ = WindowManager.Overlay.Spawn(GlobalVarPool.MainForm, (ErrorForm)GlobalVarPool.MainForm.Invoke(new Func<ErrorForm>(() => new ErrorForm(message, "Network Exception"))));
            }
            /// <summary>
            /// Creates a custom error message box for network exceptions.
            /// </summary>
            /// <param name="message">The error message that explains the reason for the exception.</param>
            /// <param name="code">The unique error code for this exception type.</param>
            public static void NetworkException(string message, string code)
            {
                _ = WindowManager.Overlay.Spawn(GlobalVarPool.MainForm, (ErrorForm)GlobalVarPool.MainForm.Invoke(new Func<ErrorForm>(() => new ErrorForm(message, "Network Exception"))));
            }
            /// <summary>
            /// Creates a custom error message box that can be used for any type of exception.
            /// </summary>
            public static void GenericException()
            {
                _ = WindowManager.Overlay.Spawn(GlobalVarPool.MainForm, (ErrorForm)GlobalVarPool.MainForm.Invoke(new Func<ErrorForm>(() => new ErrorForm("An unknown error has occurred.", "Generic Exception"))));
            }
            /// <summary>
            /// Creates a custom error message box that can be used for any type of exception.
            /// </summary>
            /// <param name="message">The error message that explains the reason for the exception.</param>
            public static void GenericException(string message)
            {
                _ = WindowManager.Overlay.Spawn(GlobalVarPool.MainForm, (ErrorForm)GlobalVarPool.MainForm.Invoke(new Func<ErrorForm>(() => new ErrorForm(message, "Generic Exception"))));
            }
            /// <summary>
            /// Creates a custom error message box for cryptographic exceptions.
            /// </summary>
            public static void CryptographicException()
            {
                _ = WindowManager.Overlay.Spawn(GlobalVarPool.MainForm, (ErrorForm)GlobalVarPool.MainForm.Invoke(new Func<ErrorForm>(() => new ErrorForm("An unknown cryptographic error has occurred.", "Cryptographic Exception"))));
            }
            /// <summary>
            /// Creates a custom error message box for cryptographic exceptions.
            /// </summary>
            /// <param name="message">The error message that explains the reason for the exception.</param>
            public static void CryptographicException(string message)
            {
                _ = WindowManager.Overlay.Spawn(GlobalVarPool.MainForm, (ErrorForm)GlobalVarPool.MainForm.Invoke(new Func<ErrorForm>(() => new ErrorForm(message, "Cryptographic Exception"))));
            }
            /// <summary>
            /// Creates a custom error message box for exceptions related to invalid formatting.
            /// </summary>
            public static void FormatException()
            {
                _ = WindowManager.Overlay.Spawn(GlobalVarPool.MainForm, (ErrorForm)GlobalVarPool.MainForm.Invoke(new Func<ErrorForm>(() => new ErrorForm("An unknown format error has occured.", "Format Exception"))));
            }
            /// <summary>
            /// Creates a custom error message box for exceptions related to invalid formatting.
            /// </summary>
            /// <param name="message">The error message that explains the reason for the exception.</param>
            public static void FormatException(string message)
            {
                _ = WindowManager.Overlay.Spawn(GlobalVarPool.MainForm, (ErrorForm)GlobalVarPool.MainForm.Invoke(new Func<ErrorForm>(() => new ErrorForm(message, "Format Exception"))));
            }
            /// <summary>
            /// Creates a custom error message box indicating that a requested feature is not yet available.
            /// </summary>
            public static void NotImplementedException()
            {
                _ = WindowManager.Overlay.Spawn(GlobalVarPool.MainForm, (ErrorForm)GlobalVarPool.MainForm.Invoke(new Func<ErrorForm>(() => new ErrorForm("This feature is not available yet.", "Not Implemented Exception"))));
            }
            /// <summary>
            /// Creates a custom error message box indicating that a requested feature is not yet available.
            /// </summary>
            /// <param name="message">The error message that explains the reason for the exception.</param>
            public static void NotImplementedException(string message)
            {
                _ = WindowManager.Overlay.Spawn(GlobalVarPool.MainForm, (ErrorForm)GlobalVarPool.MainForm.Invoke(new Func<ErrorForm>(() => new ErrorForm(message, "Not Implemented Exception"))));
            }
            /// <summary>
            /// Creates a custom error message box for IndexOutOfRangeExceptions.
            /// </summary>
            public static void IndexOutOfRangeException()
            {
                _ = WindowManager.Overlay.Spawn(GlobalVarPool.MainForm, (ErrorForm)GlobalVarPool.MainForm.Invoke(new Func<ErrorForm>(() => new ErrorForm("An IndexOutOfRange Exception has occured.", "Index Out Of Range Exception"))));
            }
            /// <summary>
            /// Creates a custom error message box for IndexOutOfRangeExceptions.
            /// </summary>
            /// <param name="message">The error message that explains the reason for the exception.</param>
            public static void IndexOutOfRangeException(string message)
            {
                _ = WindowManager.Overlay.Spawn(GlobalVarPool.MainForm, (ErrorForm)GlobalVarPool.MainForm.Invoke(new Func<ErrorForm>(() => new ErrorForm(message, "Index Out Of Range Exception"))));
            }
        }
    }
}
