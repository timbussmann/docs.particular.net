﻿#pragma warning disable 649

namespace Core7
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;

    class CriticalErrors
    {
        CriticalErrors(EndpointConfiguration endpointConfiguration)
        {
            #region DefiningCustomHostErrorHandlingAction

            endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);

            #endregion
        }

        #region CustomHostErrorHandlingAction

        async Task OnCriticalError(ICriticalErrorContext context)
        {
            try
            {
                // To leave the process active, stop the endpoint.
                // When it is stopped, attempts to send messages will cause an ObjectDisposedException.
                await context.Stop().ConfigureAwait(false);
                // Perform custom actions here, e.g.
                // NLog.LogManager.Shutdown();
            }
            finally
            {
                var failMessage = $"Critical error shutting down:'{context.Error}'.";
                Environment.FailFast(failMessage, context.Exception);
            }
        }

        #endregion

        async Task CriticalErrorHandler(ICriticalErrorContext criticalErrorContext)
        {
            #region StopEndpointInCriticalError

            await criticalErrorContext.Stop();

            #endregion
        }


        void InvokeCriticalError(CriticalError criticalError, string errorMessage, Exception exception)
        {
            #region InvokeCriticalError

            // 'criticalError' is an instance of NServiceBus.CriticalError
            // This instance can be resolved from dependency injection.
            criticalError.Raise(errorMessage, exception);

            #endregion
        }
    }
}
